using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.ViewModels;

namespace PrototypeForAnkiEsque.Views
{
    /// <summary>
    /// Interaction logic for FlashcardDeckCreatorView.xaml
    /// </summary>
    public partial class FlashcardDeckCreatorUserControl : UserControl
    {
        private FlashcardDeckCreatorViewModel _viewModel;

        public FlashcardDeckCreatorUserControl()
        {
            InitializeComponent();

            Loaded += FlashcardDeckCreatorView_Loaded;
        }

        private void FlashcardDeckCreatorView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = App.ServiceProvider.GetRequiredService<FlashcardDeckCreatorViewModel>();
            DataContext = _viewModel;

            // Manually sync DataGrid selections
            FlashcardDataGrid.SelectionChanged += FlashcardDataGrid_SelectionChanged;
        }

        private void FlashcardDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null) return;

            // Clear previous selections
            _viewModel.SelectedFlashcards.Clear();

            // Add newly selected items
            foreach (Flashcard selected in FlashcardDataGrid.SelectedItems)
            {
                // Store the flashcard ID instead of the flashcard object
                _viewModel.SelectedFlashcards[selected.Id] = true;
            }

            // Handle deselection (optional)
            foreach (Flashcard unselected in e.RemovedItems)
            {
                _viewModel.SelectedFlashcards[unselected.Id] = false; // Mark as not selected
            }
        }


    }
}
