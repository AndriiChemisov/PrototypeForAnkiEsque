using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;

public class FlashcardDeckCreatorViewModel : BaseViewModel
{
    private readonly DeckService _deckService;
    private readonly NavigationService _navigationService;
    private readonly FlashcardService _flashcardService;
    private string _deckName;
    private string _searchAvailableText;
    private string _searchSelectedText;
    private bool _isAddButtonEnabled;
    private bool _isRemoveButtonEnabled;
    private bool _areChangesMade;

    public ObservableCollection<Flashcard> AvailableFlashcards { get; } = new();
    public ObservableCollection<Flashcard> SelectedFlashcards { get; } = new();

    public ObservableCollection<Flashcard> FilteredAvailableFlashcards { get; } = new();
    public ObservableCollection<Flashcard> FilteredSelectedFlashcards { get; } = new();

    public Dictionary<int, bool> SelectedAvailableFlashcards { get; set; } = new();
    public Dictionary<int, bool> SelectedDeckFlashcards { get; set; } = new();

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
            OnPropertyChanged();
            AreChangesMade = true;
            UpdateButtonStates();
        }
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

    public FlashcardDeckCreatorViewModel(DeckService deckService, NavigationService navigationService, FlashcardService flashcardService)
    {
        _deckService = deckService;
        _navigationService = navigationService;
        _flashcardService = flashcardService;

        OpenDeckSelectionCommand = new RelayCommand(async () => await OpenDeckSelectionAsync());
        AddFlashcardsCommand = new RelayCommand(AddFlashcards);
        RemoveFlashcardsCommand = new RelayCommand(RemoveFlashcards);
        SaveDeckCommand = new RelayCommand(async () => await SaveDeckAsync());
        ToggleAvailableFlashcardSelectionCommand = new RelayCommand<int>(ToggleAvailableFlashcardSelection);
        ToggleDeckFlashcardSelectionCommand = new RelayCommand<int>(ToggleDeckFlashcardSelection);

        LoadFlashcards();
    }

    private void LoadFlashcards()
    {
        AvailableFlashcards.Clear();
        SelectedFlashcards.Clear();
        FilteredAvailableFlashcards.Clear();
        FilteredSelectedFlashcards.Clear();

        var allFlashcards = _flashcardService.GetFlashcards();

        foreach (var flashcard in allFlashcards)
        {
            AvailableFlashcards.Add(flashcard);
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
            SelectedFlashcards.Add(flashcard);
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
            .Select(x => SelectedFlashcards.First(f => f.Id == x.Key))
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
        if (string.IsNullOrWhiteSpace(DeckName) || !SelectedFlashcards.Any() || _deckService.DeckExists(DeckName))
            return;

        var flashcardIds = SelectedFlashcards.Select(f => f.Id).ToList();
        string easeRating = _deckService.CalculateEaseRating(flashcardIds);

        await _deckService.CreateDeckAsync(DeckName, flashcardIds, easeRating);

        // Reset fields
        DeckName = string.Empty;
        AreChangesMade = false;
        LoadFlashcards();
        MessageBox.Show("Deck successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

    private void ToggleAvailableFlashcardSelection(int flashcardId)
    {
        if (SelectedAvailableFlashcards.ContainsKey(flashcardId))
            SelectedAvailableFlashcards[flashcardId] = !SelectedAvailableFlashcards[flashcardId];
        else
            SelectedAvailableFlashcards[flashcardId] = true;

        // Force UI update
        OnPropertyChanged(nameof(SelectedAvailableFlashcards));
        UpdateButtonStates();
    }

    private void ToggleDeckFlashcardSelection(int flashcardId)
    {
        if (SelectedDeckFlashcards.ContainsKey(flashcardId))
            SelectedDeckFlashcards[flashcardId] = !SelectedDeckFlashcards[flashcardId];
        else
            SelectedDeckFlashcards[flashcardId] = true;

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
