using FranceJudo.Metier.Noyau.Arbitrage;
using FranceJudo.Metier.Noyau.Categories;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Logos;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.Noyau.Structures;

namespace FranceJudo.Metier.Noyau
{
    /// <summary>
    /// Interface racine représentant une vue figée et cohérente des données du serveur.
    /// </summary>
    public interface IJudoData
    {
        IDeroulementData Deroulement { get; }
        IParticipantsData Participants { get; }
        IOrganisationData Organisation { get; }
        IStructuresData Structures { get; }
        ICategoriesData Categories { get; }
        IArbitrageData Arbitrage { get; }
        ILogosData Logos { get; }
    }
}
