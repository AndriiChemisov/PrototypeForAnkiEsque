using System.Windows.Input;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;


namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public MainMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;


            // Commands for the buttons
            OpenFlashcardDatabaseViewCommand = new RelayCommand(() => OpenFlashcardDatabaseView());
            OpenFlashcardDeckSelectionViewCommand = new RelayCommand(() => OpenFlashcardDeckSelectionView());

        }

        public ICommand OpenFlashcardDatabaseViewCommand { get; }
        public ICommand OpenFlashcardDeckSelectionViewCommand { get; }

        private async void OpenFlashcardDatabaseView()
        {
            await _navigationService.GetFlashcardDatabaseViewAsync();
        }

        private async void OpenFlashcardDeckSelectionView()
        {
            await _navigationService.GetFlashcardDeckSelectionViewAsync();
        }
    }
}
