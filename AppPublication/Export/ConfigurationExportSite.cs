using FranceJudo.Core.Environment;
using FranceJudo.Metier.Resources;
using FranceJudo.Metier.XML;
using FranceJudo.Core.Utils;
using System;
using System.Xml.Linq;


namespace AppPublication.Export
{
    public class ConfigurationExportSite : IReadOnlyConfigurationExportSite, ICloneableObject<ConfigurationExportSite>
    {
        public ConfigurationExportSite(bool pubPC = false, bool pubAT = true, bool pubP = true, bool pubS = false, bool partAbsent = false, bool partTC = false, bool scoreGP = false, bool affPosC = false, long delAC = 30, int nbPC = 6, string pMsg = "", string pLogo = "", bool pec = false, bool ptec = false, int maxpc = 5, bool pUseIC = false, string pIC = "")
        {
            PublierProchainsCombats = pubPC;
            PublierAffectationTapis = pubAT;
            PublierEngagements = pubP;
            PublierStatistiques = pubS;
            EngagementsAbsents = partAbsent;
            EngagementsTousCombats = partTC;
            EngagementsScoreGP = scoreGP;
            AfficherPositionCombat = affPosC;
            DelaiActualisationClientSec = delAC;
            NbProchainsCombats = nbPC;
            MsgProchainsCombats = pMsg;
            Logo = string.IsNullOrEmpty(pLogo) ? MetierResources.Files.DefaultLogo : pLogo;
            PouleEnColonnes = pec;
            PouleToujoursEnColonnes = ptec;
            TailleMaxPouleColonnes = maxpc;
            UseIntituleCommun = pUseIC;
            IntituleCommun = pIC;

        }

        public bool PublierProchainsCombats { get; set; } = false;
        public bool PublierAffectationTapis { get; set; } = true;
        public bool PublierEngagements { get; set; } = false;
        public bool PublierStatistiques { get; set; } = false;
        public bool EngagementsAbsents { get; set; } = false;
        public bool EngagementsTousCombats { get; set; } = false;
        public bool EngagementsScoreGP { get; set; } = false;
        public bool AfficherPositionCombat { get; set; } = false;
        public long DelaiActualisationClientSec { get; set; } = 30;
        public int NbProchainsCombats { get; set; } = 6;
        public string MsgProchainsCombats { get; set; } = string.Empty;
        public string Logo { get; set; } = MetierResources.Files.DefaultLogo;

        public bool PouleEnColonnes { get; set; } = false;
        public bool PouleToujoursEnColonnes { get; set; } = false;
        public int TailleMaxPouleColonnes { get; set; } = 5;

        public bool UseIntituleCommun { get; set; } = false;
        public string IntituleCommun { get; set; } = string.Empty;

        public ConfigurationExportSite Clone()
        {
            // Crée une copie indépendante de l'objet (parfait pour les types natifs et string)
            return (ConfigurationExportSite)this.MemberwiseClone();
        }

        public XElement ToXml()
        {
            // Utilisation de la constante pour le nom de la balise racine de configuration
            return new XElement(ConstantXML.SiteConfiguration,

                // --- Booléens (Conversion native XML) ---
                new XAttribute(ConstantXML.publierProchainsCombats, System.Xml.XmlConvert.ToString(PublierProchainsCombats)),
                new XAttribute(ConstantXML.publierAffectationTapis, System.Xml.XmlConvert.ToString(PublierAffectationTapis)),
                new XAttribute(ConstantXML.publierEngagements, System.Xml.XmlConvert.ToString(PublierEngagements)),
                new XAttribute(ConstantXML.publierStatistiques, System.Xml.XmlConvert.ToString(PublierStatistiques)),
                new XAttribute(ConstantXML.EngagementsAbsents, System.Xml.XmlConvert.ToString(EngagementsAbsents)),
                new XAttribute(ConstantXML.EngagementsTousCombats, System.Xml.XmlConvert.ToString(EngagementsTousCombats)),
                new XAttribute(ConstantXML.EngagementsScoreGP, System.Xml.XmlConvert.ToString(EngagementsScoreGP)),
                new XAttribute(ConstantXML.EngagementsPositionCombat, System.Xml.XmlConvert.ToString(AfficherPositionCombat)),

                // Utilisation des constantes pour les nouvelles propriétés de poules
                new XAttribute(ConstantXML.useIntituleCommun, System.Xml.XmlConvert.ToString(UseIntituleCommun)),

                // --- Nombres ---
                new XAttribute(ConstantXML.delaiActualisationClientSec, DelaiActualisationClientSec.ToString()),
                new XAttribute(ConstantXML.nbProchainsCombats, NbProchainsCombats.ToString()),

                // --- Chaînes et Dates ---
                new XAttribute(ConstantXML.DateGeneration, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")),
                new XAttribute(ConstantXML.AppVersion, AppInformation.Instance.AppVersion ?? string.Empty),
                new XAttribute(ConstantXML.msgProchainsCombats, MsgProchainsCombats ?? string.Empty),
                new XAttribute(ConstantXML.Logo, Logo ?? string.Empty),
                new XAttribute(ConstantXML.intituleCommun, IntituleCommun ?? string.Empty)
                );
        }
    }
}
