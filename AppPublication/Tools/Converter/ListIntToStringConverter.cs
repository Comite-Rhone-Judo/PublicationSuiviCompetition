using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    /// <summary>
    /// Convertit une collection d'entiers en une chaîne séparée par des virgules.
    /// </summary>
    public class ListIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<int> list)
            {
                if (!list.Any())
                {
                    return "Aucun";
                }
                // Trie les numéros avant de les joindre
                return string.Join(", ", list.OrderBy(t => t));
            }
            return "Aucun";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Non nécessaire pour cet usage
            throw new NotImplementedException();
        }
    }
}