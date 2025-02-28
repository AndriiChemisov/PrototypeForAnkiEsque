using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the FlashcardDeckSelectionUserControl class. This class
// is used to define the code-behind logic for the FlashcardDeckSelectionUserControl.xaml file.
// The FlashcardDeckSelectionUserControl class is a UserControl that is used to display the flashcard deck selection view.
// The FlashcardDeckSelectionUserControl class defines a constructor that initializes the DataContext property of the UserControl to an instance of the FlashcardDeckSelectionViewModel class.
// The FlashcardDeckSelectionUserControl class also defines event handlers for the ExportDecks and ImportDecks buttons, which allow the user to export and import flashcard decks.
// Simple explanation: This class is used to define the code-behind logic for the FlashcardDeckSelectionUserControl.xaml file.
namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardDeckSelectionUserControl : UserControl
    {
        public FlashcardDeckSelectionUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<FlashcardDeckSelectionViewModel>();
        }

        private async void ExportDecks(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                FileName = "FlashcardDecks.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var viewmodel = (FlashcardDeckSelectionViewModel)DataContext;
                await viewmodel.ExportDecksAsync(saveFileDialog.FileName);
            }
        }

        private async void ImportDecks(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var viewmodel = (FlashcardDeckSelectionViewModel)DataContext;
                await viewmodel.ImportDecksAsync(openFileDialog.FileName);
            }
        }
    }
}
