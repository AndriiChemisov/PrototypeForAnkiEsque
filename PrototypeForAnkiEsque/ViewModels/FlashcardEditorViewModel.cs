using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Commands;
using PrototypeForAnkiEsque.Resources;
// This file is used to define the FlashcardEditorViewModel class. The FlashcardEditorViewModel class is used to handle the logic for the FlashcardEditorUserControl view.
// The FlashcardEditorViewModel class inherits from the BaseViewModel class and implements the INotifyPropertyChanged interface.
// The FlashcardEditorViewModel class defines properties for the Flashcard, Front, Back, EditableFront, EditableBack, SavedMessage, and IsSavedMessageVisible.
// The FlashcardEditorViewModel class defines a SaveFlashcardCommand and a BackCommand property, which are used to save the flashcard and return to the previous view, respectively.
// The FlashcardEditorViewModel class defines methods to save the flashcard, show a save message, and trigger a fade-out animation.
// The FlashcardEditorViewModel class also defines events for validation errors and fading out the save message.
// Simple explanation: This class is used to handle the logic for the FlashcardEditorUserControl view.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardEditorViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly ApplicationDbContext _dbContext;
        private readonly IFlashcardService _flashcardService;
        private readonly IFlashcardNavigationService _flashcardNavigationService;
        private readonly IMessageService _messageService;
        private Flashcard _flashcard;
        private string _front;
        private string _back;
        private string _editableFront;
        private string _editableBack;
        private string _savedMessage;
        private bool _isSavedMessageVisible;
        private string _backButtonContext;
        private string _saveButtonContext;
        private string _enterFrontTextBoxContext;
        private string _enterBackTextBoxContext;

        #endregion

        #region CONSTRUCTOR
        public FlashcardEditorViewModel(IFlashcardService flashcardService, IFlashcardNavigationService flashcardNavigationService, IMessageService messageService, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _flashcardService = flashcardService;
            _flashcardNavigationService = flashcardNavigationService;
            _messageService = messageService;
            SaveFlashcardCommand = new AsyncRelayCommand(SaveFlashcardAsync);
            BackCommand = new AsyncRelayCommand(ReturnAsync);
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
            get => Flashcard?.Front;
            set
            {
                if (_front != value)
                {
                    _front = value;
                    OnPropertyChanged(nameof(Front));
                }
            }
        }
        
        public string Back
        {
            get => Flashcard?.Back;
            set
            {
                if (_back != value)
                {
                    _back = value;
                    OnPropertyChanged(nameof(Back));
                }
            }
        }

        public string EditableFront
        {
            get => _editableFront;
            set
            {
                if (_editableFront != value)
                {
                    _editableFront = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EditableBack
        {
            get => _editableBack;
            set
            {
                if (_editableBack != value)
                {
                    _editableBack = value;
                    OnPropertyChanged();
                }
            }
        }

        public Flashcard Flashcard
        {
            get => _flashcard;
            set => SetProperty(ref _flashcard, value);
        }

        public string SavedMessage
        {
            get => _savedMessage;
            set => SetProperty(ref _savedMessage, value);
        }

        public bool IsSavedMessageVisible
        {
            get => _isSavedMessageVisible;
            set => SetProperty(ref _isSavedMessageVisible, value);
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
        public ICommand BackCommand { get; }
        #endregion

        #region METHODS
        public void Initialize(Flashcard flashcard)
        {
            Flashcard = flashcard;
            Front = flashcard.Front;
            Back = flashcard.Back;
            EditableFront = _front;
            EditableBack = _back;
        }

        private async Task SaveFlashcardAsync()
        {
            if (string.IsNullOrWhiteSpace(EditableFront) || string.IsNullOrWhiteSpace(EditableBack))
            {
                _messageService.ShowMessage("The flashcard cannot have blank sides!",  "Validation Error!", System.Windows.MessageBoxImage.Error);
                return;
            }

            if (!string.Equals(EditableFront, Front, StringComparison.OrdinalIgnoreCase))
            {
                var existingFlashcard = await _dbContext.Flashcards
                    .FirstOrDefaultAsync(f => f.Front.ToLower() == EditableFront.ToLower());

                if (existingFlashcard != null)
                {
                    _messageService.ShowMessage("This flashcard already exists!", "Validation Error!", System.Windows.MessageBoxImage.Error);
                    return;
                }
            }

            if (_flashcard != null)
            {
                try
                {

                    _flashcard.Front = EditableFront;
                    _flashcard.Back = EditableBack;

                    await _flashcardService.UpdateFlashcardAsync(_flashcard);

                    ShowSaveMessage();
                }
                catch (Exception ex)
                {
                    _messageService.ShowMessage($"An error occurred while saving the flashcard: {ex.Message}", "Validation Error!", System.Windows.MessageBoxImage.Error);
                }
            }
            else
            {
                _messageService.ShowMessage("No flashcard to save!", "Validation Error!", System.Windows.MessageBoxImage.Error);
            }
        }

        private void ShowSaveMessage()
        {
            SavedMessage = "Changes saved!";
            IsSavedMessageVisible = true;
            TriggerFadeOutAnimation();
        }

        private void TriggerFadeOutAnimation()
        {
            OnFadeOutMessage?.Invoke();
        }

        private async Task ReturnAsync()
        {
            await _flashcardNavigationService.GetFlashcardDatabaseViewAsync();
        }

        public void LoadLocalizedTexts()
        {
            BackButtonContext = Strings.BttnBack;
            SaveButtonContext = Strings.BttnSave;
            EnterFrontTextBoxContext = Strings.TxtBlkEnterFront;
            EnterBackTextBoxContext = Strings.TxtBlkEnterBack;
        }
        #endregion
    }
}
