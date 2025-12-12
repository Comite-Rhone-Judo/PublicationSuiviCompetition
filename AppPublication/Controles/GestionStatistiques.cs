using AppPublication.Statistiques;
using System.Collections.Generic;
using Tools.Outils;

namespace AppPublication.Controles
{
    public class GestionStatistiques : NotificationBase
    {
        public enum CompteurGenerationEnum
        {
            TempsGeneration = 0,
            DelaiGeneration = 1,
            NbGeneration = 2,
            NbErreurGeneration = 3
        }

        public enum CompteurSynchronisationEnum
        {
            TempsSynchronisation = 0,
            NbSynchronisation = 1,
            NbErreurSynchronisation = 2,
            NbFichierSynchronisation = 3
        }

        public enum CompteurDonneesEnum
        {
            NbDemandeSnapshot = 0
        }

        #region MEMBRES
        #endregion

        #region CONSTRUCTEURS
        public GestionStatistiques()
        {
            Dictionary<CompteurGenerationEnum, StatistiqueItem> cpt = new Dictionary<CompteurGenerationEnum, StatistiqueItem>();
            cpt.Add(CompteurGenerationEnum.TempsGeneration, new StatistiqueItemMoyenneur("TempsGeneration", "Durée de génération (Sec.)"));
            cpt.Add(CompteurGenerationEnum.DelaiGeneration, new StatistiqueItemMoyenneur("DelaiGeneration", "Délai entre génération (Sec.)"));
            cpt.Add(CompteurGenerationEnum.NbGeneration, new StatistiqueItemCompteur("NbGeneration", "Nb de génération"));
            cpt.Add(CompteurGenerationEnum.NbErreurGeneration, new StatistiqueItemCompteur("NbErreurGeneration", "Nb d'erreur de génération"));
            CompteursGeneration = cpt;

            Dictionary<CompteurSynchronisationEnum, StatistiqueItem> cptSyncC = new Dictionary<CompteurSynchronisationEnum, StatistiqueItem>();
            cptSyncC.Add(CompteurSynchronisationEnum.TempsSynchronisation, new StatistiqueItemMoyenneur("TempsSynchronisation", "Durée de Synchronisation (Sec.)"));
            cptSyncC.Add(CompteurSynchronisationEnum.NbFichierSynchronisation, new StatistiqueItemMoyenneur("NbFichiersSynchronises", "Nb de fichiers synchronisés"));
            cptSyncC.Add(CompteurSynchronisationEnum.NbSynchronisation, new StatistiqueItemCompteur("NbSynchronisation", "Nb de synchronisation"));
            cptSyncC.Add(CompteurSynchronisationEnum.NbErreurSynchronisation, new StatistiqueItemCompteur("NbErreurSynchronisation", "Nb d'erreur de synchronisation"));
            CompteursSynchronisationComplete = cptSyncC;

            Dictionary<CompteurSynchronisationEnum, StatistiqueItem> cptSyncD = new Dictionary<CompteurSynchronisationEnum, StatistiqueItem>();
            cptSyncD.Add(CompteurSynchronisationEnum.TempsSynchronisation, new StatistiqueItemMoyenneur("TempsSynchronisation", "Durée de Synchronisation (Sec.)"));
            cptSyncD.Add(CompteurSynchronisationEnum.NbFichierSynchronisation, new StatistiqueItemMoyenneur("NbFichiersSynchronises", "Nb de fichiers synchronisés"));
            cptSyncD.Add(CompteurSynchronisationEnum.NbSynchronisation, new StatistiqueItemCompteur("NbSynchronisation", "Nb de synchronisation"));
            cptSyncD.Add(CompteurSynchronisationEnum.NbErreurSynchronisation, new StatistiqueItemCompteur("NbErreurSynchronisation", "Nb d'erreur de synchronisation"));
            CompteursSynchronisationDifference = cptSyncD;

            // --- AJOUT : Initialisation du dictionnaire Données ---
            Dictionary<CompteurDonneesEnum, StatistiqueItem> cptData = new Dictionary<CompteurDonneesEnum, StatistiqueItem>();
            cptData.Add(CompteurDonneesEnum.NbDemandeSnapshot, new StatistiqueItemCompteur("NbDemandeSnapshot", "Resynchro. (Snapshots)"));
            CompteursDonnees = cptData;

        }
        #endregion

        #region PROPRIETES

        private Dictionary<CompteurGenerationEnum, StatistiqueItem> _compteursGeneration = null;
        public Dictionary<CompteurGenerationEnum, StatistiqueItem> CompteursGeneration
        {
            get
            {
                return _compteursGeneration;
            }
            private set
            {
                _compteursGeneration = value;
                NotifyPropertyChanged();
            }
        }

        private Dictionary<CompteurSynchronisationEnum, StatistiqueItem> _compteursSynchronisationComplete = null;
        public Dictionary<CompteurSynchronisationEnum, StatistiqueItem> CompteursSynchronisationComplete
        {
            get
            {
                return _compteursSynchronisationComplete;
            }
            private set
            {
                _compteursSynchronisationComplete = value;
                NotifyPropertyChanged();
            }
        }

        private Dictionary<CompteurSynchronisationEnum, StatistiqueItem> _compteursSynchronisationDifference = null;
        public Dictionary<CompteurSynchronisationEnum, StatistiqueItem> CompteursSynchronisationDifference
        {
            get
            {
                return _compteursSynchronisationDifference;
            }
            private set
            {
                _compteursSynchronisationDifference = value;
                NotifyPropertyChanged();
            }
        }

        private Dictionary<CompteurDonneesEnum, StatistiqueItem> _compteursDonnees = null;
        public Dictionary<CompteurDonneesEnum, StatistiqueItem> CompteursDonnees
        {
            get
            {
                return _compteursDonnees;
            }
            private set
            {
                _compteursDonnees = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region METHODES
        public void EnregistrerErreurGeneration()
        {
            StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.NbErreurGeneration];
            item.EnregistrerValeur();
        }

        public void EnregistrerGeneration(float duree)
        {

            StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.NbGeneration];
            item.EnregistrerValeur();
            item = _compteursGeneration[CompteurGenerationEnum.TempsGeneration];
            item.EnregistrerValeur(duree);
        }

        public void EnregsitrerDelaiGeneration(float delai)
        {
            StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.DelaiGeneration];
            item.EnregistrerValeur(delai);
        }

        public void EnregistrerSynchronisation(float duree, UploadStatus syncStatus)
        {
            // Selectionne le dictionnaire en fonction du type de synchronisation
            Dictionary<CompteurSynchronisationEnum, StatistiqueItem> statDict = (syncStatus.IsComplet) ? _compteursSynchronisationComplete : _compteursSynchronisationDifference;

            // Enregistre les donnees dans les compteurs respectifs
            StatistiqueItem item = statDict[CompteurSynchronisationEnum.TempsSynchronisation];
            item.EnregistrerValeur(duree);
            item = statDict[CompteurSynchronisationEnum.NbSynchronisation];
            item.EnregistrerValeur();

            if (!syncStatus.IsSuccess)
            {
                item = statDict[CompteurSynchronisationEnum.NbErreurSynchronisation];
                item.EnregistrerValeur();
            }

            if (syncStatus.nbUpload > 0)
            {
                item = statDict[CompteurSynchronisationEnum.NbFichierSynchronisation];
                item.EnregistrerValeur(syncStatus.nbUpload);
            }
        }

        public void EnregistrerDemandeSnapshot()
        {
            if (_compteursDonnees != null && _compteursDonnees.ContainsKey(CompteurDonneesEnum.NbDemandeSnapshot))
            {
                StatistiqueItem item = _compteursDonnees[CompteurDonneesEnum.NbDemandeSnapshot];
                item.EnregistrerValeur();
            }
        }

        #endregion

    }
}
