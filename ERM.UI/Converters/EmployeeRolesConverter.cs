using ERM.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ERM.UI.Converters
{
    public class EmployeeRolesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not EmployeeDto emp)
                return Enumerable.Empty<RoleBadgeInfo>();

            var roles = new List<RoleBadgeInfo>();

            if (emp.IsSeamstress)
                roles.Add(new RoleBadgeInfo
                {
                    Label = "Швея",
                    Background = "#BBDEFB",
                    Foreground = "#1565C0"
                });

            if (emp.IsCutter)
                roles.Add(new RoleBadgeInfo
                {
                    Label = "Закройщик",
                    Background = "#C8E6C9",
                    Foreground = "#2E7D32"
                });

            if (emp.IsIroner)
                roles.Add(new RoleBadgeInfo
                {
                    Label = "Гладильщица",
                    Background = "#E1BEE7", // Фиолетовый оттенок
                    Foreground = "#6A1B9A"
                });

            return roles; // пустой список — ItemsControl ничего не покажет
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class RoleBadgeInfo
    {
        public string Label { get; init; } = "";
        public string Background { get; init; } = "#E0E0E0";
        public string Foreground { get; init; } = "#616161";
    }
}
