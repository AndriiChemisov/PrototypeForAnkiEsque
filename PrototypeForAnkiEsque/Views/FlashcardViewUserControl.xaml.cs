using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using PrototypeForAnkiEsque.Services;
using System.Windows.Media.Animation;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the FlashcardEntryUserControl class, which inherits from UserControl.
// The FlashcardEntryUserControl class is used to display the flashcard entry view in the application.
// The FlashcardEntryUserControl class defines a constructor that initializes the DataContext property with an instance of the FlashcardEntryViewModel class.
// The FlashcardEntryUserControl class also subscribes to events in the FlashcardEntryViewModel class to handle fade out animations and validation errors.
// Simple explanation: This class is used to display the flashcard entry view in the application.
namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardViewUserControl : UserControl
    {
        public FlashcardViewUserControl()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<FlashcardViewModel>();

            var booleanToVisibilityConverter = App.ServiceProvider.GetRequiredService<Converters.BooleanToVisibilityConverter>();

            var backVisibilityBinding = new System.Windows.Data.Binding("IsAnswerVisible")
            {
                Source = DataContext,
                Converter = booleanToVisibilityConverter
            };

            BackTextBox.SetBinding(TextBox.VisibilityProperty, backVisibilityBinding);

            var gridVisibilityBinding = new System.Windows.Data.Binding("IsGridVisible")
            {
                Source = DataContext,
                Converter = booleanToVisibilityConverter
            };

            MainGrid.SetBinding(VisibilityProperty, gridVisibilityBinding);

            var viewModel = (FlashcardViewModel)DataContext;
            viewModel.OnFadeOutMessage += ViewModel_OnFadeOutMessage;
            viewModel.OnFadeoutMotivationalMessage += ViewModel_OnFadeoutMotivationalMessage;
        }

        private void ViewModel_OnFadeOutMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            var animation = fadeOutStoryboard.Children[0] as DoubleAnimation; 
            animation.To = 0; // Update opacity to 0 for fading out
            fadeOutStoryboard.Begin();
        }

        private void ViewModel_OnFadeoutMotivationalMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            var animation = fadeOutStoryboard.Children[1] as DoubleAnimation; 
            animation.To = 0; // Update opacity to 0 for fading out
            fadeOutStoryboard.Begin();
        }

        private void FadeOutAnimation_Completed(object sender, System.EventArgs e)
        {
            // Reset the opacity after the fade-out is complete
            RatingMessageTextBlock.Opacity = 1;
            MotivationalMessageBlock.Opacity = 1;
        }
    }
}
