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
        private StatMgrGeneration _generationSite;
        public StatMgrGeneration GenerationSite
        {
            get => _generationSite;
            private set { _generationSite = value; NotifyPropertyChanged(); }
        }

        private StatMgrGeneration _generationSiteInterne;
        public StatMgrGeneration GenerationSiteInterne
        {
            get => _generationSiteInterne;
            private set { _generationSiteInterne = value; NotifyPropertyChanged(); }
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
            GenerationSite = new StatMgrGeneration();
            GenerationSiteInterne = new StatMgrGeneration();
            Synchronisation = new StatMgrSynchronisation();
            Donnees = new StatMgrDonnees();
        }

        #endregion

    }
}
