using AppPublication.Generation;
using FranceJudo.Core.Foundation;
using FranceJudo.Core.Logging;
using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;

namespace AppPublication.Statistiques
{
    public class StatMgrSynchronisation : NotificationBase
    {
        public enum CompteurSynchronisationEnum
        {
            TempsSynchronisation = 0,
            // NbSynchronisation = 1,
            NbErreurSynchronisation = 2,
            NbFichierSynchronisation = 3,
            TauxTransfertDifferentiel = 4 // Mesure le % de fichiers transférés
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
                // Compteur de % avec le moyenneur
                cptSyncD.Add(CompteurSynchronisationEnum.TauxTransfertDifferentiel, new StatistiqueItemMoyenneur(CompteurSynchronisationEnum.TauxTransfertDifferentiel.ToString(), "Volume transféré (%)"));
                CompteursSynchronisationDifference = cptSyncD;
            }
            catch (System.Exception ex)
            {
                LogTools.Logger?.Error(ex, "Erreur lors de l'initialisation des statistiques synchronisation");
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

                // 1. Enregistre le temps
                if (statDict.TryGetValue(CompteurSynchronisationEnum.TempsSynchronisation, out StatistiqueItem itemTemps))
                {
                    itemTemps?.EnregistrerValeur(duree);
                }

                // 2. Enregistre les erreurs
                if (!syncStatus.IsSuccess)
                {
                    if (statDict.TryGetValue(CompteurSynchronisationEnum.NbErreurSynchronisation, out StatistiqueItem itemErreur))
                    {
                        itemErreur?.EnregistrerValeur();
                    }
                }

                // 3. Enregistre le nombre de fichiers
                if (syncStatus.NbElements > 0)
                {
                    if (statDict.TryGetValue(CompteurSynchronisationEnum.NbFichierSynchronisation, out StatistiqueItem itemFichier))
                    {
                        itemFichier?.EnregistrerValeur(syncStatus.NbElements);
                    }
                }

                // 4. Calcul et enregistrement de l'efficacité (uniquement en différentiel)
                if (!syncStatus.IsComplete && syncStatus.NbElementsTotal > 0)
                {
                    if (statDict.TryGetValue(CompteurSynchronisationEnum.TauxTransfertDifferentiel, out StatistiqueItem itemTaux))
                    {
                        // On utilise syncStatus.NbElementsTotal directement
                        float pourcentage = (syncStatus.NbElements / (float)syncStatus.NbElementsTotal) * 100f;
                        itemTaux?.EnregistrerValeur(pourcentage);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogTools.Logger?.Error(ex, "Erreur lors de l'enregistrement d'une synchronisation");
            }
        }

        #endregion
    }
}