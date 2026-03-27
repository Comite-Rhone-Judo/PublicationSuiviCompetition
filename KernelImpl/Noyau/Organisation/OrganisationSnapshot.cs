using System.Collections.Generic;
using FranceJudo.Metier.Noyau.Organisation;

namespace KernelImpl.Noyau.Organisation
{
    public class OrganisationSnapshot : IOrganisationData
    {
        public ICompetition Competition { get; private set; }

        public IReadOnlyList<ICompetition> Competitions { get; private set; }
        public IReadOnlyList<IEpreuve> Epreuves { get; private set; }
        public IReadOnlyList<IEpreuve_Equipe> EpreuveEquipes { get; private set; }
        public IReadOnlyList<Ivue_epreuve_equipe> VueEpreuveEquipes { get; private set; }
        public IReadOnlyList<Ivue_epreuve> VueEpreuves { get; private set; }

        public OrganisationSnapshot(DataOrganisation source)
        {
            if (source == null) return;
            Competition = source.Competition;
            Competitions = source.Competitions;
            Epreuves = source.Epreuves;
            EpreuveEquipes = source.EpreuveEquipes;
            VueEpreuveEquipes = source.VueEpreuveEquipes;
            VueEpreuves = source.VueEpreuves;
        }
    }
}
