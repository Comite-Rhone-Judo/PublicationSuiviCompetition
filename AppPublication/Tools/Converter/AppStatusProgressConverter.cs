using AppPublication.Tools.Enum;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class AppStatusProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            int output = 0;
            if (value != null)
            {
                Enum.BusyStatusEnum status = (Enum.BusyStatusEnum) value;
                int statusVal = (int)status;

                // Sequence d'initialisation
                // 1 - Structures (Dem/Rep)         - 
                // 2 - Categories (Dem/Rep)
                // 3 - Logos (Dem/Rep)
                // 4 - Organisation (Dem/Rep)
                // 5 - Judokas/Equipes (Dem/Rep)
                // 6 - Phases (Dem/Rep)
                // 7 - Combats (Dem/Rep)
                // 8 - Arbitres (Dem/Rep)

                // Nombre d'etapes declare dans l'enum
                int nbStep = (int) System.Enum.GetValues(typeof(BusyStatusEnum)).Cast<BusyStatusEnum>().Max() + 1;

                if(statusVal >= 0)
                {
                    output = (int) Math.Truncate(((statusVal +1 ) * 100.0) / nbStep);
                }
            }
            return output;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}

