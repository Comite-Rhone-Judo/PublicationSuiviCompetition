using System.Collections.Generic;

namespace FranceJudo.Metier.Noyau.Organisation
{
    public interface IOrganisationData
    {
        ICompetition Competition { get; }
        IReadOnlyList<ICompetition> Competitions { get; }
        IReadOnlyList<IEpreuve> Epreuves { get; }
        IReadOnlyList<IEpreuve_Equipe> EpreuveEquipes { get; }
        IReadOnlyList<IVueEpreuveEquipe> VueEpreuveEquipes { get; }
        IReadOnlyList<IVueEpreuve> VueEpreuves { get; }
    }
}
