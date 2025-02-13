using PrototypeForAnkiEsque.ViewModels;
using PrototypeForAnkiEsque;
using System.Windows.Controls;
using PrototypeForAnkiEsque.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media.Animation;
using System.Windows;

namespace PrototypeForAnkiEsque.Views
{

    public partial class FlashcardEditorUserControl : UserControl
    {
        public FlashcardEditorUserControl()
        {
            InitializeComponent();

            // Initialize ViewModel and set DataContext
            DataContext = App.ServiceProvider.GetRequiredService<FlashcardEditorViewModel>();
            var viewModel = (FlashcardEditorViewModel)DataContext;

            // Subscribe to events in ViewModel
            viewModel.OnFadeOutMessage += ViewModel_OnFadeOutMessage;  // Hook up fade-out animation
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
