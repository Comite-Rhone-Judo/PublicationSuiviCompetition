using System;
using System.Globalization;
using System.Windows.Data;
using static AppPublication.Models.EcransAppel.EcranAppelModel;

namespace AppPublication.Tools.Converter
{
    public class ResolutionToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ScreenResolution resolution)
            {
                switch (resolution)
                {
                    case ScreenResolution.FullHd_1080p: return "Full HD (1080p)";
                    case ScreenResolution.UltraHd_4K: return "Ultra HD (4K)";
                    case ScreenResolution.UltraHd_8K: return "Ultra HD (8K)";
                    default: return resolution.ToString();
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}