using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Text.Json;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using System.Collections.ObjectModel;
using PrototypeForAnkiEsque.Commands;
using PrototypeForAnkiEsque.Resources;
// This file is used to define the FlashcardDatabaseViewModel class. The FlashcardDatabaseViewModel class is used to handle the logic for the FlashcardDatabaseView.
// The FlashcardDatabaseViewModel class defines the properties, commands, and methods that are used to interact with the FlashcardDatabaseView.
// Simple explanation: This class is used to handle the logic for the FlashcardDatabaseView.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDatabaseViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly IFlashcardService _flashcardService;
        private readonly IMainMenuNavigationService _mainMenuNavigationService;
        private readonly IFlashcardNavigationService _flashcardNavigationService;
        private readonly IMessageService _messageService;
        private readonly ILocalizationService _localizationService;
        private Flashcard _selectedFlashcard = null!;
        private ObservableCollection<Flashcard> _allFlashcards = new();
        private ObservableCollection<Flashcard> _filteredFlashcards = new();
        private string _searchText = string.Empty;
        private string _backButtonContext;
        private string _previousPageButtonContext;
        private string _nextPageButtonContext;
        private string _newFlashcardButtonContext;
        private string _importFlashcardsButtonContext;
        private string _exportFlashcardsButtonContext;
        private string _editButtonContext;
        private string _deleteButtonContext;
        private string _searchTextBlockContext;

        private const int PageSize = 15;
        private int _currentPage = 1;
        #endregion

        #region CONSTRUCTOR
        public FlashcardDatabaseViewModel(IFlashcardService flashcardService, IMainMenuNavigationService mainMenuNavigationService, 
                                          IFlashcardNavigationService flashcardNavigationService, IMessageService messageService,
                                          ILocalizationService localizationService)
        {
            _flashcardService = flashcardService;
            _mainMenuNavigationService = mainMenuNavigationService;
            _messageService = messageService;
            _flashcardNavigationService = flashcardNavigationService;
            _localizationService = localizationService;
            _localizationService.LanguageChanged += OnLanguageChanged;


            LoadFlashcardsAsync().Wait();

            Flashcards = new ObservableCollection<Flashcard>();

            LoadPage(1);

            EditCommand = new AsyncRelayCommand(EditFlashcardAsync, CanEditFlashcard);
            DeleteCommand = new AsyncRelayCommand(DeleteFlashcardAsync, CanDeleteFlashcard);
            OpenMainMenuCommand = new AsyncRelayCommand(OpenMainMenuAsync);
            PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, CanGoToPreviousPage);
            NextPageCommand = new AsyncRelayCommand(NextPageAsync, CanGoToNextPage);
            OpenFlashcardEntryCommand = new AsyncRelayCommand(OpenFlashcardEntryAsync);
            _messageService = messageService;
            LoadLocalizedTexts();
            _localizationService = localizationService;
        }
        #endregion

        #region PROPERTIES
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
        public Flashcard SelectedFlashcard
        {
            get => _selectedFlashcard;
            set
            {
                SetProperty(ref _selectedFlashcard, value);
                (EditCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string BackButtonContext
        {
            get => _backButtonContext;
            set => SetProperty(ref _backButtonContext, value);
        }

        public string PreviousPageButtonContext
        {
            get => _previousPageButtonContext;
            set => SetProperty(ref _previousPageButtonContext, value);
        }

        public string NextPageButtonContext
        {
            get => _nextPageButtonContext;
            set => SetProperty(ref _nextPageButtonContext, value);
        }

        public string NewFlashcardButtonContext
        {
            get => _newFlashcardButtonContext;
            set => SetProperty(ref _newFlashcardButtonContext, value);
        }

        public string ImportFlashcardsButtonContext
        {
            get => _importFlashcardsButtonContext;
            set => SetProperty(ref _importFlashcardsButtonContext, value);
        }

        public string ExportFlashcardsButtonContext
        {
            get => _exportFlashcardsButtonContext;
            set => SetProperty(ref _exportFlashcardsButtonContext, value);
        }
        
        public string EditButtonContext
        {
            get => _editButtonContext;
            set => SetProperty(ref _editButtonContext, value);
        }

        public string DeleteButtonContext
        {
            get => _deleteButtonContext;
            set => SetProperty(ref _deleteButtonContext, value);
        }

        public string SearchTextBlockContext
        {
            get => _searchTextBlockContext;
            set
            {
                if (_searchTextBlockContext != value)
                {
                    _searchTextBlockContext = value;
                    OnPropertyChanged();
                    ApplySearchFilter();
                }
            }
        }
        #endregion

        #region COMMANDS
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenMainMenuCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand OpenFlashcardEntryCommand { get; }
        #endregion

        #region METHODS
        private async Task LoadFlashcardsAsync()
        {
            var flashcards = await _flashcardService.GetFlashcardsAsync();
            _allFlashcards = new ObservableCollection<Flashcard>(flashcards);
            _filteredFlashcards = new ObservableCollection<Flashcard>(_allFlashcards);
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

        private void LoadPage(int pageNumber)
        {
            _currentPage = pageNumber;
            var pagedFlashcards = _filteredFlashcards.Skip((pageNumber - 1) * PageSize).Take(PageSize);

            // Ensure that UI-bound collection modifications happen on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                Flashcards.Clear();
                foreach (var flashcard in pagedFlashcards)
                {
                    Flashcards.Add(flashcard);
                }
                UpdatePaginationButtons();
            });
        }


        private async Task EditFlashcardAsync()
        {
            if (SelectedFlashcard != null)
            {
                await _flashcardNavigationService.GetFlashcardEditorViewAsync(SelectedFlashcard);
            }
        }

        private bool CanEditFlashcard() => SelectedFlashcard != null;

        public async Task DeleteFlashcardAsync()
        {
            var result = _messageService.ShowMessageWithButton("Are you sure you want to delete this flashcard?", "Delete Flashcard", MessageBoxImage.Question, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {

                    await _flashcardService.DeleteFlashcardAsync(SelectedFlashcard.Id);
                    _allFlashcards.Remove(SelectedFlashcard);
                    ApplySearchFilter();
                }
                catch (Exception ex)
                {
                    _messageService.ShowMessage(ex.Message, "Error deleting flashcard!", MessageBoxImage.Error);
                }

            }

        }

        private bool CanDeleteFlashcard() => SelectedFlashcard != null;

        private async Task OpenMainMenuAsync()
        {
            await _mainMenuNavigationService.GetMainMenuViewAsync();
        }

        private async Task OpenFlashcardEntryAsync()
        {
            await _flashcardNavigationService.GetFlashcardEntryViewAsync();
        }

        private async Task PreviousPageAsync()
        {
            if (_currentPage > 1)
            {
                CurrentPage--;
            }
        }

        private bool CanGoToPreviousPage() => _currentPage > 1;

        private async Task NextPageAsync()
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
            (PreviousPageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            (NextPageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }

        public async Task ExportFlashcardsAsync(string filePath)
        {
            var flashcardDtos = _allFlashcards.Select(f => new FlashcardDto { Front = f.Front, Back = f.Back }).ToList();

            var json = JsonSerializer.Serialize(flashcardDtos);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task ImportFlashcardsAsync(string filePath)

        {
            var json = await File.ReadAllTextAsync(filePath);
            var flashcardDtos = JsonSerializer.Deserialize<List<FlashcardDto>>(json);

            if (flashcardDtos == null)
            {
                return;
            }

            var existingFlashcards = await _flashcardService.GetFlashcardsAsync();
            var newFlashcards = flashcardDtos
                .Where(dto => !existingFlashcards.Any(f => f.Front == dto.Front && f.Back == dto.Back))
                .Select(dto => new Flashcard { Front = dto.Front, Back = dto.Back })
                .ToList();

            if (newFlashcards.Any())
            {
                foreach (var flashcard in newFlashcards)
                {
                    await _flashcardService.AddFlashcardAsync(flashcard);
                }
                _allFlashcards = new ObservableCollection<Flashcard>(await _flashcardService.GetFlashcardsAsync());
                ApplySearchFilter();
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            LoadLocalizedTexts();  // Refresh localized texts when language changes
        }

        private void LoadLocalizedTexts()
        {
            BackButtonContext = Strings.BttnBack;
            PreviousPageButtonContext = Strings.BttnPreviousPage;
            NextPageButtonContext = Strings.BttnNextPage;
            NewFlashcardButtonContext = Strings.BttnNewFlashcard;
            ImportFlashcardsButtonContext = Strings.BttnImportFlashcards;
            ExportFlashcardsButtonContext = Strings.BttnExportFlashcards;
            EditButtonContext = Strings.BttnEdit;
            DeleteButtonContext = Strings.BttnDelete;
            SearchTextBlockContext = Strings.TxtBlkSearch;
        }
            #endregion
    }
}
