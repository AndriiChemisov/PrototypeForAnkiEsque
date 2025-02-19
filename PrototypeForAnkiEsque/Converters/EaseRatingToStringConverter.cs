using System;
using System.Globalization;
using System.Windows.Data;

namespace PrototypeForAnkiEsque.Converters
{
    public class EaseRatingToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int easeRating)
            {
                switch (easeRating)
                {
                    case 0:
                        return "Easy";
                    case 1:
                        return "Good";
                    case 2:
                        return "Hard";
                    default:
                        return "Unknown";
                }
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No need to convert back for now - leaving unimplemented
            return null;
        }
    }
}
