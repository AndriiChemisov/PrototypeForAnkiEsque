using System.Windows.Input;
using System.Windows;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using System.Collections.ObjectModel;
// This file is used to define the FlashcardDeckEditorViewModel class, which inherits from the BaseViewModel class.
// The FlashcardDeckEditorViewModel class is used to provide the logic for the FlashcardDeckEditorView.
// The FlashcardDeckEditorViewModel class defines properties and methods that are used to interact with the FlashcardDeckEditorView.
// Simple explanation: This class is used to provide the logic for the FlashcardDeckEditorView.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckEditorViewModel : BaseViewModel
    {
        #region FILEDS DECLARATION
        private readonly IDeckService _deckService;
        private readonly IDeckNavigationService _deckNavigationService;
        private readonly IFlashcardService _flashcardService;
        private FlashcardDeck _selectedDeck;
        private string _searchAvailableText;
        private string _searchSelectedText;


        // Button enable/disable states
        private bool _isAddButtonEnabled;
        private bool _isRemoveButtonEnabled;
        private bool _isSaveButtonEnabled;
        private string _tempDeckName;
        private string _originalDeckName;
        #endregion

        #region CONSTRUCTOR
        public FlashcardDeckEditorViewModel(IDeckService deckService, IDeckNavigationService deckNavigationService, IFlashcardService flashcardService)
        {
            _deckService = deckService;
            _deckNavigationService = deckNavigationService;
            _flashcardService = flashcardService;

            OpenDeckSelectionCommand = new AsyncRelayCommand(OpenDeckSelectionAsync);
            AddFlashcardsCommand = new AsyncRelayCommand(AddFlashcardsAsync);
            RemoveFlashcardsCommand = new AsyncRelayCommand(RemoveFlashcardsAsync);
            SaveDeckCommand = new AsyncRelayCommand(SaveDeckAsync);
            ToggleAvailableFlashcardSelectionCommand = new AsyncRelayCommand<string>(ToggleAvailableFlashcardSelectionAsync);
            ToggleDeckFlashcardSelectionCommand = new AsyncRelayCommand<string>(ToggleDeckFlashcardSelectionAsync);

            // Initial states for buttons
            IsAddButtonEnabled = false;
            IsRemoveButtonEnabled = false;
            IsSaveButtonEnabled = false;

            LoadFlashcards();
        }
        #endregion

        #region PROPERTIES
        public ObservableCollection<Flashcard> DeckFlashcards { get; } = new();
        public ObservableCollection<Flashcard> AvailableFlashcards { get; } = new();
        public Dictionary<string, bool> SelectedAvailableFlashcards { get; set; } = new();
        public Dictionary<string, bool> SelectedDeckFlashcards { get; set; } = new();
        public ObservableCollection<Flashcard> FilteredAvailableFlashcards { get; set; } = new();
        public ObservableCollection<Flashcard> FilteredSelectedFlashcards { get; set; } = new();

        public bool IsAddButtonEnabled
        {
            get => _isAddButtonEnabled;
            set
            {
                if (_isAddButtonEnabled != value)
                {
                    _isAddButtonEnabled = value;
                    OnPropertyChanged(nameof(IsAddButtonEnabled));  // Notify UI of change
                }
            }
        }
        public bool IsRemoveButtonEnabled
        {
            get => _isRemoveButtonEnabled;
            set
            {
                if (_isRemoveButtonEnabled != value)
                {
                    _isRemoveButtonEnabled = value;
                    OnPropertyChanged(nameof(IsRemoveButtonEnabled));  // Notify UI of change
                }
            }
        }
        public bool IsSaveButtonEnabled
        {
            get => _isSaveButtonEnabled;
            set
            {
                if (_isSaveButtonEnabled != value)
                {
                    _isSaveButtonEnabled = value;
                    OnPropertyChanged(nameof(IsSaveButtonEnabled));  // Notify UI of change
                }
            }
        }

        public string DeckName
        {
            get => _tempDeckName;
            set
            {
                if (_tempDeckName != value)
                {
                    _tempDeckName = value;
                    OnPropertyChanged();
                    // If the name is changed, enable the Save button immediately
                    IsSaveButtonEnabled = true;

                    // Track if the name has changed from the original deck name
                    if (_originalDeckName != _tempDeckName)
                    {
                        AreChangesMade = true;  // Mark that changes have been made
                    }
                }
            }
        }

        public string SearchAvailableText
        {
            get => _searchAvailableText;
            set
            {
                if (_searchAvailableText != value)
                {
                    _searchAvailableText = value;
                    OnPropertyChanged();
                    UpdateFilteredAvailableFlashcards();
                }
            }
        }

        public string SearchSelectedText
        {
            get => _searchSelectedText;
            set
            {
                if (_searchSelectedText != value)
                {
                    _searchSelectedText = value;
                    OnPropertyChanged();
                    UpdateFilteredSelectedFlashcards();
                }
            }
        }
        public bool AreChangesMade { get; set; }
        #endregion

        #region COMMANDS
        public ICommand AddFlashcardsCommand { get; }
        public ICommand RemoveFlashcardsCommand { get; }
        public ICommand SaveDeckCommand { get; }
        public ICommand OpenDeckSelectionCommand { get; }
        public ICommand ToggleAvailableFlashcardSelectionCommand { get; }
        public ICommand ToggleDeckFlashcardSelectionCommand { get; }
        #endregion

        #region METHODS
        private async void LoadFlashcards()
        {
            if (_selectedDeck == null) return;

            DeckFlashcards.Clear();
            AvailableFlashcards.Clear();
            FilteredAvailableFlashcards.Clear();
            FilteredSelectedFlashcards.Clear();

            var allFlashcards = await _flashcardService.GetFlashcardsAsync();
            var deckFlashcardFronts = _selectedDeck.FlashcardFronts;

            foreach (var flashcard in allFlashcards)
            {
                if (deckFlashcardFronts.Contains(flashcard.Front))
                {
                    DeckFlashcards.Add(flashcard);
                }
                else
                {
                    AvailableFlashcards.Add(flashcard);
                }
            }

            UpdateFilteredAvailableFlashcards();
            UpdateFilteredSelectedFlashcards();
            UpdateButtonStates();
        }

        private async Task AddFlashcardsAsync()
        {
            var selected = SelectedAvailableFlashcards
                .Where(x => x.Value)
                .Select(x => AvailableFlashcards.First(f => f.Front == x.Key))
                .ToList();

            foreach (var flashcard in selected)
            {
                DeckFlashcards.Add(flashcard);
                AvailableFlashcards.Remove(flashcard);
            }

            SelectedAvailableFlashcards.Clear();
            UpdateFilteredAvailableFlashcards();
            UpdateFilteredSelectedFlashcards();
            AreChangesMade = true;
            UpdateButtonStates();
        }

        private async Task RemoveFlashcardsAsync()
        {
            var selected = SelectedDeckFlashcards
                .Where(x => x.Value)
                .Select(x => DeckFlashcards.FirstOrDefault(f => f.Front == x.Key))
                .ToList();

            foreach (var flashcard in selected)
            {
                DeckFlashcards.Remove(flashcard);
                AvailableFlashcards.Add(flashcard);
            }

            SelectedDeckFlashcards.Clear();
            UpdateFilteredAvailableFlashcards();
            UpdateFilteredSelectedFlashcards();
            AreChangesMade = true;
            UpdateButtonStates();
        }

        private async Task SaveDeckAsync()
        {
            if (DeckFlashcards.Count == 0)
            {
                MessageBox.Show("The deck must contain at least one flashcard!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(DeckName))
            {
                MessageBox.Show("The Deck name cannot be blank!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_originalDeckName != DeckName && await _deckService.DeckExistsAsync(DeckName))
            {
                MessageBox.Show("A deck with this name already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedDeck.Name = DeckName;
            _selectedDeck.FlashcardFronts = DeckFlashcards.Select(f => f.Front).ToList();
            await _deckService.UpdateDeckAsync(_selectedDeck);

            MessageBox.Show("Deck saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            IsSaveButtonEnabled = false;
        }

        private async Task OpenDeckSelectionAsync()
        {
            await _deckNavigationService.GetFlashcardDeckSelectionViewAsync();
        }

        public void Initialize(FlashcardDeck selectedDeck)
        {
            _selectedDeck = selectedDeck;
            _originalDeckName = selectedDeck.Name;
            DeckName = selectedDeck.Name;
            LoadFlashcards();
        }

        private async Task ToggleAvailableFlashcardSelectionAsync(string flashcardFront)
        {
            if (SelectedAvailableFlashcards.ContainsKey(flashcardFront))
                SelectedAvailableFlashcards[flashcardFront] = !SelectedAvailableFlashcards[flashcardFront];
            else
                SelectedAvailableFlashcards[flashcardFront] = true;

            OnPropertyChanged(nameof(SelectedAvailableFlashcards));
            UpdateButtonStates();
        }

        private async Task ToggleDeckFlashcardSelectionAsync(string flashcardFront)
        {
            if (SelectedDeckFlashcards.ContainsKey(flashcardFront))
                SelectedDeckFlashcards[flashcardFront] = !SelectedDeckFlashcards[flashcardFront];
            else
                SelectedDeckFlashcards[flashcardFront] = true;

            OnPropertyChanged(nameof(SelectedDeckFlashcards));
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            IsAddButtonEnabled = SelectedAvailableFlashcards.Any(x => x.Value);
            IsRemoveButtonEnabled = SelectedDeckFlashcards.Any(x => x.Value);
        }

        private void UpdateFilteredAvailableFlashcards()
        {
            FilteredAvailableFlashcards.Clear();
            foreach (var flashcard in AvailableFlashcards
                .Where(f => (f.Front.Contains(SearchAvailableText ?? "", System.StringComparison.OrdinalIgnoreCase) ||
                            f.Back.Contains(SearchAvailableText ?? "", System.StringComparison.OrdinalIgnoreCase)))
            )
            {
                FilteredAvailableFlashcards.Add(flashcard);
            }
        }

        private void UpdateFilteredSelectedFlashcards()
        {
            FilteredSelectedFlashcards.Clear();
            foreach (var flashcard in DeckFlashcards
                .Where(f => (f.Front.Contains(SearchSelectedText ?? "", System.StringComparison.OrdinalIgnoreCase) ||
                            f.Back.Contains(SearchSelectedText ?? "", System.StringComparison.OrdinalIgnoreCase)))
            )
            {
                FilteredSelectedFlashcards.Add(flashcard);
            }
        }
        #endregion
    }
}
