using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class DisplayURLStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string output = "URL indéfinie";
            // Verifie le type des donnees en entree
            if (values.Count() >= 2)
            {
                string url = "URL indéfinie";
                bool actif = false;

                foreach (object item in values)
                {
                    if (item != null)
                    {
                        if (item.GetType() == typeof(string))
                        {
                            url = (string)item;
                        }
                        if (item.GetType() == typeof(bool))
                        {
                            actif = (bool)item;
                        }
                    }
                }

                output = (actif) ? url : "N/A";
            }

            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}