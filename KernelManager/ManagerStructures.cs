using KernelImpl.Noyau.Structures;


namespace KernelManager
{
    public class ManagerStructures
    {
        public Pays CreatePays()
        {
            return new Pays();
        }

        public Ligue CreateLigue()
        {
            return new Ligue();
        }
        public Comite CreateComite()
        {
            return new Comite();
        }
        public Club CreateClub()
        {
            return new Club();
        }

    }
}
