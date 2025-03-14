﻿using System.Windows.Input;
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
        private readonly ILocalizationService _localizationService;

        private FlashcardDeck _selectedDeck;
        private string _searchAvailableText;
        private string _searchSelectedText;


        // Button enable/disable states
        private bool _isAddButtonEnabled;
        private bool _isRemoveButtonEnabled;
        private bool _isSaveButtonEnabled;

        private string _tempDeckName;
        private string _originalDeckName;

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
        public FlashcardDeckEditorViewModel(IDeckService deckService, IDeckNavigationService deckNavigationService, 
                                            IFlashcardService flashcardService, ILocalizationService localizationService)
        {
            _deckService = deckService;
            _deckNavigationService = deckNavigationService;
            _flashcardService = flashcardService;
            _localizationService = localizationService;

            _localizationService.LanguageChanged += OnLanguageChanged;


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
            _localizationService = localizationService;
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
                MessageBox.Show(DeckHasNoFlashcardsErrorContext, ValidationErrorContext, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(DeckName))
            {
                MessageBox.Show(DeckNameBlankErrorContext, ValidationErrorContext, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_originalDeckName != DeckName && await _deckService.DeckExistsAsync(DeckName))
            {
                MessageBox.Show(DeckDuplicateErrorContext, ValidationErrorContext, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedDeck.Name = DeckName;
            _selectedDeck.FlashcardFronts = DeckFlashcards.Select(f => f.Front).ToList();
            await _deckService.UpdateDeckAsync(_selectedDeck);

            MessageBox.Show(DeckSaveSuccessMessageContext, DeckSaveSuccessTitleContext, MessageBoxButton.OK, MessageBoxImage.Information);
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
