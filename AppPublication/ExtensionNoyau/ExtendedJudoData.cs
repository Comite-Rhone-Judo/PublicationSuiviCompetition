using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.ExtensionNoyau.StatistiquesCombats;
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
        private readonly Lazy<DataStatistiquesCombats> _statistiquesCombats;
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

            _statistiquesCombats = new Lazy<DataStatistiquesCombats>(
                () => new DataStatistiquesCombats(snapshot), // <-- La magie est ici !
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        #endregion

        #region PROPERTIES

        public IJudoData CoreData { get; }

        public IDataEngagement Engagement => _engagement.Value;

        public IDataStatistiquesCombats StatistiquesCombats => _statistiquesCombats.Value;

        #endregion
    }
}
