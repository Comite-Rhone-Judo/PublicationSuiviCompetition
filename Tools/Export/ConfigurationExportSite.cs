using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Export
{
    public class ConfigurationExportSite
    {

        public ConfigurationExportSite(bool pubPC = false, bool pubAT = true, long delAC = 30, int nbPC = 6)
        {
            PublierProchainsCombats = pubPC;
            PublierAffectationTapis = pubAT;
            DelaiActualisationClientSec = delAC;
            NbProchainsCombats = nbPC;
        }

        public bool PublierProchainsCombats = false;
        public bool PublierAffectationTapis = true;
        public long DelaiActualisationClientSec = 30;
        public int NbProchainsCombats = 6;
    }
}
