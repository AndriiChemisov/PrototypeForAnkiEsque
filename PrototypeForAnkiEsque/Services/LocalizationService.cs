using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using PrototypeForAnkiEsque.Resources;
using System;
using System.Diagnostics;

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
            // Set the culture synchronously to avoid any lag
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);

            // Reload resources for the new culture
            _resourceManager = new ResourceManager(ResourceBaseName, typeof(Strings).Assembly);

            // Save the selected language to settings
            _settingsManager.SaveLanguageSetting(culture);

            // Notify listeners about the language change
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
