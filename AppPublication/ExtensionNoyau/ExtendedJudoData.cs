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

        #region CONSTRUCTEURS
        public ExtendedJudoData()
        {
            _engagement = new ExtensionNoyau.Engagement.DataEngagement();
        }

        #endregion

        #region PROPERTIES
        private ExtensionNoyau.Engagement.DataEngagement _engagement = null;
        public ExtensionNoyau.Engagement.DataEngagement Engagement
        {
            get { return _engagement; }
            set { _engagement = value; }
        }
        #endregion

        #region METHODES
        public void SyncAll(IJudoData snapshot)
        {
            Engagement.SyncAll(snapshot);
        }

        #endregion
    }
}
