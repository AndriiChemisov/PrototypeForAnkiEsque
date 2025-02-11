using System.Windows.Input;
using PrototypeForAnkiEsque.Services;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;

        public MainMenuViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;

            // Commands for the buttons
            OpenFlashcardViewCommand = new RelayCommand(() => OpenFlashcardView());
            OpenFlashcardEntryViewCommand = new RelayCommand(() => OpenFlashcardEntryView());
        }

        public ICommand OpenFlashcardViewCommand { get; }
        public ICommand OpenFlashcardEntryViewCommand { get; }

        // Navigate to Flashcard view
        private void OpenFlashcardView()
        {
            _navigationService.GetFlashcardView(); // Get Flashcard view
        }

        // Navigate to Settings view
        private void OpenFlashcardEntryView()
        {
            _navigationService.GetFlashcardEntryView(); // Get Settings view
        }
    }
}
