using System;
using System.Globalization;
using System.Windows.Data;

namespace AppPublication.Tools.Converter
{
    public class AppStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string output = "Valeur Inconnue";
            if (value != null)
            {
                Enum.BusyStatusEnum status = (Enum.BusyStatusEnum)value;

                switch (status)
                {
                    case Enum.BusyStatusEnum.InitDonneesNone:
                        {
                            output = "Valeur Inconnue";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesClub:
                        {
                            output = "Initialisation des données (Clubs)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesCategories:
                        {
                            output = "Initialisation des données (Catégories)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesLogos:
                        {
                            output = "Initialisation des données (Clubs)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesEpreuves:
                        {
                            output = "Initialisation des données (Epreuves)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesJudokas:
                        {
                            output = "Initialisation des données (Judokas)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesPhases:
                        {
                            output = "Initialisation des données (Phases)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesCombats:
                        {
                            output = "Initialisation des données (Combats)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesArbitres:
                        {
                            output = "Initialisation des données (Arbitres)";
                            break;
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

