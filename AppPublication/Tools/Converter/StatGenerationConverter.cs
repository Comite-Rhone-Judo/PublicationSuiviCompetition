using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using Tools.Outils;
using Tools.Export;
using KernelImpl.Noyau.Organisation;
using AppPublication.Tools;

namespace AppPublication.Tools.Converter
{
    public class StatGenerationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string output = "-";

            // Verifie le type des donnees en entree
            if (values.Count() >= 2)
            {
                StatExecution theStat = null;
                bool genere = false;

                foreach (object item in values)
                {
                    if (item != null)
                    {
                        if (item.GetType() == typeof(StatExecution))
                        {
                            theStat = (StatExecution)item;
                        }
                        if (item.GetType() == typeof(bool))
                        {
                            genere = (bool)item;
                        }
                    }
                }

                if (genere && theStat != null)
                {
                    output = string.Format("{0} ({1} sec.)", theStat.DateFin.ToString("dd/MM/yyyy HH:mm:ss"), (int) Math.Round(theStat.DelaiExecutionMs / 1000.0));
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
