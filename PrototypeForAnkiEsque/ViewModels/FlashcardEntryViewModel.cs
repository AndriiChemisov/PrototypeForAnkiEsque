using System.Windows.Input;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using System.Threading.Tasks;
using System.Windows;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardEntryViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly NavigationService _navigationService;
        private string _front;
        private string _back;
        private string _savedMessage;
        private Visibility _isSavedMessageVisible;

        public FlashcardEntryViewModel(ApplicationDbContext dbContext, NavigationService navigationService)
        {
            _dbContext = dbContext;
            _navigationService = navigationService;
            SaveFlashcardCommand = new RelayCommand(SaveFlashcard);
            OpenMainMenuCommand = new RelayCommand(OpenMainMenu);
            OpenFlashcardViewCommand = new RelayCommand(OpenFlashcardView);
            IsSavedMessageVisible = Visibility.Collapsed; // Initially hide the saved message
        }

        public string Front
        {
            get => _front;
            set
            {
                _front = value;
                OnPropertyChanged();
            }
        }

        public string Back
        {
            get => _back;
            set
            {
                _back = value;
                OnPropertyChanged();
            }
        }

        public string SavedMessage
        {
            get => _savedMessage;
            set
            {
                _savedMessage = value;
                OnPropertyChanged();
            }
        }

        public Visibility IsSavedMessageVisible
        {
            get => _isSavedMessageVisible;
            set
            {
                _isSavedMessageVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveFlashcardCommand { get; }
        public ICommand OpenMainMenuCommand { get; }
        public ICommand OpenFlashcardViewCommand { get; }

        private async void SaveFlashcard()
        {

            if (string.IsNullOrWhiteSpace(Front) || string.IsNullOrWhiteSpace(Back))
            {
                // Show alert if front or back are blank
                MessageBox.Show("The flashcard cannot be blank!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var flashcard = new Flashcard
            {
                Front = Front,
                Back = Back
            };

            _dbContext.Flashcards.Add(flashcard);
            _dbContext.SaveChanges();

            // Clear fields after saving
            Front = string.Empty;
            Back = string.Empty;

            // Show confirmation message
            SavedMessage = "Flashcard saved successfully!";
            IsSavedMessageVisible = Visibility.Visible;

            // Start the fade-out animation
            TriggerFadeOutAnimation();
        }

        private void TriggerFadeOutAnimation()
        {
            // This calls the animation from the view
            OnFadeOutMessage?.Invoke();
        }

        // Event for view to subscribe and trigger animation
        public event Action OnFadeOutMessage;

        private async void OpenMainMenu()
        {
            await _navigationService.GetMainMenuViewAsync();
        }

        private async void OpenFlashcardView()
        {
            await _navigationService.GetFlashcardViewAsync();
        }
    }
}
