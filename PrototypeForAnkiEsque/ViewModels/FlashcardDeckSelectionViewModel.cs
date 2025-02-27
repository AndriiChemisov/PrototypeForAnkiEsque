using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using System.IO;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckSelectionViewModel : BaseViewModel
    {
        private readonly IDeckService _deckService;
        private readonly INavigationService _navigationService;
        private readonly IMessageService _messageService;
        private FlashcardDeck _selectedDeck;
        private string _errorMessage;
        private string _searchText;

        public ObservableCollection<FlashcardDeck> Decks { get; private set; } = new();
        public ObservableCollection<FlashcardDeck> FilteredDecks { get; private set; } = new();

        public ICommand ReviewDeckCommand { get; }
        public ICommand EditDeckCommand { get; }
        public ICommand DeleteDeckCommand { get; }
        public ICommand OpenMainMenuCommand { get; }
        public ICommand OpenDeckCreatorCommand { get; }
        public ICommand ImportDecksCommand { get; }
        public ICommand ExportDecksCommand { get; }

        public FlashcardDeck SelectedDeck
        {
            get => _selectedDeck;
            set => SetProperty(ref _selectedDeck, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    UpdateFilteredDecks();
                }
            }
        }

        public FlashcardDeckSelectionViewModel(IDeckService deckService, INavigationService navigationService, IMessageService messageService)
        {
            _deckService = deckService;
            _navigationService = navigationService;
            _messageService = messageService;

            ReviewDeckCommand = new AsyncRelayCommand(ReviewDeckAsync);
            EditDeckCommand = new AsyncRelayCommand(EditDeckAsync);
            DeleteDeckCommand = new AsyncRelayCommand(DeleteDeckAsync);
            OpenMainMenuCommand = new AsyncRelayCommand(OpenMainMenuAsync);
            OpenDeckCreatorCommand = new AsyncRelayCommand(OpenDeckCreatorAsync);
            ImportDecksCommand = new AsyncRelayCommand<string>(ImportDecksAsync);
            ExportDecksCommand = new AsyncRelayCommand<string>(ExportDecksAsync);

            LoadDecksAsync();
        }

        private async void LoadDecksAsync()
        {
            try
            {
                var decks = await _deckService.GetPagedDecksAsync(1, int.MaxValue);
                Decks = new ObservableCollection<FlashcardDeck>(decks);
                UpdateFilteredDecks();
            }
            catch (Exception ex)
            {
                _messageService.ShowMessage($"An error occurred while loading decks: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        private void UpdateFilteredDecks()
        {
            FilteredDecks.Clear();
            foreach (var deck in Decks.Where(d => string.IsNullOrEmpty(SearchText) || d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredDecks.Add(deck);
            }
        }

        private async Task ReviewDeckAsync()
        {
            if (SelectedDeck == null)
            {
                _messageService.ShowMessage("Please select a deck to review.", "Warning", MessageBoxImage.Warning);
                return;
            }

            await _navigationService.GetFlashcardViewAsync(SelectedDeck);
        }

        private async Task EditDeckAsync()
        {
            if (SelectedDeck == null)
            {
                _messageService.ShowMessage("Please select a deck to edit.", "Warning", MessageBoxImage.Warning);
                return;
            }

            await _navigationService.GetFlashcardDeckEditorViewAsync(SelectedDeck);
        }

        private async Task DeleteDeckAsync()
        {
            if (SelectedDeck == null)
            {
                _messageService.ShowMessage("Please select a deck to delete.", "Warning", MessageBoxImage.Warning);
                return;
            }

            var result = _messageService.ShowMessageWithButton("Are you sure you want to delete this deck?", "Confirm Deletion", MessageBoxImage.Question, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await _deckService.DeleteDeckAsync(SelectedDeck.Id);
                Decks.Remove(SelectedDeck); // Remove the deck from the Decks collection
                UpdateFilteredDecks(); // Update the filtered decks after deletion
            }
        }


        private async Task OpenMainMenuAsync()
        {
            await _navigationService.GetMainMenuViewAsync();
        }

        private async Task OpenDeckCreatorAsync()
        {
            await _navigationService.GetFlashcardDeckCreatorViewAsync();
        }

        public async Task ExportDecksAsync(string filePath)
        {
            var decksToExport = SelectedDeck != null ? new List<FlashcardDeck> { SelectedDeck } : Decks.ToList();
            var deckDtos = decksToExport.Select(d => new FlashcardDeckDto
            {
                DeckName = d.Name,
                Flashcards = d.FlashcardFronts.Select(f => new FlashcardDto { Front = f, Back = "" }).ToList(),
                EaseRating = d.EaseRating
            }).ToList();

            var json = JsonSerializer.Serialize(deckDtos);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task ImportDecksAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var deckDtos = JsonSerializer.Deserialize<List<FlashcardDeckDto>>(json);

            var existingDecks = await _deckService.GetPagedDecksAsync(1, int.MaxValue);
            var newDecks = deckDtos
                .Where(dto => !existingDecks.Any(d => d.Name == dto.DeckName))
                .Select(dto => new FlashcardDeck
                {
                    Name = dto.DeckName,
                    FlashcardFronts = dto.Flashcards.Select(f => f.Front).ToList(),
                    EaseRating = dto.EaseRating ?? "Hard"
                })
                .ToList();

            if (newDecks.Any())
            {
                foreach (var deck in newDecks)
                {
                    await _deckService.CreateDeckAsync(deck.Name, deck.FlashcardFronts, deck.EaseRating);
                }
                LoadDecksAsync();
            }
        }
    }
}
