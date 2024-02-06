using System.Web.UI.WebControls;
using Tools.Enum;

namespace Tools.Export
{
    public class ConfigurationExportSite
    {

        public ConfigurationExportSite(bool pubPC = false, bool pubAT = true, long delAC = 30, int nbPC = 6, string pMsg = "", string pLogo = "")
        {
            PublierProchainsCombats = pubPC;
            PublierAffectationTapis = pubAT;
            DelaiActualisationClientSec = delAC;
            NbProchainsCombats = nbPC;
            MsgProchainCombats = pMsg;
            Logo = string.IsNullOrEmpty(pLogo) ? ConstantResource.Export_DefaultLogo : pLogo;
        }

        public bool PublierProchainsCombats = false;
        public bool PublierAffectationTapis = true;
        public long DelaiActualisationClientSec = 30;
        public int NbProchainsCombats = 6;
        public string MsgProchainCombats = string.Empty;
        public string Logo = ConstantResource.Export_DefaultLogo;
    }
}
