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

        public bool AfficheCategorieAge { get; set; } = false;

        public int NbCombatsPage { get; set; }

        public int Groupement { get; set; }

        public DispositionAffichage Disposition { get; set; }

        public DispositionAffichage DispositionCombat { get; set; }
        #endregion

        #region CONSTRUCTEUR
        public EcranAppelModel(int id = 0, string description = "Nouvel Écran", string hostname = "", IPAddress adresseIP = null, List<int> tapisIds = null, int groupement = 1, DispositionAffichage disposition = DispositionAffichage.Colonne, DispositionAffichage dispositionCombat = DispositionAffichage.Colonne, bool ajusteTexte = false, int nbCombatPage = 8, bool afficheCategorieAge = false)
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
            NbCombatsPage = nbCombatPage;
            AfficheCategorieAge = false;
        }
        #endregion

        #region METHODES
        public EcranAppelModel Clone()
        {
            return new EcranAppelModel
            {
                Id = this.Id,
                Description = this.Description,
                Hostname = this.Hostname,
                AdresseIP = this.AdresseIP,
                TapisIds = new List<int>(this.TapisIds ?? new List<int>()), // Clonage de la liste pour éviter les références partagées
                Groupement = this.Groupement,
                Disposition = this.Disposition,
                DispositionCombat = this.DispositionCombat,
                AjusteTailleTexte = this.AjusteTailleTexte,
                NbCombatsPage = this.NbCombatsPage,
                AfficheCategorieAge = this.AfficheCategorieAge
            };
        }
        #endregion
    }
}