using FranceJudo.Metier.Noyau.Deroulement;
using System.Collections.Generic;

namespace KernelImpl.Noyau.Deroulement
{
    public class DeroulementSnapshot : IDeroulementData
    {
        public IReadOnlyList<ICombat> Combats { get; private set; }
        public IReadOnlyList<IRencontre> Rencontres { get; private set; }
        public IReadOnlyList<IFeuille> Feuilles { get; private set; }
        public IReadOnlyList<IPhase_Decoupage> Decoupages { get; private set; }
        public IReadOnlyList<IGroupe_Combats> Groupes { get; private set; }
        public IReadOnlyList<IPhase> Phases { get; private set; }
        public IReadOnlyList<IPoule> Poules { get; private set; }
        public IReadOnlyList<IParticipant> Participants { get; private set; }
        public IReadOnlyList<Ivue_groupe> VueGroupes { get; private set; }
        public IReadOnlyList<Ivue_combat> VueCombats { get; private set; }

        public DeroulementSnapshot(DataDeroulement source)
        {
            if (source == null) return;
            // Capture atomique des références des listes (DeduplicatedCachedData)
            Combats = source.Combats;
            Rencontres = source.Rencontres;
            Feuilles = source.Feuilles;
            Decoupages = source.Decoupages;
            Groupes = source.Groupes;
            Phases = source.Phases;
            Poules = source.Poules;
            Participants = source.Participants;
            VueGroupes = source.VueGroupes;
            VueCombats = source.VueCombats;
        }
    }
}
