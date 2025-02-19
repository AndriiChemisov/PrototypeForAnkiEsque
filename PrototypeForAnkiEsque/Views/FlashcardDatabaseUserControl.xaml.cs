using System.Windows;
using PrototypeForAnkiEsque.Models;
using System.Windows.Controls;
using PrototypeForAnkiEsque.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Converters;

namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardDatabaseUserControl : UserControl
    {
        public FlashcardDatabaseUserControl(FlashcardDatabaseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void FlashcardDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var viewModel = (FlashcardDatabaseViewModel)DataContext;
            if (e.AddedItems.Count > 0)
            {
                viewModel.SelectedFlashcard = (Flashcard)e.AddedItems[0];
            }
        }
    }
}
