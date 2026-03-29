using AppPublication.Generation;
using AppPublication.Models.Statistiques;
using AppPublication.Statistiques;
using System;
using System.Net;
using FranceJudo.UI.Wpf.Foundation;
using FranceJudo.UI.Wpf.ViewModels.Network;
using FranceJudo.Core.Logging;
using FranceJudo.Core.IO;
using FranceJudo.Metier.Noyau;

namespace AppPublication.Models.Publication
{
    public abstract class GestionSiteBase : NotificationBase
    {
        #region MEMBRES PROTEGES
        protected GestionStatistiques _statMgr = null;
        protected IJudoDataManager _judoDataManager;                // Le gestionnaire de données interne
        protected GenerationScheduler _schedulerSite = null;        // Le scheduler de generation Site
        protected IProgress<OperationProgress> _progressHandler = null;
        protected MiniSite _siteLocal = null;                       // Le site de publication local
        #endregion

        #region PROPRIETES COMMUNES

        /// <summary>
        /// Indique si le gestionnaire est dans un état qui permet de changer les propriétés de configuration (true) ou si une generation est en cours et bloque les changements (false)
        /// </summary>
        public virtual bool CanChangeProperties
        {
            get
            {
                return SiteLocal == null || !SiteLocal.IsActif && !IsGenerationActive;
            }
        }

        private string _repertoireRacine;
        /// <summary>
        /// Le répertoire Racine configuré par l'utilisateur
        /// </summary>
        public string RepertoireRacine
        {
            get { return _repertoireRacine; }
            set
            {
                if (value != _repertoireRacine)
                {
                    _repertoireRacine = value;
                    NotifyPropertyChanged();
                    OnRepertoireRacineChanged(value); // Hook pour propager aux structures enfants
                }
            }
        }

        private FilteredFileInfo _selectedLogo = null;
        /// <summary>
        /// Le fichier logo sélectionné
        /// </summary>
        public FilteredFileInfo SelectedLogo
        {
            get { return _selectedLogo; }
            set
            {
                if (_selectedLogo != value)
                {
                    _selectedLogo = value;
                    string logoName = (value != null) ? value.Name : string.Empty;
                    OnSelectedLogoChanged(logoName); // Hook pour propager au générateur
                    NotifyPropertyChanged();
                }
            }
        }

        private TaskExecutionInformation _statGeneration;
        /// <summary>
        /// Statistique de derniere generation - lecture seule
        /// </summary>
        public TaskExecutionInformation DerniereGeneration
        {
            get { return _statGeneration; }
            protected set
            {
                _statGeneration = value;
                NotifyPropertyChanged();
            }
        }

        private bool _siteGenere = false;
        /// <summary>
        /// Indique si le site a ete bien genere (true) - lecture seule
        /// </summary>
        public bool SiteGenere
        {
            get { return _siteGenere; }
            protected set
            {
                _siteGenere = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Le site de publication local
        /// </summary>
        public MiniSite SiteLocal
        {
            get { return _siteLocal; }
        }

        /// <summary>
        /// Propriete passerelle pour selectionner l'interface de publication du site local
        /// Permet de tenir a jour le QR code de l'URL de publication
        /// </summary>
        public IPAddress InterfaceLocalPublication
        {
            get { return SiteLocal?.InterfaceLocalPublication; }
            set
            {
                try
                {
                    if (SiteLocal != null)
                    {
                        // Verifie que la valeur selectionnee est bien dans la liste des interfaces
                        SiteLocal.InterfaceLocalPublication = value;
                        NotifyPropertyChanged();
                        OnInterfaceLocalPublicationChanged(); // Hook pour recalculer les URLs
                    }
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        private bool _generationActive = false;
        /// <summary>
        /// Etat de la generation du site
        /// </summary>
        public bool IsGenerationActive
        {
            get { return _generationActive; }
            protected set
            {
                _generationActive = value;
                NotifyPropertyChanged();
            }
        }

        private int _delaiGenerationSec = 30;
        /// <summary>
        /// Delai entre 2 generations du site
        /// </summary>
        public int DelaiGenerationSec
        {
            get { return _delaiGenerationSec; }
            set
            {
                if (_delaiGenerationSec != value)
                {
                    _delaiGenerationSec = value;
                    // Configure le scheduler
                    _schedulerSite?.DelaiGenerationSec = value;
                    UpdateDelaiGenerationConfig(value); // Hook pour sauvegarder la bonne config
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _effacerAuDemarrage = true;
        /// <summary>
        /// Indique si on doit faire un RAZ du contenu du répertoire au demarrage de la generation
        /// </summary>
        public bool EffacerAuDemarrage
        {
            get { return _effacerAuDemarrage; }
            set
            {
                if (_effacerAuDemarrage != value)
                {
                    _effacerAuDemarrage = value;
                    // Configure le scheduler
                    _schedulerSite?.EffacerAuDemarrage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _idCompetition = string.Empty;
        /// <summary>
        /// ID de la competition en cours
        /// </summary>
        public string IdCompetition
        {
            get { return _idCompetition; }
            set
            {
                _idCompetition = value;
                NotifyPropertyChanged();
                OnIdCompetitionChanged(value); // Hook pour recalculer les structures et URL
            }
        }

        private StatusGenerationSite _status;
        /// <summary>
        /// Le statut de generation du site
        /// </summary>
        public StatusGenerationSite Status
        {
            get
            {
                if (null == _status)
                {
                    _status = new StatusGenerationSite();
                }
                return _status;
            }
            set
            {
                _status = value;
                NotifyPropertyChanged();
                IsGenerationActive = !(_status.State == StateGenerationEnum.Stopped);
            }
        }
        #endregion

        #region CONSTRUCTEUR
        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="dataManager">Le gestionnaire de données</param>
        /// <param name="statMgr">le gestionnaire de statistiques</param>
        protected GestionSiteBase(IJudoDataManager dataManager, GestionStatistiques statMgr)
        {
            _judoDataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            _statMgr = statMgr ?? new GestionStatistiques();

            // Initialise le progress handler pour la generation de site
            _progressHandler = new Progress<OperationProgress>(OnGenerationSiteProgressReport);
        }
        #endregion

        #region METHODES ABSTRAITES (A implémenter par les enfants)
        /// <summary>Initialise les donnees a partir du cache de fichier AppConfig</summary>
        public abstract void InitFromConfigFile();

        /// <summary>Hook exécuté lorsque le répertoire racine change</summary>
        protected abstract void OnRepertoireRacineChanged(string newValue);

        /// <summary>Hook exécuté lorsque le logo sélectionné change</summary>
        protected abstract void OnSelectedLogoChanged(string logoName);

        /// <summary>Hook exécuté lorsque l'interface locale change</summary>
        protected abstract void OnInterfaceLocalPublicationChanged();

        /// <summary>Hook pour mettre à jour l'élément de configuration du délai</summary>
        protected abstract void UpdateDelaiGenerationConfig(int newValue);

        /// <summary>Hook exécuté lorsque l'ID de compétition change</summary>
        protected abstract void OnIdCompetitionChanged(string newValue);
        #endregion

        /// <summary>Force le recalcul et le rafraîchissement des URLs de publication pour l'interface</summary>
        public abstract void ForceRefreshUrls();
        #region METHODES COMMUNES

        /// <summary>
        /// Demarre le thread de generation du site
        /// </summary>
        public void StartGeneration()
        {
            _schedulerSite?.StartGeneration();
        }

        /// <summary>
        /// Arrete le thread de generation du site
        /// </summary>
        public void StopGeneration()
        {
            _schedulerSite?.StopGeneration();
        }

        /// <summary>
        /// Methode interne pour aiguiller le progress vers la bonne propriete de status
        /// </summary>
        protected virtual void OnGenerationSiteProgressReport(OperationProgress valueReported)
        {
            LogTools.Logger.Debug($"Progress {valueReported} signale par le generateur");

            // on doit juste s'assurer que tout est bien execute dans le UI Thread
            System.Windows.Application.Current.ExecOnUiThread(() =>
            {
                if (valueReported != null && valueReported.Etape == EtapeGenerateurSiteEnum.ExecuteGeneration)
                {
                    // Clone le status courant
                    StatusGenerationSite cpy = Status.Clone();

                    // Met a jour le status avec la nouvelle progression et notifie les changements
                    cpy.Progress = (int)Math.Round(valueReported.ProgressPercent * 100);
                    Status = cpy;
                }
            });
        }

        /// <summary>
        /// Gestionnaire d'evenement pour les changements d'etat du scheduler
        /// </summary>
        protected virtual void OnSchedulerSiteStateChanged(object sender, SchedulerStateEventArgs evt)
        {
            LogTools.Logger.Debug($"Event {evt.State} signale par le scheduler");

            // on doit juste s'assurer que tout est bien execute dans le UI Thread
            System.Windows.Application.Current.ExecOnUiThread(() =>
            {
                // Clone le status courant
                StatusGenerationSite cpy = Status.Clone();

                // Met a jour l'etat avec celui reçu s'il est documente, notifie les changements en assignant la propriete
                if (evt.State != StateGenerationEnum.None) { cpy.State = evt.State; }
                Status = cpy;

                // Verifie si on a des infos d'exécution signalées
                if (evt.InfosExecution != null)
                {
                    if (evt.State != StateGenerationEnum.Syncing)
                    {
                        SiteGenere = evt.InfosExecution.IsSuccess;
                        DerniereGeneration = evt.InfosExecution;
                    }
                }

                // Met a jour le delai avant la prochaine generation s'il est documente
                if (evt.DelaiNextSec != long.MinValue)
                {
                    // On n'a pas de delai, on met a zero
                    cpy.NextGenerationSec = (int)evt.DelaiNextSec;
                }
            });
        }
        #endregion
    }
}