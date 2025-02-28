using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the FlashcardDeckCreatorUserControl class. This class is used to create a new instance of the FlashcardDeckCreatorViewModel class
// and set it as the DataContext of the FlashcardDeckCreatorUserControl.
// The FlashcardDeckCreatorUserControl class is a UserControl that is used to display the view for creating a new flashcard deck.
// The FlashcardDeckCreatorUserControl class defines a constructor that initializes the DataContext property of the UserControl with a new instance of the FlashcardDeckCreatorViewModel class.
// Simple explanation: This class is used to display FlashcardDeckCreator view in the application.
namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardDeckCreatorUserControl : UserControl
    {
        public FlashcardDeckCreatorUserControl()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<FlashcardDeckCreatorViewModel>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
