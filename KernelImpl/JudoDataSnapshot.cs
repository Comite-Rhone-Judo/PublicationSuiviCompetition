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
    /// Implémentation concrète du snapshot.
    /// Cet objet est immuable : une fois créé, ses listes ne changent jamais.
    /// </summary>
    public class JudoDataSnapshot : IJudoData
    {
        public IDeroulementData Deroulement { get; private set; }
        public IParticipantsData Participants { get; private set; }
        public IOrganisationData Organisation { get; private set; }
        public IStructuresData Structures { get; private set; }
        public ICategoriesData Categories { get; private set; }
        public IArbitrageData Arbitrage { get; private set; }
        public ILogosData Logos { get; private set; }

        internal JudoDataSnapshot(JudoData source)
        {
            // Capture des références vers les gestionnaires existants
            // Note: Les sous-snapshots capturent les LISTES, pas les objets DataManager
            Deroulement = new DeroulementSnapshot(source.Deroulement);
            Participants = new ParticipantsSnapshot(source.Participants);
            Organisation = new OrganisationSnapshot(source.Organisation);
            Structures = new StructuresSnapshot(source.Structures);
            Categories = new CategoriesSnapshot(source.Categories);
            Arbitrage = new ArbitrageSnapshot(source.Arbitrage);
            Logos = new LogosSnapshot(source.Logos);
        }
    }
}
