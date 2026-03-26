using System;
using System.Globalization;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class BoolNotConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
