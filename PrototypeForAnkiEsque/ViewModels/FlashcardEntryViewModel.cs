using System.Windows.Input;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        private async void SaveFlashcard()
        {
            //Ensure that the flashcard is not blank
            if (string.IsNullOrWhiteSpace(Front) || string.IsNullOrWhiteSpace(Back))
            {
                // Show alert if front or back are blank
                MessageBox.Show("The flashcard cannot have blank sides!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check for duplicates (case insensitive due to how EntityFramework works)
            var existingFlashcard = await _dbContext.Flashcards
                                .FirstOrDefaultAsync(f => f.Front.ToLower() == Front.ToLower());

            if (existingFlashcard != null)
            {
                // If a duplicate is found, show a warning message
                MessageBox.Show("This flashcard already exists!", "Duplicate Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

    }
}
