using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using System.Windows;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckCreatorViewModel : BaseViewModel
    {
        private readonly IDeckService _deckService;
        private readonly INavigationService _navigationService;
        private readonly IFlashcardService _flashcardService;
        private readonly IMessageService _messageService;
        private string _deckName;
        private string _searchAvailableText;
        private string _searchSelectedText;
        private bool _isAddButtonEnabled;
        private bool _isRemoveButtonEnabled;
        private bool _areChangesMade;
        private readonly DispatcherTimer _debounceTimer;

        public ObservableCollection<Flashcard> AvailableFlashcards { get; } = new();
        public ObservableCollection<Flashcard> SelectedFlashcards { get; } = new();

        public ObservableCollection<Flashcard> FilteredAvailableFlashcards { get; } = new();
        public ObservableCollection<Flashcard> FilteredSelectedFlashcards { get; } = new();

        public Dictionary<string, bool> SelectedAvailableFlashcards { get; set; } = new();
        public Dictionary<string, bool> SelectedDeckFlashcards { get; set; } = new();

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

        public string DeckName
        {
            get => _deckName;
            set
            {
                _deckName = value;
                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            OnPropertyChanged(nameof(DeckName));
            AreChangesMade = true;
            UpdateButtonStates();
        }

        public bool IsAddButtonEnabled
        {
            get => _isAddButtonEnabled;
            set
            {
                _isAddButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsRemoveButtonEnabled
        {
            get => _isRemoveButtonEnabled;
            set
            {
                _isRemoveButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool AreChangesMade
        {
            get => _areChangesMade;
            set
            {
                _areChangesMade = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddFlashcardsCommand { get; }
        public ICommand RemoveFlashcardsCommand { get; }
        public ICommand SaveDeckCommand { get; }
        public ICommand OpenDeckSelectionCommand { get; }
        public ICommand ToggleAvailableFlashcardSelectionCommand { get; }
        public ICommand ToggleDeckFlashcardSelectionCommand { get; }

        public FlashcardDeckCreatorViewModel(IDeckService deckService, INavigationService navigationService, IFlashcardService flashcardService, IMessageService messageService)
        {
            _deckService = deckService;
            _navigationService = navigationService;
            _flashcardService = flashcardService;
            _messageService = messageService;

            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _debounceTimer.Tick += DebounceTimer_Tick;

            OpenDeckSelectionCommand = new AsyncRelayCommand(async () => await OpenDeckSelectionAsync());
            AddFlashcardsCommand = new AsyncRelayCommand(AddFlashcardsAsync);
            RemoveFlashcardsCommand = new AsyncRelayCommand(RemoveFlashcardsAsync);
            SaveDeckCommand = new AsyncRelayCommand(SaveDeckAsync);
            ToggleAvailableFlashcardSelectionCommand = new AsyncRelayCommand<string>(ToggleAvailableFlashcardSelectionAsync);
            ToggleDeckFlashcardSelectionCommand = new AsyncRelayCommand<string>(ToggleDeckFlashcardSelectionAsync);

            LoadFlashcardsAsync();
        }

        private async Task LoadFlashcardsAsync()
        {
            AvailableFlashcards.Clear();
            SelectedFlashcards.Clear();
            FilteredAvailableFlashcards.Clear();
            FilteredSelectedFlashcards.Clear();

            var allFlashcards = await _flashcardService.GetFlashcardsAsync();

            foreach (var flashcard in allFlashcards)
            {
                AvailableFlashcards.Add(flashcard);
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
                SelectedFlashcards.Add(flashcard);
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
                .Select(x => SelectedFlashcards.First(f => f.Front == x.Key))
                .ToList();

            foreach (var flashcard in selected)
            {
                SelectedFlashcards.Remove(flashcard);
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
            if (string.IsNullOrWhiteSpace(DeckName))
            {
                _messageService.ShowMessage("The Deck name cannot be blank!", "Validation Error", MessageBoxImage.Warning);
                return;
            }

            if (!SelectedFlashcards.Any())
            {
                _messageService.ShowMessage("The Deck must contain at least one flashcard!", "Validation Error", MessageBoxImage.Warning);
                return;
            }

            if (await _deckService.DeckExistsAsync(DeckName))
            {
                _messageService.ShowMessage("A deck with that name already exists!", "Validation Error", MessageBoxImage.Warning);
                return;
            }

            var flashcardFronts = SelectedFlashcards.Select(f => f.Front).ToList();
            string easeRating = await _deckService.CalculateEaseRatingAsync(flashcardFronts);

            await _deckService.CreateDeckAsync(DeckName, flashcardFronts, easeRating);

            // Reset fields
            DeckName = string.Empty;
            AreChangesMade = false;
            await LoadFlashcardsAsync();
            _messageService.ShowMessage("Deck successfully created!", "Success", MessageBoxImage.Information);
        }

        private async Task OpenDeckSelectionAsync()
        {
            if (AreChangesMade)
            {
                // Auto-save logic or user prompt could be added here
            }

            await _navigationService.GetFlashcardDeckSelectionViewAsync();
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
            foreach (var flashcard in SelectedFlashcards
                .Where(f => (f.Front.Contains(SearchSelectedText ?? "", System.StringComparison.OrdinalIgnoreCase) ||
                            f.Back.Contains(SearchSelectedText ?? "", System.StringComparison.OrdinalIgnoreCase)))
            )
            {
                FilteredSelectedFlashcards.Add(flashcard);
            }
        }

        private async Task ToggleAvailableFlashcardSelectionAsync(string flashcardFront)
        {
            if (SelectedAvailableFlashcards.ContainsKey(flashcardFront))
                SelectedAvailableFlashcards[flashcardFront] = !SelectedAvailableFlashcards[flashcardFront];
            else
                SelectedAvailableFlashcards[flashcardFront] = true;

            // Force UI update
            OnPropertyChanged(nameof(SelectedAvailableFlashcards));
            UpdateButtonStates();
        }

        private async Task ToggleDeckFlashcardSelectionAsync(string flashcardFront)
        {
            if (SelectedDeckFlashcards.ContainsKey(flashcardFront))
                SelectedDeckFlashcards[flashcardFront] = !SelectedDeckFlashcards[flashcardFront];
            else
                SelectedDeckFlashcards[flashcardFront] = true;

            // Force UI update
            OnPropertyChanged(nameof(SelectedDeckFlashcards));
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            IsAddButtonEnabled = SelectedAvailableFlashcards.Any(x => x.Value);
            IsRemoveButtonEnabled = SelectedDeckFlashcards.Any(x => x.Value);
        }
    }
}
