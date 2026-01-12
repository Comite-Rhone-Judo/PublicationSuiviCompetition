using AppPublication.Generation;
using System.Collections.Generic;
using Tools.Framework;
using Tools.Logging;
using Tools.Net; // Nécessaire pour ResultatOperation

namespace AppPublication.Statistiques
{
    public class StatMgrSynchronisation : NotificationBase
    {
        public enum CompteurSynchronisationEnum
        {
            TempsSynchronisation = 0,
            // NbSynchronisation = 1,
            NbErreurSynchronisation = 2,
            NbFichierSynchronisation = 3
        }

        #region PROPRIETES

        // On garde les noms de propriétés exacts pour faciliter le Binding : Synchronisation.CompteursSynchronisationComplete
        private Dictionary<CompteurSynchronisationEnum, StatistiqueItem> _compteursSynchronisationComplete;
        public Dictionary<CompteurSynchronisationEnum, StatistiqueItem> CompteursSynchronisationComplete
        {
            get => _compteursSynchronisationComplete;
            private set { _compteursSynchronisationComplete = value; NotifyPropertyChanged(); }
        }

        private Dictionary<CompteurSynchronisationEnum, StatistiqueItem> _compteursSynchronisationDifference;
        public Dictionary<CompteurSynchronisationEnum, StatistiqueItem> CompteursSynchronisationDifference
        {
            get => _compteursSynchronisationDifference;
            private set { _compteursSynchronisationDifference = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region CONSTRUCTEUR

        public StatMgrSynchronisation()
        {
            try
            {
                // Init Dictionnaire Complet
                var cptSyncC = new Dictionary<CompteurSynchronisationEnum, StatistiqueItem>();
                cptSyncC.Add(CompteurSynchronisationEnum.TempsSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.TempsSynchronisation.ToString(), "Durée de Synchronisation (Sec.)"));
                cptSyncC.Add(CompteurSynchronisationEnum.NbFichierSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.NbFichierSynchronisation.ToString(), "Nb de fichiers synchronisés"));
                cptSyncC.Add(CompteurSynchronisationEnum.NbErreurSynchronisation, new StatistiqueItemCompteur(CompteurSynchronisationEnum.NbErreurSynchronisation.ToString(), "Nb d'erreurs de synchronisation"));
                CompteursSynchronisationComplete = cptSyncC;

                // Init Dictionnaire Différentiel
                var cptSyncD = new Dictionary<CompteurSynchronisationEnum, StatistiqueItem>();
                cptSyncD.Add(CompteurSynchronisationEnum.TempsSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.TempsSynchronisation.ToString(), "Durée de Synchronisation (Sec.)"));
                cptSyncD.Add(CompteurSynchronisationEnum.NbFichierSynchronisation, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.NbFichierSynchronisation.ToString(), "Nb de fichiers synchronisés"));
                cptSyncD.Add(CompteurSynchronisationEnum.NbErreurSynchronisation, new StatistiqueItemCompteur(CompteurSynchronisationEnum.NbErreurSynchronisation.ToString(), "Nb d'erreurs de synchronisation"));
                CompteursSynchronisationDifference = cptSyncD;
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'initialisation des statistiques synchronisation");
            }
        }

        #endregion

        #region METHODES

        // Signature d'origine conservée
        public void EnregistrerSynchronisation(float duree, ResultatOperation syncStatus)
        {
            try
            {
                // Selectionne le dictionnaire en fonction du type de synchronisation (Logique d'origine)
                Dictionary<CompteurSynchronisationEnum, StatistiqueItem> statDict = (syncStatus.IsComplete) ? _compteursSynchronisationComplete : _compteursSynchronisationDifference;

                // Enregistre les donnees dans les compteurs respectifs
                if (statDict.ContainsKey(CompteurSynchronisationEnum.TempsSynchronisation))
                {
                    StatistiqueItem item = statDict[CompteurSynchronisationEnum.TempsSynchronisation];
                    item?.EnregistrerValeur(duree);
                }

                if (!syncStatus.IsSuccess)
                {
                    if (statDict.ContainsKey(CompteurSynchronisationEnum.NbErreurSynchronisation))
                    {
                        StatistiqueItem item = statDict[CompteurSynchronisationEnum.NbErreurSynchronisation];
                        item?.EnregistrerValeur();
                    }
                }

                if (syncStatus.NbElements > 0)
                {
                    if (statDict.ContainsKey(CompteurSynchronisationEnum.NbFichierSynchronisation))
                    {
                        StatistiqueItem item = statDict[CompteurSynchronisationEnum.NbFichierSynchronisation];
                        item?.EnregistrerValeur(syncStatus.NbElements);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement d'une synchronisation");
            }
        }

        #endregion
    }
}