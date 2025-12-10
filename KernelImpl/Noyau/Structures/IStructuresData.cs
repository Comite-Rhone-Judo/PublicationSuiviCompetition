using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Structures
{
    public interface IStructuresData
    {
        IReadOnlyList<Club> Clubs { get; }
        IReadOnlyList<Comite> Comites { get; }
        IReadOnlyList<Ligue> Ligues { get; }
        IReadOnlyList<Secteur> Secteurs { get; }
        IReadOnlyList<Pays> LesPays { get; }
    }
}
