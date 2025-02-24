using System.Windows.Controls;

namespace PrototypeForAnkiEsque.Views
{
    public partial class FlashcardDatabaseUserControl : UserControl
    {
        public FlashcardDatabaseUserControl(FlashcardDatabaseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
