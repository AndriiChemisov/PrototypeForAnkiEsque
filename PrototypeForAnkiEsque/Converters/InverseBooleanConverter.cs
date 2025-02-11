using System;
using System.Globalization;
using System.Windows.Data;

namespace PrototypeForAnkiEsque.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Invert the boolean value
            return value is bool && (bool)value == false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not needed, but if you plan to convert back, return the same value
            return value;
        }
    }
}
