using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using PrototypeForAnkiEsque.Services;
using System.Windows.Media.Animation;

namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardViewUserControl : UserControl
    {
        public FlashcardViewUserControl()
        {
            InitializeComponent();

            // Resolve the FlashcardViewModel via DI
            DataContext = App.ServiceProvider.GetRequiredService<FlashcardViewModel>();

            // Resolve the BooleanToVisibilityConverter and NavigationService from DI
            var booleanToVisibilityConverter = App.ServiceProvider.GetRequiredService<Converters.BooleanToVisibilityConverter>();
            var navigationService = App.ServiceProvider.GetRequiredService<NavigationService>();

            // Create and set up the binding for the visibility of the back of the card
            var backVisibilityBinding = new System.Windows.Data.Binding("IsAnswerVisible")
            {
                Source = DataContext,
                Converter = booleanToVisibilityConverter
            };

            // Apply the binding to the visibility of the back TextBox
            BackTextBox.SetBinding(TextBox.VisibilityProperty, backVisibilityBinding);

            var gridVisibilityBinding = new System.Windows.Data.Binding("IsGridVisible")
            {
                Source = DataContext,
                Converter = booleanToVisibilityConverter
            };

            MainGrid.SetBinding(VisibilityProperty, gridVisibilityBinding);

            // Subscribe to the ViewModel event for triggering the fade-out animation
            var viewModel = (FlashcardViewModel)DataContext;
            viewModel.OnFadeOutMessage += ViewModel_OnFadeOutMessage;
            viewModel.OnFadeoutMotivationalMessage += ViewModel_OnFadeoutMotivationalMessage;
        }

        // Trigger the fade-out animation when the event is called from the ViewModel
        private void ViewModel_OnFadeOutMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            var animation = fadeOutStoryboard.Children[0] as DoubleAnimation; // Assuming RatingMessageTextBlock is the 1st child
            animation.To = 0; // Update opacity to 0 for fading out
            fadeOutStoryboard.Begin();
        }

        // Trigger the fade-out animation for MotivationalMessageBlock
        private void ViewModel_OnFadeoutMotivationalMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            var animation = fadeOutStoryboard.Children[1] as DoubleAnimation; // Assuming MotivationalMessageBlock is the 2nd child
            animation.To = 0; // Update opacity to 0 for fading out
            fadeOutStoryboard.Begin();
        }

        // Handle the completion of the fade-out animation
        private void FadeOutAnimation_Completed(object sender, System.EventArgs e)
        {
            // Reset the opacity after the fade-out is complete
            RatingMessageTextBlock.Opacity = 1;
            MotivationalMessageBlock.Opacity = 1;
        }
    }
}
