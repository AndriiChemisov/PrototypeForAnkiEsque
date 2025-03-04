using System.Windows.Controls;
using System.Windows;
using Microsoft.Win32;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the FlashcardDatabaseUserControl class, which inherits from UserControl.
// The FlashcardDatabaseUserControl class is used to display the flashcard database view in the application.
// The FlashcardDatabaseUserControl class defines a constructor that takes an instance of the FlashcardDatabaseViewModel class as a parameter.
// This allows the FlashcardDatabaseUserControl class to set its DataContext to the FlashcardDatabaseViewModel instance.
// Simple explanation: This class is used to display the flashcard database view in the application.
namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardDatabaseUserControl : UserControl
    {
        public FlashcardDatabaseUserControl(FlashcardDatabaseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private async void ExportFlashcards(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                FileName = "Flashcards.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var viewmodel = (FlashcardDatabaseViewModel)DataContext;
                await viewmodel.ExportFlashcardsAsync(saveFileDialog.FileName);
            }
        }

        private async void ImportFlashcards(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var viewmodel = (FlashcardDatabaseViewModel)DataContext;
                await viewmodel.ImportFlashcardsAsync(openFileDialog.FileName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
