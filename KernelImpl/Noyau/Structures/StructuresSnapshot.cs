using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Structures
{
    public class StructuresSnapshot : IStructuresData
    {
        public IReadOnlyList<Club> Clubs { get; private set; }
        public IReadOnlyList<Comite> Comites { get; private set; }
        public IReadOnlyList<Ligue> Ligues { get; private set; }
        public IReadOnlyList<Secteur> Secteurs { get;   private set;     }
        public IReadOnlyList<Pays> LesPays { get; private set; }

        public StructuresSnapshot(DataStructures source)
        {
            if (source == null) return;
            Clubs = source.Clubs;
            Comites = source.Comites;
            Ligues = source.Ligues;
            Secteurs = source.Secteurs;
            LesPays = source.LesPays;
        }
    }
}
