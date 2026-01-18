using KernelImpl.Noyau.Arbitrage;
using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Logos;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using KernelImpl.Noyau.Structures;

namespace KernelImpl
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
