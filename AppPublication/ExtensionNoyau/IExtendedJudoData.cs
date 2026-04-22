using AppPublication.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau;

namespace AppPublication.ExtensionNoyau
{
    public interface IExtendedJudoData
    {
        /// <summary>
        /// Retourne la section de donnees d'engagement
        /// </summary>
        DataEngagement Engagement { get; }
    }
}
