using System.Collections.Generic;
using System.Net;

namespace AppPublication.Models.EcransAppel
{
    public class EcranAppelModel
    {
        private static int _compteurGlobal = 1;

        public static int LastId
        {
            get { return _compteurGlobal++; }
            set { _compteurGlobal = value; }
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public string Hostname { get; set; }
        public IPAddress AdresseIP { get; set; }
        public List<int> TapisIds { get; set; }

        public int Groupement { get; set; }

        public EcranAppelModel(int id = 0, string description = "Nouvel Écran", string hostname = "", IPAddress adresseIP = null, List<int> tapisIds = null, int groupement = 1)
        {
            Id = id;
            Description = description;
            Hostname = hostname;
            AdresseIP = adresseIP ?? IPAddress.None;
            TapisIds = tapisIds ?? new List<int>();
            Groupement = groupement;
        }
    }
}