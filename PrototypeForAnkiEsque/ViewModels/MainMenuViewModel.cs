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
            OpenFlashcardDatabaseViewCommand = new AsyncRelayCommand(OpenFlashcardDatabaseViewAsync);
            OpenFlashcardDeckSelectionViewCommand = new AsyncRelayCommand(OpenFlashcardDeckSelectionViewAsync);

        }

        public ICommand OpenFlashcardDatabaseViewCommand { get; }
        public ICommand OpenFlashcardDeckSelectionViewCommand { get; }

        private async Task OpenFlashcardDatabaseViewAsync()
        {
            await _navigationService.GetFlashcardDatabaseViewAsync();
        }

        private async Task OpenFlashcardDeckSelectionViewAsync()
        {
            await _navigationService.GetFlashcardDeckSelectionViewAsync();
        }
    }
}
