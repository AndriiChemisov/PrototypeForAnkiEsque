using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using System.IO;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckSelectionViewModel : BaseViewModel
    {
        private readonly DeckService _deckService;
        private readonly NavigationService _navigationService;
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

        public FlashcardDeckSelectionViewModel(DeckService deckService, NavigationService navigationService)
        {
            _deckService = deckService;
            _navigationService = navigationService;

            ReviewDeckCommand = new RelayCommand(ReviewDeckAsync);
            EditDeckCommand = new RelayCommand(EditDeck);
            DeleteDeckCommand = new RelayCommand(DeleteDeckAsync);
            OpenMainMenuCommand = new RelayCommand(OpenMainMenuAsync);
            OpenDeckCreatorCommand = new RelayCommand(OpenDeckCreatorAsync);
            ImportDecksCommand = new RelayCommand(async () => await ImportDecksAsync());
            ExportDecksCommand = new RelayCommand(async () => await ExportDecksAsync());

            LoadDecksAsync();
        }

        private async void LoadDecksAsync()
        {
            var decks = await Task.Run(() => _deckService.GetPagedDecks(1, int.MaxValue).ToList());
            Decks = new ObservableCollection<FlashcardDeck>(decks);
            UpdateFilteredDecks();
        }

        private void UpdateFilteredDecks()
        {
            FilteredDecks.Clear();
            foreach (var deck in Decks.Where(d => string.IsNullOrEmpty(SearchText) || d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredDecks.Add(deck);
            }
        }

        private async void ReviewDeckAsync()
        {
            if (SelectedDeck == null)
            {
                ShowErrorMessage("Please select a deck to review.");
                return;
            }

            await _navigationService.GetFlashcardViewAsync(SelectedDeck);
        }

        private async void EditDeck()
        {
            if (SelectedDeck == null)
            {
                ShowErrorMessage("Please select a deck to edit.");
                return;
            }

            await _navigationService.GetFlashcardDeckEditorViewAsync(SelectedDeck);
        }

        private async void DeleteDeckAsync()
        {
            if (SelectedDeck == null)
            {
                ShowErrorMessage("Please select a deck to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this deck?",
                "Confirm Deletion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _deckService.DeleteDeckAsync(SelectedDeck.Id);
                LoadDecksAsync();
            }
        }

        private async void OpenMainMenuAsync()
        {
            await _navigationService.GetMainMenuViewAsync();
        }

        private async void OpenDeckCreatorAsync()
        {
            await _navigationService.GetFlashcardDeckCreatorViewAsync();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task ExportDecksAsync()
        {
            var decksToExport = SelectedDeck != null ? new List<FlashcardDeck> { SelectedDeck } : Decks.ToList();
            var deckDtos = decksToExport.Select(d => new FlashcardDeckDto
            {
                DeckName = d.Name,
                Flashcards = d.FlashcardFronts.Select(f => new FlashcardDto { Front = f, Back = "" }).ToList(),
                EaseRating = d.EaseRating
            }).ToList();

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                FileName = "FlashcardDecks.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var json = JsonSerializer.Serialize(deckDtos);
                await File.WriteAllTextAsync(saveFileDialog.FileName, json);
            }
        }

        public async Task ImportDecksAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var json = await File.ReadAllTextAsync(openFileDialog.FileName);
                var deckDtos = JsonSerializer.Deserialize<List<FlashcardDeckDto>>(json);

                var existingDecks = _deckService.GetPagedDecks(1, int.MaxValue);
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
}

