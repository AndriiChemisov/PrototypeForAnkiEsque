using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using System.Windows;
// This file is used to define the FlashcardDeckCreatorViewModel class. The FlashcardDeckCreatorViewModel class is used to handle the logic for the FlashcardDeckCreatorView.
// The FlashcardDeckCreatorViewModel class contains properties and methods that are used to interact with the FlashcardDeckCreatorView.
// The FlashcardDeckCreatorViewModel class inherits from the BaseViewModel class, which implements the INotifyPropertyChanged interface.
// Simple explanation: This class is used to handle the logic for the FlashcardDeckCreatorView.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckCreatorViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly IDeckService _deckService;
        private readonly IDeckNavigationService _deckNavigationService;
        private readonly IFlashcardService _flashcardService;
        private readonly IMessageService _messageService;
        private readonly ILocalizationService _localizationService;
        private string _deckName;
        private string _searchAvailableText;
        private string _searchSelectedText;
        private bool _isAddButtonEnabled;
        private bool _isRemoveButtonEnabled;
        private bool _areChangesMade;
        private readonly DispatcherTimer _debounceTimer;
        private string _backButtonContext;
        private string _saveButtonContext;
        private string _deckNameTextBlockContext;
        private string _searchFlashcardsTextBlockContext;
        private string _gridSelectHeaderContext;
        private string _gridFrontHeaderContext;
        private string _gridBackHeaderContext;
        private string _deckNameBlankErrorContext;
        private string _deckHasNoFlashcardsErrorContext;
        private string _deckSaveSuccessTitleContext;
        private string _deckSaveSuccessMessageContext;
        private string _deckDuplicateErrorContext;
        private string _validationErrorContext;
        #endregion

        #region CONSTRUCTOR
        public FlashcardDeckCreatorViewModel(IDeckService deckService, IDeckNavigationService deckNavigationService,
                                             IFlashcardService flashcardService, IMessageService messageService,
                                             ILocalizationService localizationservice)
        {
            _deckService = deckService;
            _deckNavigationService = deckNavigationService;
            _flashcardService = flashcardService;
            _messageService = messageService;
            _localizationService = localizationservice;

            _localizationService.LanguageChanged += OnLanguageChanged;

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
        #endregion

        #region PROPERTIES
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

        public string BackButtonContext
        {
            get => _backButtonContext;
            set
            {
                _backButtonContext = value;
                OnPropertyChanged();
            }
        }

        public string SaveButtonContext
        {
            get => _saveButtonContext;
            set
            {
                _saveButtonContext = value;
                OnPropertyChanged();
            }
        }

        public string DeckNameTextBlockContext
        {
            get => _deckNameTextBlockContext;
            set
            {
                _deckNameTextBlockContext = value;
                OnPropertyChanged();
            }
        }

        public string SearchFlashcardsTextBlockContext
        {
            get => _searchFlashcardsTextBlockContext;
            set
            {
                _searchFlashcardsTextBlockContext = value;
                OnPropertyChanged();
            }
        }

        public string GridSelectHeaderContext
        {
            get => _gridSelectHeaderContext;
            set
            {
                _gridSelectHeaderContext = value;
                OnPropertyChanged();
            }
        }

        public string GridFrontHeaderContext
        {
            get => _gridFrontHeaderContext;
            set
            {
                _gridFrontHeaderContext = value;
                OnPropertyChanged();
            }
        }

        public string GridBackHeaderContext
        {
            get => _gridBackHeaderContext;
            set
            {
                _gridBackHeaderContext = value;
                OnPropertyChanged();
            }
        }

        public string DeckNameBlankErrorContext
        {
            get => _deckNameBlankErrorContext;
            set
            {
                _deckNameBlankErrorContext = value;
                OnPropertyChanged();
            }
        }

        public string DeckHasNoFlashcardsErrorContext
        {
            get => _deckHasNoFlashcardsErrorContext;
            set
            {
                _deckHasNoFlashcardsErrorContext = value;
                OnPropertyChanged();
            }
        }

        public string DeckSaveSuccessTitleContext
        {
            get => _deckSaveSuccessTitleContext;
            set
            {
                _deckSaveSuccessTitleContext = value;
                OnPropertyChanged();
            }
        }

        public string DeckSaveSuccessMessageContext
        {
            get => _deckSaveSuccessMessageContext;
            set
            {
                _deckSaveSuccessMessageContext = value;
                OnPropertyChanged();
            }
        }

        public string ValidationErrorContext
        {
            get => _validationErrorContext;
            set
            {
                _validationErrorContext = value;
                OnPropertyChanged();
            }
        }

        public string DeckDuplicateErrorContext
        {
            get => _deckDuplicateErrorContext;
            set
            {
                _deckDuplicateErrorContext = value;
                OnPropertyChanged();
            }
        }
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
                _messageService.ShowMessage(DeckNameBlankErrorContext, ValidationErrorContext, MessageBoxImage.Warning);
                return;
            }

            if (!SelectedFlashcards.Any())
            {
                _messageService.ShowMessage(DeckHasNoFlashcardsErrorContext, ValidationErrorContext, MessageBoxImage.Warning);
                return;
            }

            if (await _deckService.DeckExistsAsync(DeckName))
            {
                _messageService.ShowMessage(DeckDuplicateErrorContext, ValidationErrorContext, MessageBoxImage.Warning);
                return;
            }

            var flashcardFronts = SelectedFlashcards.Select(f => f.Front).ToList();
            string easeRating = await _deckService.CalculateEaseRatingAsync(flashcardFronts);

            await _deckService.CreateDeckAsync(DeckName, flashcardFronts, easeRating);

            // Reset fields
            DeckName = string.Empty;
            AreChangesMade = false;
            await LoadFlashcardsAsync();
            _messageService.ShowMessage(DeckSaveSuccessMessageContext, DeckSaveSuccessTitleContext, MessageBoxImage.Information);
        }

        private async Task OpenDeckSelectionAsync()
        {
            if (AreChangesMade)
            {
                // Auto-save logic or user prompt could be added here
            }

            await _deckNavigationService.GetFlashcardDeckSelectionViewAsync();
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

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            LoadLocalizedText();
        }

        private void LoadLocalizedText()
        {
            BackButtonContext = _localizationService.GetString("BttnBack");
            SaveButtonContext = _localizationService.GetString("BttnSave");
            DeckNameTextBlockContext = _localizationService.GetString("GrdHdrDeckName") + ":";
            SearchFlashcardsTextBlockContext = _localizationService.GetString("TxtBlkSearch");
            GridSelectHeaderContext = _localizationService.GetString("GrdHdrSelection");
            GridFrontHeaderContext = _localizationService.GetString("GrdHdrFront");
            GridBackHeaderContext = _localizationService.GetString("GrdHdrBack");
            DeckNameBlankErrorContext = _localizationService.GetString("MssgDeckNameBlank");
            DeckHasNoFlashcardsErrorContext = _localizationService.GetString("MssgDeckHasNoCards");
            DeckSaveSuccessTitleContext = _localizationService.GetString("MssgSuccessTitle");
            DeckSaveSuccessMessageContext = _localizationService.GetString("MssgDeckSaveSuccess");
            ValidationErrorContext = _localizationService.GetString("MssgValidationError");
            DeckDuplicateErrorContext = _localizationService.GetString("MssgDeckExistsError");
        }
        #endregion
    }
}
