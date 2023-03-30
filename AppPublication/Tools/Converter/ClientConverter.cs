using System;
using System.Globalization;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class ClientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((value != null && parameter == null) || (value == null && parameter != null))
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Hidden;
            }

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
