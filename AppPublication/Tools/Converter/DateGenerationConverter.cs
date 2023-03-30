using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using Tools.Outils;
using Tools.Export;
using KernelImpl.Noyau.Organisation;

namespace AppPublication.Tools.Converter
{
    public class DateGenerationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string output = "-";

            // Verifie le type des donnees en entree
            if (values.Count() >= 2)
            {
                DateTime theDate = DateTime.Now;
                bool genere = false;

                foreach (object item in values)
                {
                    if (item.GetType() == typeof(DateTime))
                    {
                        theDate = (DateTime)item;
                    }
                    if (item.GetType() == typeof(bool))
                    {
                        genere = (bool)item;
                    }
                }

                if (genere)
                {
                    output = theDate.ToString("dd MMM yyyy HH:mm:ss");
                }
            }

            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
