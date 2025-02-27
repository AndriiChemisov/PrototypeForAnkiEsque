using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;

namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardEntryUserControl : UserControl
    {
        public FlashcardEntryUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<FlashcardEntryViewModel>();
            var viewModel = (FlashcardEntryViewModel)DataContext;
            viewModel.OnFadeOutMessage += ViewModel_OnFadeOutMessage;
            viewModel.OnValidationError += ViewModel_OnValidationError;
        }

        private void ViewModel_OnFadeOutMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            fadeOutStoryboard.Begin();
        }

        private void ViewModel_OnValidationError(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void FadeOutAnimation_Completed(object sender, System.EventArgs e)
        {
            SavedMessageText.Opacity = 1;
            var viewModel = (FlashcardEntryViewModel)DataContext;
            viewModel.IsSavedMessageVisible = false;
        }
    }
}

