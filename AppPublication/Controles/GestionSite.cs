using AppPublication.Export;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools.Enum;
using Tools.Outils;
using Tools.Export;
using System.IO;

namespace AppPublication.Controles
{
    /// <summary>
    /// Classe de gestion du Site auto-généré tout au long de la compétition. Il assure la generation du site et contient les objets de publication
    /// locaux et distants.
    /// </summary>
    public class GestionSite : NotificationBase
    {
        #region MEMBRES

        private CancellationTokenSource _tokenSource;   // Taken pour la gestion de la thread de lecture
        #endregion



        public class GenereSiteStruct
        {
            public SiteEnum type { get; set; }
            public Phase phase { get; set; }
            public int? tapis { get; set; }
        }

        #region Properties

        DateTime _dateGeneration = DateTime.Now; 
        public DateTime DerniereGeneration
        {
            get
            {
                return _dateGeneration;
            }
            private set
            {
                _dateGeneration = value;
                NotifyPropertyChanged("DerniereGeneration");
            }
        }

        DateTime _dateSyncDistant = DateTime.Now;
        public DateTime DerniereSynchronisation
        {
            get
            {
                return _dateSyncDistant;
            }
            private set
            {
                _dateSyncDistant = value;
                NotifyPropertyChanged("DerniereSynchronisation");
            }
        }

        bool _siteGenere = false;
        public bool SiteGenere
        {
            get
            {
                return _siteGenere;
            }
            private set
            {
                _siteGenere = value;
                NotifyPropertyChanged("SiteGenere");
            }
        }


        bool _siteSynchronise = false;
        public bool SiteSynchronise
        {
            get
            {
                return _siteSynchronise;
            }
            private set
            {
                _siteSynchronise = value;
                NotifyPropertyChanged("SiteSynchronise");
            }
        }

        private MiniSite _siteLocal = null;
        /// <summary>
        /// Le site de publication local
        /// </summary>
        public MiniSite MiniSiteLocal {
            get {
                return _siteLocal;
            }
        }

        private MiniSite _siteDistant = null;
        /// <summary>
        /// Le site de publication distant
        /// </summary>
        public MiniSite MiniSiteDistant {
            get {
                return _siteDistant;
            }
        }

        bool _generationActive = false;
        /// <summary>
        /// Etat de la generation du site
        /// </summary>
        public bool IsGenerationActive
        {
            get
            {
                return _generationActive;
            }
            private set
            {
                _generationActive = value;
                NotifyPropertyChanged("IsGenerationActive");
            }
        }

        int _delaiGenerationSec = 60;
        /// <summary>
        /// Delai entre 2 generations du site
        /// </summary>
        public int DelaiGenerationSec
        {
            get
            {
                return _delaiGenerationSec;
            }
            set
            {
                _delaiGenerationSec = value;
                NotifyPropertyChanged("DelaiGenerationSec");
            }
        }

        #endregion

        #region Constructeurs

        public GestionSite()
        {
            try
            {
                // Initialise les objets de gestion des sites Web
                _siteLocal = new MiniSite(true);
                _siteDistant = new MiniSite(false);
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
        }

        #endregion

        #region METHODES

        public void StartGeneration()
        {
            IsGenerationActive = true;

            // Reset le token d'arret
            if (_tokenSource != null)
            {
                _tokenSource = null;
            }
            _tokenSource = new CancellationTokenSource();

            try
            {
                Task.Factory.StartNew(() =>
                {
                    while (!_tokenSource.Token.IsCancellationRequested)
                    {
                        // Pousse les commandes de generation dans le thread de travail
                        SiteGenere = GenereAll();
                        if (SiteGenere)
                        {
                            DerniereGeneration = DateTime.Now;

                            // Si le site distant est actif, transfere la mise a jour
                            if(MiniSiteDistant.IsActif)
                            {
                                string localRoot = Path.Combine(ConstantFile.ExportSite_dir, DialogControleur.Instance.ServerData.competition.remoteId);
                                string distantRoot = DialogControleur.Instance.ServerData.competition.remoteId;
                                SiteSynchronise = MiniSiteDistant.UploadSite(localRoot, distantRoot);        
                                if(SiteSynchronise)
                                {
                                    DerniereSynchronisation = DateTime.Now;
                                }
                            }
                        }

                        // Met le thread en attente pour la prochaine generation
                        Thread.Sleep(DelaiGenerationSec * 1000);
                    }
                }, _tokenSource.Token);
            }
            catch (Exception ex)
            {
                // On RAZ l'etat du lecteur
                throw new Exception("Erreur lors du lancement de la generation du site", ex);
            }
        }

        public void StopGeneration()
        {
            // Arrete le thread de generation
            _tokenSource.Cancel();
            
            // Etat de la generation
            IsGenerationActive = false;
        }

        private void Exporter(GenereSiteStruct genere)
        {
            try
            {
                JudoData DC = DialogControleur.Instance.ServerData;
                List<string> urls = new List<string>();
                switch (genere.type)
                {
                    case SiteEnum.All:
                        urls = ExportSite.GenereWebSite(DC);
                        break;
                    case SiteEnum.AllTapis:
                        urls = ExportSite.GenereWebSiteAllTapis(DC);
                        break;
                    case SiteEnum.Classement:
                        urls = ExportSite.GenereWebSiteClassement(DC, genere.phase.GetVueEpreuve(DC));
                        break;
                    case SiteEnum.Index:
                        urls = ExportSite.GenereWebSiteIndex();
                        break;
                    case SiteEnum.Menu:
                        urls = ExportSite.GenereWebSiteMenu(DC);
                        break;
                    case SiteEnum.Phase:
                        urls = ExportSite.GenereWebSitePhase(DC, genere.phase);
                        break;
                    case SiteEnum.Tapis:
                        urls = ExportSite.GenereWebSiteTapis(DC, (int)genere.tapis);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogTools.Trace(ex);
            }
        }

        public Task AddWork(SiteEnum type, Phase phase, int? tapis)
        {
            Task output = null;

            if (IsGenerationActive)
            {
                GenereSiteStruct export = new GenereSiteStruct
                {
                    type = type,
                    phase = phase,
                    tapis = tapis
                };

                output = OutilsTools.Factory.StartNew(() => Exporter(export));
            }

            return output;
        }

        public bool GenereAll()
        {
            // TODO Virer le AddWork et lancer directement les taches a partir d'ici
            bool output = false;
            if (IsGenerationActive)
            {
                JudoData DC = DialogControleur.Instance.ServerData;
                if (DC.Organisation.Competitions.Count > 0)
                {
                    List<Task> listTaskGeneration = new List<Task>();

                    listTaskGeneration.Add(AddWork(SiteEnum.Index, null, null));
                    listTaskGeneration.Add(AddWork(SiteEnum.Menu, null, null));
                    listTaskGeneration.Add(AddWork(SiteEnum.AllTapis, null, null));
                    for (int i = 1; i <= DC.competition.nbTapis; i++)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.Tapis, null, i));
                    }

                    foreach (Phase phase in DC.Deroulement.Phases)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.Phase, phase, null));
                        listTaskGeneration.Add(AddWork(SiteEnum.Classement, phase, null));
                    }

                    try
                    {
                        // Elimine les elements null
                        listTaskGeneration.RemoveAll(item => item == null);
                        if (listTaskGeneration.Count > 0)
                        {
                            // Attend la fin de la generation pour rendre la main
                            output = Task.WaitAll(listTaskGeneration.ToArray(), DelaiGenerationSec * 1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Trace(ex);
                    }
                }
            }

            return output;
        }
        #endregion
    }
}
