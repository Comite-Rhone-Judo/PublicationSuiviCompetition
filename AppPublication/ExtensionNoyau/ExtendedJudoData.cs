using AppPublication.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau;
using System;

namespace AppPublication.ExtensionNoyau
{
    public class ExtendedJudoData : IExtendedJudoData
    {
        #region MEMBRES
        private readonly IJudoData _coreData;

        // On encapsule les données calculées dans des objets Lazy
        private readonly Lazy<DataEngagement> _engagement;
        #endregion

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
