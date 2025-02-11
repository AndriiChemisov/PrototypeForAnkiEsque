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
        }

        // Trigger the fade-out animation when the event is called from the ViewModel
        private void ViewModel_OnFadeOutMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            fadeOutStoryboard.Begin();
        }

        // Handle the completion of the fade-out animation
        private void FadeOutAnimation_Completed(object sender, System.EventArgs e)
        {
            // Reset the opacity after the fade-out is complete
            SavedMessageText.Opacity = 1;
        }
    }
}
