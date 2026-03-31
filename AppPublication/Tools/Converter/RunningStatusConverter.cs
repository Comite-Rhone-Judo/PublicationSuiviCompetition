using System;
using System.Globalization;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class RunningStatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool ? (bool)value ? "Démarré" : "Arrêté" : "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
