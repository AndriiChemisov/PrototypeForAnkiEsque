using PrototypeForAnkiEsque.Commands;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using PrototypeForAnkiEsque.Resources;
using System.Windows.Input;

public class MainMenuViewModel : BaseViewModel
{
    #region FIELD DECLARATIONS
    private readonly IFlashcardNavigationService _flashcardNavigationService;
    private readonly IDeckNavigationService _deckNavigationService;
    private readonly ILocalizationService _localizationService;
    private string _allFlashcardsButtonContent;
    private string _selectDeckButtonContent;

    #endregion

    #region CONSTRUCTOR
    public MainMenuViewModel(IFlashcardNavigationService flashcardNavigationService,
                             IDeckNavigationService deckNavigationService,
                             ILocalizationService localizationService)
    {
        _flashcardNavigationService = flashcardNavigationService;
        _deckNavigationService = deckNavigationService;
        _localizationService = localizationService;

        OpenFlashcardDatabaseViewCommand = new AsyncRelayCommand(OpenFlashcardDatabaseViewAsync);
        OpenFlashcardDeckSelectionViewCommand = new AsyncRelayCommand(OpenFlashcardDeckSelectionViewAsync);
        ChangeLanguageCommand = new AsyncRelayCommand<string>(culture => Task.Run(() => ChangeLanguage(culture)));
        UpdateLocalizedTexts();
    }
    #endregion

    #region COMMANDS
    public ICommand OpenFlashcardDatabaseViewCommand { get; }
    public ICommand OpenFlashcardDeckSelectionViewCommand { get; }

    // Commands for language change
    public ICommand ChangeLanguageCommand { get; }  // Add command to change language
    #endregion

    #region PROPERTIES
    public string AllFlashcardsButtonContent
    {
        get => _allFlashcardsButtonContent;
        set => SetProperty(ref _allFlashcardsButtonContent, value);
    }

    public string SelectDeckButtonContent
    {
        get => _selectDeckButtonContent;
        set => SetProperty(ref _selectDeckButtonContent, value);
    }
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

    // Language changing method
    private void ChangeLanguage(string culture)
    {
        _localizationService.ChangeLanguage(culture);
        UpdateLocalizedTexts();
    }

    private void UpdateLocalizedTexts()
    {
        AllFlashcardsButtonContent = Strings.BttnAllFlashcards;
        SelectDeckButtonContent = Strings.BttnSelectDeck;
    }
    #endregion
}
