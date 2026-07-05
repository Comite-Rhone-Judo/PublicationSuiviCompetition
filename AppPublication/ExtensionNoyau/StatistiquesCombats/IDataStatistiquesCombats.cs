using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using System.Collections.Generic;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    public interface IDataStatistiquesCombats
    {
        public IReadOnlyDictionary<int, IStatistiquesItem> StatsJudokas { get; }
        public IReadOnlyDictionary<GroupeStatistiques, IStatistiquesItem> StatsStructures { get; }
        public IReadOnlyDictionary<GroupeStatistiques, List<IVueJudoka>> JudokasParGroupe { get; }

        public IReadOnlyList<GroupeStatistiques> GroupesStatistiques { get; }
        public IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes { get; }
    }
}