using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
using PrototypeForAnkiEsque.Services;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PrototypeForAnkiEsque.Views
{
    public partial class MainMenuUserControl : UserControl
    {
        public MainMenuUserControl()
        {
            InitializeComponent();

            var navigationService = App.ServiceProvider.GetRequiredService<NavigationService>();
            var viewModel = App.ServiceProvider.GetRequiredService<MainMenuViewModel>();
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
