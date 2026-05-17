using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ERM.UI.Converters
{
    public class AmountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                // параметр определяет, запрошен лицвет фона (Background) или текста (Foreground)
                string type = parameter as string ?? "Foreground";

                if (amount > 0)
                {
                    return type == "Background"
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E9"))
                        : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"));
                }
                if (amount < 0) 
                {
                    return type == "Background"
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEE"))
                        : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828"));
                }

                return type == "Background"
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E9E9E"));
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}