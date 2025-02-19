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
            OpenFlashcardEntryViewCommand = new RelayCommand(() => OpenFlashcardEntryView());
            OpenFlashcardDatabaseViewCommand = new RelayCommand(() => OpenFlashcardDatabaseView());
            OpenFlashcardDeckCreatorViewCommand = new RelayCommand(() => OpenFlashcardDeckCreatorView());
            OpenFlashcardDeckSelectionViewCommand = new RelayCommand(() => OpenFlashcardDeckSelectionView());

        }

        public ICommand OpenFlashcardEntryViewCommand { get; }
        public ICommand OpenFlashcardDatabaseViewCommand { get; }
        public ICommand OpenFlashcardDeckCreatorViewCommand { get; }
        public ICommand OpenFlashcardDeckSelectionViewCommand { get; }

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

        private async void OpenFlashcardDeckSelectionView()
        {
            await _navigationService.GetFlashcardDeckSelectionViewAsync();
        }
    }
}
