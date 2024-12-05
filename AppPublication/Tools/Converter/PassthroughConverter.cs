using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class PassthroughConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object obj1 = null, obj2 = null;
            if(values.Length > 0)
            {
                obj1 = values[0];
            }

            if (values.Length > 1)
            {
                obj2 = values[1];
            }

            return new Tuple<object, object>(obj1, obj2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}