using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Views;
using System.Windows;

namespace PrototypeForAnkiEsque.Services
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Get the MainMenu view (user control)
        public void GetMainMenuView()
        {
            Application.Current.MainWindow.Content = _serviceProvider.GetRequiredService<MainMenuUserControl>();
        }

        // Get the Flashcard view (user control)
        public void GetFlashcardView()
        {
            Application.Current.MainWindow.Content = _serviceProvider.GetRequiredService<FlashcardViewUserControl>();
        }

        // Get the FlashcardEntry view (user control)
        public void GetFlashcardEntryView()
        {
            Application.Current.MainWindow.Content = _serviceProvider.GetRequiredService<FlashcardEntryUserControl>();
        }
    }
}
