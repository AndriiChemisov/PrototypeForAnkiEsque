using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using System.Windows.Input;
// This file is used to define the FlashcardViewModel class, which inherits from the BaseViewModel class.
// The FlashcardViewModel class defines a constructor that takes instances of the IFlashcardService, IMainMenuNavigationService, IDeckNavigationService, and IDeckService interfaces as parameters.
// This allows the FlashcardViewModel class to interact with the FlashcardService, MainMenuNavigationService, DeckNavigationService, and DeckService classes.
// Simple explanation: This class is used to handle the logic for the FlashcardViewUserControl view.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly IFlashcardService _flashcardService;
        private readonly IMainMenuNavigationService _mainMenuNavigationService;
        private readonly IDeckNavigationService _deckNavigationService;
        private readonly IDeckService _deckService;

        private List<Flashcard> _flashcards;
        private int _currentCardIndex;
        private Flashcard _currentCard;
        private FlashcardDeck _selectedDeck;
        private string _ratingMessage;
        private string _motivationalMessage;
        private bool _isRatingClicked;
        private bool _isAnswerVisible = false;
        private bool _isGridVisible = true;
        #endregion

        #region CONSTRUCTOR
        public FlashcardViewModel(IFlashcardService flashcardService, IMainMenuNavigationService mainMenuNavigationService, IDeckNavigationService deckNavigationService, IDeckService deckService, MainMenuViewModel mainMenuViewModel)
        {
            _flashcardService = flashcardService;
            _mainMenuNavigationService = mainMenuNavigationService;
            _deckNavigationService = deckNavigationService;
            _deckService = deckService;

            ShowAnswerCommand = new AsyncRelayCommand(ShowAnswerAsync);
            NextCommand = new AsyncRelayCommand(NextCardAsync);
            EaseCommand = new AsyncRelayCommand<string>(SetEaseAsync);
            OpenDeckSelectionCommand = new AsyncRelayCommand(OpenDeckSelectionAsync);

            _flashcards = new List<Flashcard>(); // Initialize as empty
            _currentCardIndex = 0;
        }
        #endregion

        #region PROPERTIES
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

        public Flashcard CurrentCard
        {
            get => _currentCard;
            set => SetProperty(ref _currentCard, value);
        }

        public string RatingMessage
        {
            get => _ratingMessage;
            set => SetProperty(ref _ratingMessage, value);
        }

        public string MotivationalMessage
        {
            get => _motivationalMessage;
            set => SetProperty(ref _motivationalMessage, value);
        }

        public bool IsRatingClicked
        {
            get => _isRatingClicked;
            set => SetProperty(ref _isRatingClicked, value);
        }

        public bool IsAnswerVisible
        {
            get => _isAnswerVisible;
            set => SetProperty(ref _isAnswerVisible, value);
        }

        public bool IsGridVisible
        {
            get => _isGridVisible;
            set => SetProperty(ref _isGridVisible, value);
        }
        #endregion

        #region COMMANDS
        public ICommand ShowAnswerCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand EaseCommand { get; }
        public ICommand OpenDeckSelectionCommand { get; }
        #endregion  

        #region METHODS
        private async Task ShowAnswerAsync()
        {
            IsAnswerVisible = !IsAnswerVisible;
        }

        private async Task NextCardAsync()
        {
            IsRatingClicked = false;
            IsAnswerVisible = false;

            if (_currentCardIndex >= _flashcards.Count - 1)
            {
                IsGridVisible = !IsGridVisible;
                SelectedDeck.EaseRating = await _deckService.CalculateEaseRatingAsync(SelectedDeck.FlashcardFronts);
                await _deckService.UpdateDeckAsync(SelectedDeck);

                MotivationalMessage = "Well done - you finished the deck!";
                TriggerFadeOutAnimationForMotivationalMessage();
                await Task.Delay(2000);
                await _mainMenuNavigationService.GetMainMenuViewAsync();
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

        private async Task LoadFlashcardsFromDeck()
        {
            if (SelectedDeck != null)
            {
                // Assuming FlashcardService can handle loading by deck
                _flashcards = await _flashcardService.GetFlashcardsByDeckAsync(SelectedDeck.Id);

                // Shuffle with ease bias (higher ease rating => more likely to appear first)
                _flashcards = _flashcards
                    .OrderByDescending(f => f.EaseRating) // Prioritize higher ease
                    .ThenBy(f => Guid.NewGuid()) // Randomize within the same ease group
                    .ToList();

                _currentCardIndex = 0;
                LoadCurrentCard();
            }
        }


        private async Task SetEaseAsync(string easeString)
        {
            int ease = Int32.Parse(easeString);
            if (CurrentCard == null)
                return;

            CurrentCard.EaseRating = ease;
            await _flashcardService.UpdateFlashcardAsync(CurrentCard);

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

        private async Task OpenDeckSelectionAsync()
        {
            await _deckNavigationService.GetFlashcardDeckSelectionViewAsync();
        }

        public void SetSelectedDeck(FlashcardDeck selectedDeck)
        {
            SelectedDeck = selectedDeck;
        }
        #endregion
    }
}