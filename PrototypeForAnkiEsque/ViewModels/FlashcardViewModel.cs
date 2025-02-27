using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using System.Windows.Input;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardViewModel : BaseViewModel
    {
        private readonly IFlashcardService _flashcardService;
        private readonly IMainMenuNavigationService _mainMenuNavigationService;
        private readonly IDeckNavigationService _deckNavigationService;
        private readonly IDeckService _deckService;

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

        public ICommand ShowAnswerCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand EaseCommand { get; }
        public ICommand OpenDeckSelectionCommand { get; }

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

        // Method to set the selected deck when passed from navigation
        public void SetSelectedDeck(FlashcardDeck selectedDeck)
        {
            SelectedDeck = selectedDeck; // This triggers the loading of the flashcards from the deck
        }
    }

}