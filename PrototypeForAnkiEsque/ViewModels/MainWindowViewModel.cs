using PrototypeForAnkiEsque.Services;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private object _currentContent;

        public MainWindowViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
            // You can default to a user control if needed, but navigation is handled by each user control's ViewModel.
            _navigationService.GetMainMenuView(); // Or any other starting view
        }

        public object CurrentContent
        {
            get => _currentContent;
            set { SetProperty(ref _currentContent, value); }
        }
    }
}
