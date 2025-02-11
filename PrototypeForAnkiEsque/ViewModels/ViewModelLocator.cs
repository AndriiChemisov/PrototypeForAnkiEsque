using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PrototypeForAnkiEsque.ViewModels
{
    public class ViewModelLocator
    {
        public FlashcardViewModel MainViewModel => App.ServiceProvider.GetService<FlashcardViewModel>();
        public FlashcardEntryViewModel FlashcardEntryViewModel => App.ServiceProvider.GetService<FlashcardEntryViewModel>();
        public MainMenuViewModel MainMenuViewModel => App.ServiceProvider.GetService<MainMenuViewModel>();
    }

}
