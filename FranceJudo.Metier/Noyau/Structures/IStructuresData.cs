using System.Collections.Generic;

namespace FranceJudo.Metier.Noyau.Structures
{
    public interface IStructuresData
    {
        IReadOnlyList<IClub> Clubs { get; }
        IReadOnlyList<IComite> Comites { get; }
        IReadOnlyList<ILigue> Ligues { get; }
        IReadOnlyList<ISecteur> Secteurs { get; }
        IReadOnlyList<IPays> LesPays { get; }
    }
}
