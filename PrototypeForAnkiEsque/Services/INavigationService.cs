using PrototypeForAnkiEsque.Models;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrototypeForAnkiEsque.Services
{
    public interface INavigationService
    {
        Task GetMainMenuViewAsync();
        Task GetFlashcardViewAsync(FlashcardDeck selectedDeck);
        Task GetFlashcardEntryViewAsync();
        Task GetFlashcardDatabaseViewAsync();
        Task GetFlashcardEditorViewAsync(Flashcard flashcard);
        Task GetFlashcardDeckCreatorViewAsync();
        Task GetFlashcardDeckSelectionViewAsync();
        Task GetFlashcardDeckEditorViewAsync(FlashcardDeck selectedDeck);
        UserControl LastNavigatedView { get; }
    }
}
