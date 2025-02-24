using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace PrototypeForAnkiEsque.Converters
{
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
