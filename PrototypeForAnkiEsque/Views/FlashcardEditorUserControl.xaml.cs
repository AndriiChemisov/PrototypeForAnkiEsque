using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;

namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardEditorUserControl : UserControl
    {
        public FlashcardEditorUserControl()
        {
            InitializeComponent();
            Loaded += FlashcardEditorUserControl_Loaded;
            DataContext = App.ServiceProvider.GetRequiredService<FlashcardEditorViewModel>();
        }

        private void FlashcardEditorUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is FlashcardEditorViewModel viewModel)
            {
                viewModel.OnFadeOutMessage -= ViewModel_OnFadeOutMessage;
                viewModel.OnFadeOutMessage += ViewModel_OnFadeOutMessage;
                viewModel.OnValidationError -= ViewModel_OnValidationError;
                viewModel.OnValidationError += ViewModel_OnValidationError;
            }
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

        private void FadeOutAnimation_Completed(object sender, EventArgs e)
        {
            SavedMessageText.Opacity = 1;
            var viewModel = (FlashcardEditorViewModel)DataContext;
            viewModel.IsSavedMessageVisible = false;
        }
    }
}
