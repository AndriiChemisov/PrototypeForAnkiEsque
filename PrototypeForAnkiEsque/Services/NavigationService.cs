using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Views;
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

        // Get the MainMenu view (user control)
        public void GetMainMenuView()
        {
            var mainMenuView = _serviceProvider.GetRequiredService<MainMenuUserControl>();
            Application.Current.MainWindow.Content = mainMenuView;
            _lastNavigatedView = mainMenuView; // Store the last navigated view
        }

        // Get the Flashcard view (user control)
        public void GetFlashcardView()
        {
            var flashcardView = _serviceProvider.GetRequiredService<FlashcardViewUserControl>();
            Application.Current.MainWindow.Content = flashcardView;
            _lastNavigatedView = flashcardView; // Store the last navigated view
        }

        // Get the FlashcardEntry view (user control)
        public void GetFlashcardEntryView()
        {
            var flashcardEntryView = _serviceProvider.GetRequiredService<FlashcardEntryUserControl>();
            Application.Current.MainWindow.Content = flashcardEntryView;
            _lastNavigatedView = flashcardEntryView; // Store the last navigated view
        }

        // Expose the last navigated view
        public UserControl LastNavigatedView => _lastNavigatedView;
    }
}
