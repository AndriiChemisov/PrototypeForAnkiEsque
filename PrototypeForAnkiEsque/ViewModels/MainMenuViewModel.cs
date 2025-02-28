using System.Windows.Input;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
// This file is used to define the MainMenuViewModel class, which inherits from the BaseViewModel class.
// The MainMenuViewModel class is used to define the view model for the main menu view.
// This class, mainly, exists to be a starting point of the applications and to navigate to the most prominent views in the application.
// Simple explanation: This class is used to define the view model for the main menu view.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly IFlashcardNavigationService _flashcardNavigationService;
        private readonly IDeckNavigationService _deckNavigationService;
        #endregion

        #region CONSTUCTOR
        public MainMenuViewModel(IFlashcardNavigationService flashcardNavigationService, IDeckNavigationService deckNavigationService)
        {
            _flashcardNavigationService = flashcardNavigationService;
            _deckNavigationService = deckNavigationService;

            OpenFlashcardDatabaseViewCommand = new AsyncRelayCommand(OpenFlashcardDatabaseViewAsync);
            OpenFlashcardDeckSelectionViewCommand = new AsyncRelayCommand(OpenFlashcardDeckSelectionViewAsync);
        }
        #endregion

        #region COMMANDS
        public ICommand OpenFlashcardDatabaseViewCommand { get; }
        public ICommand OpenFlashcardDeckSelectionViewCommand { get; }
        #endregion

        #region METHODS
        private async Task OpenFlashcardDatabaseViewAsync()
        {
            await _flashcardNavigationService.GetFlashcardDatabaseViewAsync();
        }

        private async Task OpenFlashcardDeckSelectionViewAsync()
        {
            await _deckNavigationService.GetFlashcardDeckSelectionViewAsync();
        }
        #endregion
    }
}
