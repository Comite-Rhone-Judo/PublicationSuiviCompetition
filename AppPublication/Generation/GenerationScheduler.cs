
using AppPublication.Statistiques;
using FranceJudo.Core.Diagnostic;
using FranceJudo.Core.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static FranceJudo.Core.Diagnostic.ActionWatcher;

namespace AppPublication.Generation
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
        readonly private StatMgrGeneration _statMgrGeneration = null;    // Pour le gestion des statistiques
        readonly private StatMgrSynchronisation _statMgrSynchronisation = null;    // Pour le gestion des statistiques
        readonly private IGenerateurSite _generateur;            // le generateur de site

        private long _generationCounter = 0;                        // Nombre de generation realisees depuis le demarrage

        private bool _isClientConnected = true;
        private bool _derniereGenerationDeSecuriteEffectuee = false;
        // --- Événement unique pour tout _statMgr d'état (Interne ou Métier) ---
        public event EventHandler<SchedulerStateEventArgs> StateChanged;

        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="statMgr">le gestionnaire de statitiques</param>
        /// <param name="generateur">Le generateur de donnees</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GenerationScheduler(StatMgrGeneration statMgrGen, StatMgrSynchronisation statMgrSync, IGenerateurSite generateur)
        {
            // Impossible d'etre null
            ArgumentNullException.ThrowIfNull(generateur);

            try
            {
                // Initialise les objets de gestion des sites Web. Ils chargent automatiquement leur configuration
                _statMgrGeneration = statMgrGen;
                _statMgrSynchronisation = statMgrSync;
                _generateur = generateur;
            }
            catch (Exception ex)
            {
                // on se contente de logger l'erreur et de relancer l'exception dans la classe de base
                LogTools.Logger?.Error(ex, "Erreur lors de l'initialisation du scheduler de generation");
                throw new Exception("Erreur lors de l'initialisation du scheduler de generation", ex);
            }
        }

        #endregion

        #region PROPRIETES

        /// <summary>
        /// Indique si le client est actuellement connecté au réseau
        /// </summary>
        public bool IsClientConnected
        {
            get { return _isClientConnected; }
            set
            {
                if (_isClientConnected != value)
                {
                    _isClientConnected = value;
                    if (_isClientConnected)
                    {
                        // Reconnexion : On RAZ le drapeau pour la prochaine boucle
                        _derniereGenerationDeSecuriteEffectuee = false;
                        LogTools.Logger?.Info("Connexion rétablie. Reprise de la génération planifiée.");
                    }
                    else
                    {
                        LogTools.Logger?.Warn("Perte de connexion. Une dernière génération de sécurité sera effectuée.");
                    }
                }
            }
        }

        TaskExecutionInformation _statGeneration;
        /// <summary>
        /// Statistique de derniere generation - lecture seule
        /// </summary>
        public TaskExecutionInformation DerniereGeneration
        {
            get
            {
                _statGeneration ??= new TaskExecutionInformation();
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
                _statSync ??= new TaskExecutionInformation();
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
                    _delaiGenerationSec = value;
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
                    _effacerAuDemarrage = value;
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
        public  async Task StartGeneration()
        {
            // Passe en etat Idle mais on n'a pas encore d'information sur les temps
            await RaiseStateAsync(StateGenerationEnum.Idle);

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
                    // Lance la tache de fond de generation
                    _taskGeneration = Task.Factory.StartNew(async () =>
                    {
                        // Nettoie si necessaire le repertoire avant de lancer la tache
                        if (EffacerAuDemarrage)
                        {
                            await RaiseStateAsync(StateGenerationEnum.Cleaning);
                            _generateur?.CleanupInitial();
                        }

                        // Execute les taches de demarrage du generateur
                        await RaiseStateAsync(StateGenerationEnum.Starting);
                        _generateur?.Demarrage();

                        await GenerationRun();
                    },
                        _tokenSource.Token,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default).Unwrap();
                }
                catch (Exception ex)
                {
                    LogTools.Logger?.Error(ex, "Erreur lors du lancement de la generation");
                    throw new Exception("Erreur lors du lancement de la generation", ex);
                }
            }
            else
            {
                LogTools.Logger?.Error("Une tache de generation est deja en cours d'execution");
                throw new Exception("Une tache de generation est deja en cours d'execution");
            }
        }

        /// <summary>
        /// Arrete le thread de generation du site
        /// </summary>
        public async Task StopGeneration()
        {
            if (_tokenSource != null)
            {
                // Demande l'arrêt (Ceci déclenche l'Exception dans le Task.Delay)
                _tokenSource.Cancel();

                try
                {
                    if (_taskGeneration != null)
                    {
                        // On utilise 'await' au lieu de '.Wait()'
                        await _taskGeneration;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    // Comportement normal et attendu : la tâche a bien obéi à l'annulation.
                    LogTools.Logger?.Debug(ex, "Arrêt de la génération");
                }
                catch (Exception ex)
                {
                    // Si une VRAIE erreur se produit au moment de l'arrêt
                    LogTools.Logger?.Error(ex, "Erreur inattendue lors de l'arrêt de la génération");
                }
            }

            // Etat de la generation (utilise votre nouvelle méthode fluide)
            await RaiseStateAsync(StateGenerationEnum.Stopped);
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

        /// <summary>
        /// Signale le changement de statut et laisse le temps à l'UI de s'afficher
        /// </summary>
        private async Task RaiseStateAsync(StateGenerationEnum state, TaskExecutionInformation statExec = null, long delaiNextSec = -1)
        {
            RaiseState(state, statExec, delaiNextSec);

            // On libère le thread de fond pendant 50ms à chaque changement d'état.
            // L'UI a ainsi la garantie absolue de pouvoir se redessiner avant la suite des calculs.
            await Task.Delay(50);
        }

        #endregion

        /// <summary>
        /// Execute un Run de generation
        /// </summary>
        private async Task GenerationRun()
        {
            DateTime wakeUpTime = DateTime.Now;
            int delaiScrutationMs = 1000;

            while (!_tokenSource.Token.IsCancellationRequested)
            {
                if (DateTime.Now >= wakeUpTime)
                {
                    // --- LOGIQUE DE PAUSE ACTIVE ---
                    if (!IsClientConnected)
                    {
                        if (_derniereGenerationDeSecuriteEffectuee)
                        {
                            // On utilise le délai normal de génération pour le prochain essai de reconnexion
                            wakeUpTime = DateTime.Now.AddSeconds(DelaiGenerationSec);

                            // ON MET À JOUR UNIQUEMENT LA PROCHAINE DATE
                            // La date de dernière génération (DateDemarrage) reste intacte !
                            DerniereGeneration.DateProchaineGeneration = wakeUpTime;

                            // On notifie l'IHM (le Converter fera le reste automatiquement)
                            await RaiseStateAsync(StateGenerationEnum.Suspended, DerniereGeneration, -1);

                            continue;
                        }
                        else
                        {
                            LogTools.Logger?.Debug("Exécution de la génération de sécurité post-déconnexion.");
                            _derniereGenerationDeSecuriteEffectuee = true;
                        }
                    }
                    // ----------------------------------------

                    // Pour controler la duree total par rapport au timer
                    Stopwatch watcherTotal = new Stopwatch();
                    watcherTotal.Start();

                    try
                    {
                        await RaiseStateAsync(StateGenerationEnum.Generating);
                        SiteGenere = false; // Reset du flag de succès pour ce cycle

                        ResultatOperation generationPrete = _generateur.PrepareGeneration();
                        if (generationPrete.IsSuccess)
                        {
                            try
                            {
                                // Juste un compteur pour les traces
                                if (_generationCounter < long.MaxValue) { _generationCounter++; }

                                // Enregistre le demarrage de la generation via StatExecution
                                TaskExecutionInformation statGeneration = new TaskExecutionInformation();

                                // Lance la tache du generateur en mesurant son temps de travail
                                TimedResult<ResultatOperation> genTime = await ActionWatcher.ExecuteAsync(async () =>
                                {
                                    return await _generateur.ExecuteGeneration();
                                });

                                // Recupere le resultat et les stats
                                statGeneration.DelaiExecutionMs = genTime.DurationMs;
                                statGeneration.IsSuccess = genTime.Result.IsSuccess;
                                SiteGenere = genTime.Result.IsSuccess;

                                _statMgrGeneration?.EnregistrerGeneration((float)genTime.DurationMs / 1000F);

                                if (SiteGenere)
                                {
                                    try
                                    {
                                        // Met a jour les dernieres info de generation puisque le site a ete traite
                                        DerniereGeneration = statGeneration;
                                        await RaiseStateAsync(StateGenerationEnum.Generating, statGeneration);

                                        // Signale le debut de la synchronisation
                                        await RaiseStateAsync(StateGenerationEnum.Syncing);

                                        // Enregistre le demarrage de la generation via StatExecution
                                        TaskExecutionInformation statSync = new TaskExecutionInformation();

                                        // Execute l'etape de synchronisation du generateur
                                        TimedResult<ResultatOperation> postTime = await ActionWatcher.ExecuteAsync(async () =>
                                        {
                                            return await _generateur.ExecuteSynchronisation();
                                        });

                                        // Verifie si la synchronisation est active et a reussi
                                        if (postTime.Result.IsActive)
                                        {

                                            SiteSynchronise = postTime.Result.IsSuccess;
                                            statSync.DelaiExecutionMs = postTime.DurationMs;
                                            statSync.IsSuccess = postTime.Result.IsSuccess;

                                            // Met a jour les informations de la tache
                                            _statMgrSynchronisation?.EnregistrerSynchronisation((float)postTime.DurationMs / 1000F, postTime.Result);

                                            if (SiteSynchronise)
                                            {
                                                DerniereSynchronisation = statSync;
                                                await RaiseStateAsync(StateGenerationEnum.Syncing, statSync);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogTools.Logger?.Error(ex, "Une erreur est survenue pendant la tentative de synchronisation");
                                        SiteSynchronise = false;
                                    }
                                }
                                else
                                {
                                    // Juste le log debug
                                    LogTools.Logger?.Debug("Site non genere");
                                }
                            }
                            catch (Exception ex)
                            {
                                LogTools.Logger?.Error(ex, "Une erreur est survenue durant la sequence de generation du site");
                                SiteGenere = false;
                            }
                        }
                        else
                        {
                            // Le controle d'integrite a echoue
                            LogTools.Logger?.Warn("Impossible de valider l'integrite des donnees combats (Timeout ou deconnexion).");
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
                        DerniereGeneration.DateProchaineGeneration = (wakeUpTime = DateTime.Now.AddMilliseconds(delaiThread));

                        // Dans tous les cas, on repasse Idle
                        await RaiseStateAsync(StateGenerationEnum.Idle, DerniereGeneration, (int)Math.Round(delaiThread / 1000.0));

                        // Controle final si tout s'est bien passe
                        if (!SiteGenere)
                        {
                            _statMgrGeneration?.EnregistrerErreurGeneration();
                        }

                        _statMgrGeneration?.EnregistrerDelaiGeneration(delaiThread / 1000F);

                    }
                }

                try
                {
                    // Attente coopérative
                    await Task.Delay(delaiScrutationMs, _tokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // La tâche a été annulée (StopGeneration a été appelé)
                    // On sort proprement de la boucle while infinie
                    break;
                }
            }
        }
        #endregion      
    }
}
