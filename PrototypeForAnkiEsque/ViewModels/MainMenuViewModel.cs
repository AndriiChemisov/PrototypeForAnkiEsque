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
        }

        public ICommand OpenFlashcardViewCommand { get; }
        public ICommand OpenFlashcardEntryViewCommand { get; }

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

    }
}
