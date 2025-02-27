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
        public FlashcardDeckEditorUserControl()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<FlashcardDeckEditorViewModel>();
        }
    }
}
