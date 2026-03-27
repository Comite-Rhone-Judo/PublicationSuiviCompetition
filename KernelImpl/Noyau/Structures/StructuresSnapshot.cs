using System;
using System.Collections.Generic;
using FranceJudo.Core.XML;
using FranceJudo.Metier.XML;
using FranceJudo.Metier.Noyau.Structures;


namespace KernelImpl.Noyau.Structures
{
    public class StructuresSnapshot : IStructuresData
    {
        public IReadOnlyList<IClub> Clubs { get; private set; }
        public IReadOnlyList<IComite> Comites { get; private set; }
        public IReadOnlyList<ILigue> Ligues { get; private set; }
        public IReadOnlyList<ISecteur> Secteurs { get;   private set;     }
        public IReadOnlyList<IPays> LesPays { get; private set; }

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
