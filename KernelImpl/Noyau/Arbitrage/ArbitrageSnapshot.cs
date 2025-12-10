using KernelImpl.Noyau.Participants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl.Noyau.Arbitrage
{
    public class ArbitrageSnapshot : IArbitrageData
    {
        public IReadOnlyList<Commissaire> Commissaires { get; }
        public IReadOnlyList<Arbitre> Arbitres { get; }
        public IReadOnlyList<Delegue> Delegues { get; }

        public ArbitrageSnapshot(DataArbitrage source)
        {
            if (source == null) return;
            Commissaires = source.Commissaires;
            Arbitres = source.Arbitres;
            Delegues = source.Delegues;
        }
    }
}
