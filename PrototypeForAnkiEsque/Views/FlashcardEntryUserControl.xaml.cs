using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the FlashcardEntryUserControl class. The FlashcardEntryUserControl class is a user control that is used to display the flashcard entry view in the application.
// The FlashcardEntryUserControl class inherits from the UserControl class provided by WPF. It defines a constructor that initializes the data context of the user control to an instance of the FlashcardEntryViewModel class.
// The FlashcardEntryUserControl class also subscribes to the OnFadeOutMessage and OnValidationError events of the FlashcardEntryViewModel class to handle fade out animations and validation errors.
// The FlashcardEntryUserControl class defines event handlers for the OnFadeOutMessage and OnValidationError events that are raised by the FlashcardEntryViewModel class.
// The event handlers show a fade out animation for the saved message text and display a message box with the validation error message.
// Simple explanation: This class is used to display the flashcard entry view in the application.
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

