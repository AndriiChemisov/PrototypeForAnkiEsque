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
using PrototypeForAnkiEsque.ViewModels;

namespace PrototypeForAnkiEsque.Views
{
    /// <summary>
    /// Interaction logic for FlashcardDeckEditorUserControl.xaml
    /// </summary>
    public partial class FlashcardDeckEditorUserControl : UserControl
    {
        private FlashcardDeckEditorViewModel _viewModel;
        public FlashcardDeckEditorUserControl()
        {
            InitializeComponent();

            _viewModel = App.ServiceProvider.GetRequiredService<FlashcardDeckEditorViewModel>();
            DataContext = _viewModel;
        }
    }
}
