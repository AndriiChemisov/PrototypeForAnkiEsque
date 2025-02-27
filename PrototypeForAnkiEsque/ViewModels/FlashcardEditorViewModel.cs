using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Commands;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardEditorViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IFlashcardService _flashcardService;
        private readonly INavigationService _navigationService;
        private Flashcard _flashcard;
        private string _front;
        private string _back;
        private string _editableFront;
        private string _editableBack;
        private string _savedMessage;
        private bool _isSavedMessageVisible;

        public FlashcardEditorViewModel(IFlashcardService flashcardService, INavigationService navigationService, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _flashcardService = flashcardService;
            _navigationService = navigationService;
            SaveFlashcardCommand = new AsyncRelayCommand(SaveFlashcardAsync);
            BackCommand = new AsyncRelayCommand(ReturnAsync);
            IsSavedMessageVisible = false;
        }

        public ICommand SaveFlashcardCommand { get; }
        public ICommand BackCommand { get; }

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
                OnValidationError?.Invoke("The flashcard cannot have blank sides!");
                return;
            }

            if (!string.Equals(EditableFront, Front, StringComparison.OrdinalIgnoreCase))
            {
                var existingFlashcard = await _dbContext.Flashcards
                    .FirstOrDefaultAsync(f => f.Front.ToLower() == EditableFront.ToLower());

                if (existingFlashcard != null)
                {
                    OnValidationError?.Invoke("This flashcard already exists!");
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
                    OnValidationError?.Invoke($"An error occurred while saving the flashcard: {ex.Message}");
                }
            }
            else
            {
                OnValidationError?.Invoke("No flashcard to save!");
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
            await _navigationService.GetFlashcardDatabaseViewAsync();
        }

        public event Action<string> OnValidationError;
        public event Action OnFadeOutMessage;
    }
}
