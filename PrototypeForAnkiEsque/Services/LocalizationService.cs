using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using PrototypeForAnkiEsque.Resources;
using System;
using System.Diagnostics;
// This is the file that handles the localization service for the application. It is used to change the language of the application and notify subscribers of the change.
// The LocalizationService class implements the ILocalizationService interface and provides methods to change the language and get localized strings.
// The main way it changes the language is by changing the culture of the current thread to the new culture, which we get from the settings.
namespace PrototypeForAnkiEsque.Services
{
    public class LocalizationService : ILocalizationService
    {
        public event EventHandler LanguageChanged;

        private ResourceManager _resourceManager;
        private readonly ISettingsManager _settingsManager;
        private const string _defaultLanguage = "en-US"; // Default language
        private const string ResourceBaseName = "PrototypeForAnkiEsque.Resources.Strings"; // Base name of resource file

        private string _currentCulture;

        public string DefaultLanguage { get; } = _defaultLanguage;

        public LocalizationService(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            // Load the saved language from JSON or default to "en-US"
            string savedLanguage = _settingsManager.GetSavedLanguage() ?? DefaultLanguage;
            ChangeLanguage(savedLanguage);
        }

        public string CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    ChangeLanguage(value);
                    OnLanguageChanged();  // Notify subscribers of the language change
                }
            }
        }

        public string GetString(string key)
        {
            // Return localized string or the key itself if not found
            return _resourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? key;
        }

        public void ChangeLanguage(string culture)
        {
            _settingsManager.SaveLanguageSetting(culture); // Save first
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);

            _resourceManager = new ResourceManager(ResourceBaseName, typeof(Strings).Assembly);

            OnLanguageChanged();
        }


        protected virtual void OnLanguageChanged()
        {
            LanguageChanged?.Invoke(this, EventArgs.Empty); // Trigger event
        }
    }


    // Helper class to represent settings structure
    public class Settings
    {
        public string Culture { get; set; }
    }
}
