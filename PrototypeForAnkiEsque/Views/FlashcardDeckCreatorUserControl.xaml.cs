using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
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

            _viewModel = App.ServiceProvider.GetRequiredService<FlashcardDeckCreatorViewModel>();
            DataContext = _viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
