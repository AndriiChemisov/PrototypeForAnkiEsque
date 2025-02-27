using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.ViewModels;
using PrototypeForAnkiEsque.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            var flashcardDeckSelectionView = _serviceProvider.GetRequiredService<FlashcardDeckSelectionUserControl>();
            await NavigateAsync(flashcardDeckSelectionView, "Deck Selection");
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
                Application.Current.MainWindow.Title = title;
                Application.Current.MainWindow.Content = userControl;
                _lastNavigatedView = userControl;
            });
        }

        public UserControl LastNavigatedView => _lastNavigatedView;
    }
}
