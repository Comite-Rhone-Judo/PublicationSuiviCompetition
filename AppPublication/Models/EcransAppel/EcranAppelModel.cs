using System.Collections.Generic;
using System.Net;

namespace AppPublication.Models.EcransAppel
{
    public class EcranAppelModel
    {
        #region MEMBRES
        private static int _compteurGlobal = 1;
        #endregion

        #region PROPERTIES
        /// <summary>
        ///  Type de disposition d'affichage
        /// </summary>
        public enum DispositionAffichage
        {
            Ligne,
            Colonne
        }

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

        public bool AjusteTailleTexte { get; set; }
        public int NbCombatsPage { get; set; }

        public int Groupement { get; set; }

        public DispositionAffichage Disposition { get; set; }

        public DispositionAffichage DispositionCombat { get; set; }
        #endregion

        #region CONSTRUCTEUR
        public EcranAppelModel(int id = 0, string description = "Nouvel Écran", string hostname = "", IPAddress adresseIP = null, List<int> tapisIds = null, int groupement = 1, DispositionAffichage disposition = DispositionAffichage.Colonne, DispositionAffichage dispositionCombat = DispositionAffichage.Colonne, bool ajusteTexte = false)
        {
            Id = id;
            Description = description;
            Hostname = hostname;
            AdresseIP = adresseIP ?? IPAddress.None;
            TapisIds = tapisIds ?? new List<int>();
            Groupement = groupement;
            Disposition = disposition; // Initialisation
            DispositionCombat = dispositionCombat; // Initialisation
            AjusteTailleTexte = ajusteTexte; 
        }
        #endregion
    }
}