using PrototypeForAnkiEsque.Models;
using System.Threading.Tasks;
using System.Windows.Controls;
// This file is used to define the navigation services interfaces. These interfaces define the methods that the navigation services classes must implement.
// These interfaces are used to navigate between different views in the application.
// Simple explanation: These interfaces are used to navigate between different views in the application.
// The reason there are multiple interfaces as opposed to one is due to the Interface Segregation Principle.
// As such, with each new view, a new interface needs to be created to handle the navigation to that view.
// And NavigationService.cs needs to inherit it, as well as registered in the DI container in App.xaml.cs.
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
