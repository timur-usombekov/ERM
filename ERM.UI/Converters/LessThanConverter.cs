using System.Globalization;
using System.Windows.Data;

namespace ERM.UI.Converters
{
    internal class LessThanConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var parsedValues = values.Select(v =>
            {
                if (v is string s && int.TryParse(s, out var i))
                    return i;
                if (v is int j)
                    return j;
                throw new ArgumentException("Value is not a valid integer");
            }).ToArray();

            return parsedValues[0] < parsedValues[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

    }
}
