using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Deroulement
{
    /// <summary>
    /// Contrat garantissant la cohérence des données exposées entre le Live (DataDeroulement)
    /// et le Figé (DeroulementSnapshot).
    /// Ne contient QUE des accesseurs de listes, aucune méthode de service (lecture XML, etc.).
    /// </summary>
    public interface IDeroulementData
    {
        IReadOnlyList<Combat> Combats { get; }
        IReadOnlyList<Rencontre> Rencontres { get; }
        IReadOnlyList<Feuille> Feuilles { get; }
        IReadOnlyList<Phase_Decoupage> Decoupages { get; }
        IReadOnlyList<Groupe_Combats> Groupes { get; }
        IReadOnlyList<Phase> Phases { get; }
        IReadOnlyList<Poule> Poules { get; }
        IReadOnlyList<Participant> Participants { get; }

        // Vues
        IReadOnlyList<vue_groupe> VueGroupes { get; }
        IReadOnlyList<vue_combat> VueCombats { get; }
    }
}
