using KernelImpl.Noyau.Arbitrage;


namespace KernelManager
{
    public class ManagerArbitrage
    {
        public Arbitre CreateArbitre()
        {
            return new Arbitre();
        }

        public Commissaire CreateCommissaire()
        {
            return new Commissaire();
        }

        public Delegue CreateDelegue()
        {
            return new Delegue();
        }
    }
}
