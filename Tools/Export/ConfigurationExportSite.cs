using HttpServer.Helpers;
using System.Web.UI.WebControls;
using Tools.Enum;

namespace Tools.Export
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
            MsgProchainCombats = pMsg;
            Logo = string.IsNullOrEmpty(pLogo) ? ConstantResource.Export_DefaultLogo : pLogo;
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
        public string MsgProchainCombats = string.Empty;
        public string Logo = ConstantResource.Export_DefaultLogo;

        public bool PouleEnColonnes = false;
        public bool PouleToujoursEnColonnes = false;
        public int TailleMaxPouleColonnes = 5;

        public bool UseIntituleCommun = false;
        public string IntituleCommun = string.Empty;
    }
}
