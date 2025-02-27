using System.Windows.Input;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;


namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly IFlashcardNavigationService _flashcardNavigationService;
        private readonly IDeckNavigationService _deckNavigationService;

        public MainMenuViewModel(IFlashcardNavigationService flashcardNavigationService, IDeckNavigationService deckNavigationService)
        {
            _flashcardNavigationService = flashcardNavigationService;
            _deckNavigationService = deckNavigationService;


            // Commands for the buttons
            OpenFlashcardDatabaseViewCommand = new AsyncRelayCommand(OpenFlashcardDatabaseViewAsync);
            OpenFlashcardDeckSelectionViewCommand = new AsyncRelayCommand(OpenFlashcardDeckSelectionViewAsync);

        }

        public ICommand OpenFlashcardDatabaseViewCommand { get; }
        public ICommand OpenFlashcardDeckSelectionViewCommand { get; }

        private async Task OpenFlashcardDatabaseViewAsync()
        {
            await _flashcardNavigationService.GetFlashcardDatabaseViewAsync();
        }

        private async Task OpenFlashcardDeckSelectionViewAsync()
        {
            await _deckNavigationService.GetFlashcardDeckSelectionViewAsync();
        }
    }
}
