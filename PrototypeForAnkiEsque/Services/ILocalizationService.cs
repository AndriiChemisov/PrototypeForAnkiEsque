using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeForAnkiEsque.Services
{
    public interface ILocalizationService
    {
        string GetString(string key);
        void ChangeLanguage(string culture);

        event EventHandler LanguageChanged; // Event to notify language change

    }
}
