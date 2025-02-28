using PrototypeForAnkiEsque.Services;
// This file is used to define the MainWindowViewModel class, which inherits from the BaseViewModel class.
// The MainWindowViewModel class is used to define the view model for the MainWindow view.
// The MainWindowViewModel class defines a constructor that takes an instance of the IMainMenuNavigationService interface as a parameter.
// This allows the MainWindowViewModel class to navigate to the main menu view when the application starts.
// Simple explanation: This class is used to define the view model for the MainWindow view.
// Side note: while the main window has no content of it's own, we use UserControls to inject content into it, so this is, perhaps, the most important view model in the application.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region FIELD DECLARATIONS
        private readonly IMainMenuNavigationService _mainMenuNavigationService;
        #endregion

        #region CONSTRUCTOR
        public MainWindowViewModel(IMainMenuNavigationService mainMenuNavigationService)
        {
            _mainMenuNavigationService = mainMenuNavigationService;
            _mainMenuNavigationService.GetMainMenuViewAsync(); 
        }
        #endregion
    }
}

