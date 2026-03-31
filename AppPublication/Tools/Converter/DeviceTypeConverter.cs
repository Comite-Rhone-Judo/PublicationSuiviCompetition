using FranceJudo.Core.Network.Scanner;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class DeviceTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DeviceType type)
            {
                return type switch
                {
                    DeviceType.WindowsPc => "PC (Windows)",
                    DeviceType.Mac => "Mac (Apple)",
                    DeviceType.LinuxOrServer => "PC (Linux)",
                    DeviceType.SmartTvOrStreaming => "Smart TV",
                    DeviceType.GenericNetworkDevice => "Appareil réseau générique",
                    _ => type.ToString()
                };
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}