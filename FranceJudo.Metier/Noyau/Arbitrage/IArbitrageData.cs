using System.Collections.Generic;


namespace FranceJudo.Metier.Noyau.Arbitrage
{
    public interface IArbitrageData
    {
        IReadOnlyList<ICommissaire> Commissaires { get; }
        IReadOnlyList<IArbitre> Arbitres { get; }
        IReadOnlyList<IDelegue> Delegues { get; }
    }
}
