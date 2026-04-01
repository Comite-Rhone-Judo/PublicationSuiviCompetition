using System;
using System.Globalization;
using System.Windows.Data;

namespace FranceJudo.UI.Wpf.Converters
{
    public class BoolNotConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool ? !(bool)value : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return null;
            }
        }
    }
}
