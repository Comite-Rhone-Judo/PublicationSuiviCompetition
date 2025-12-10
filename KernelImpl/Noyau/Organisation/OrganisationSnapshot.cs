using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Organisation
{
    public class OrganisationSnapshot : IOrganisationData
    {
        public IReadOnlyList<Competition> Competitions { get; }
        public IReadOnlyList<Epreuve> Epreuves { get; }
        public IReadOnlyList<Epreuve_Equipe> EpreuveEquipes { get; }
        public IReadOnlyList<vue_epreuve_equipe> VueEpreuveEquipes { get; }
        public IReadOnlyList<vue_epreuve> VueEpreuves { get; }

        public OrganisationSnapshot(DataOrganisation source)
        {
            if (source == null) return;
            Competitions = source.Competitions;
            Epreuves = source.Epreuves;
            EpreuveEquipes = source.EpreuveEquipes;
            VueEpreuveEquipes = source.VueEpreuveEquipes;
            VueEpreuves = source.VueEpreuves;
        }
    }
}
