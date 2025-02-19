using System.Windows;
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
            OpenFlashcardDatabaseViewCommand = new RelayCommand(() => OpenFlashcardDatabaseView());
            OpenFlashcardDeckCreatorViewCommand = new RelayCommand(() => OpenFlashcardDeckCreatorView());

        }

        public ICommand OpenFlashcardViewCommand { get; }
        public ICommand OpenFlashcardEntryViewCommand { get; }
        public ICommand OpenFlashcardDatabaseViewCommand { get; }
        public ICommand OpenFlashcardDeckCreatorViewCommand { get; }


        // Navigate to Flashcard view
        private async void OpenFlashcardView()
        {
            await _navigationService.GetFlashcardViewAsync(); // Get Flashcard view
        }

        // Navigate to Settings view
        private async void OpenFlashcardEntryView()
        {
            await _navigationService.GetFlashcardEntryViewAsync(); // Get Settings view
        }

        private async void OpenFlashcardDatabaseView()
        {
            await _navigationService.GetFlashcardDatabaseViewAsync();
        }

        private async void OpenFlashcardDeckCreatorView()
        {
            await _navigationService.GetFlashcardDeckCreatorViewAsync();
        }
    }
}
