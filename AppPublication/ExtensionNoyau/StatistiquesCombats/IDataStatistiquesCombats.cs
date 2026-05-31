using System;
using System.Collections.Generic;

namespace AppPublication.ExtensionNoyau.StatistiquesCombats
{
    /// <summary>
    /// Conteneur unique exposant les statistiques de toute la compétition.
    /// </summary>
    public interface IDataStatistiquesCombats
    {
        IReadOnlyDictionary<string, IStatistiquesItem> Statistiques { get; }
    }
}