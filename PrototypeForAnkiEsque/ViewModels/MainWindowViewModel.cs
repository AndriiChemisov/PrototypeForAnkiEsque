using PrototypeForAnkiEsque.Services;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.GetMainMenuViewAsync(); 
        }
    }
}

