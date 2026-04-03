using FranceJudo.Core.Environment;
using FranceJudo.Metier.Resources;
using FranceJudo.Metier.XML;
using FranceJudo.Core.Utils;
using System;
using System.Xml.Linq;

namespace AppPublication.Export
{
    public class ConfigurationExportSiteInterne : IReadOnlyConfigurationExportSiteInterne, ICloneableObject<ConfigurationExportSiteInterne>
    {
        public ConfigurationExportSiteInterne(string pLogo = "", long pDelaiDeroulementSec = 10, int pNbProchainsCombats = 6)
        {
            Logo = string.IsNullOrEmpty(pLogo) ? MetierResources.Files.DefaultLogo : pLogo;
            DelaiDeroulementSec = pDelaiDeroulementSec;
            NbProchainsCombats = pNbProchainsCombats;
        }

        public string Logo { get; set; } = MetierResources.Files.DefaultLogo;
        public long DelaiDeroulementSec { get; set; } = 10;
        public int NbProchainsCombats { get; set; } = 6;
        public string UrlRedirecteur { get; set; } = string.Empty;

        public ConfigurationExportSiteInterne Clone()
        {
            // Crée une copie indépendante de l'objet (parfait pour les types natifs et string)
            return (ConfigurationExportSiteInterne) this.MemberwiseClone();
        }

        public XElement ToXml()
        {
            // Utilisation de la constante pour le nom de la balise racine de configuration
            return new XElement(ConstantXML.SiteConfiguration,

                new XAttribute(ConstantXML.delaiDeroulementSec, DelaiDeroulementSec.ToString()),
                new XAttribute(ConstantXML.nbProchainsCombats, NbProchainsCombats.ToString()),

                // --- Chaînes et Dates ---
                new XAttribute(ConstantXML.DateGeneration, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")),
                new XAttribute(ConstantXML.AppVersion, AppInformation.Instance.AppVersion ?? string.Empty),
                new XAttribute(ConstantXML.Logo, Logo ?? string.Empty),
                new XAttribute(ConstantXML.urlRedirecteur, UrlRedirecteur ?? string.Empty)
            );
        }
    }
}
