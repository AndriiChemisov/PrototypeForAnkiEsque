using System;
using System.Globalization;
using System.Windows.Data;
using PrototypeForAnkiEsque.Services; // Assuming your localization service is in this namespace
// This converter is used to bind an integer value to a string value. The converter is used in the following way:
// <TextBlock Text="{Binding Path=EaseRating, Converter={StaticResource EaseRatingToStringConverter}}"/>
// Simple explanation: The TextBlock will display the string representation of the ease rating.

namespace PrototypeForAnkiEsque.Converters
{
    public class EaseRatingToStringConverter : IValueConverter
    {
        private ILocalizationService _localizationService;

        // Constructor that takes the ILocalizationService to retrieve localized strings
        public EaseRatingToStringConverter()
        {
            _localizationService = (ILocalizationService)App.ServiceProvider.GetService(typeof(ILocalizationService));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int easeRating)
            {
                switch (easeRating)
                {
                    case 0:
                        return _localizationService.GetString("BttnEaseEasy"); // Localized string for "Easy"
                    case 1:
                        return _localizationService.GetString("BttnEaseGood"); // Localized string for "Good"
                    case 2:
                        return _localizationService.GetString("BttnEaseHard"); // Localized string for "Hard"
                    default:
                        return _localizationService.GetString("EaseRatingUnknown"); // Localized string for "Unknown"
                }
            }
            return _localizationService.GetString("EaseRatingUnknown"); // Default to "Unknown" if invalid value
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No need to implement this for now
            return null;
        }
    }
}
