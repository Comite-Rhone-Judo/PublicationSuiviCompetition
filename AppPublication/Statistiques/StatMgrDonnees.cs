using System.Collections.Generic;
using Tools.Framework;
using Tools.Logging;

namespace AppPublication.Statistiques
{
    public class StatMgrDonnees : NotificationBase
    {
        public enum CompteurDonneesEnum
        {
            NbDemandeSnapshot = 0,
            NbSnapshotInvalidesRecus = 3,
            NbSnapshotIgnores = 4,
            NbConnexion = 5,
            NbDeconnexion = 6,
            DelaiEchange = 7,
            DelaiIntegrationSnapshotComplet = 8,
            DelaiIntegrationSnapshotDifferentiel = 9,
            NbErreurReceptionDonnees = 10
        }

        #region PROPRIETES

        private Dictionary<CompteurDonneesEnum, StatistiqueItem> _compteursDonnees;
        public Dictionary<CompteurDonneesEnum, StatistiqueItem> CompteursDonnees
        {
            get => _compteursDonnees;
            private set { _compteursDonnees = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region CONSTRUCTEUR

        public StatMgrDonnees()
        {
            try
            {
                Dictionary<CompteurDonneesEnum, StatistiqueItem> cptData = new Dictionary<CompteurDonneesEnum, StatistiqueItem>();

                cptData.Add(CompteurDonneesEnum.NbDemandeSnapshot, new StatistiqueItemCompteur(CompteurDonneesEnum.NbDemandeSnapshot.ToString(), "Resynchro. (Snapshots)"));
                cptData.Add(CompteurDonneesEnum.DelaiIntegrationSnapshotComplet, new StatistiqueItemMoyenneur(CompteurDonneesEnum.DelaiIntegrationSnapshotComplet.ToString(), "Intégration Snapshot complet (Ms)"));
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
                LogTools.Logger.Error(ex, "Erreur lors de l'initialisation des statistiques données");
            }
        }

        #endregion

        #region METHODES D'ENREGISTREMENT (Noms d'origine conservés)

        public void EnregistrerDemandeSnapshot()
        {
            EnregistreCompteurInterne(CompteurDonneesEnum.NbDemandeSnapshot);
        }

        public void EnregistrerSnapshotCompletRecu(double delai)
        {
            try
            {
                StatistiqueItem item = _compteursDonnees[CompteurDonneesEnum.DelaiIntegrationSnapshotComplet];
                item?.EnregistrerValeur((float)delai);
                LogTools.Logger.Debug("delai integration snapshot complet: {0} ms", delai);
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
                StatistiqueItem item = _compteursDonnees[CompteurDonneesEnum.DelaiIntegrationSnapshotDifferentiel];
                item?.EnregistrerValeur((float)delai);
                LogTools.Logger.Debug("delai integration snapshot differentiel: {0} ms", delai);
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du delai d'integration du snapshot differentiel");
            }
        }

        public void EnregistrerSnapshotInvalideRecu()
        {
            EnregistreCompteurInterne(CompteurDonneesEnum.NbSnapshotInvalidesRecus);
        }

        public void EnregistrerSnapshotIgnore()
        {
            EnregistreCompteurInterne(CompteurDonneesEnum.NbSnapshotIgnores);
        }

        public void EnregistrerConnexion()
        {
            EnregistreCompteurInterne(CompteurDonneesEnum.NbConnexion);
        }

        public void EnregistrerDeconnexion()
        {
            EnregistreCompteurInterne(CompteurDonneesEnum.NbDeconnexion);
        }

        public void EnregistrerErreurDonnees()
        {
            EnregistreCompteurInterne(CompteurDonneesEnum.NbErreurReceptionDonnees);
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

        private void EnregistreCompteurInterne(CompteurDonneesEnum idCpt)
        {
            try
            {
                if (_compteursDonnees != null && _compteursDonnees.ContainsKey(idCpt))
                {
                    StatistiqueItem item = _compteursDonnees[idCpt];
                    item?.EnregistrerValeur();
                }
            }
            catch (System.Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de l'enregistrement du compteur de donnees : " + idCpt.ToString());
            }
        }

        #endregion
    }
}