using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

public class FlashcardDatabaseViewModel : BaseViewModel
{
    private readonly FlashcardService _flashcardService;
    private readonly NavigationService _navigationService;
    private Flashcard _selectedFlashcard;
    private ObservableCollection<Flashcard> _allFlashcards;
    private ObservableCollection<Flashcard> _filteredFlashcards;
    private string _searchText = string.Empty;

    private const int PageSize = 15;
    private int _currentPage = 1;

    public FlashcardDatabaseViewModel(FlashcardService flashcardService, NavigationService navigationService)
    {
        _flashcardService = flashcardService;
        _navigationService = navigationService;

        _allFlashcards = new ObservableCollection<Flashcard>(_flashcardService.GetFlashcards());
        _filteredFlashcards = new ObservableCollection<Flashcard>(_allFlashcards);
        Flashcards = new ObservableCollection<Flashcard>();

        LoadPage(1);

        EditCommand = new RelayCommand(EditFlashcard, CanEditFlashcard);
        DeleteCommand = new RelayCommand(DeleteFlashcard, CanDeleteFlashcard);
        OpenMainMenuCommand = new RelayCommand(OpenMainMenu);
        PreviousPageCommand = new RelayCommand(PreviousPage, CanGoToPreviousPage);
        NextPageCommand = new RelayCommand(NextPage, CanGoToNextPage);
        OpenFlashcardEntryCommand = new RelayCommand(OpenFlashcardEntryAsync);
    }

    public ObservableCollection<Flashcard> Flashcards { get; set; }

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                LoadPage(_currentPage);
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
                ApplySearchFilter();
            }
        }
    }

    private void ApplySearchFilter()
    {
        _filteredFlashcards = string.IsNullOrWhiteSpace(SearchText)
            ? new ObservableCollection<Flashcard>(_allFlashcards)
            : new ObservableCollection<Flashcard>(
                _allFlashcards.Where(f =>
                    f.Front.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    f.Back.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );

        CurrentPage = 1;
        LoadPage(CurrentPage);
    }

    public Flashcard SelectedFlashcard
    {
        get => _selectedFlashcard;
        set
        {
            SetProperty(ref _selectedFlashcard, value);
            (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand OpenMainMenuCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand OpenFlashcardEntryCommand { get; }

    private void LoadPage(int pageNumber)
    {
        _currentPage = pageNumber;
        var pagedFlashcards = _filteredFlashcards.Skip((pageNumber - 1) * PageSize).Take(PageSize);
        Flashcards.Clear();
        foreach (var flashcard in pagedFlashcards)
        {
            Flashcards.Add(flashcard);
        }
        UpdatePaginationButtons();
    }

    private void EditFlashcard()
    {
        if (SelectedFlashcard != null)
        {
            _navigationService.GetFlashcardEditorViewAsync(SelectedFlashcard);
        }
    }

    private bool CanEditFlashcard() => SelectedFlashcard != null;

    private async void DeleteFlashcard()
    {
        if (SelectedFlashcard != null)
        {
            await _flashcardService.DeleteFlashcardAsync(SelectedFlashcard.Id);
            _allFlashcards.Remove(SelectedFlashcard);
            ApplySearchFilter();
        }
    }

    private bool CanDeleteFlashcard() => SelectedFlashcard != null;

    private async void OpenMainMenu()
    {
        await _navigationService.GetMainMenuViewAsync();
    }

    private async void OpenFlashcardEntryAsync()
    {
        await _navigationService.GetFlashcardEntryViewAsync();
    }

    private void PreviousPage()
    {
        if (_currentPage > 1)
        {
            CurrentPage--;
        }
    }

    private bool CanGoToPreviousPage() => _currentPage > 1;

    private void NextPage()
    {
        int totalPages = (_filteredFlashcards.Count + PageSize - 1) / PageSize;
        if (_currentPage < totalPages)
        {
            CurrentPage++;
        }
    }

    private bool CanGoToNextPage()
    {
        int totalPages = (_filteredFlashcards.Count + PageSize - 1) / PageSize;
        return _currentPage < totalPages;
    }

    private void UpdatePaginationButtons()
    {
        (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
