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
                    case Enum.BusyStatusEnum.DemandeDonneesStructures:
                        {
                            output = "Demande les données (Clubs, Ligues, etc.)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesStructures:
                        {
                            output = "Initialisation des données (Clubs,  Ligues, etc.)";
                            break;
                        }
                    case Enum.BusyStatusEnum.DemandeDonneesCategories:
                        {
                            output = "Demande les données (Catégories)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesCategories:
                        {
                            output = "Initialisation des données (Catégories)";
                            break;
                        }
                    case Enum.BusyStatusEnum.DemandeDonneesLogos:
                        {
                            output = "Demande les données (Logos)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesLogos:
                        {
                            output = "Initialisation des données (Logos)";
                            break;
                        }
                    case Enum.BusyStatusEnum.DemandeDonneesOrganisation:
                        {
                            output = "Demande les données (Epreuves)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesOrganisation:
                        {
                            output = "Initialisation des données (Epreuves)";
                            break;
                        }
                    case Enum.BusyStatusEnum.DemandeDonneesJudokas:
                        {
                            output = "Demande les données (Judokas)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesJudokas:
                        {
                            output = "Initialisation des données (Judokas)";
                            break;
                        }
                    case Enum.BusyStatusEnum.DemandeDonneesPhases:
                        {
                            output = "Demande les données (Phases)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesPhases:
                        {
                            output = "Initialisation des données (Phases)";
                            break;
                        }
                    case Enum.BusyStatusEnum.DemandeDonneesCombats:
                        {
                            output = "Demande les données (Combats)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesCombats:
                        {
                            output = "Initialisation des données (Combats)";
                            break;
                        }
                    case Enum.BusyStatusEnum.DemandeDonneesArbitres:
                        {
                            output = "Demande les données (Arbitres)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesArbitres:
                        {
                            output = "Initialisation des données (Arbitres)";
                            break;
                        }
                    case Enum.BusyStatusEnum.InitDonneesNone:
                    default:
                        {
                            output = string.Empty;
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

