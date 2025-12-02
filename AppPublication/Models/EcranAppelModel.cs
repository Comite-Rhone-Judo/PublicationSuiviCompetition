using System.Collections.Generic;
using System.Net;

namespace AppPublication.Models
{
    public class EcranAppelModel
    {
        private static int _compteurGlobal = 1;

        public static int NextId
        {
            get { return _compteurGlobal++; }
            set { _compteurGlobal = value; }
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public string Hostname { get; set; }
        public IPAddress AdresseIP { get; set; }
        public List<int> TapisIds { get; set; }

        public EcranAppelModel()
        {
            Id = NextId;
            TapisIds = new List<int>();
            Description = "Nouvel Écran";
            Hostname = string.Empty;
            AdresseIP = IPAddress.None;
        }
    }
}