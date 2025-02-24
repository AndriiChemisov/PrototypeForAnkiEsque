using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;

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
    }
}

