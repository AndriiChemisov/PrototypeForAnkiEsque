using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using System.Threading.Tasks;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckSelectionViewModel : BaseViewModel
    {
        private readonly DeckService _deckService;
        private readonly Services.NavigationService _navigationService;
        private FlashcardDeck _selectedDeck;
        private string _errorMessage;

        public ObservableCollection<FlashcardDeck> Decks { get; set; }

        public ICommand ReviewDeckCommand { get; }
        public ICommand EditDeckCommand { get; }
        public ICommand DeleteDeckCommand { get; }
        public ICommand OpenMainMenuCommand { get; }

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

        public FlashcardDeckSelectionViewModel(DeckService deckService, Services.NavigationService navigationService)
        {
            _deckService = deckService;
            _navigationService = navigationService;

            ReviewDeckCommand = new RelayCommand(ReviewDeck);
            EditDeckCommand = new RelayCommand(EditDeck);
            DeleteDeckCommand = new RelayCommand(DeleteDeck);
            OpenMainMenuCommand = new RelayCommand(OpenMainMenu);

            LoadDecks();
        }

        private void LoadDecks()
        {
            Decks = new ObservableCollection<FlashcardDeck>(_deckService.GetAllDecks());
        }

        private async void ReviewDeck()
        {
            if (SelectedDeck == null)
            {
                ShowErrorMessage("Please select a deck to review.");
                return;
            }

            // Proceed with reviewing the selected deck
            // Navigate to FlashcardView and pass the selected deck
            await _navigationService.GetFlashcardViewAsync(SelectedDeck);
        }

        private void EditDeck()
        {
            if (SelectedDeck == null)
            {
                ShowErrorMessage("Please select a deck to edit.");
                return;
            }

            // Proceed with editing the selected deck
            // Navigate to DeckEditorView and pass the selected deck
        }

        private async void DeleteDeck()
        {
            if (SelectedDeck == null)
            {
                ShowErrorMessage("Please select a deck to delete.");
                return;
            }

            // Confirm deletion
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this deck?", "Confirm Deletion", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Proceed with deck deletion
                await _deckService.DeleteDeckAsync(SelectedDeck.Id);
                LoadDecks(); // Reload deck list after deletion
                OnPropertyChanged(nameof(Decks)); // This should force the UI to update
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void OpenMainMenu()
        {
            await _navigationService.GetMainMenuViewAsync();
        }
    }
}
