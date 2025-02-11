using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Data;

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
