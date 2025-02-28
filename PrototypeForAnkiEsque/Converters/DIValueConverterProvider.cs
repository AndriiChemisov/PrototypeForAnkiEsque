using Microsoft.Extensions.DependencyInjection;
using System.Windows.Data;
// This converter is used to bind a boolean value to a Visibility value. The converter is used in the following way:
// <CheckBox IsChecked="{Binding Path=IsChecked}"/>
// <TextBlock Visibility="{Binding Path=IsChecked, Converter={local:DIValueConverterProvider}}"/>
// Simple explanation: The TextBlock is visible if the CheckBox is checked.

namespace PrototypeForAnkiEsque.Converters
{
    public class DIValueConverterProvider : IValueConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public DIValueConverterProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var converter = _serviceProvider.GetRequiredService<BooleanToVisibilityConverter>();
            return converter.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
