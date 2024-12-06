using System.Web.UI.WebControls;
using Tools.Enum;

namespace Tools.Export
{
    public class ConfigurationExportSite
    {

        public ConfigurationExportSite(bool pubPC = false, bool pubAT = true, bool pubP = true, bool partClub = true, long delAC = 30, int nbPC = 6, string pMsg = "", string pLogo = "", bool pec = false, bool ptec = false, int maxpc = 5)
        {
            PublierProchainsCombats = pubPC;
            PublierAffectationTapis = pubAT;
            PublierParticpants = pubP;
            ParticipantsParClub = partClub;
            DelaiActualisationClientSec = delAC;
            NbProchainsCombats = nbPC;
            MsgProchainCombats = pMsg;
            Logo = string.IsNullOrEmpty(pLogo) ? ConstantResource.Export_DefaultLogo : pLogo;
            PouleEnColonnes = pec;
            PouleToujoursEnColonnes = ptec;
            TailleMaxPouleColonnes = maxpc;
        }

        public bool PublierProchainsCombats = false;
        public bool PublierAffectationTapis = true;
        public bool PublierParticpants = false;
        public bool ParticipantsParClub = true;
        public long DelaiActualisationClientSec = 30;
        public int NbProchainsCombats = 6;
        public string MsgProchainCombats = string.Empty;
        public string Logo = ConstantResource.Export_DefaultLogo;

        public bool PouleEnColonnes = false;
        public bool PouleToujoursEnColonnes = false;
        public int TailleMaxPouleColonnes = 5;
    }
}
