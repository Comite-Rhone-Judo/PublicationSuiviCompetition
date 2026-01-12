using AppPublication.Generation;
using AppPublication.Statistiques;
using System.Collections.Generic;
using Tools.Framework;
using Tools.Logging;
using Tools.Net;

namespace AppPublication.Controles
{
    public class GestionStatistiques : NotificationBase
    {
        #region PROPRIETES

        // 1. Module GENERATION
        private StatMgrGeneration _generation;
        public StatMgrGeneration Generation
        {
            get => _generation;
            private set { _generation = value; NotifyPropertyChanged(); }
        }

        // 2. Module SYNCHRONISATION (Gère Complete et Diff en interne)
        private StatMgrSynchronisation _synchronisation;
        public StatMgrSynchronisation Synchronisation
        {
            get => _synchronisation;
            private set { _synchronisation = value; NotifyPropertyChanged(); }
        }

        // 3. Module DONNEES
        private StatMgrDonnees _donnees;
        public StatMgrDonnees Donnees
        {
            get => _donnees;
            private set { _donnees = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region CONSTRUCTEUR

        public GestionStatistiques()
        {
            Generation = new StatMgrGeneration();
            Synchronisation = new StatMgrSynchronisation();
            Donnees = new StatMgrDonnees();
        }

        #endregion

        /*
        public enum CompteurGenerationEnum
        {
            TempsGeneration = 0,
            DelaiGeneration = 1,
            // NbGeneration = 2,
            NbErreurGeneration = 3
        }

        public enum CompteurSynchronisationEnum
        {
            TempsSynchronisation = 0,
            // NbSynchronisation = 1,
            NbErreurSynchronisation = 2,
            NbFichierSynchronisation = 3
        }

        public enum CompteurDonneesEnum
        {
            NbDemandeSnapshot = 0,
            // NbSnapshotCompletsRecus = 1,
            // NbSnapshotDifferentielRecus = 2,
            NbSnapshotInvalidesRecus = 3,
            NbSnapshotIgnores = 4,
            NbConnexion = 5,
            NbDeconnexion = 6,
            DelaiEchange = 7,
            DelaiIntegrationSnapshotComplet = 8,
            DelaiIntegrationSnapshotDifferentiel = 9,
            NbErreurReceptionDonnees = 10
        }

        #region MEMBRES
        #endregion

        #region CONSTRUCTEURS
        public GestionStatistiques()
        {
            // TODO il faut revoir cette partie pour que on ne passe plus GestionStat au complet mais le bloc correspondant via une classe qui encapsule les 

            try
            {
                Dictionary<CompteurGenerationEnum, StatistiqueItem> cpt = new Dictionary<CompteurGenerationEnum, StatistiqueItem>();
                cpt.Add(CompteurGenerationEnum.TempsGeneration, new StatistiqueItemMoyenneur(CompteurGenerationEnum.TempsGeneration.ToString(), "Durée de génération (Sec.)"));
                cpt.Add(CompteurGenerationEnum.DelaiGeneration, new StatistiqueItemMoyenneur(CompteurGenerationEnum.DelaiGeneration.ToString(), "Délai entre génération (Sec.)"));
                // cpt.Add(CompteurGenerationEnum.NbGeneration, new StatistiqueItemCompteur(CompteurGenerationEnum.NbGeneration.ToString(), "Nb de générations"));
                cpt.Add(CompteurGenerationEnum.NbErreurGeneration, new StatistiqueItemCompteur(CompteurGenerationEnum.NbErreurGeneration.ToString(), "Nb d'erreurs de génération"));
                CompteursGeneration = cpt;

                Dictionary<CompteurSynchronisationEnum, StatistiqueItem> cptSyncC = new Dictionary<CompteurSynchronisationEnum, StatistiqueItem>();
                cptSyncC.Add(CompteurSynchronisationEnum.TempsSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.TempsSynchronisation.ToString(), "Durée de Synchronisation (Sec.)"));
                cptSyncC.Add(CompteurSynchronisationEnum.NbFichierSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.NbFichierSynchronisation.ToString(), "Nb de fichiers synchronisés"));
                // cptSyncC.Add(CompteurSynchronisationEnum.NbSynchronisation, new StatistiqueItemCompteur(CompteurSynchronisationEnum.NbSynchronisation.ToString(), "Nb de synchronisatiosn"));
                cptSyncC.Add(CompteurSynchronisationEnum.NbErreurSynchronisation, new StatistiqueItemCompteur(CompteurSynchronisationEnum.NbErreurSynchronisation.ToString(), "Nb d'erreurs de synchronisation"));
                CompteursSynchronisationComplete = cptSyncC;

                Dictionary<CompteurSynchronisationEnum, StatistiqueItem> cptSyncD = new Dictionary<CompteurSynchronisationEnum, StatistiqueItem>();
                cptSyncD.Add(CompteurSynchronisationEnum.TempsSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.TempsSynchronisation.ToString(), "Durée de Synchronisation (Sec.)"));
                cptSyncD.Add(CompteurSynchronisationEnum.NbFichierSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.NbFichierSynchronisation.ToString(), "Nb de fichiers synchronisés"));
                // cptSyncD.Add(CompteurSynchronisationEnum.NbSynchronisation, new StatistiqueItemCompteur(CompteurSynchronisationEnum.NbSynchronisation.ToString(), "Nb de synchronisations"));
                cptSyncD.Add(CompteurSynchronisationEnum.NbErreurSynchronisation, new StatistiqueItemCompteur(CompteurSynchronisationEnum.NbErreurSynchronisation.ToString(), "Nb d'erreurs de synchronisation"));
                CompteursSynchronisationDifference = cptSyncD;

                // --- AJOUT : Initialisation du dictionnaire Données ---
                Dictionary<CompteurDonneesEnum, StatistiqueItem> cptData = new Dictionary<CompteurDonneesEnum, StatistiqueItem>();
                cptData.Add(CompteurDonneesEnum.NbDemandeSnapshot, new StatistiqueItemCompteur(CompteurDonneesEnum.NbDemandeSnapshot.ToString(), "Resynchro. (Snapshots)"));
                // cptData.Add(CompteurDonneesEnum.NbSnapshotCompletsRecus, new StatistiqueItemCompteur(CompteurDonneesEnum.NbSnapshotCompletsRecus.ToString(), "Nb de snapshots complets reçus"));
                cptData.Add(CompteurDonneesEnum.DelaiIntegrationSnapshotComplet, new StatistiqueItemMoyenneur(CompteurDonneesEnum.DelaiIntegrationSnapshotComplet.ToString(), "Intégration Snapshot complet (Ms)"));
                // cptData.Add(CompteurDonneesEnum.NbSnapshotDifferentielRecus, new StatistiqueItemCompteur(CompteurDonneesEnum.NbSnapshotDifferentielRecus.ToString(), "Nb de snapshots différentiels reçus"));
                cptData.Add(CompteurDonneesEnum.DelaiIntegrationSnapshotDifferentiel, new StatistiqueItemMoyenneur(CompteurDonneesEnum.DelaiIntegrationSnapshotDifferentiel.ToString(), "Intégration Snapshot diff. (Ms)"));
                cptData.Add(CompteurDonneesEnum.NbSnapshotInvalidesRecus, new StatistiqueItemCompteur(CompteurDonneesEnum.NbSnapshotInvalidesRecus.ToString(), "Nb de snapshots invalides reçus"));
                cptData.Add(CompteurDonneesEnum.NbSnapshotIgnores, new StatistiqueItemCompteur(CompteurDonneesEnum.NbSnapshotIgnores.ToString(), "Nb de snapshots ignores"));
                cptData.Add(CompteurDonneesEnum.NbConnexion, new StatistiqueItemCompteur(CompteurDonneesEnum.NbConnexion.ToString(), "Nb de connexions"));
                cptData.Add(CompteurDonneesEnum.NbDeconnexion, new StatistiqueItemCompteur(CompteurDonneesEnum.NbDeconnexion.ToString(), "Nb de déconnexions"));
                cptData.Add(CompteurDonneesEnum.DelaiEchange, new StatistiqueItemMoyenneur(CompteurDonneesEnum.DelaiEchange.ToString(), "Délai Demande/Réponse (Ms)"));
                cptData.Add(CompteurDonneesEnum.NbErreurReceptionDonnees, new StatistiqueItemMoyenneur(CompteurDonneesEnum.NbErreurReceptionDonnees.ToString(), "Nb d'erreur de données"));
                CompteursDonnees = cptData;
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'initialisation des statistiques");
            }
        }
        #endregion

        #region PROPRIETES

        // TODO Ajoute les statitiques de generation privee

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
            try
            {
                StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.NbErreurGeneration];
                item?.EnregistrerValeur();
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement d'une erreur de generation");
            }
        }

        public void EnregistrerGeneration(float duree)
        {
            try
            {
                // StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.NbGeneration];
                // item.EnregistrerValeur();
                StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.TempsGeneration];
                item?.EnregistrerValeur(duree);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement d'une generation");
            }
        }

        public void EnregistrerDelaiGeneration(float delai)
        {
            try
            {
                StatistiqueItem item = _compteursGeneration[CompteurGenerationEnum.DelaiGeneration];
                item?.EnregistrerValeur(delai);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du delai de generation");
            }
        }

        public void EnregistrerSynchronisation(float duree, ResultatOperation syncStatus)
        {
            try
            {
                // Selectionne le dictionnaire en fonction du type de synchronisation
                Dictionary<CompteurSynchronisationEnum, StatistiqueItem> statDict = (syncStatus.IsComplete) ? _compteursSynchronisationComplete : _compteursSynchronisationDifference;

                // Enregistre les donnees dans les compteurs respectifs
                StatistiqueItem item = statDict[CompteurSynchronisationEnum.TempsSynchronisation];
                item?.EnregistrerValeur(duree);
                // item = statDict[CompteurSynchronisationEnum.NbSynchronisation];
                // item.EnregistrerValeur();

                if (!syncStatus.IsSuccess)
                {
                    item = statDict[CompteurSynchronisationEnum.NbErreurSynchronisation];
                    item?.EnregistrerValeur();
                }

                if (syncStatus.NbElements > 0)
                {
                    item = statDict[CompteurSynchronisationEnum.NbFichierSynchronisation];
                    item?.EnregistrerValeur(syncStatus.NbElements);
                }
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement d'une synchronisation");
            }
        }

        public void EnregistrerDemandeSnapshot()
        {
            EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbDemandeSnapshot);
        }

        public void EnregistrerSnapshotCompletRecu(double delai)
        {
            try
            {
                // EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbSnapshotCompletsRecus);
                StatistiqueItem item = _compteursDonnees[CompteurDonneesEnum.DelaiIntegrationSnapshotComplet];
                item?.EnregistrerValeur((float)delai);
                LogTools.Logger.Debug("delai integration: {0} ms", delai);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du delai d'integration du snapshot complet");
            }
        }

        public void EnregistrerSnapshotDifferentielRecu(double delai)
        {
            try
            {
                // EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbSnapshotDifferentielRecus);
                StatistiqueItem item = _compteursDonnees[CompteurDonneesEnum.DelaiIntegrationSnapshotDifferentiel];
                item?.EnregistrerValeur((float)delai);
                LogTools.Logger.Debug("delai integration: {0} ms", delai);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du delai d'integration du snapshot differentiel");
            }
        }

        public void EnregistrerSnapshotInvalideRecu()
        {
            EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbSnapshotInvalidesRecus);
        }

        public void EnregistrerSnapshotIgnore()
        {
            EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbSnapshotIgnores);
        }

        public void EnregistrerConnexion()
        {
            EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbConnexion);
        }

        public void EnregistrerDeconnexion()
        {
            EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbDeconnexion);
        }

        public void EnregistrerErreurDonnees()
        {
            EnregistreCompteurDonnees(_compteursDonnees, CompteurDonneesEnum.NbErreurReceptionDonnees);
        }


        public void EnregistrerDelaiEchange(double delai)
        {
            try
            {
                StatistiqueItem item = _compteursDonnees[CompteurDonneesEnum.DelaiEchange];
                item?.EnregistrerValeur((float)delai);
                LogTools.Logger.Debug("delai d'echange: {0} ms", delai);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du delai d'echange");
            }
        }

        #endregion

        #region METHODES PRIVEES

        public void EnregistreCompteurDonnees(Dictionary<CompteurDonneesEnum, StatistiqueItem> cpt, CompteurDonneesEnum idCpt)
        {
            try
            {
                if (cpt != null && cpt.ContainsKey(idCpt))
                {
                    StatistiqueItem item = cpt[idCpt];
                    item?.EnregistrerValeur();
                }
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du compteur de donnees : " + idCpt.ToString());
            }
        }

        #endregion

        */
    }
}
