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
        private IServiceProvider _serviceProvider;
        private ILocalizationService _localizationService;
        private UserControl _lastNavigatedView;

        private string _mainWindowTitleContext;
        private string _mainMenuTitleContext;
        private string _flashcardDatabaseTitleContext;
        private string _flashcardEntryTitleContext;
        private string _flashcardEditorTitleContext;
        private string _flashcardViewTitleContext;
        private string _flashcardDeckSelectionTitleContext;
        private string _flashcardDeckCreatorTitleContext;
        private string _flashcardDeckEditorTitleContext;

        public NavigationService(IServiceProvider serviceProvider, ILocalizationService localizationService)
        {
            _serviceProvider = serviceProvider;
            _localizationService = localizationService;

            _localizationService.LanguageChanged += OnLanguageChanged;
            LoadLocalizedTitles();
        }

        #region PROPERTIES
        public string MainWindowTitleContext
        {
            get => _mainWindowTitleContext;
            set => _mainWindowTitleContext = value;
        }

        public string MainMenuTitleContext
        {
            get => _mainMenuTitleContext;
            set => _mainMenuTitleContext = value;
        }

        public string FlashcardDatabaseTitleContext
        {
            get => _flashcardDatabaseTitleContext;
            set => _flashcardDatabaseTitleContext = value;
        }

        public string FlashcardEntryTitleContext
        {
            get => _flashcardEntryTitleContext;
            set => _flashcardEntryTitleContext = value;
        }

        public string FlashcardEditorTitleContext
        {
            get => _flashcardEditorTitleContext;
            set => _flashcardEditorTitleContext = value;
        }

        public string FlashcardViewTitleContext
        {
            get => _flashcardViewTitleContext;
            set => _flashcardViewTitleContext = value;
        }

        public string FlashcardDeckSelectionTitleContext
        {
            get => _flashcardDeckSelectionTitleContext;
            set => _flashcardDeckSelectionTitleContext = value;
        }

        public string FlashcardDeckCreatorTitleContext
        {
            get => _flashcardDeckCreatorTitleContext;
            set => _flashcardDeckCreatorTitleContext = value;
        }

        public string FlashcardDeckEditorTitleContext
        {
            get => _flashcardDeckEditorTitleContext;
            set => _flashcardDeckEditorTitleContext = value;
        }


        #endregion
        public async Task GetMainMenuViewAsync()
        {
            var mainMenuView = _serviceProvider.GetRequiredService<MainMenuUserControl>();
            await NavigateAsync(mainMenuView, _mainMenuTitleContext);
        }

        public async Task GetFlashcardViewAsync(FlashcardDeck selectedDeck)
        {
            var flashcardView = _serviceProvider.GetRequiredService<FlashcardViewUserControl>();
            var viewModel = (FlashcardViewModel)flashcardView.DataContext;
            viewModel.SetSelectedDeck(selectedDeck);
            await NavigateAsync(flashcardView, _flashcardViewTitleContext);
        }

        public async Task GetFlashcardEntryViewAsync()
        {
            var flashcardEntryView = _serviceProvider.GetRequiredService<FlashcardEntryUserControl>();
            await NavigateAsync(flashcardEntryView, _flashcardEntryTitleContext);
        }

        public async Task GetFlashcardDatabaseViewAsync()
        {
            var flashcardDatabaseView = _serviceProvider.GetRequiredService<FlashcardDatabaseUserControl>();
            await NavigateAsync(flashcardDatabaseView, _flashcardDatabaseTitleContext);
        }

        public async Task GetFlashcardEditorViewAsync(Flashcard flashcard)
        {
            var flashcardEditorView = _serviceProvider.GetRequiredService<FlashcardEditorUserControl>();
            var viewModel = _serviceProvider.GetRequiredService<FlashcardEditorViewModel>();
            viewModel.Initialize(flashcard);
            flashcardEditorView.DataContext = viewModel;
            await NavigateAsync(flashcardEditorView, _flashcardEditorTitleContext);
        }

        public async Task GetFlashcardDeckCreatorViewAsync()
        {
            var flashcardDeckCreatorView = _serviceProvider.GetRequiredService<FlashcardDeckCreatorUserControl>();
            await NavigateAsync(flashcardDeckCreatorView, _flashcardDeckCreatorTitleContext);
        }

        public async Task GetFlashcardDeckSelectionViewAsync()
        {
            // Ensure a fresh ViewModel is created instead of reusing the DI container's instance
            var viewModel = App.ServiceProvider.GetRequiredService<FlashcardDeckSelectionViewModel>();
            var flashcardDeckSelectionView = new FlashcardDeckSelectionUserControl
            {
                DataContext = viewModel
            };

            await NavigateAsync(flashcardDeckSelectionView, _flashcardDeckSelectionTitleContext);
        }



        public async Task GetFlashcardDeckEditorViewAsync(FlashcardDeck selectedDeck)
        {
            var flashcardDeckEditorView = _serviceProvider.GetRequiredService<FlashcardDeckEditorUserControl>();
            var viewModel = _serviceProvider.GetRequiredService<FlashcardDeckEditorViewModel>();
            viewModel.Initialize(selectedDeck);
            flashcardDeckEditorView.DataContext = viewModel;
            await NavigateAsync(flashcardDeckEditorView, _flashcardDeckEditorTitleContext);
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

                // Update window title
                Application.Current.MainWindow.Title = title;
                Application.Current.MainWindow.Content = userControl; // Show UI 
                _lastNavigatedView = userControl;
            });
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            LoadLocalizedTitles();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.MainWindow.Title = _mainMenuTitleContext;
            });
        }

        private void LoadLocalizedTitles()
        {
            _mainWindowTitleContext = _localizationService.GetString("WndwTitleMainWindow");
            _mainMenuTitleContext = _localizationService.GetString("WndwTitleMainMenu");
            _flashcardDatabaseTitleContext = _localizationService.GetString("WndwTitleFlashcardDatabase");
            _flashcardEntryTitleContext = _localizationService.GetString("WndwTitleFlashcardEntry");
            _flashcardEditorTitleContext = _localizationService.GetString("WndwTitleFlashcardEditor");
            _flashcardViewTitleContext = _localizationService.GetString("WndwTitleFlashcardView");
            _flashcardDeckSelectionTitleContext = _localizationService.GetString("WndwTitleDeckSelection");
            _flashcardDeckCreatorTitleContext = _localizationService.GetString("WndwTitleDeckCreator");
            _flashcardDeckEditorTitleContext = _localizationService.GetString("WndwTitleDeckEditor");
        }

        public UserControl LastNavigatedView => _lastNavigatedView;
    }
}
