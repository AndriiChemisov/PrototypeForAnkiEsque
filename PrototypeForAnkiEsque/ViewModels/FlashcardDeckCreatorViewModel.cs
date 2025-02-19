using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class FlashcardDeckCreatorViewModel : BaseViewModel
    {
        private readonly FlashcardService _flashcardService;
        private readonly DeckService _deckService;
        private readonly NavigationService _navigationService;
        private ObservableCollection<Flashcard> _allFlashcards;
        private const int PageSize = 15;
        private int _currentPage = 1;

        public FlashcardDeckCreatorViewModel(FlashcardService flashcardService, DeckService deckService, NavigationService navigationService)
        {
            _flashcardService = flashcardService;
            _deckService = deckService;
            _navigationService = navigationService;

            _allFlashcards = new ObservableCollection<Flashcard>(_flashcardService.GetFlashcards());
            Flashcards = new ObservableCollection<Flashcard>();
            SelectedFlashcards = new Dictionary<int, bool>(); // Track selection by ID

            LoadPage(_currentPage);

            PreviousPageCommand = new RelayCommand(PreviousPage, CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(NextPage, CanGoToNextPage);
            CreateDeckCommand = new RelayCommand(CreateDeck, CanCreateDeck);
            OpenMainMenuCommand = new RelayCommand(OpenMainMenu);
            ToggleFlashcardSelectionCommand = new RelayCommand<int>(ToggleSelection);

        }

        public ObservableCollection<Flashcard> Flashcards { get; set; }
        public Dictionary<int, bool> SelectedFlashcards { get; set; } // Track selected flashcard IDs

        public string DeckName { get; set; }

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

        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand CreateDeckCommand { get; }
        public ICommand OpenMainMenuCommand { get; }
        public ICommand ToggleFlashcardSelectionCommand { get; }

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

        private void PreviousPage()
        {
            if (_currentPage > 1) CurrentPage--;
        }

        private bool CanGoToPreviousPage() => _currentPage > 1;

        private void NextPage()
        {
            int totalPages = (_allFlashcards.Count + PageSize - 1) / PageSize;
            if (_currentPage < totalPages) CurrentPage++;
        }

        private bool CanGoToNextPage() => _currentPage < (_allFlashcards.Count + PageSize - 1) / PageSize;

        private async void CreateDeck()
        {
            var selectedIds = Flashcards.Where(f => SelectedFlashcards.ContainsKey(f.Id) && SelectedFlashcards[f.Id])
                                        .Select(f => f.Id)
                                        .ToList();

            if (!selectedIds.Any())
            {
                MessageBox.Show("No flashcards selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_deckService.DeckExists(DeckName))
            {
                MessageBox.Show("A deck with that name already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string easeRating = _deckService.CalculateEaseRating(selectedIds);

            await _deckService.CreateDeckAsync(DeckName, selectedIds, easeRating);
            MessageBox.Show("Deck created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            SelectedFlashcards.Clear();
            DeckName = string.Empty;
        }

        private bool CanCreateDeck() => !string.IsNullOrEmpty(DeckName) && SelectedFlashcards.Values.Any(v => v);

        private async void OpenMainMenu()
        {
            await _navigationService.GetMainMenuViewAsync();
        }

        private void ToggleSelection(int flashcardId)
        {
            if (SelectedFlashcards.ContainsKey(flashcardId))
            {
                SelectedFlashcards[flashcardId] = !SelectedFlashcards[flashcardId];
            }
            else
            {
                SelectedFlashcards[flashcardId] = true; // Add to selection if not already present
            }
            // Ensure the "Create Deck" button gets enabled/disabled based on the selection
            (CreateDeckCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

    }

}
