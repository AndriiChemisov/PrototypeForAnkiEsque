using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        // Get the MainMenu view (user control) asynchronously
        public async Task GetMainMenuViewAsync()
        {
            var mainMenuView = _serviceProvider.GetRequiredService<MainMenuUserControl>();
            await NavigateAsync(mainMenuView); // Asynchronously set the content
        }

        // Get the Flashcard view (user control) asynchronously
        public async Task GetFlashcardViewAsync()
        {
            var flashcardView = _serviceProvider.GetRequiredService<FlashcardViewUserControl>();
            await NavigateAsync(flashcardView); // Asynchronously set the content
        }

        // Get the FlashcardEntry view (user control) asynchronously
        public async Task GetFlashcardEntryViewAsync()
        {
            var flashcardEntryView = _serviceProvider.GetRequiredService<FlashcardEntryUserControl>();
            await NavigateAsync(flashcardEntryView); // Asynchronously set the content
        }

        // General method to perform navigation asynchronously
        private async Task NavigateAsync(UserControl userControl)
        {
            // Use the Dispatcher to ensure this runs on the UI thread
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Application.Current.MainWindow.Content = userControl;
                _lastNavigatedView = userControl; // Store the last navigated view
            });
        }

        // Expose the last navigated view
        public UserControl LastNavigatedView => _lastNavigatedView;
    }
}
