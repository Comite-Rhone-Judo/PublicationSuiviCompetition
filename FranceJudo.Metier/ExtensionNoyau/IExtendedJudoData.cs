using FranceJudo.Metier.ExtensionNoyau.Engagement;
using FranceJudo.Metier.ExtensionNoyau.StatistiquesCombats;
using FranceJudo.Metier.Noyau;

namespace FranceJudo.Metier.ExtensionNoyau
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
