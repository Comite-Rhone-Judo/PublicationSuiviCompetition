using Tools.Enum;

namespace Tools.Export
{
    public class ConfigurationExportSiteInterne
    {

        public ConfigurationExportSiteInterne(string pLogo = "")
        {
            // TODO Ajouter ici les autres parametres
            Logo = string.IsNullOrEmpty(pLogo) ? ConstantResource.Export_DefaultLogo : pLogo;
        }

        public string Logo = ConstantResource.Export_DefaultLogo;
    }
}
