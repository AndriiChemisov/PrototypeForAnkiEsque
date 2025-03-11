using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.Commands;
using PrototypeForAnkiEsque.Resources;
using System.IO;
using System.Windows;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckSelectionViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly IDeckService _deckService;
        private readonly IMainMenuNavigationService _mainMenuNavigationService;
        private readonly IFlashcardNavigationService _flashcardNavigationService;
        private readonly IDeckNavigationService _deckNavigationService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageService _messageService;
        private FlashcardDeck _selectedDeck;
        private string _errorMessage;
        private string _searchText;

        private string _mainMenuButtonContent;
        private string _deckCreatorButtonContent;
        private string _importDecksButtonContent;
        private string _exportDecksButtonContent;
        private string _reviewDeckButtonContent;
        private string _editDeckButtonContent;
        private string _deleteDeckButtonContent;
        private string _searchTextBoxContext;
        private string _gridDeckNameHeaderContext;
        private string _gridProgressHeaderContext;
        #endregion

        #region CONSTRUCTOR
        public FlashcardDeckSelectionViewModel(
            IDeckService deckService,
            IMainMenuNavigationService mainMenuNavigationService,
            IFlashcardNavigationService flashcardNavigationService,
            IDeckNavigationService deckNavigationService,
            IMessageService messageService,
            ILocalizationService localizationService)
        {
            _deckService = deckService;
            _mainMenuNavigationService = mainMenuNavigationService;
            _flashcardNavigationService = flashcardNavigationService;
            _deckNavigationService = deckNavigationService;
            _messageService = messageService;
            _localizationService = localizationService;

            _localizationService.LanguageChanged += OnLanguageChanged;

            ReviewDeckCommand = new AsyncRelayCommand(ReviewDeckAsync);
            EditDeckCommand = new AsyncRelayCommand(EditDeckAsync);
            DeleteDeckCommand = new AsyncRelayCommand(DeleteDeckAsync);
            OpenMainMenuCommand = new AsyncRelayCommand(OpenMainMenuAsync);
            OpenDeckCreatorCommand = new AsyncRelayCommand(OpenDeckCreatorAsync);
            ImportDecksCommand = new AsyncRelayCommand<string>(ImportDecksAsync);
            ExportDecksCommand = new AsyncRelayCommand<string>(ExportDecksAsync);

            LoadDecksAsync();
        }
        #endregion

        #region PROPERTIES
        public ObservableCollection<FlashcardDeck> Decks { get; private set; } = new();
        public ObservableCollection<FlashcardDeck> FilteredDecks { get; private set; } = new();

        public FlashcardDeck SelectedDeck
        {
            get => _selectedDeck;
            set => SetProperty(ref _selectedDeck, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
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
                    UpdateFilteredDecks();
                }
            }
        }

        public string MainMenuButtonContent
        {
            get => _mainMenuButtonContent;
            set => SetProperty(ref _mainMenuButtonContent, value);
        }

        public string DeckCreatorButtonContent
        {
            get => _deckCreatorButtonContent;
            set => SetProperty(ref _deckCreatorButtonContent, value);
        }

        public string ImportDecksButtonContent
        {
            get => _importDecksButtonContent;
            set => SetProperty(ref _importDecksButtonContent, value);
        }

        public string ExportDecksButtonContent
        {
            get => _exportDecksButtonContent;
            set => SetProperty(ref _exportDecksButtonContent, value);
        }

        public string ReviewDeckButtonContent
        {
            get => _reviewDeckButtonContent;
            set => SetProperty(ref _reviewDeckButtonContent, value);
        }

        public string EditDeckButtonContent
        {
            get => _editDeckButtonContent;
            set => SetProperty(ref _editDeckButtonContent, value);
        }

        public string DeleteDeckButtonContent
        {
            get => _deleteDeckButtonContent;
            set => SetProperty(ref _deleteDeckButtonContent, value);
        }

        public string SearchTextBoxContext
        {
            get => _searchTextBoxContext;
            set => SetProperty(ref _searchTextBoxContext, value);
        }

        public string GridDeckNameHeaderContext
        {
            get => _gridDeckNameHeaderContext;
            set => SetProperty(ref _gridDeckNameHeaderContext, value);
        }

        public string GridProgressHeaderContext
        {
            get => _gridProgressHeaderContext;
            set => SetProperty(ref _gridProgressHeaderContext, value);
        }
        #endregion

        #region COMMANDS
        public ICommand ReviewDeckCommand { get; }
        public ICommand EditDeckCommand { get; }
        public ICommand DeleteDeckCommand { get; }
        public ICommand OpenMainMenuCommand { get; }
        public ICommand OpenDeckCreatorCommand { get; }
        public ICommand ImportDecksCommand { get; }
        public ICommand ExportDecksCommand { get; }
        #endregion

        #region METHODS
        private async void LoadDecksAsync()
        {
            try
            {
                var decks = await _deckService.GetPagedDecksAsync(1, int.MaxValue);
                Decks = new ObservableCollection<FlashcardDeck>(decks);
                UpdateFilteredDecks();
                LoadLocalizedTexts();
            }
            catch (Exception ex)
            {
                _messageService.ShowMessage($"An error occurred while loading decks: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        private void UpdateFilteredDecks()
        {
            FilteredDecks.Clear();
            foreach (var deck in Decks.Where(d => string.IsNullOrEmpty(SearchText) || d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredDecks.Add(deck);
            }
        }

        private async Task ReviewDeckAsync()
        {
            if (SelectedDeck == null)
            {
                _messageService.ShowMessage("Please select a deck to review.", "Warning", MessageBoxImage.Warning);
                return;
            }

            await _flashcardNavigationService.GetFlashcardViewAsync(SelectedDeck);
        }

        private async Task EditDeckAsync()
        {
            if (SelectedDeck == null)
            {
                _messageService.ShowMessage("Please select a deck to edit.", "Warning", MessageBoxImage.Warning);
                return;
            }

            await _deckNavigationService.GetFlashcardDeckEditorViewAsync(SelectedDeck);
        }

        private async Task DeleteDeckAsync()
        {
            if (SelectedDeck == null)
            {
                _messageService.ShowMessage("Please select a deck to delete.", "Warning", MessageBoxImage.Warning);
                return;
            }

            var result = _messageService.ShowMessageWithButton("Are you sure you want to delete this deck?", "Confirm Deletion", MessageBoxImage.Question, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                await _deckService.DeleteDeckAsync(SelectedDeck.Id);
                Decks.Remove(SelectedDeck);
                UpdateFilteredDecks();
            }
        }

        private async Task OpenMainMenuAsync()
        {
            await _mainMenuNavigationService.GetMainMenuViewAsync();
        }

        private async Task OpenDeckCreatorAsync()
        {
            await _deckNavigationService.GetFlashcardDeckCreatorViewAsync();
        }

        public async Task ExportDecksAsync(string filePath)
        {
            var decksToExport = SelectedDeck != null ? new List<FlashcardDeck> { SelectedDeck } : Decks.ToList();
            var deckDtos = decksToExport.Select(d => new FlashcardDeckDto
            {
                DeckName = d.Name,
                Flashcards = d.FlashcardFronts.Select(f => new FlashcardDto { Front = f, Back = "" }).ToList(),
                EaseRating = d.EaseRating
            }).ToList();

            var json = JsonSerializer.Serialize(deckDtos);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task ImportDecksAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var deckDtos = JsonSerializer.Deserialize<List<FlashcardDeckDto>>(json);

            var existingDecks = await _deckService.GetPagedDecksAsync(1, int.MaxValue);
            var newDecks = deckDtos
                .Where(dto => !existingDecks.Any(d => d.Name == dto.DeckName))
                .Select(dto => new FlashcardDeck
                {
                    Name = dto.DeckName,
                    FlashcardFronts = dto.Flashcards.Select(f => f.Front).ToList(),
                    EaseRating = dto.EaseRating ?? "Hard"
                })
                .ToList();

            if (newDecks.Any())
            {
                foreach (var deck in newDecks)
                {
                    await _deckService.CreateDeckAsync(deck.Name, deck.FlashcardFronts, deck.EaseRating);
                }
                LoadDecksAsync();
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            LoadLocalizedTexts();  // Refresh localized texts when language changes
        }

        private void LoadLocalizedTexts()
        {
            MainMenuButtonContent = _localizationService.GetString("BttnMainMenu");
            DeckCreatorButtonContent = _localizationService.GetString("BttnNewDeck");
            ImportDecksButtonContent = _localizationService.GetString("BttnImportDeck");
            ExportDecksButtonContent = _localizationService.GetString("BttnExportDeck");
            ReviewDeckButtonContent = _localizationService.GetString("BttnReview");
            EditDeckButtonContent = _localizationService.GetString("BttnEdit");
            DeleteDeckButtonContent = _localizationService.GetString("BttnDelete");
            SearchTextBoxContext = _localizationService.GetString("TxtBlkSearch");
            GridDeckNameHeaderContext = _localizationService.GetString("GrdHdrDeckName");
            GridProgressHeaderContext = _localizationService.GetString("GrdHdrProgress");
        }
        #endregion
    }
}
