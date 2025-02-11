using System.Windows;
using System.Windows.Input;
using PrototypeForAnkiEsque.Services;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private string _motivationalMessage;
        private Visibility _messageVisibility;

        public MainMenuViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
            _motivationalMessage = "Well done - keep going!";
            _messageVisibility = Visibility.Collapsed;

            // Commands for the buttons
            OpenFlashcardViewCommand = new RelayCommand(() => OpenFlashcardView());
            OpenFlashcardEntryViewCommand = new RelayCommand(() => OpenFlashcardEntryView());
        }

        public ICommand OpenFlashcardViewCommand { get; }
        public ICommand OpenFlashcardEntryViewCommand { get; }

        public string MotivationalMessage
        {
            get => _motivationalMessage;
            set
            {
                _motivationalMessage = value;
                OnPropertyChanged(nameof(MotivationalMessage)); // Notify the UI of changes
            }
        }

        public Visibility MessageVisibility
        {
            get => _messageVisibility;
            set
            {
                _messageVisibility = value;
                OnPropertyChanged(nameof(MessageVisibility));
            }
        }

        public void UpdateMessageVisibility(Visibility visibility) 
        {
            MessageVisibility = visibility;
        }

        // Navigate to Flashcard view
        private void OpenFlashcardView()
        {
            _navigationService.GetFlashcardView(); // Get Flashcard view
        }

        // Navigate to Settings view
        private void OpenFlashcardEntryView()
        {
            _navigationService.GetFlashcardEntryView(); // Get Settings view
        }

        private void TriggerFadeOutAnimation()
        {
            // This calls the animation from the view
            OnFadeOutMessage?.Invoke();
        }

        // Event for view to subscribe and trigger animation
        public event Action OnFadeOutMessage;
    }
}
