using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
