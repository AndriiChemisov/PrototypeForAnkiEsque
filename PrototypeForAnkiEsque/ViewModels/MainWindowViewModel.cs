using PrototypeForAnkiEsque.Services;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IMainMenuNavigationService _mainMenuNavigationService;

        public MainWindowViewModel(IMainMenuNavigationService mainMenuNavigationService)
        {
            _mainMenuNavigationService = mainMenuNavigationService;
            _mainMenuNavigationService.GetMainMenuViewAsync(); 
        }
    }
}

