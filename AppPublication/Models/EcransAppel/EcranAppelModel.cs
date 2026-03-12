using System.Collections.Generic;
using System.Net;

namespace AppPublication.Models.EcransAppel
{
    public class EcranAppelModel
    {
        public enum DispositionAffichage
        {
            Ligne,
            Colonne
        }

        public enum ScreenResolution
        {
            FullHd_1080p,   // 1920 x 1080
            UltraHd_4K,     // 3840 x 2160
            UltraHd_8K      // 7680 x 4320
        }

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

        public DispositionAffichage Disposition { get; set; }
        public ScreenResolution Resolution { get; set; }
        public bool Eloigne { get; set; }

        public EcranAppelModel(int id = 0, string description = "Nouvel Écran", string hostname = "", IPAddress adresseIP = null, List<int> tapisIds = null, int groupement = 1, DispositionAffichage disposition = DispositionAffichage.Colonne, ScreenResolution res =ScreenResolution.FullHd_1080p, bool elg = false)
        {
            Id = id;
            Description = description;
            Hostname = hostname;
            AdresseIP = adresseIP ?? IPAddress.None;
            TapisIds = tapisIds ?? new List<int>();
            Groupement = groupement;
            Disposition = disposition; // Initialisation
            Resolution = res;
            Eloigne = elg;
        }
    }
}