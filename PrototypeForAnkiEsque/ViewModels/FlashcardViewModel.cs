using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Windows.Input;

public class FlashcardViewModel : BaseViewModel
{
    private readonly FlashcardService _flashcardService;
    private readonly NavigationService _navigationService;
    private readonly DeckService _deckService;
    private readonly MainMenuViewModel _mainMenuViewModel;

    private List<Flashcard> _flashcards;
    private int _currentCardIndex;

    private FlashcardDeck _selectedDeck;
    public FlashcardDeck SelectedDeck
    {
        get => _selectedDeck;
        set
        {
            if (SetProperty(ref _selectedDeck, value))
            {
                LoadFlashcardsFromDeck();
            }
        }
    }

    public FlashcardViewModel(FlashcardService flashcardService, NavigationService navigationService, DeckService deckService, MainMenuViewModel mainMenuViewModel)
    {
        _flashcardService = flashcardService;
        _navigationService = navigationService;
        _mainMenuViewModel = mainMenuViewModel;
        _deckService = deckService;

        ShowAnswerCommand = new RelayCommand(ShowAnswer);
        NextCommand = new RelayCommand(NextCard);
        EaseCommand = new RelayCommand<string>(SetEase);
        OpenFlashcardEntryCommand = new RelayCommand(OpenFlashcardEntry);

        _flashcards = new List<Flashcard>(); // Initialize as empty
        _currentCardIndex = 0;
    }

    public ICommand ShowAnswerCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand EaseCommand { get; }
    public ICommand OpenFlashcardEntryCommand { get; }

    private Flashcard _currentCard;
    public Flashcard CurrentCard
    {
        get => _currentCard;
        set => SetProperty(ref _currentCard, value);
    }

    private string _ratingMessage;
    public string RatingMessage
    {
        get => _ratingMessage;
        set => SetProperty(ref _ratingMessage, value);
    }

    private string _motivationalMessage;
    public string MotivationalMessage
    {
        get => _motivationalMessage;
        set => SetProperty(ref _motivationalMessage, value);
    }

    private bool _isRatingClicked;
    public bool IsRatingClicked
    {
        get => _isRatingClicked;
        set => SetProperty(ref _isRatingClicked, value);
    }

    private bool _isAnswerVisible = false;
    public bool IsAnswerVisible
    {
        get => _isAnswerVisible;
        set => SetProperty(ref _isAnswerVisible, value);
    }

    private bool _isGridVisible = true;
    public bool IsGridVisible
    {
        get => _isGridVisible;
        set => SetProperty(ref _isGridVisible, value);
    }

    private void ShowAnswer()
    {
        IsAnswerVisible = !IsAnswerVisible;
    }

    private async void NextCard()
    {
        IsRatingClicked = false;
        IsAnswerVisible = false;

        if (_currentCardIndex >= _flashcards.Count - 1)
        {
            IsGridVisible = !IsGridVisible;
            SelectedDeck.EaseRating = _deckService.CalculateEaseRating(SelectedDeck.FlashcardIds);
            await _deckService.UpdateDeckAsync(SelectedDeck);

            MotivationalMessage = "Well done - you finished the deck!";
            TriggerFadeOutAnimationForMotivationalMessage();
            await Task.Delay(2000);
            await _navigationService.GetMainMenuViewAsync();
            return;
        }

        _currentCardIndex++;
        LoadCurrentCard();
    }

    private void LoadCurrentCard()
    {
        if (_flashcards.Any())
        {
            CurrentCard = _flashcards[_currentCardIndex];
        }
    }

    private void LoadFlashcardsFromDeck()
    {
        if (SelectedDeck != null)
        {
            // Assuming FlashcardService can handle loading by deck
            _flashcards = _flashcardService.GetFlashcardsByDeck(SelectedDeck.Id);

            // Shuffle with ease bias (higher ease rating => more likely to appear first)
            _flashcards = _flashcards
                .OrderByDescending(f => f.EaseRating) // Prioritize higher ease
                .ThenBy(f => Guid.NewGuid()) // Randomize within the same ease group
                .ToList();

            _currentCardIndex = 0;
            LoadCurrentCard();
        }
    }


    private void SetEase(string easeString)
    {
        int ease = Int32.Parse(easeString);
        if (CurrentCard == null)
            return;

        CurrentCard.EaseRating = ease;
        _flashcardService.UpdateFlashcard(CurrentCard);

        RatingMessage = ease switch
        {
            0 => "Great! You're mastering this card.",
            1 => "Good! You're doing well.",
            2 => "Keep going! You'll get it soon.",
            _ => "Keep practicing!",
        };

        IsRatingClicked = true;
        TriggerFadeOutAnimation();
    }

    private void TriggerFadeOutAnimation()
    {
        OnFadeOutMessage?.Invoke();
    }

    private void TriggerFadeOutAnimationForMotivationalMessage()
    {
        OnFadeoutMotivationalMessage?.Invoke();
    }

    public event Action OnFadeOutMessage;
    public event Action OnFadeoutMotivationalMessage;

    private async void OpenFlashcardEntry()
    {
        await _navigationService.GetFlashcardEntryViewAsync();
    }

    // Method to set the selected deck when passed from navigation
    public void SetSelectedDeck(FlashcardDeck selectedDeck)
    {
        SelectedDeck = selectedDeck; // This triggers the loading of the flashcards from the deck
    }
}
