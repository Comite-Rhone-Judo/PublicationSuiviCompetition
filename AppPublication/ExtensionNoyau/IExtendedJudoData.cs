using AppPublication.ExtensionNoyau.Engagement;
using AppPublication.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau;

namespace AppPublication.ExtensionNoyau
{
    public interface IExtendedJudoData
    {
        /// <summary>
        /// Retourne la section de donnees d'engagement
        /// </summary>
        IDataEngagement Engagement { get; }

        IDataStatistiquesCombats StatistiquesCombats { get; }
    }
}
