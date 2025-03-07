using Newtonsoft.Json;
using System.IO;


namespace PrototypeForAnkiEsque.Services
{
    // Implementation of settings manager for JSON file
    public class SettingsManager : ISettingsManager
    {
        private const string SettingsFilePath = "appsettings.json"; // Path to JSON settings file
        private const string DefaultLanguage = "en-US"; // Default language

        public string GetSavedLanguage()
        {
            // Retrieve the saved language from the JSON settings file
            if (File.Exists(SettingsFilePath))
            {
                var settings = LoadSettings();
                return settings.Culture;
            }
            return DefaultLanguage; // Return default language if not set
        }

        public void SaveLanguageSetting(string language)
        {
            var settings = LoadSettings();
            settings.Culture = language;
            SaveSettings(settings);
        }

        private Settings LoadSettings()
        {
            // Load settings from JSON file, or return default settings if file doesn't exist
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonConvert.DeserializeObject<Settings>(json);
            }
            return new Settings { Culture = DefaultLanguage }; // Default setting
        }

        private void SaveSettings(Settings settings)
        {
            // Save the updated settings to the JSON file
            var json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }
    }
}
