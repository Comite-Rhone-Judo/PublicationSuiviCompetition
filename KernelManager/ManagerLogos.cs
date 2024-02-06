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
    public class ManagerLogos
    {
        private static JudoData _data = null;

        public static JudoData m_JudoData
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

        private static JudoData CreateJudoData()
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
