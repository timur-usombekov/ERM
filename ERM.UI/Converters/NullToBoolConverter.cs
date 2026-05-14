using System.Globalization;
using System.Windows.Data;

namespace ERM.UI.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        // Возвращает True, если значение НЕ null (есть ошибка)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
