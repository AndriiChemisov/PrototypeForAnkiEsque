using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;

namespace PrototypeForAnkiEsque.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(NavigationService navigationService)
        {
            InitializeComponent();

            // Set the DataContext of the MainWindow to the MainWindowViewModel
            DataContext = new MainWindowViewModel(navigationService);
        }
    }
}
