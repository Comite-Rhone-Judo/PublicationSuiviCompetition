using KernelImpl;
using KernelImpl.Noyau.Arbitrage;
using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Logos;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using KernelImpl.Noyau.Structures;


namespace KernelManager
{
    public class Manager
    {
        public static Manager manager = new Manager();


        private JudoData _data = null;

        public JudoData m_JudoData
        {
            get
            {
                if (_data == null)
                {
                    _data = CreateJudoData();
                }
                return _data;
            }
        }

        private ManagerArbitrage _arbitrage = new ManagerArbitrage();
        public ManagerArbitrage ManagerArbitrage { get { return _arbitrage; } }

        private ManagerCategories _cate = new ManagerCategories();
        public ManagerCategories ManagerCategories { get { return _cate; } }

        private ManagerDeroulement _deroulement = new ManagerDeroulement();
        public ManagerDeroulement ManagerDeroulement { get { return _deroulement; } }

        private ManagerLogos _logos = new ManagerLogos();
        public ManagerLogos ManagerLogos { get { return _logos; } }

        private ManagerOrganisation _organisation = new ManagerOrganisation();
        public ManagerOrganisation ManagerOrganisation { get { return _organisation; } }

        private ManagerParticipants _participant = new ManagerParticipants();
        public ManagerParticipants ManagerParticipants { get { return _participant; } }

        private ManagerStructures _structure = new ManagerStructures();
        public ManagerStructures ManagerStructures { get { return _structure; } }




        private JudoData CreateJudoData()
        {
            JudoData data = new JudoData();
            data.Arbitrage = new DataArbitrage();
            data.Categories = new DataCategories();
            data.Deroulement = new DataDeroulement();
            data.Logos = new DataLogos();
            data.Organisation = new DataOrganisation();
            data.Participants = new DataParticipants();
            data.Structures = new DataStructures();

            return data;
        }


    }
}
