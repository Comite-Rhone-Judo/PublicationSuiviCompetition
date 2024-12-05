using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class BoolOperationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool output = false;
            // Verifie le type des donnees en entree
            if (values.Count() >= 2 && parameter != null)
            {
                bool? ope1 = null;
                bool? ope2 = null;
                string operation = string.Empty;

                foreach (object item in values)
                {
                    if (item != null)
                    {
                        if (item.GetType() == typeof(bool))
                        {
                            if (ope1 == null)
                            {
                                ope1 = (bool)item;
                            }
                            else if (ope2 == null)
                            {
                                ope2 = (bool)item;
                            }
                        }
                    }
                }

                if (parameter.GetType() == typeof(string))
                {
                    operation = ((string)parameter).ToLower();
                }

                if (ope1 != null && ope2 != null && !String.IsNullOrEmpty(operation))
                {
                    switch (operation)
                    {
                        case "not_a_and_b":
                            {
                                output = !ope1.Value && ope2.Value;
                                break;
                            }
                        case "a_and_not_b":
                            {
                                output = ope1.Value && !ope2.Value;
                                break;
                            }
                        case "not_a_and_not_b":
                            {
                                output = !ope1.Value && !ope2.Value;
                                break;
                            }
                        case "a_and_b":
                            {
                                output = ope1.Value && ope2.Value;
                                break;
                            }
                        case "not_a_or_b":
                            {
                                output = !ope1.Value || ope2.Value;
                                break;
                            }
                        case "not_a_or_not_b":
                            {
                                output = !ope1.Value || !ope2.Value;
                                break;
                            }
                        case "a_or_b":
                            {
                                output = ope1.Value || ope2.Value;
                                break;
                            }
                        default:
                            {
                                output = false;
                                break;
                            }
                    }
                }
            }

            // return (output) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}