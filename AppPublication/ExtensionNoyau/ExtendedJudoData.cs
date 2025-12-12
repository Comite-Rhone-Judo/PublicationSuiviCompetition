using KernelImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.ExtensionNoyau
{
    public class ExtendedJudoData : IExtendedJudoData
    {
        #region MEMBRES
        private JudoData _serverData = null;
        #endregion

        #region CONSTRUCTEURS
        // TODO a voir pour que la donnees soit reprise a chaque fois par rapport au snapshot
        public ExtendedJudoData(JudoData serverData)
        {
            _serverData = serverData;
            _deroulement = new ExtensionNoyau.Engagement.DataEngagement();
        }

        #endregion

        #region PROPERTIES
        private ExtensionNoyau.Engagement.DataEngagement _deroulement = null;
        public ExtensionNoyau.Engagement.DataEngagement Deroulement
        {
            get { return _deroulement; }
            set { _deroulement = value; }
        }
        #endregion

        #region METHODES
        public void SyncAll()
        {
            IJudoData snapshot = _serverData?.GetSnapshot();
            Deroulement.SyncAll(snapshot);
        }

        #endregion
    }
}
