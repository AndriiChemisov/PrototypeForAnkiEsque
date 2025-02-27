using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using PrototypeForAnkiEsque.ViewModels;

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
