using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Linq;

public class FlashcardDatabaseViewModel : BaseViewModel
{
    private readonly FlashcardService _flashcardService;
    private readonly NavigationService _navigationService;
    private Flashcard _selectedFlashcard;
    private ObservableCollection<Flashcard> _allFlashcards;

    private const int PageSize = 15; // Number of items per page
    private int _currentPage = 1; // Current page, starts from 1

    public FlashcardDatabaseViewModel(FlashcardService flashcardService, NavigationService navigationService)
    {
        _flashcardService = flashcardService;
        _navigationService = navigationService;

        _allFlashcards = new ObservableCollection<Flashcard>(_flashcardService.GetFlashcards());
        Flashcards = new ObservableCollection<Flashcard>();

        LoadPage(_currentPage);

        EditCommand = new RelayCommand(EditFlashcard, CanEditFlashcard);
        DeleteCommand = new RelayCommand(DeleteFlashcard, CanDeleteFlashcard);
        OpenMainMenuCommand = new RelayCommand(OpenMainMenu);
        PreviousPageCommand = new RelayCommand(PreviousPage, CanGoToPreviousPage);
        NextPageCommand = new RelayCommand(NextPage, CanGoToNextPage);
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

    public Flashcard SelectedFlashcard
    {
        get => _selectedFlashcard;
        set
        {
            if (value != null && !string.IsNullOrWhiteSpace(value.Front)) // Check if the flashcard is valid (not empty)
            {
                SetProperty(ref _selectedFlashcard, value);
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Enable the Edit button
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Enable the Delete button
            }
            else
            {
                // If it's the extra row (null or empty), set the selected flashcard to null
                _selectedFlashcard = null;
            }
        }
    }

    // Command for editing the selected flashcard
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand OpenMainMenuCommand { get; }
    // Commands for pagination
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }


    // Pagination logic
    private void LoadPage(int pageNumber)
    {
        _currentPage = pageNumber;
        var pagedFlashcards = _allFlashcards.Skip((pageNumber - 1) * PageSize).Take(PageSize);
        Flashcards.Clear();
        foreach (var flashcard in pagedFlashcards)
        {
            Flashcards.Add(flashcard);
        }
        (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void EditFlashcard()
    {
        if (SelectedFlashcard != null)
        {
            // Navigate to the FlashcardEditorView with the selected flashcard
            _navigationService.GetFlashcardEditorViewAsync(SelectedFlashcard);
        }
    }

    private bool CanEditFlashcard()
    {
        return SelectedFlashcard != null;
    }

    // Command for deleting the selected flashcard

    private async void DeleteFlashcard()
    {
        if (SelectedFlashcard != null)
        {
            var result = MessageBox.Show("Are you sure you want to delete this flashcard?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _flashcardService.DeleteFlashcardAsync(SelectedFlashcard.Id);
                _allFlashcards.Remove(SelectedFlashcard);
                SelectedFlashcard = null; // Deselect the flashcard
                CalculatePagination();
            }
        }
    }

    // Recalculate pagination after a deletion
    private void CalculatePagination()
    {
        int totalItems = _allFlashcards.Count;
        int totalPages = (totalItems + PageSize - 1) / PageSize;

        // Adjust current page if it's out of range
        if (_currentPage > totalPages)
        {
            _currentPage = totalPages > 0 ? totalPages : 1; // Go to the last page if current is invalid, or first if no pages
        }

        // Update page buttons visibility based on current page and total pages
        LoadPage(_currentPage);
        UpdatePaginationButtons();
    }

    private void UpdatePaginationButtons()
    {
        // Enable/Disable buttons based on current page and total pages
        (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool CanDeleteFlashcard()
    {
        return SelectedFlashcard != null;
    }

    // Command for opening the main menu

    private void OpenMainMenu()
    {
        _navigationService.GetMainMenuViewAsync();
    }

    // Previous page command logic
    private void PreviousPage()
    {
        if (_currentPage > 1)
        {
            CurrentPage--;
        }
    }

    private bool CanGoToPreviousPage()
    {
        return _currentPage > 1;
    }

    // Next page command logic
    private void NextPage()
    {
        int totalPages = (_allFlashcards.Count + PageSize - 1) / PageSize; // Calculate total pages
        if (_currentPage < totalPages)
        {
            CurrentPage++;
        }
    }

    private bool CanGoToNextPage()
    {
        int totalPages = (_allFlashcards.Count + PageSize - 1) / PageSize; // Calculate total pages
        return _currentPage < totalPages;
    }
}
