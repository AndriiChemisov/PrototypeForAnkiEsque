using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
using System.Windows.Controls;
// This file is used to define the MainMenuUserControl class. The MainMenuUserControl class is a user control that represents the main menu of the application.
// The MainMenuUserControl class is used to display the main menu view to the user.
// Simple explanation: This class is used to display the main menu view to the user.
namespace PrototypeForAnkiEsque.Views
{
    public partial class MainMenuUserControl : UserControl
    {
        public MainMenuUserControl()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<MainMenuViewModel>();
        }

    }

}
