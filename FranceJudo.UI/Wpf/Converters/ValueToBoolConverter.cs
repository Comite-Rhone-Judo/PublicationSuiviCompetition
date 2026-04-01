using System;
using System.Globalization;
using System.Windows.Data;

namespace FranceJudo.UI.Wpf.Converters
{
    public class ValueToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si la valeur est un booléen true, on renvoie true.
            // Sinon (false, null, ou autre type), on renvoie false.
            if (value is bool b)
                return b;

            if(value != null)
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
