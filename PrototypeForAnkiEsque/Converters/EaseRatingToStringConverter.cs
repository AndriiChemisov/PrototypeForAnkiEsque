using System.Globalization;
using System.Windows.Data;
// This converter is used to bind an integer value to a string value. The converter is used in the following way:
// <TextBlock Text="{Binding Path=EaseRating, Converter={StaticResource EaseRatingToStringConverter}}"/>
// Simple explanation: The TextBlock will display the string representation of the ease rating.

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
