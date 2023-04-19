using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Globalization;

namespace AppPublication.Tools.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Visibility output = System.Windows.Visibility.Visible;
            if (value is bool)
            {
                output =  ( (bool)value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
