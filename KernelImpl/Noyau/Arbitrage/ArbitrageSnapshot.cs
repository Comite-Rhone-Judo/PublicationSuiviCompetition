using KernelImpl.Noyau.Participants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FranceJudo.Metier.Noyau.Arbitrage;

namespace KernelImpl.Noyau.Arbitrage
{
    public class ArbitrageSnapshot : IArbitrageData
    {
        public IReadOnlyList<ICommissaire> Commissaires { get; private set; }
        public IReadOnlyList<IArbitre> Arbitres { get; private set; }
        public IReadOnlyList<IDelegue> Delegues { get; private set; }

        public ArbitrageSnapshot(DataArbitrage source)
        {
            if (source == null) return;
            Commissaires = source.Commissaires;
            Arbitres = source.Arbitres;
            Delegues = source.Delegues;
        }
    }
}
