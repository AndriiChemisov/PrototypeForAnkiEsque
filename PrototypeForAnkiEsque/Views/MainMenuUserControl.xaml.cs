using Microsoft.Extensions.DependencyInjection;
using PrototypeForAnkiEsque.ViewModels;
using System.Windows.Controls;

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
