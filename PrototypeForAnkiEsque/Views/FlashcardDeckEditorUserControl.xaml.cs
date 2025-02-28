using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the FlashcardDeckCreatorUserControl class. The FlashcardDeckCreatorUserControl class is a user control that is used to create a new flashcard deck.
// The FlashcardDeckCreatorUserControl class inherits from the UserControl class provided by WPF.
// The FlashcardDeckCreatorUserControl class defines a constructor that initializes the component and sets the DataContext to an instance of the FlashcardDeckCreatorViewModel class.
// The FlashcardDeckCreatorUserControl class also defines an event handler for the Button_Click event.
// Simple explanation: This class is displays the FlashcardDeckCreatorUserControl user control, which is used to create a new flashcard deck.
namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardDeckEditorUserControl : UserControl
    {
        public FlashcardDeckEditorUserControl()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<FlashcardDeckEditorViewModel>();
        }
    }
}
