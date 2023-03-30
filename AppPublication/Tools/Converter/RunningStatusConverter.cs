using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Globalization;

namespace AppPublication.Tools.Converter
{
    public class RunningStatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? "Démarré" : "Arrêté";
            }
            else
            {
                return "N/A";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
