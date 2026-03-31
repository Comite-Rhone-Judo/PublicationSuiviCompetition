using FranceJudo.Core.Environment;
using FranceJudo.Metier.Resources;
using FranceJudo.Metier.XML;
using System;
using System.Xml.Linq;


namespace AppPublication.Export
{
    public class ConfigurationExportSite
    {
        public ConfigurationExportSite(bool pubPC = false, bool pubAT = true, bool pubP = true, bool partAbsent = false, bool partTC = false, bool scoreGP = false, bool affPosC = false, long delAC = 30, int nbPC = 6, string pMsg = "", string pLogo = "", bool pec = false, bool ptec = false, int maxpc = 5, bool pUseIC = false, string pIC = "")
        {
            PublierProchainsCombats = pubPC;
            PublierAffectationTapis = pubAT;
            PublierEngagements = pubP;
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

        public bool PublierProchainsCombats = false;
        public bool PublierAffectationTapis = true;
        public bool PublierEngagements = false;
        public bool EngagementsAbsents = false;
        public bool EngagementsTousCombats = false;
        public bool EngagementsScoreGP = false;
        public bool AfficherPositionCombat = false;
        public long DelaiActualisationClientSec = 30;
        public int NbProchainsCombats = 6;
        public string MsgProchainsCombats = string.Empty;
        public string Logo = MetierResources.Files.DefaultLogo;

        public bool PouleEnColonnes = false;
        public bool PouleToujoursEnColonnes = false;
        public int TailleMaxPouleColonnes = 5;

        public bool UseIntituleCommun = false;
        public string IntituleCommun = string.Empty;

        public XElement ToXml()
        {
            // Utilisation de la constante pour le nom de la balise racine de configuration
            return new XElement(ConstantXML.SiteConfiguration,

                // --- Booléens (Conversion native XML) ---
                new XAttribute(ConstantXML.publierProchainsCombats, System.Xml.XmlConvert.ToString(PublierProchainsCombats)),
                new XAttribute(ConstantXML.publierAffectationTapis, System.Xml.XmlConvert.ToString(PublierAffectationTapis)),
                new XAttribute(ConstantXML.publierEngagements, System.Xml.XmlConvert.ToString(PublierEngagements)),
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
