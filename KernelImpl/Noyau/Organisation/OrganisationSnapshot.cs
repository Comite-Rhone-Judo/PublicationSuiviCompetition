using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Organisation
{
    public class OrganisationSnapshot : IOrganisationData
    {
        public Competition Competition { get; private set; }

        public IReadOnlyList<Competition> Competitions { get; private set; }
        public IReadOnlyList<Epreuve> Epreuves { get; private set; }
        public IReadOnlyList<Epreuve_Equipe> EpreuveEquipes { get; private set; }
        public IReadOnlyList<vue_epreuve_equipe> VueEpreuveEquipes { get; private set; }
        public IReadOnlyList<vue_epreuve> VueEpreuves { get; private set; }

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
