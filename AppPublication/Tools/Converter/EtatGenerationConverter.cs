using KernelImpl.Noyau.Organisation;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class EtatGenerationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool output = false;
            // Verifie le type des donnees en entree
            if (values.Count() >= 2)
            {
                bool actif = false;
                Competition compet = null;

                foreach (object item in values)
                {
                    if (item != null)
                    {
                        if (item.GetType() == typeof(bool))
                        {
                            actif = (bool)item;
                        }
                        if (item.GetType() == typeof(Competition))
                        {
                            compet = (Competition)item;
                        }
                    }
                }

                if (compet != null)
                {
                    output = !String.IsNullOrEmpty(compet.remoteId) && !actif;
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