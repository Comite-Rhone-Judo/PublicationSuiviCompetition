using KernelImpl.Noyau.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Logos
{
    public interface ILogosData
    {
        IReadOnlyList<string> Fede { get; }
        IReadOnlyList<string> Ligue { get; }
        IReadOnlyList<string> Sponsors { get; }
    }
}
