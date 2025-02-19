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
    }
}
