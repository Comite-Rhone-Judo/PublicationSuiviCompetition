using FranceJudo.Metier.ExtensionNoyau.Engagement;
using FranceJudo.Metier.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau;
using System;
using System.Threading;

namespace FranceJudo.Metier.ExtensionNoyau
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
                () => new DataEngagement(snapshot),
                LazyThreadSafetyMode.ExecutionAndPublication
            );

            _statistiquesCombats = new Lazy<DataStatistiquesCombats>(
                () => new DataStatistiquesCombats(snapshot),
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        #endregion

        #region PROPERTIES

        public IJudoData CoreData { get; }

        public IDataEngagement Engagement => _engagement.Value;

        public IDataStatistiquesCombats StatistiquesCombats => _statistiquesCombats.Value;

        #endregion

        #region PROPERTIES
        /// <summary>
        /// Force le calcul et la mise en cache des statistiques si ce n'est pas déjà fait.
        /// </summary>
        public void EnsureStatistiquesLoaded()
        {
            var _ = _statistiquesCombats.Value; 
        }

        /// <summary>
        /// Force le calcul et la mise en cache des engagements si ce n'est pas déjà fait.
        /// </summary>
        public void EnsureEngagementsLoaded()
        {
            var _ = _engagement.Value;
        }
        #endregion
    }
}
