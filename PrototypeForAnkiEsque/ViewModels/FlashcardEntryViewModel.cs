using System.Windows.Input;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using Microsoft.EntityFrameworkCore;
// This file is used to define the FlashcardEntryViewModel class. The FlashcardEntryViewModel class is used to handle the logic for the FlashcardEntryUserControl view.
// The FlashcardEntryViewModel class defines properties for the front and back of the flashcard, as well as a saved message and a boolean to determine if the saved message is visible.
// The FlashcardEntryViewModel class also defines commands for saving a flashcard and opening the flashcard database view.
// Simple explanation: This class is used to handle the logic for the FlashcardEntryUserControl view.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardEntryViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly ApplicationDbContext _dbContext;
        private readonly IFlashcardNavigationService _flashcardNavigationService;
        private string _front;
        private string _back;
        private string _savedMessage;
        private bool _isSavedMessageVisible;
        #endregion

        #region CONSTRUCTOR
        public FlashcardEntryViewModel(ApplicationDbContext dbContext, IFlashcardNavigationService flashcardNavigationService)
        {
            _dbContext = dbContext;
            _flashcardNavigationService = flashcardNavigationService;
            SaveFlashcardCommand = new AsyncRelayCommand(SaveFlashcardAsync);
            OpenFlashcardDatabaseCommand = new AsyncRelayCommand(OpenFlashcardDatabaseAsync);
            IsSavedMessageVisible = false;
        }
        #endregion

        #region EVENTS
        public event Action<string> OnValidationError;
        public event Action OnFadeOutMessage;
        #endregion

        #region PROPERTIES
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

        public bool IsSavedMessageVisible
        {
            get => _isSavedMessageVisible;
            set
            {
                _isSavedMessageVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region COMMANDS
        public ICommand SaveFlashcardCommand { get; }
        public ICommand OpenFlashcardDatabaseCommand { get; }
        #endregion

        #region METHODS
        private async Task SaveFlashcardAsync()
        {
            if (string.IsNullOrWhiteSpace(Front) || string.IsNullOrWhiteSpace(Back))
            {
                OnValidationError?.Invoke("The flashcard cannot have blank sides!");
                return;
            }

            var existingFlashcard = await _dbContext.Flashcards
                .FirstOrDefaultAsync(f => f.Front.ToLower() == Front.ToLower());

            if (existingFlashcard != null)
            {
                OnValidationError?.Invoke("This flashcard already exists!");
                return;
            }

            var flashcard = new Flashcard
            {
                Front = Front,
                Back = Back,
                EaseRating = 2
            };

            try
            {

                _dbContext.Flashcards.Add(flashcard);
                _dbContext.SaveChanges();

                Front = string.Empty;
                Back = string.Empty;

                SavedMessage = "Flashcard saved successfully!";
                IsSavedMessageVisible = true;

                TriggerFadeOutAnimation();
            }

            catch (Exception ex)
            {
                OnValidationError?.Invoke($"An error occurred while saving the flashcard: {ex.Message}");
            }
        }

        private void TriggerFadeOutAnimation()
        {
            OnFadeOutMessage?.Invoke();
        }


        private async Task OpenFlashcardDatabaseAsync()
        {
            await _flashcardNavigationService.GetFlashcardDatabaseViewAsync();
        }
        #endregion
    }
}

