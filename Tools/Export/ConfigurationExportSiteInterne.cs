using Tools.Enum;

namespace Tools.Export
{
    public class ConfigurationExportSiteInterne
    {
        // TODO Ajouter ici les autres parametres de configuration du site interne: frequence de rotation des ecrans, profondeur de l'affichage
        public ConfigurationExportSiteInterne(string pLogo = "")
        {
            // TODO Ajouter ici les autres parametres
            Logo = string.IsNullOrEmpty(pLogo) ? ConstantResource.Export_DefaultLogo : pLogo;
        }

        public string Logo = ConstantResource.Export_DefaultLogo;
    }
}
