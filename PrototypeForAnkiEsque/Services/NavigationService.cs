using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.ViewModels;
using PrototypeForAnkiEsque.Views;
using System.Windows;
using System.Windows.Controls;
// This file is used to define the NavigationService class, which implements the IMainMenuNavigationService, IFlashcardNavigationService, IDeckNavigationService, and ILastNavigatedViewService interfaces.
// The NavigationService class is used to navigate between different views in the application.
// The NavigationService class defines a constructor that takes an instance of the IServiceProvider interface as a parameter. This allows the NavigationService class to get instances of the required services from the DI container.
// Simple explanation: This class is used to navigate between different views in the application.
// Ensure that with each new interface, this class inherits from it and implements the required methods.
namespace PrototypeForAnkiEsque.Services
{
    public class NavigationService : IMainMenuNavigationService, IFlashcardNavigationService, IDeckNavigationService, ILastNavigatedViewService
    {
        private readonly IServiceProvider _serviceProvider;
        private UserControl _lastNavigatedView;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task GetMainMenuViewAsync()
        {
            var mainMenuView = _serviceProvider.GetRequiredService<MainMenuUserControl>();
            await NavigateAsync(mainMenuView, "Main Menu");
        }

        public async Task GetFlashcardViewAsync(FlashcardDeck selectedDeck)
        {
            var flashcardView = _serviceProvider.GetRequiredService<FlashcardViewUserControl>();
            var viewModel = (FlashcardViewModel)flashcardView.DataContext;
            viewModel.SetSelectedDeck(selectedDeck);
            await NavigateAsync(flashcardView, "Flashcard View");
        }

        public async Task GetFlashcardEntryViewAsync()
        {
            var flashcardEntryView = _serviceProvider.GetRequiredService<FlashcardEntryUserControl>();
            await NavigateAsync(flashcardEntryView, "Flashcard Entry");
        }

        public async Task GetFlashcardDatabaseViewAsync()
        {
            var flashcardDatabaseView = _serviceProvider.GetRequiredService<FlashcardDatabaseUserControl>();
            await NavigateAsync(flashcardDatabaseView, "Flashcard Database");
        }

        public async Task GetFlashcardEditorViewAsync(Flashcard flashcard)
        {
            var flashcardEditorView = _serviceProvider.GetRequiredService<FlashcardEditorUserControl>();
            var viewModel = _serviceProvider.GetRequiredService<FlashcardEditorViewModel>();
            viewModel.Initialize(flashcard);
            flashcardEditorView.DataContext = viewModel;
            await NavigateAsync(flashcardEditorView, "Flashcard Editor");
        }

        public async Task GetFlashcardDeckCreatorViewAsync()
        {
            var flashcardDeckCreatorView = _serviceProvider.GetRequiredService<FlashcardDeckCreatorUserControl>();
            await NavigateAsync(flashcardDeckCreatorView, "Deck Creator");
        }

        public async Task GetFlashcardDeckSelectionViewAsync()
        {
            // Create a fresh instance instead of reusing one from the DI container
            var flashcardDeckSelectionView = new FlashcardDeckSelectionUserControl();
            var viewModel = _serviceProvider.GetRequiredService<FlashcardDeckSelectionViewModel>();

            flashcardDeckSelectionView.DataContext = viewModel;

            await NavigateAsync(flashcardDeckSelectionView, "Deck Selection");
            viewModel.RefreshLocalization();
        }


        public async Task GetFlashcardDeckEditorViewAsync(FlashcardDeck selectedDeck)
        {
            var flashcardDeckEditorView = _serviceProvider.GetRequiredService<FlashcardDeckEditorUserControl>();
            var viewModel = _serviceProvider.GetRequiredService<FlashcardDeckEditorViewModel>();
            viewModel.Initialize(selectedDeck);
            flashcardDeckEditorView.DataContext = viewModel;
            await NavigateAsync(flashcardDeckEditorView, "Deck Editor");
        }

        private async Task NavigateAsync(UserControl userControl, string title)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                // Get the current language setting
                var localizationService = _serviceProvider.GetRequiredService<ILocalizationService>();
                var settingsManager = _serviceProvider.GetRequiredService<ISettingsManager>();
                string savedLanguage = settingsManager.GetSavedLanguage();

                // Ensure language is changed immediately
                localizationService.ChangeLanguage(savedLanguage);

                // Force the language to be reflected in the UI elements before the navigation
                userControl.Language = System.Windows.Markup.XmlLanguage.GetLanguage(savedLanguage);
                userControl.UpdateLayout(); // Force layout update to apply language

                // Update window title
                Application.Current.MainWindow.Title = title;

                // Set the content of the main window
                Application.Current.MainWindow.Content = null; // Hide UI
                Application.Current.MainWindow.Content = userControl; // Show UI again
                _lastNavigatedView = userControl;
            });
        }

        public UserControl LastNavigatedView => _lastNavigatedView;
    }
}
