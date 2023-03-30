using KernelImpl.Noyau.Organisation;

using System.Linq;
using System.Net;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl
{
    public class JudoData : NotificationBase
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }

        private Competition _competition = new Competition { type = (int)CompetitionTypeEnum.Individuel };
        public Competition competition
        {
            get { return _competition; }
            set { _competition = value; NotifyPropertyChanged("competition"); }
        }


        private Noyau.Structures.DataStructures _structures = null;
        public Noyau.Structures.DataStructures Structures
        {
            get { return _structures; }
            set { _structures = value; }
        }

        private Noyau.Logos.DataLogos _logos = null;
        public Noyau.Logos.DataLogos Logos
        {
            get { return _logos; }
            set { _logos = value; }
        }

        private Noyau.Categories.DataCategories _categories = null;
        public Noyau.Categories.DataCategories Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        private  DataOrganisation _organisation = null;
        public DataOrganisation Organisation
        {
            get { return _organisation; }
            set { _organisation = value; }
        }

        private Noyau.Participants.DataParticipants _participants = null;
        public Noyau.Participants.DataParticipants Participants
        {
            get { return _participants; }
            set { _participants = value; }
        }

        private Noyau.Deroulement.DataDeroulement _deroulement = null;
        public Noyau.Deroulement.DataDeroulement Deroulement
        {
            get { return _deroulement; }
            set { _deroulement = value; }
        }

        private Noyau.Arbitrage.DataArbitrage _arbitrage = null;
        public Noyau.Arbitrage.DataArbitrage Arbitrage
        {
            get { return _arbitrage; }
            set { _arbitrage = value; }
        }


        public int GetCategorie(int annee)
        {
            try
            {
                return _categories.CAges.FirstOrDefault(o => o.anneeMin <= annee && annee <= o.anneeMax).id;
            }
            catch
            {
                return 0;
            }
        }
    }
}
