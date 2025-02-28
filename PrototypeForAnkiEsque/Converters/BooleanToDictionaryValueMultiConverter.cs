using System.Globalization;
using System.Windows.Data;

namespace PrototypeForAnkiEsque.Converters
{
    //This converter is used to bind a boolean value to a dictionary value. The dictionary key is passed as a parameter.
    //The converter is used in the following way:
    //<CheckBox IsChecked="{Binding Path=(local:MainWindowViewModel.Dictionary), Converter={StaticResource BooleanToDictionaryValueMultiConverter}, ConverterParameter=Key}"/>
    //Simple explanation: The CheckBox is checked if the dictionary contains the key "Key" and the value is true.
    public class BooleanToDictionaryValueMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Dictionary<string, bool> dictionary && values[1] is string key)
            {
                return dictionary.ContainsKey(key) && dictionary[key];
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string key)
            {
                return new object[] { new KeyValuePair<string, bool>(key, boolValue) };
            }
            return null;
        }
    }
}
