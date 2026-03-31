using FranceJudo.Metier.Noyau.Logos;
using System.Collections.Generic;

namespace KernelImpl.Noyau.Logos
{
    public class LogosSnapshot : ILogosData
    {
        public IReadOnlyList<string> Fede { get; private set; }
        public IReadOnlyList<string> Ligue { get; private set; }
        public IReadOnlyList<string> Sponsors { get; private set; }

        public LogosSnapshot(DataLogos source)
        {
            if (source == null) return;
            Fede = source.Fede;
            Ligue = source.Ligue;
            Sponsors = source.Sponsors;
        }
    }
}
