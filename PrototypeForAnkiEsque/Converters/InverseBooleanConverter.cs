using System.Globalization;
using System.Windows.Data;
// This converter is used to bind a boolean value to its inverse. The converter is used in the following way:
// <CheckBox IsChecked="{Binding Path=IsChecked}"/>
// <TextBlock Visibility="{Binding Path=IsChecked, Converter={StaticResource InverseBooleanConverter}}"/>
// Simple explanation: The TextBlock is visible if the CheckBox is not checked.

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
