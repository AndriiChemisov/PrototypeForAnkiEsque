using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;

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
        ImportFlashcardsCommand = new RelayCommand(async () => await ImportFlashcardsAsync());
        ExportFlashcardsCommand = new RelayCommand(async () => await ExportFlashcardsAsync());
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
    public ICommand ImportFlashcardsCommand { get; }
    public ICommand ExportFlashcardsCommand { get; }

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
            var result = MessageBox.Show(
                "Are you sure you want to delete this flashcard?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await _flashcardService.DeleteFlashcardAsync(SelectedFlashcard.Id);
                _allFlashcards.Remove(SelectedFlashcard);
                ApplySearchFilter();
            }
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

    public async Task ExportFlashcardsAsync()
    {
        var flashcardDtos = _allFlashcards.Select(f => new FlashcardDto { Front = f.Front, Back = f.Back }).ToList();

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            FileName = "Flashcards.json"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var json = JsonSerializer.Serialize(flashcardDtos);
            await File.WriteAllTextAsync(saveFileDialog.FileName, json);
        }
    }

    public async Task ImportFlashcardsAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var json = await File.ReadAllTextAsync(openFileDialog.FileName);
            var flashcardDtos = JsonSerializer.Deserialize<List<FlashcardDto>>(json);

            var existingFlashcards = _flashcardService.GetFlashcards();
            var newFlashcards = flashcardDtos
                .Where(dto => !existingFlashcards.Any(f => f.Front == dto.Front && f.Back == dto.Back))
                .Select(dto => new Flashcard { Front = dto.Front, Back = dto.Back })
                .ToList();

            if (newFlashcards.Any())
            {
                foreach (var flashcard in newFlashcards)
                {
                    _flashcardService.AddFlashcard(flashcard);
                }
                _allFlashcards = new ObservableCollection<Flashcard>(_flashcardService.GetFlashcards());
                ApplySearchFilter();
            }
        }
    }
}
