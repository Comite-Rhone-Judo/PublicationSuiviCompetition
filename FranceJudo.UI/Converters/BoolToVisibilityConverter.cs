using System;
using System.Globalization;
using System.Windows.Data;
using Telerik.Windows.Controls.Calculator;

namespace AppPublication.Tools.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Visibility output = System.Windows.Visibility.Visible;
            if (value is bool)
            {
                bool valTest = (bool)value;

                if (parameter != null && parameter.GetType() == typeof(string))
                {
                    string operation = ((string)parameter).ToLower();

                    valTest = (operation == "not") ? !(bool)value : (bool)value;  
                }

                output = ((bool)valTest) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
