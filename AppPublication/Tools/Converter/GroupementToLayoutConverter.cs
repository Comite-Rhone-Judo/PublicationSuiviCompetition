using System;
using System.Globalization;
using System.Windows.Data;
using static AppPublication.Models.EcransAppel.EcranAppelModel;

namespace AppPublication.Tools.Converter
{
    public class GroupementToLayoutConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is int total) || !(values[1] is DispositionAffichage disp))
                return values[0]?.ToString() ?? "";

            /*
                # Tapis Ligne   Colonne
                1       1x1     1x1
                2       2x1     1x2
                4       2x2     2x2
                6       3x2     2x3
                8       4x2     2x4
            */

            string layout = total switch
            {
                1 => "1x1",
                2 => (disp == DispositionAffichage.Ligne) ? "2x1" : "1x2",
                4 => (disp == DispositionAffichage.Ligne) ? "2x2" : "2x2",
                6 => (disp == DispositionAffichage.Ligne) ? "3x2" : "2x3",
                8 => (disp == DispositionAffichage.Ligne) ? "4x2" : "2x4",
                _ => total.ToString(),
            };
            return $"{layout} ({total} tapis)";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null; // Pas de conversion inverse nécessaire pour un ItemsControl
        }
    }
}