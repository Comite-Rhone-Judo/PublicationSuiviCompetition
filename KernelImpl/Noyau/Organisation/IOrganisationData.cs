using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Organisation
{
    public interface IOrganisationData
    {
        IReadOnlyList<Competition> Competitions { get; }
        IReadOnlyList<Epreuve> Epreuves { get; }
        IReadOnlyList<Epreuve_Equipe> EpreuveEquipes { get; }
        IReadOnlyList<vue_epreuve_equipe> VueEpreuveEquipes { get; }
        IReadOnlyList<vue_epreuve> VueEpreuves { get; }
    }
}
