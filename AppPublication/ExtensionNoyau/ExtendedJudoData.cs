using AppPublication.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau;
using System;
using System.Threading;

namespace AppPublication.ExtensionNoyau
{
    public class ExtendedJudoData : IExtendedJudoData
    {
        #region MEMBRES
        // On encapsule les données calculées dans des objets Lazy
        private readonly Lazy<DataEngagement> _engagement;
        #endregion

        #region CONSTRUCTEURS
        public ExtendedJudoData(IJudoData snapshot)
        {
            CoreData = snapshot;

            // On "arme" le cache. Le 'new DataEngagement(snapshot)' ne s'exécutera 
            // QUE si le générateur demande la propriété 'Engagements'.
            _engagement = new Lazy<DataEngagement>(
                () => new DataEngagement(snapshot), // <-- La magie est ici !
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        #endregion

        #region PROPERTIES

        public IJudoData CoreData { get; }

        public DataEngagement Engagement => _engagement.Value;

        #endregion
    }
}
