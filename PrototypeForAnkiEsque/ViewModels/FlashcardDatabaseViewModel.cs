using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

public class FlashcardDatabaseViewModel : BaseViewModel
{
    private readonly FlashcardService _flashcardService;
    private readonly NavigationService _navigationService;
    private Flashcard _selectedFlashcard;

    public FlashcardDatabaseViewModel(FlashcardService flashcardService, NavigationService navigationService)
    {
        _flashcardService = flashcardService;
        _navigationService = navigationService;

        Flashcards = new ObservableCollection<Flashcard>(_flashcardService.GetFlashcards());
        EditCommand = new RelayCommand(EditFlashcard, CanEditFlashcard);
        DeleteCommand = new RelayCommand(DeleteFlashcard, CanDeleteFlashcard);
        OpenMainMenuCommand = new RelayCommand(OpenMainMenu);
    }

    public ObservableCollection<Flashcard> Flashcards { get; set; }

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
    public ICommand DeleteCommand { get; }

    private async void DeleteFlashcard()
    {
        if (SelectedFlashcard != null)
        {
            var result = MessageBox.Show("Are you sure you want to delete this flashcard?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _flashcardService.DeleteFlashcardAsync(SelectedFlashcard.Id);
                Flashcards.Remove(SelectedFlashcard);
                SelectedFlashcard = null; // Deselect the flashcard
            }
        }
    }

    private bool CanDeleteFlashcard()
    {
        return SelectedFlashcard != null;
    }

    // Command for opening the main menu
    public ICommand OpenMainMenuCommand { get; }

    private void OpenMainMenu()
    {
        _navigationService.GetMainMenuViewAsync();
    }
}
