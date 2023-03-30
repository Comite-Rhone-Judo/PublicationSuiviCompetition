using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using Tools.Outils;
using Tools.Export;
using KernelImpl.Noyau.Organisation;

namespace AppPublication.Tools.Converter
{
    public class EtatPublicationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool output = false;
            // Verifie le type des donnees en entree
            if (values.Count() >= 2)
            {
                MiniSite site = null;
                Competition compet = null;

                foreach (object item in values)
                {
                    if (item.GetType() == typeof(MiniSite))
                    {
                        site = (MiniSite)item;
                    }
                    if (item.GetType() == typeof(Competition))
                    {
                        compet = (Competition)item;
                    }
                }

                if (site != null && compet != null)
                {
                    output = !String.IsNullOrEmpty(compet.remoteId) && !site.IsActif;
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