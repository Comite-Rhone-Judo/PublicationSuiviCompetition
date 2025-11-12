using AppPublication.Tools.Enum;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class AppStatusTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool output = true;
            if (value != null)
            {
                Enum.BusyStatusEnum status = (Enum.BusyStatusEnum) value;

                switch (status)
                {
                    case BusyStatusEnum.AttenteFinGeneration:
                        {
                            output = true; //  indeterminate processes
                            break;
                        }
                    default:
                        {
                            output = false; break;  // Deteminate processes
                        }
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

