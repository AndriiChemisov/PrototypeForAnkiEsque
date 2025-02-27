using System.Windows.Controls;
using System.Windows;
using Microsoft.Win32;
using PrototypeForAnkiEsque.ViewModels;

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
    }
}
