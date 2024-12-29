using KernelImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.ExtensionNoyau
{
    public class ExtensionJudoData
    {
        #region MEMBRES
        private JudoData _serverData = null;
        #endregion

        #region CONSTRUCTEURS
        public ExtensionJudoData(JudoData serverData)
        {
            _serverData = serverData;
            _deroulement = new ExtensionNoyau.Deroulement.DataDeroulement();
        }

        #endregion

        #region PROPERTIES
        private ExtensionNoyau.Deroulement.DataDeroulement _deroulement = null;
        public ExtensionNoyau.Deroulement.DataDeroulement Deroulement
        {
            get { return _deroulement; }
            set { _deroulement = value; }
        }
        #endregion

        #region METHODES
        public void SyncAll()
        {
            Deroulement.SyncAll(_serverData);
        }

        #endregion
    }
}
