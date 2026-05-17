using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERM.UI.Converters
{
    public class ZeroToInverseVisibilityConverter : IValueConverter
    {
        // Возвращает Visible, если число равно 0 (или меньше)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d) return d <= 0 ? Visibility.Visible : Visibility.Collapsed;
            if (value is int i) return i <= 0 ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
