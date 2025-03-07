using System.Globalization;
using System.Resources;
using System.Threading;
using PrototypeForAnkiEsque.Resources;

namespace PrototypeForAnkiEsque.Services
{
    public class LocalizationService : ILocalizationService
    {
        private ResourceManager _resourceManager = new ResourceManager(typeof(Strings));

        public string GetString(string key)
        {
            return _resourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? key;
        }

        public void ChangeLanguage(string culture)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }
    }
}
