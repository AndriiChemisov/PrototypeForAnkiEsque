using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
using PrototypeForAnkiEsque.Services;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PrototypeForAnkiEsque.Views
{
    public partial class MainMenuUserControl : UserControl
    {
        public MainMenuUserControl()
        {
            InitializeComponent();

            var navigationService = App.ServiceProvider.GetRequiredService<NavigationService>();
            var viewModel = App.ServiceProvider.GetRequiredService<MainMenuViewModel>();

            DataContext = viewModel;
            viewModel.OnFadeOutMessage += ViewModel_OnFadeOutMessage;

            // Check if we came from FlashcardViewUserControl and trigger animation if true
            if (navigationService.LastNavigatedView is FlashcardViewUserControl)
            {
                TriggerFadeOutAnimation();
            }
        }

        // Trigger the fade-out animation when the event is called from the ViewModel
        private void ViewModel_OnFadeOutMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            fadeOutStoryboard.Begin();
        }

        // This can be called from the NavigationService to trigger the animation
        public void TriggerFadeOutAnimation()
        {
            var viewModel = DataContext as MainMenuViewModel;
            viewModel?.UpdateMessageVisibility(Visibility.Visible);
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            fadeOutStoryboard.Begin();
        }

        // Handle the completion of the fade-out animation
        private void FadeOutAnimation_Completed(object sender, System.EventArgs e)
        {
            // Reset the opacity after the fade-out is complete
            MotivationalMessageTextBlock.Opacity = 1;

            var viewModel = DataContext as MainMenuViewModel;
            viewModel?.UpdateMessageVisibility(Visibility.Collapsed);
        }
    }

}
