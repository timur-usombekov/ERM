using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERM.UI.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        // Если объект есть -> показываем (Visible). Если null -> скрываем (Collapsed)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}