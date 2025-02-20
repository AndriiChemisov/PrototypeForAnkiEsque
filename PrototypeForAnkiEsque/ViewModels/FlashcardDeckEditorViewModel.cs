using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

public class FlashcardDeckEditorViewModel : BaseViewModel
{
    private readonly DeckService _deckService;
    private readonly NavigationService _navigationService;
    private readonly FlashcardService _flashcardService;
    private FlashcardDeck _selectedDeck;

    public ObservableCollection<Flashcard> DeckFlashcards { get; } = new();
    public ObservableCollection<Flashcard> AvailableFlashcards { get; } = new();

    // Track selected flashcards by their ID for Available and Deck lists
    public Dictionary<int, bool> SelectedAvailableFlashcards { get; set; } = new();
    public Dictionary<int, bool> SelectedDeckFlashcards { get; set; } = new();

    // Button enable/disable states
    private bool _isAddButtonEnabled;
    private bool _isRemoveButtonEnabled;
    private bool _isSaveButtonEnabled;

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

    public ICommand AddFlashcardsCommand { get; }
    public ICommand RemoveFlashcardsCommand { get; }
    public ICommand SaveDeckCommand { get; }
    public ICommand OpenMainMenuCommand { get; }
    public ICommand ToggleAvailableFlashcardSelectionCommand { get; }
    public ICommand ToggleDeckFlashcardSelectionCommand { get; }

    public FlashcardDeckEditorViewModel(DeckService deckService, NavigationService navigationService, FlashcardService flashcardService)
    {
        _deckService = deckService;
        _navigationService = navigationService;
        _flashcardService = flashcardService;

        OpenMainMenuCommand = new RelayCommand(OpenMainMenuAsync);
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

        UpdateButtonStates();
    }

    private void AddFlashcards()
    {
        foreach (var flashcard in SelectedAvailableFlashcards.Where(x => x.Value).Select(x => AvailableFlashcards.First(f => f.Id == x.Key)).ToList())
        {
            if (!DeckFlashcards.Contains(flashcard))
            {
                DeckFlashcards.Add(flashcard);
                AvailableFlashcards.Remove(flashcard);
            }
        }

        // Clear the selection after adding
        foreach (var flashcard in SelectedAvailableFlashcards.Keys.ToList())
        {
            SelectedAvailableFlashcards[flashcard] = false;
        }

        // Disable the Add button after action
        IsAddButtonEnabled = false;

        // Mark changes
        AreChangesMade = true;

        // Update Save button state
        UpdateButtonStates();
    }

    private void RemoveFlashcards()
    {
        foreach (var flashcard in SelectedDeckFlashcards.Where(x => x.Value).Select(x => DeckFlashcards.First(f => f.Id == x.Key)).ToList())
        {
            DeckFlashcards.Remove(flashcard);
            AvailableFlashcards.Add(flashcard);
        }

        // Clear the selection after removing
        foreach (var flashcard in SelectedDeckFlashcards.Keys.ToList())
        {
            SelectedDeckFlashcards[flashcard] = false;
        }

        // Disable the Remove button after action
        IsRemoveButtonEnabled = false;

        // Mark changes
        AreChangesMade = true;

        // Update Save button state
        UpdateButtonStates();
    }

    private async void SaveDeck()
    {
        if (DeckFlashcards.Count == 0)
        {
            MessageBox.Show("The deck must contain at least one flashcard!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _selectedDeck.FlashcardIds = DeckFlashcards.Select(f => f.Id).ToList();
        await _deckService.UpdateDeckAsync(_selectedDeck);

        MessageBox.Show("Deck saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        IsSaveButtonEnabled = false;
    }

    private async void OpenMainMenuAsync()
    {
        await _navigationService.GetMainMenuViewAsync();
    }

    public void Initialize(FlashcardDeck selectedDeck)
    {
        _selectedDeck = selectedDeck;
        LoadFlashcards();
    }

    private void ToggleAvailableFlashcardSelection(int flashcardId)
    {
        if (SelectedAvailableFlashcards.ContainsKey(flashcardId))
        {
            SelectedAvailableFlashcards[flashcardId] = !SelectedAvailableFlashcards[flashcardId];
        }
        else
        {
            SelectedAvailableFlashcards[flashcardId] = true;
        }

        UpdateButtonStates();
    }

    private void ToggleDeckFlashcardSelection(int flashcardId)
    {
        if (SelectedDeckFlashcards.ContainsKey(flashcardId))
        {
            SelectedDeckFlashcards[flashcardId] = !SelectedDeckFlashcards[flashcardId];
        }
        else
        {
            SelectedDeckFlashcards[flashcardId] = true;
        }

        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        // Enable Add button if any checkbox is checked in Available Flashcards
        IsAddButtonEnabled = SelectedAvailableFlashcards.Any(x => x.Value);

        // Enable Remove button if any checkbox is checked in Deck Flashcards
        IsRemoveButtonEnabled = SelectedDeckFlashcards.Any(x => x.Value);

        // Enable Save button only if changes have been made (either -> or <- was clicked)
        IsSaveButtonEnabled = AreChangesMade;
    }
}
