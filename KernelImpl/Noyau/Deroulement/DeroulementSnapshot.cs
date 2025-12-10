using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Deroulement
{
    public class DeroulementSnapshot : IDeroulementData
    {
        public IReadOnlyList<Combat> Combats { get; }
        public IReadOnlyList<Rencontre> Rencontres { get; }
        public IReadOnlyList<Feuille> Feuilles { get; }
        public IReadOnlyList<Phase_Decoupage> Decoupages { get; }
        public IReadOnlyList<Groupe_Combats> Groupes { get; }
        public IReadOnlyList<Phase> Phases { get; }
        public IReadOnlyList<Poule> Poules { get; }
        public IReadOnlyList<Participant> Participants { get; }
        public IReadOnlyList<vue_groupe> VueGroupes { get; }
        public IReadOnlyList<vue_combat> VueCombats { get; }

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
