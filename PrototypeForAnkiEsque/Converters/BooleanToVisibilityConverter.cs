﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

// This converter is used to bind a boolean value to a Visibility value. The converter is used in the following way:
// <CheckBox IsChecked="{Binding Path=IsChecked}"/>
// <TextBlock Visibility="{Binding Path=IsChecked, Converter={StaticResource BooleanToVisibilityConverter}}"/>
// Simple explanation: The TextBlock is visible if the CheckBox is checked.

namespace PrototypeForAnkiEsque.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        // Convert boolean to Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // If parameter is passed, use it as an override
                if (parameter != null && bool.TryParse(parameter.ToString(), out bool parameterValue))
                {
                    return parameterValue == boolValue ? Visibility.Visible : Visibility.Collapsed;
                }
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed; // Default to Collapsed if not a boolean
        }

        // Convert back is not needed for this case
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
