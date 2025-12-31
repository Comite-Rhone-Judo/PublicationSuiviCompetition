using AppPublication.Config.Publication;
using AppPublication.Export;
using AppPublication.Generation;
using AppPublication.Statistiques;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tools.Outils;


namespace AppPublication.Controles
{
    /// <summary>
    /// Events de notification de changement d'etat du scheduler
    /// </summary>
    public class SchedulerStateEventArgs : EventArgs
    {
        /// <summary>
        /// L'état du scheduler qui est notifie
        /// </summary>
        public StateGenerationEnum State { get; }
        /// <summary>
        /// Les statistiques d'execution de la derniere etape realisee
        /// </summary>
        public TaskExecutionInformation InfosExecution { get; }

        public long DelaiNextSec { get; }

        public SchedulerStateEventArgs(StateGenerationEnum state, TaskExecutionInformation statExec = null, long delaiNextSec = long.MinValue)
        {
            State = state;
            InfosExecution = statExec;
            DelaiNextSec = delaiNextSec;
        }
    }


    /// <summary>
    /// Classe de gestion de la generation periodique du site Web, incluant la synchronisation des contenus
    /// Il emet des events pour signaler les changements d'etat. 
    /// La progression est de la responsabilité du générateur de site (IGenerateurSite) pour les opérations
    /// locaux et distants.
    /// </summary>
    public class GenerationScheduler
    {
        #region MEMBRES
        private CancellationTokenSource _tokenSource;   // Token pour la gestion de la thread de lecture
        private Task _taskGeneration = null;            // La tache de generation
        private GestionStatistiques _statMgr = null;    // Pour le gestion des statistiques
        private IGenerateurSite _generateur;            // le generateur de site

        private long _generationCounter = 0;                        // Nombre de generation realisees depuis le demarrage
        // --- Événement unique pour tout changement d'état (Interne ou Métier) ---
        public event EventHandler<SchedulerStateEventArgs> StateChanged;

        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="statMgr">le gestionnaire de statitiques</param>
        /// <param name="generateur">Le generateur de donnees</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GenerationScheduler(GestionStatistiques statMgr, IGenerateurSite generateur)
        {
            // Impossible d'etre null
            if (statMgr == null) throw new ArgumentNullException();
            if (generateur == null) throw new ArgumentNullException();

            try
            {
                // Initialise les objets de gestion des sites Web. Ils chargent automatiquement leur configuration
                _statMgr = statMgr;
                _generateur = generateur;
            }
            catch (Exception ex)
            {
                // on se contente de logger l'erreur et de relancer l'exception dans la classe de base
                LogTools.Logger.Error(ex, "Erreur lors de l'initialisation du scheduler de generation");
                throw new Exception("Erreur lors de l'initialisation du scheduler de generation", ex);
            }
        }

        #endregion

        #region PROPRIETES

        TaskExecutionInformation _statGeneration;
        /// <summary>
        /// Statistique de derniere generation - lecture seule
        /// </summary>
        public TaskExecutionInformation DerniereGeneration
        {
            get
            {
                return _statGeneration;
            }
            private set
            {
                _statGeneration = value;
            }
        }

        TaskExecutionInformation _statSync;
        /// <summary>
        /// Statistiques de derniere synchronisation - lecture seule
        /// </summary>
        public TaskExecutionInformation DerniereSynchronisation
        {
            get
            {
                return _statSync;
            }
            private set
            {
                _statSync = value;
            }
        }

        bool _siteGenere = false;
        /// <summary>
        /// Indique si le site a ete bien genere (true) - lecture seule
        /// </summary>
        public bool SiteGenere
        {
            get
            {
                return _siteGenere;
            }
            private set
            {
                _siteGenere = value;
            }
        }

        bool _siteSynchronise = false;
        /// <summary>
        /// Indique si le site a bien ete synchronnise - lecture seule
        /// </summary>
        public bool SiteSynchronise
        {
            get
            {
                return _siteSynchronise;
            }
            private set
            {
                _siteSynchronise = value;
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
            }
        }


        int _delaiGenerationSec = 30;
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
                if (_delaiGenerationSec != value)
                {
                    PublicationConfigSection.Instance.DelaiGenerationSec = (_delaiGenerationSec = value);
                }
            }
        }

        bool _effacerAuDemarrage = true;
        /// <summary>
        /// Indique si on doit faire un RAZ du contenu du répertoire au demarrage de la generation
        /// </summary>
        public bool EffacerAuDemarrage
        {
            get
            {
                return _effacerAuDemarrage;
            }
            set
            {
                if (_effacerAuDemarrage != value)
                {
                    PublicationConfigSection.Instance.EffacerAuDemarrage = (_effacerAuDemarrage = value);
                }
            }
        }

        private StateGenerationEnum _status;
        /// <summary>
        /// Le statut de generation du site
        /// </summary>
        public StateGenerationEnum State
        {
            get
            {
                return _status;
            }
            private set
            {
                _status = value;
                IsGenerationActive = !(_status == StateGenerationEnum.Stopped);
            }
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Demarre le thread de generation du site
        /// </summary>
        /// <param name="progressHandler">Gestionnaire de progression</param>
        /// <exception cref="Exception"></exception>
        public void StartGeneration()
        {
            // Passe en etat Idle mais on n'a pas encore d'information sur les temps
            RaiseState(StateGenerationEnum.Idle);

            // Reset le token d'arret
            if (_tokenSource != null)
            {
                _tokenSource = null;
            }
            _tokenSource = new CancellationTokenSource();

            if (_taskGeneration == null || _taskGeneration.IsCompleted)
            {
                try
                {
                    // Nettoie si necessaire le repertoire avant de lancer la tache
                    if (EffacerAuDemarrage)
                    {
                        RaiseState(StateGenerationEnum.Cleaning);
                        _generateur?.CleanupInitial();
                    }

                    // Execute les taches de demarrage du generateur
                    RaiseState(StateGenerationEnum.Starting);
                    _generateur?.Demarrage();

                    // Lance la tache de fond de generation
                    _taskGeneration = Task.Factory.StartNew( () => { GenerationRun(); }, _tokenSource.Token);
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur lors du lancement de la generation");
                    throw new Exception("Erreur lors du lancement de la generation", ex);
                }
            }
            else
            {
                LogTools.Logger.Error("Une tache de generation est deja en cours d'execution");
                throw new Exception("Une tache de generation est deja en cours d'execution");
            }
        }

        /// <summary>
        /// Arrete le thread de generation du site
        /// </summary>
        public void StopGeneration()
        {
            if (_tokenSource != null)
            {
                // Arrete le thread de generation
                _tokenSource.Cancel();
                _taskGeneration.Wait();
            }

            // Etat de la generation
            RaiseState(StateGenerationEnum.Stopped);
        }

        #region METHODES PRIVEES

        /// <summary>
        /// signale le changement de status
        /// </summary>
        /// <param name="state"></param>
        private void RaiseState(StateGenerationEnum state, TaskExecutionInformation statExec = null, long delaiNextSec = -1)
        {
            // On laisse meme si c'est la meme valeur pour forcer la notification
            if (state != StateGenerationEnum.None) { State = state; }
            StateChanged?.Invoke(this, new SchedulerStateEventArgs(state, statExec, delaiNextSec));
        }

        #endregion

        /// <summary>
        /// Execute un Run de generation
        /// </summary>
        private void GenerationRun()
        {
            DateTime wakeUpTime = DateTime.Now;
            int delaiScrutationMs = 1000;

            while (!_tokenSource.Token.IsCancellationRequested)
            {
                if (DateTime.Now >= wakeUpTime)
                {
                    // Pour controler la duree total par rapport au timer
                    Stopwatch watcherTotal = new Stopwatch();
                    watcherTotal.Start();

                    try
                    {
                        RaiseState(StateGenerationEnum.Generating);
                        SiteGenere = false; // Reset du flag de succès pour ce cycle

                        bool generationPrete = _generateur.PrepareGeneration();
                        if (generationPrete)
                        {
                            try
                            {
                                // Juste un compteur pour les traces
                                if (_generationCounter < long.MaxValue) { _generationCounter++; }

                                // Enregistre le demarrage de la generation via StatExecution
                                TaskExecutionInformation statGeneration = new TaskExecutionInformation();

                                // Lance la tache du generateyr en mesurant son temps de travail
                                var genTime = ActionWatcher.Execute<bool>(() => { return _generateur.ExecuteGeneration(); });
                                
                                // Recupere le resultat et les stats
                                statGeneration.DelaiExecutionMs = genTime.DurationMs;
                                SiteGenere = genTime.Result;

                                _statMgr.EnregistrerGeneration( (float) genTime.DurationMs / 1000F);

                                if(SiteGenere)
                                {
                                    try
                                    {
                                        // Met a jour les dernieres info de generation puisque le site a ete traite
                                        DerniereGeneration = statGeneration;
                                        RaiseState(StateGenerationEnum.Generating, statGeneration);

                                        // Signale le debut de la synchronisation
                                        RaiseState(StateGenerationEnum.Syncing);

                                        // Enregistre le demarrage de la generation via StatExecution
                                        TaskExecutionInformation statSync = new TaskExecutionInformation();

                                        // Execute l'etape de synchronisation du generateur
                                        var postTime = ActionWatcher.Execute<UploadStatus>(() => { return _generateur.ExecuteSynchronisation(); });

                                        SiteSynchronise = postTime.Result.IsSuccess;
                                        statSync.DelaiExecutionMs = postTime.DurationMs;

                                        // Met a jour les informations de la tache
                                        _statMgr.EnregistrerSynchronisation((float)postTime.DurationMs / 1000F, postTime.Result);

                                        if (SiteSynchronise)
                                        {
                                            DerniereSynchronisation = statSync;
                                            RaiseState(StateGenerationEnum.Syncing, statSync);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogTools.Logger.Error(ex, "Une erreur est survenue pendant la tentative de synchronisation");
                                        SiteSynchronise = false;
                                    }
                                }
                                else
                                {
                                    // Juste le log debug
                                    LogTools.Logger.Debug("Site non genere");
                                }
                            }
                            catch (Exception ex)
                            {
                                LogTools.Logger.Error(ex, "Une erreur est survenue durant la sequence de generation du site");
                                SiteGenere = false;
                            }
                        }
                        else
                        {
                            // Le controle d'integrite a echoue
                            LogTools.Logger.Warn("Impossible de valider l'integrite des donnees combats (Timeout ou deconnexion).");
                        }
                    }
                    finally
                    {
                        // Arrete le watcher total pour connaitre le temps passe dans le cycle
                        watcherTotal.Stop();
                        // Si le transfert a duree plus que le temps d'attente, on attend au plus 5 sec
                        // Sinon, on attend la difference restante
                        int delaiThread = (int)Math.Max(DelaiGenerationSec * 1000 - watcherTotal.ElapsedMilliseconds, 5000);
                        // prochaine heure de generation
                        DerniereGeneration.DateProchaineGeneration = DateTime.Now.AddMilliseconds(delaiThread);

                        // Dans tous les cas, on repasse Idle
                        RaiseState(StateGenerationEnum.Idle, DerniereGeneration, (int)Math.Round(delaiThread / 1000.0));

                        // Controle final si tout s'est bien passe
                        if (!SiteGenere)
                        {
                            _statMgr.EnregistrerErreurGeneration();
                        }

                        _statMgr.EnregistrerDelaiGeneration(delaiThread / 1000F);

                    }
                }

                // Endort le thread pour le delai de scrutation
                Thread.Sleep(delaiScrutationMs);
            }
        }
        #endregion
    }
}
