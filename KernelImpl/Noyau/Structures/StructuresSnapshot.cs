using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Structures
{
    public class StructuresSnapshot : IStructuresData
    {
        public IReadOnlyList<Club> Clubs { get; }
        public IReadOnlyList<Comite> Comites { get; }
        public IReadOnlyList<Ligue> Ligues { get; }
        public IReadOnlyList<Secteur> Secteurs { get; }
        public IReadOnlyList<Pays> LesPays { get; }

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
