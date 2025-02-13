using System;
using System.Windows;
using System.Windows.Input;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardEditorViewModel : BaseViewModel
    {
        private readonly FlashcardService _flashcardService;
        private readonly NavigationService _navigationService;
        private Flashcard _flashcard;
        private string _front;
        private string _back;
        private string _editableFront;
        private string _editableBack;
        private string _savedMessage;
        private bool _isSavedMessageVisible;

        public FlashcardEditorViewModel(FlashcardService flashcardService, NavigationService navigationService)
        {
            _flashcardService = flashcardService;
            _navigationService = navigationService;
            SaveFlashcardCommand = new RelayCommand(SaveFlashcard);
            BackCommand = new RelayCommand(Return);
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
                    OnPropertyChanged(nameof(Front));  // Set a breakpoint here
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
                    OnPropertyChanged(nameof(Front));  // Set a breakpoint here
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
                    OnPropertyChanged(); // Notify the UI that EditableFront changed
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
                    OnPropertyChanged(); // Notify the UI that EditableBack changed
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

        private void SaveFlashcard()
        {
            if (string.IsNullOrWhiteSpace(Front) || string.IsNullOrWhiteSpace(Back))
            {
                // Show alert if front or back are blank
                MessageBox.Show("The flashcard cannot be blank!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update the flashcard in the database
            _flashcard.Front = EditableFront;
            _flashcard.Back = EditableBack;
            _flashcardService.UpdateFlashcard(_flashcard);

            // Show confirmation message and trigger animation
            SavedMessage = "Changes saved!";
            IsSavedMessageVisible = true;

            // Trigger the fade-out animation
            TriggerFadeOutAnimation();
        }

        private async void Return()
        {
            // Navigate back without saving
            await _navigationService.GetFlashcardDatabaseViewAsync(); // Go back to FlashcardDatabaseView
        }

        public event Action OnFadeOutMessage;

        private void TriggerFadeOutAnimation()
        {
            // This calls the animation from the view
            OnFadeOutMessage?.Invoke();
        }
    }

}
