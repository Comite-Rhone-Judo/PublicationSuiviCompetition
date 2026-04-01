using System.Collections.Generic;

namespace FranceJudo.Metier.Noyau.Logos
{
    public interface ILogosData
    {
        IReadOnlyList<string> Fede { get; }
        IReadOnlyList<string> Ligue { get; }
        IReadOnlyList<string> Sponsors { get; }
    }
}
