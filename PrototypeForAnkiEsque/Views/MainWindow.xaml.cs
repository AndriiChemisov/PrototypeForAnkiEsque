using System.Windows;
using PrototypeForAnkiEsque.Services;
using PrototypeForAnkiEsque.ViewModels;
// This file is used to define the MainWindow class, which is the main window of the application.
// The MainWindow class defines a constructor that takes an instance of the IMainMenuNavigationService interface as a parameter.
// This allows the MainWindow class to navigate between different views in the application using the main menu navigation service.
// Simple explanation: This class is the main window of the application and is used to navigate between different views in the application.
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
