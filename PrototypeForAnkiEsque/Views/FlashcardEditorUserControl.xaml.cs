using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the FlashcardEditorUserControl class. The FlashcardEditorUserControl class is a user control that is used to edit flashcards.
// The FlashcardEditorUserControl class defines a constructor that initializes the component and sets the data context to an instance of the FlashcardEditorViewModel class.
// The FlashcardEditorUserControl class also subscribes to the OnFadeOutMessage and OnValidationError events of the FlashcardEditorViewModel class to handle fade out animations and validation errors.
// Simple explanation: This class is a user control that is used to edit flashcards.
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
            }
        }

        private void ViewModel_OnFadeOutMessage()
        {
            var fadeOutStoryboard = (Storyboard)Resources["FadeOutAnimation"];
            fadeOutStoryboard.Begin();
        }

        private void FadeOutAnimation_Completed(object sender, EventArgs e)
        {
            SavedMessageText.Opacity = 1;
            var viewModel = (FlashcardEditorViewModel)DataContext;
            viewModel.IsSavedMessageVisible = false;
        }
    }
}
