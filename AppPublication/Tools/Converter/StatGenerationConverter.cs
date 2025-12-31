using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using AppPublication.Statistiques;

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
                TaskExecutionInformation theStat = null;
                bool genere = false;

                foreach (object item in values)
                {
                    if (item != null)
                    {
                        if (item.GetType() == typeof(TaskExecutionInformation))
                        {
                            theStat = (TaskExecutionInformation)item;
                        }
                        if (item.GetType() == typeof(bool))
                        {
                            genere = (bool)item;
                        }
                    }
                }

                if (genere && theStat != null)
                {
                    if (theStat.DateProchaineGeneration == DateTime.MinValue)
                    {
                        output = string.Format("Dernière à {0} (en {1}s)", theStat.DateDemarrage.ToString("HH:mm:ss"), (int)Math.Round(theStat.DelaiExecutionMs / 1000.0));
                    }
                    else
                    {
                        output = string.Format("Dernière à {0} (en {1}s), Prochaine à {2}", theStat.DateDemarrage.ToString("HH:mm:ss"), (int)Math.Round(theStat.DelaiExecutionMs / 1000.0), theStat.DateProchaineGeneration.ToString("HH:mm:ss"));
                    }
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
