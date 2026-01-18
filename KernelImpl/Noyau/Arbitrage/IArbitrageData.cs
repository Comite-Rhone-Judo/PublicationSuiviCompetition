using KernelImpl.Noyau.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Arbitrage
{
    public interface IArbitrageData
    {
        IReadOnlyList<Commissaire> Commissaires { get; }
        IReadOnlyList<Arbitre> Arbitres { get; }
        IReadOnlyList<Delegue> Delegues { get; }
    }
}
