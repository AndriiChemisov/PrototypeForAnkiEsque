using System.Windows.Controls;
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
