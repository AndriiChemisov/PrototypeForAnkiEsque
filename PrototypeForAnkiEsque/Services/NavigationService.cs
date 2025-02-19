using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.ViewModels;
using PrototypeForAnkiEsque.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PrototypeForAnkiEsque.Services
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        // This stores the last navigated view
        private UserControl _lastNavigatedView;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Navigate to the MainMenu view (user control) asynchronously
        public async Task GetMainMenuViewAsync()
        {
            var mainMenuView = _serviceProvider.GetRequiredService<MainMenuUserControl>();
            await NavigateAsync(mainMenuView); // Asynchronously set the content
        }

        // Navigate to the Flashcard view (user control) asynchronously
        public async Task GetFlashcardViewAsync()
        {
            var flashcardView = _serviceProvider.GetRequiredService<FlashcardViewUserControl>();
            await NavigateAsync(flashcardView); // Asynchronously set the content
        }

        // Navigate to the Flashcard Entry view (user control) asynchronously
        public async Task GetFlashcardEntryViewAsync()
        {
            var flashcardEntryView = _serviceProvider.GetRequiredService<FlashcardEntryUserControl>();
            await NavigateAsync(flashcardEntryView); // Asynchronously set the content
        }

        // Navigate to the Flashcard Database view (user control)
        public async Task GetFlashcardDatabaseViewAsync()
        {
            var flashcardDatabaseView = _serviceProvider.GetRequiredService<FlashcardDatabaseUserControl>();
            await NavigateAsync(flashcardDatabaseView); // Asynchronously set the content
        }

        //Navigate to the Flashcard Editor view(user control)
        public async Task GetFlashcardEditorViewAsync(Flashcard flashcard)
        {
            var flashcardEditorView = _serviceProvider.GetRequiredService<FlashcardEditorUserControl>();
            var viewModel = new FlashcardEditorViewModel(
                _serviceProvider.GetRequiredService<FlashcardService>(),
                _serviceProvider.GetRequiredService<NavigationService>(),
                _serviceProvider.GetRequiredService<ApplicationDbContext>()
            );
            viewModel.Initialize(flashcard);
            flashcardEditorView.DataContext = viewModel; // Bind the ViewModel to the UserControl

            await NavigateAsync(flashcardEditorView); // Asynchronously set the content
        }

        // Navigate to the Flashcard Deck Creator view (user control) asynchronously
        public async Task GetFlashcardDeckCreatorViewAsync()
        {
            var flashcardDeckCreatorView = _serviceProvider.GetRequiredService<FlashcardDeckCreatorUserControl>();
            await NavigateAsync(flashcardDeckCreatorView); // Asynchronously set the content
        }

        // General method to perform navigation asynchronously
        private async Task NavigateAsync(UserControl userControl)
        {
            // Ensure this runs on the UI thread
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Application.Current.MainWindow.Content = userControl;
                _lastNavigatedView = userControl; // Store the last navigated view
            });
        }

        // Expose the last navigated view (for potential back/forward navigation)
        public UserControl LastNavigatedView => _lastNavigatedView;
    }
}
