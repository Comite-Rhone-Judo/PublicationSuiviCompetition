using FranceJudo.Metier.Noyau.Organisation;
using System.Collections.Generic;


namespace FranceJudo.Metier.ExtensionNoyau.Engagement
{
    public interface IDataEngagement
    {
        /// <summary>
        /// Les groupes d'engages
        /// </summary>
        IReadOnlyList<GroupeEngagements> GroupesEngages { get; }


        /// <summary>
        /// Les types de groupes pour chaque competition
        /// </summary>
        IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes { get; }
    }
}
