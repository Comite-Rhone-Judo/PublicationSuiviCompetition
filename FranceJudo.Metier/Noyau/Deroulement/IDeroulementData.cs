using System.Collections.Generic;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    /// <summary>
    /// Contrat garantissant la cohérence des données exposées entre le Live (DataDeroulement)
    /// et le Figé (DeroulementSnapshot).
    /// Ne contient QUE des accesseurs de listes, aucune méthode de service (lecture XML, etc.).
    /// </summary>
    public interface IDeroulementData
    {
        IReadOnlyList<ICombat> Combats { get; }
        IReadOnlyList<IRencontre> Rencontres { get; }
        IReadOnlyList<IFeuille> Feuilles { get; }
        IReadOnlyList<IPhase_Decoupage> Decoupages { get; }
        IReadOnlyList<IGroupe_Combats> Groupes { get; }
        IReadOnlyList<IPhase> Phases { get; }
        IReadOnlyList<IPoule> Poules { get; }
        IReadOnlyList<IParticipant> Participants { get; }

        // Vues
        IReadOnlyList<IVueGroupe> VueGroupes { get; }
        IReadOnlyList<IVueCombat> VueCombats { get; }
    }
}
