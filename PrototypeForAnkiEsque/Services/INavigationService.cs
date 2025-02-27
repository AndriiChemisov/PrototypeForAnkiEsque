using PrototypeForAnkiEsque.Models;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrototypeForAnkiEsque.Services
{
    public interface IMainMenuNavigationService
    {
        Task GetMainMenuViewAsync();
    }

    public interface IFlashcardNavigationService
    {
        Task GetFlashcardViewAsync(FlashcardDeck selectedDeck);
        Task GetFlashcardEntryViewAsync();
        Task GetFlashcardDatabaseViewAsync();
        Task GetFlashcardEditorViewAsync(Flashcard flashcard);
    }

    public interface IDeckNavigationService
    {
        Task GetFlashcardDeckCreatorViewAsync();
        Task GetFlashcardDeckSelectionViewAsync();
        Task GetFlashcardDeckEditorViewAsync(FlashcardDeck selectedDeck);
    }

    public interface ILastNavigatedViewService
    {
        UserControl LastNavigatedView { get; }
    }
}
