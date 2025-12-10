using KernelImpl.Noyau.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Logos
{
    public class LogosSnapshot : ILogosData
    {
        public IReadOnlyList<string> Fede { get; }
        public IReadOnlyList<string> Ligue { get; }
        public IReadOnlyList<string> Sponsors { get; }

        public LogosSnapshot(DataLogos   source)
        {
            if (source == null) return;
            Fede = source.Fede;
            Ligue = source.Ligue;
            Sponsors = source.Sponsors;
        }
    }
}
