using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Deroulement
{
    public class DeroulementSnapshot : IDeroulementData
    {
        public IReadOnlyList<Combat> Combats { get; private set; }
        public IReadOnlyList<Rencontre> Rencontres { get; private set; }
        public IReadOnlyList<Feuille> Feuilles { get; private set; }
        public IReadOnlyList<Phase_Decoupage> Decoupages { get; private set; }
        public IReadOnlyList<Groupe_Combats> Groupes { get; private set; }
        public IReadOnlyList<Phase> Phases { get; private set; }
        public IReadOnlyList<Poule> Poules { get; private set; }
        public IReadOnlyList<Participant> Participants { get; private set; }
        public IReadOnlyList<vue_groupe> VueGroupes { get; private set; }
        public IReadOnlyList<vue_combat> VueCombats { get; private set; }

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
