using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

public class FlashcardDeckCreatorViewModel : BaseViewModel
{
    private readonly DeckService _deckService;
    private readonly NavigationService _navigationService;
    private readonly FlashcardService _flashcardService;
    private string _deckName;

    public ObservableCollection<Flashcard> AvailableFlashcards { get; } = new();
    public ObservableCollection<Flashcard> SelectedFlashcards { get; } = new();

    public Dictionary<int, bool> SelectedAvailableFlashcards { get; set; } = new();
    public Dictionary<int, bool> SelectedDeckFlashcards { get; set; } = new();

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
                OnPropertyChanged(nameof(IsAddButtonEnabled));
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
                OnPropertyChanged(nameof(IsRemoveButtonEnabled));
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
                OnPropertyChanged(nameof(IsSaveButtonEnabled));
            }
        }
    }

    public string DeckName
    {
        get => _deckName;
        set
        {
            _deckName = value;
            OnPropertyChanged();
            AreChangesMade = true;  // Mark as changed when the name changes
            UpdateButtonStates();
        }
    }

    public ICommand AddFlashcardsCommand { get; }
    public ICommand RemoveFlashcardsCommand { get; }
    public ICommand SaveDeckCommand { get; }
    public ICommand OpenDeckSelectionCommand { get; }
    public ICommand ToggleAvailableFlashcardSelectionCommand { get; }
    public ICommand ToggleDeckFlashcardSelectionCommand { get; }

    public FlashcardDeckCreatorViewModel(DeckService deckService, NavigationService navigationService, FlashcardService flashcardService)
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
        AvailableFlashcards.Clear();
        SelectedFlashcards.Clear();

        var allFlashcards = _flashcardService.GetFlashcards();

        foreach (var flashcard in allFlashcards)
        {
            AvailableFlashcards.Add(flashcard);
        }

        UpdateButtonStates();
    }

    private void AddFlashcards()
    {
        foreach (var flashcard in SelectedAvailableFlashcards.Where(x => x.Value).Select(x => AvailableFlashcards.First(f => f.Id == x.Key)).ToList())
        {
            if (!SelectedFlashcards.Contains(flashcard))
            {
                SelectedFlashcards.Add(flashcard);
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
        foreach (var flashcard in SelectedDeckFlashcards.Where(x => x.Value).Select(x => SelectedFlashcards.First(f => f.Id == x.Key)).ToList())
        {
            SelectedFlashcards.Remove(flashcard);
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
        // Check if deck name is empty or contains only spaces
        if (string.IsNullOrWhiteSpace(DeckName))
        {
            MessageBox.Show("The deck must have a name!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Check if the deck name already exists
        if (_deckService.CheckIfDeckNameExists(DeckName))
        {
            MessageBox.Show("A deck with this name already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Proceed with saving the deck
        await _deckService.CreateDeckAsync(DeckName, SelectedFlashcards.Select(f => f.Id).ToList(),
              _deckService.CalculateEaseRating(SelectedFlashcards.Select(f => f.Id).ToList()));

        MessageBox.Show("Deck saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        // Reset everything after saving
        ResetDeckCreation();

        // Disable the Save button after action
        IsSaveButtonEnabled = false;
    }

    private void ResetDeckCreation()
    {
        // Clear the selection and the deck name
        DeckName = string.Empty;
        SelectedFlashcards.Clear();
        AvailableFlashcards.Clear();
        SelectedAvailableFlashcards.Clear();
        SelectedDeckFlashcards.Clear();

        // Reload flashcards after clearing
        LoadFlashcards();

        // Reset button states
        IsAddButtonEnabled = false;
        IsRemoveButtonEnabled = false;
    }

    private async void OpenDeckSelectionAsync()
    {
        await _navigationService.GetFlashcardDeckSelectionViewAsync();
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

        // Enable Save button only if changes have been made and the deck name is valid
        IsSaveButtonEnabled = AreChangesMade && !string.IsNullOrWhiteSpace(DeckName);
    }
}
