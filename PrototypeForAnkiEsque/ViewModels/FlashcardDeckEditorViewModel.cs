using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace PrototypeForAnkiEsque.ViewModels
{


    public class FlashcardDeckEditorViewModel : BaseViewModel
    {
        private readonly DeckService _deckService;
        private readonly NavigationService _navigationService;
        private readonly FlashcardService _flashcardService;
        private FlashcardDeck _selectedDeck;
        private string _searchAvailableText;
        private string _searchSelectedText;

        public ObservableCollection<Flashcard> DeckFlashcards { get; } = new();
        public ObservableCollection<Flashcard> AvailableFlashcards { get; } = new();

        // Track selected flashcards by their ID for Available and Deck lists
        public Dictionary<int, bool> SelectedAvailableFlashcards { get; set; } = new();
        public Dictionary<int, bool> SelectedDeckFlashcards { get; set; } = new();
        public ObservableCollection<Flashcard> FilteredAvailableFlashcards { get; set; } = new();
        public ObservableCollection<Flashcard> FilteredSelectedFlashcards { get; set; } = new();

        // Button enable/disable states
        private bool _isAddButtonEnabled;
        private bool _isRemoveButtonEnabled;
        private bool _isSaveButtonEnabled;
        private string _tempDeckName;
        private string _originalDeckName;

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

        public ICommand AddFlashcardsCommand { get; }
        public ICommand RemoveFlashcardsCommand { get; }
        public ICommand SaveDeckCommand { get; }
        public ICommand OpenDeckSelectionCommand { get; }
        public ICommand ToggleAvailableFlashcardSelectionCommand { get; }
        public ICommand ToggleDeckFlashcardSelectionCommand { get; }

        public FlashcardDeckEditorViewModel(DeckService deckService, NavigationService navigationService, FlashcardService flashcardService)
        {
            _deckService = deckService;
            _navigationService = navigationService;
            _flashcardService = flashcardService;

            OpenDeckSelectionCommand = new RelayCommand(OpenDeckSelectionAsync);
            AddFlashcardsCommand = new RelayCommand(AddFlashcards);
            RemoveFlashcardsCommand = new RelayCommand(RemoveFlashcards);
            SaveDeckCommand = new RelayCommand(SaveDeck);
            ToggleAvailableFlashcardSelectionCommand = new RelayCommand<int>(ToggleAvailableFlashcardSelection);
            ToggleDeckFlashcardSelectionCommand = new RelayCommand<int>(ToggleDeckFlashcardSelection);

            // Initial states for buttons
            IsAddButtonEnabled = false;
            IsRemoveButtonEnabled = false;
            IsSaveButtonEnabled = false;

            LoadFlashcards();
        }

        public bool AreChangesMade { get; set; }

        private async void LoadFlashcards()
        {
            if (_selectedDeck == null) return;

            DeckFlashcards.Clear();
            AvailableFlashcards.Clear();
            FilteredAvailableFlashcards.Clear();
            FilteredSelectedFlashcards.Clear();

            var allFlashcards = _flashcardService.GetFlashcards();
            var deckFlashcardIds = _selectedDeck.FlashcardIds;

            foreach (var flashcard in allFlashcards)
            {
                if (deckFlashcardIds.Contains(flashcard.Id))
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

        private void AddFlashcards()
        {
            var selected = SelectedAvailableFlashcards
                .Where(x => x.Value)
                .Select(x => AvailableFlashcards.First(f => f.Id == x.Key))
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

        private void RemoveFlashcards()
        {
            var selected = SelectedDeckFlashcards
                .Where(x => x.Value)
                .Select(x => DeckFlashcards.First(f => f.Id == x.Key))
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

        private async void SaveDeck()
        {
            if (DeckFlashcards.Count == 0)
            {
                MessageBox.Show("The deck must contain at least one flashcard!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_originalDeckName != DeckName && _deckService.CheckIfDeckNameExists(DeckName))
            {
                MessageBox.Show("A deck with this name already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedDeck.Name = DeckName;
            _selectedDeck.FlashcardIds = DeckFlashcards.Select(f => f.Id).ToList();
            await _deckService.UpdateDeckAsync(_selectedDeck);

            MessageBox.Show("Deck saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            IsSaveButtonEnabled = false;
        }

        private async void OpenDeckSelectionAsync()
        {
            await _navigationService.GetFlashcardDeckSelectionViewAsync();
        }

        public void Initialize(FlashcardDeck selectedDeck)
        {
            _selectedDeck = selectedDeck;
            _originalDeckName = selectedDeck.Name;
            DeckName = selectedDeck.Name;
            LoadFlashcards();
        }

        private void ToggleAvailableFlashcardSelection(int flashcardId)
        {
            if (SelectedAvailableFlashcards.ContainsKey(flashcardId))
                SelectedAvailableFlashcards[flashcardId] = !SelectedAvailableFlashcards[flashcardId];
            else
                SelectedAvailableFlashcards[flashcardId] = true;

            OnPropertyChanged(nameof(SelectedAvailableFlashcards));
            UpdateButtonStates();
        }

        private void ToggleDeckFlashcardSelection(int flashcardId)
        {
            if (SelectedDeckFlashcards.ContainsKey(flashcardId))
                SelectedDeckFlashcards[flashcardId] = !SelectedDeckFlashcards[flashcardId];
            else
                SelectedDeckFlashcards[flashcardId] = true;

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
    }

}
