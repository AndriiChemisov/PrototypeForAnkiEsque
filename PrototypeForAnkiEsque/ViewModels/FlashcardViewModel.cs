using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using PrototypeForAnkiEsque.Resources;
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
        private readonly IDeckNavigationService _deckNavigationService;
        private readonly IDeckService _deckService;

        private List<Flashcard> _flashcards;
        private int _currentCardIndex;
        private Flashcard _currentCard;
        private string _formattedBack;
        private FlashcardDeck _selectedDeck;
        private string _ratingMessage;
        private string _motivationalMessage;
        private bool _isRatingClicked;
        private bool _isAnswerVisible = false;
        private bool _isGridVisible = true;
        private bool _isMotivationalMessageVisible = false;
        private string _backButtonContext;
        private string _flipButtonContext;
        private string _nextButtonContext;
        private string _easeButtonEasyContext;
        private string _easeButtonGoodContext;
        private string _easeButtonHardContext;
        private string _easeRatingEasyMessageContext;
        private string _easeRatingGoodMessageContext;
        private string _easeRatingHardMessageContext;
        #endregion

        #region CONSTRUCTOR
        public FlashcardViewModel(IFlashcardService flashcardService, IDeckNavigationService deckNavigationService, IDeckService deckService, MainMenuViewModel mainMenuViewModel)
        {
            _flashcardService = flashcardService;
            _deckNavigationService = deckNavigationService;
            _deckService = deckService;

            ShowAnswerCommand = new AsyncRelayCommand(ShowAnswerAsync);
            NextCommand = new AsyncRelayCommand(NextCardAsync);
            EaseCommand = new AsyncRelayCommand<string>(SetEaseAsync);
            OpenDeckSelectionCommand = new AsyncRelayCommand(OpenDeckSelectionAsync);

            _flashcards = new List<Flashcard>(); // Initialize as empty
            _currentCardIndex = 0;
            LoadLocalizedTexts();
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

        public bool IsMotivationalMessageVisible
        {
            get => _isMotivationalMessageVisible;
            set => SetProperty(ref _isMotivationalMessageVisible, value);
        }
        public string FormattedBack
        {
            get => _formattedBack;
            set
            {
                if (_formattedBack != value)
                {
                    _formattedBack = value.Replace(" ", "\n");  // Automatically replace spaces with new lines
                    OnPropertyChanged(nameof(FormattedBack));  // Notify property changed
                }
            }
        }

        public string BackButtonContext
        {
            get => _backButtonContext;
            set => SetProperty(ref _backButtonContext, value);
        }

        public string NextButtonContext
        {
            get => _nextButtonContext;
            set => SetProperty(ref _nextButtonContext, value);
        }

        public string FlipButtonContext
        {
            get => _flipButtonContext;
            set => SetProperty(ref _flipButtonContext, value);
        }

        public string EaseButtonEasyContext
        {
            get => _easeButtonEasyContext;
            set => SetProperty(ref _easeButtonEasyContext, value);
        }

        public string EaseButtonGoodContext
        {
            get => _easeButtonGoodContext;
            set => SetProperty(ref _easeButtonGoodContext, value);
        }

        public string EaseButtonHardContext
        {
            get => _easeButtonHardContext;
            set => SetProperty(ref _easeButtonHardContext, value);
        }

        public string EaseRatingEasyMessageContext
        {
            get => _easeRatingEasyMessageContext;
            set => SetProperty(ref _easeRatingEasyMessageContext, value);
        }

        public string EaseRatingGoodMessageContext
        {
            get => _easeRatingGoodMessageContext;
            set => SetProperty(ref _easeRatingGoodMessageContext, value);
        }

        public string EaseRatingHardMessageContext
        {
            get => _easeRatingHardMessageContext;
            set => SetProperty(ref _easeRatingHardMessageContext, value);
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

                IsMotivationalMessageVisible = true;
                TriggerFadeOutAnimationForMotivationalMessage();
                await Task.Delay(2000);
                await _deckNavigationService.GetFlashcardDeckSelectionViewAsync();
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
                FormattedBack = CurrentCard.Back;
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
                0 => EaseButtonEasyContext,
                1 => EaseButtonGoodContext,
                2 => EaseButtonHardContext,
                _ => "Error - no value received"
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

        private void LoadLocalizedTexts()
        {
            BackButtonContext = Strings.BttnBack;
            NextButtonContext = Strings.BttnNext;
            FlipButtonContext = Strings.BttnFlip;
            EaseButtonEasyContext = Strings.BttnEaseEasy;
            EaseButtonGoodContext = Strings.BttnEaseGood;
            EaseButtonHardContext = Strings.BttnEaseHard;
            MotivationalMessage = Strings.MssgMotivationalMessage;
        }
        #endregion
    }
}