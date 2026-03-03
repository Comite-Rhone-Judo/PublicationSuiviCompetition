using System;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace Tools.Export
{
    public class ConfigurationExportSiteInterne
    {
        public ConfigurationExportSiteInterne(string pLogo = "", long pDelaiDeroulementSec = 10, int pNbProchainsCombats = 6)
        {
            Logo = string.IsNullOrEmpty(pLogo) ? ConstantResource.Export_DefaultLogo : pLogo;
            DelaiDeroulementSec = pDelaiDeroulementSec;
            NbProchainsCombats = pNbProchainsCombats;
        }

        public string Logo = ConstantResource.Export_DefaultLogo;
        public long DelaiDeroulementSec = 10;
        public int NbProchainsCombats = 6;

        public XElement ToXml()
        {
            // Utilisation de la constante pour le nom de la balise racine de configuration
            return new XElement(ConstantXML.SiteConfiguration,

                new XAttribute(ConstantXML.delaiDeroulementSec, DelaiDeroulementSec.ToString()),
                new XAttribute(ConstantXML.nbProchainsCombats, DelaiDeroulementSec.ToString()),

                // --- Chaînes et Dates ---
                new XAttribute(ConstantXML.DateGeneration, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")),
                new XAttribute(ConstantXML.AppVersion, AppInformation.Instance.AppVersion ?? string.Empty),
                new XAttribute(ConstantXML.Logo, Logo ?? string.Empty)
            );
        }
    }
}
