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
            // Initialize ViewModel and set DataContext
            // Subscribe to events in ViewModel
            InitializeComponent();
            Loaded += FlashcardEditorUserControl_Loaded; //Without this the animation doesn't trigger
            DataContext = App.ServiceProvider.GetRequiredService<FlashcardEditorViewModel>();
        }

        private void FlashcardEditorUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is FlashcardEditorViewModel viewModel)
            {
                viewModel.OnFadeOutMessage -= ViewModel_OnFadeOutMessage; // Avoid duplicate subscriptions
                viewModel.OnFadeOutMessage += ViewModel_OnFadeOutMessage;
            }
        }

        private void ViewModel_OnFadeOutMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            fadeOutStoryboard.Begin();
        }

        // Handle the completion of the fade-out animation
        private void FadeOutAnimation_Completed(object sender, EventArgs e)
        {
            SavedMessageText.Opacity = 1;
            var viewModel = (FlashcardEditorViewModel)DataContext;
            viewModel.IsSavedMessageVisible = false;
        }

    }

}
