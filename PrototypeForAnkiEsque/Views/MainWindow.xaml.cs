using System.Windows;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;

namespace PrototypeForAnkiEsque.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(IMainMenuNavigationService mainMenuNavigationService)
        {
            InitializeComponent();

            // Set the DataContext of the MainWindow to the MainWindowViewModel
            DataContext = new MainWindowViewModel(mainMenuNavigationService);
        }
    }
}
