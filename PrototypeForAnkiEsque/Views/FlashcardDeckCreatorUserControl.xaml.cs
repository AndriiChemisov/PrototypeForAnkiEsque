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
        public FlashcardDeckCreatorUserControl()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<FlashcardDeckCreatorViewModel>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
