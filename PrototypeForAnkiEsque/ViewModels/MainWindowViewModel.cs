using PrototypeForAnkiEsque.Services;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;

        public MainWindowViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.GetMainMenuViewAsync(); 
        }
    }
}

