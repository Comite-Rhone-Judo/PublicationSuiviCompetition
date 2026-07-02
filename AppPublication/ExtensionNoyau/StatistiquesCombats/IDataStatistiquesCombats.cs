using System.Collections.Generic;
using FranceJudo.Metier.Noyau.Organisation;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    public interface IDataStatistiquesCombats
    {
        IReadOnlyDictionary<StatistiqueCle, IStatistiquesItem> Statistiques { get; }
        IReadOnlyList<GroupeStatistiques> GroupesStatistiques { get; }
        IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes { get; }
    }
}