using System.Windows.Input;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Data;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardViewModel : BaseViewModel
    {
        private readonly FlashcardService _flashcardService;
        private readonly NavigationService _navigationService;
        private readonly MainMenuViewModel _mainMenuViewModel;
        private readonly ApplicationDbContext _dbContext;

        private List<Flashcard> _flashcards;
        private int _currentCardIndex;


        public FlashcardViewModel(FlashcardService flashcardService, NavigationService navigationService, ApplicationDbContext dbContext, MainMenuViewModel mainMenuViewModel)
        {
            _flashcardService = flashcardService;
            _navigationService = navigationService;
            _mainMenuViewModel = mainMenuViewModel;
            _dbContext = dbContext;

            // Initialize commands
            ShowAnswerCommand = new RelayCommand(ShowAnswer);
            NextCommand = new RelayCommand(NextCard);
            EaseCommand = new RelayCommand<string>(SetEase);
            OpenMainMenuCommand = new RelayCommand(OpenMainMenu);
            OpenFlashcardEntryCommand = new RelayCommand(OpenFlashcardEntry);

            // Load all flashcards
            _flashcards = _flashcardService.GetFlashcards();

            // Initialize current card index to start from the first flashcard
            _currentCardIndex = 0;

            // Load the first flashcard
            LoadCurrentCard();
        }


        public ICommand ShowAnswerCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand EaseCommand { get; }
        public ICommand OpenMainMenuCommand { get; }
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
            // Reset the rating state before going to the next card
            IsRatingClicked = false;
            IsAnswerVisible = false;

            // Check if we've reached the end of the flashcards
            if (_currentCardIndex >= _flashcards.Count - 1)
            {
                // Hide content before triggering the animation
                IsGridVisible = !IsGridVisible;

                MotivationalMessage = "Well done - you finished the deck!";

                // Trigger the fade-out animation
                TriggerFadeOutAnimationForMotivationalMessage();

                // Wait for the animation to complete before navigating
                await Task.Delay(2000);  // Match the animation duration (2 seconds)

                // Navigate to the MainMenuView when animation completes
                await _navigationService.GetMainMenuViewAsync();
                return;
            }

            // Move to the next card (increment index)
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

        private void SetEase(string easeString)
        {
            int ease = Int32.Parse(easeString);
            if (CurrentCard == null)
                return;

            // Update ease rating in the database
            CurrentCard.EaseRating = ease;
            _flashcardService.UpdateFlashcard(CurrentCard); // Update flashcard using the service

            // Provide a message based on the ease rating
            RatingMessage = ease switch
            {
                0 => "Great! You're mastering this card.",
                1 => "Good! You're doing well.",
                2 => "Keep going! You'll get it soon.",
                _ => "Keep practicing!",
            };

            // Mark rating as clicked, disabling the buttons until next card
            IsRatingClicked = true;

            //Trigger fadeout animation for the message
            TriggerFadeOutAnimation(); 
        }

        private void TriggerFadeOutAnimation()
        {
            // This calls the animation from the view
            OnFadeOutMessage?.Invoke();
        }

        private void TriggerFadeOutAnimationForMotivationalMessage()
        {
            OnFadeoutMotivationalMessage?.Invoke();
        }

        // Event for view to subscribe and trigger animation
        public event Action OnFadeOutMessage;
        public event Action OnFadeoutMotivationalMessage;

        private async void OpenMainMenu()
        {
            await _navigationService.GetMainMenuViewAsync();
        }

        private async void OpenFlashcardEntry()
        {
            await _navigationService.GetFlashcardEntryViewAsync();
        }
    }
}
