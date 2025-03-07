using System.Windows.Input;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using PrototypeForAnkiEsque.Resources;
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
        private readonly IMessageService _messageService;
        private string _front;
        private string _back;
        private string _savedMessage;
        private bool _isSavedMessageVisible;
        private string _backButtonContext;
        private string _saveButtonContext;
        private string _enterFrontTextBoxContext;
        private string _enterBackTextBoxContext;
        #endregion

        #region CONSTRUCTOR
        public FlashcardEntryViewModel(ApplicationDbContext dbContext, IFlashcardNavigationService flashcardNavigationService, IMessageService messageService)
        {
            _dbContext = dbContext;
            _flashcardNavigationService = flashcardNavigationService;
            _messageService = messageService;
            SaveFlashcardCommand = new AsyncRelayCommand(SaveFlashcardAsync);
            OpenFlashcardDatabaseCommand = new AsyncRelayCommand(OpenFlashcardDatabaseAsync);
            IsSavedMessageVisible = false;
            LoadLocalizedTexts();
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

        public string BackButtonContext
        {
            get => _backButtonContext;
            set => SetProperty(ref _backButtonContext, value);
        }

        public string SaveButtonContext
        {
            get => _saveButtonContext;
            set => SetProperty(ref _saveButtonContext, value);
        }

        public string EnterFrontTextBoxContext
        {
            get => _enterFrontTextBoxContext;
            set => SetProperty(ref _enterFrontTextBoxContext, value);
        }

        public string EnterBackTextBoxContext
        {
            get => _enterBackTextBoxContext;
            set => SetProperty(ref _enterBackTextBoxContext, value);
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
                _messageService.ShowMessage("The flashcard cannot have blank sides!", "Validation Error!", System.Windows.MessageBoxImage.Error);
                return;
            }

            var existingFlashcard = await _dbContext.Flashcards
                .FirstOrDefaultAsync(f => f.Front.ToLower() == Front.ToLower());

            if (existingFlashcard != null)
            {
                _messageService.ShowMessage("This flashcard already exists!", "Validation Error!", System.Windows.MessageBoxImage.Error);
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
                _messageService.ShowMessage($"An error occurred while saving the flashcard: {ex.Message}", "Validation Error!", System.Windows.MessageBoxImage.Error);
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

        private void LoadLocalizedTexts()
        {
            BackButtonContext = Strings.BttnBack;
            SaveButtonContext = Strings.BttnSave;
            EnterFrontTextBoxContext = Strings.TxtBlkEnterFront;
            EnterBackTextBoxContext = Strings.TxtBlkEnterBack;

        }

        #endregion
    }
}

