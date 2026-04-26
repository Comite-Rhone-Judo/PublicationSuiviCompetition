using FranceJudo.Core.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Threading
{
    /// <summary>
    /// Gère l'exécution parallèle de tâches qui produisent des listes de résultats.
    /// </summary>
    /// <typeparam name="TReport">Le type de l'objet de progression vers l'UI.</typeparam>
    /// <typeparam name="TResultItem">Le type des items retournés par les tâches.</typeparam>
    public class ParallelTaskBatcher<TReport, TResultItem>
    {
        #region MEMBERS
        private readonly List<Task> _tasks = new List<Task>();
        private readonly object _lockObject = new object();
        private TaskScheduler _currentScheduler;
        private int _concurrencyLevel;

        private readonly ConcurrentBag<IEnumerable<TResultItem>> _resultsBag
            = new ConcurrentBag<IEnumerable<TResultItem>>();

        private readonly ConcurrentDictionary<Guid, TaskState> _tasksStates
            = new ConcurrentDictionary<Guid, TaskState>();

        private readonly IProgress<TReport> _globalProgressReporter;
        private readonly Func<float, TReport> _converter;
        private readonly long _throttlingIntervalTicks = 1000000; // 100ms en ticks

        private float _maxReportedPercent = 0f;
        private readonly object _reportLock = new object(); // Un petit verrou exclusif pour l'UI

        private class TaskState
        {
            public int Current { get; set; }
            public int Total { get; set; }
        }

        // NOUVEAU : Variables pour le suivi Lock-Free et le Throttling
        private long _globalTotal = 0;
        private long _globalCurrent = 0;
        private long _lastReportTicks = 0;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Définit ou obtient le niveau de concurrence des tâches.
        /// -1 : Pas de limitation (Pool natif .NET)
        ///  0 : Automatique (Nombre de coeurs - 1)
        /// >=1 : Limite stricte
        /// </summary>
        public int ConcurrencyLevel
        {
            get
            {
                lock (_lockObject) return _concurrencyLevel;
            }
            set
            {
                lock (_lockObject)
                {
                    if (value < -1) throw new ArgumentOutOfRangeException(nameof(ConcurrencyLevel), "La valeur doit être >= -1");

                    _concurrencyLevel = value;
                    UpdateSchedulerConfiguration();
                }
            }
        }
        #endregion

        #region CONSTRUCTEURS
        public ParallelTaskBatcher(IProgress<TReport> globalProgressReporter, Func<float, TReport> converter, int concurrencyLevel = 0, long throttlingIntervalTicks = 1000000)
        {
            _globalProgressReporter = globalProgressReporter;
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _throttlingIntervalTicks = throttlingIntervalTicks;

            // On initialise le scheduler via la propriété pour centraliser la logique
            ConcurrencyLevel = concurrencyLevel;
        }
        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// Ajoute une tâche. Capture et logue les exceptions internes.
        /// </summary>
        public void AddWork(Func<IProgress<BatchProgressInfo>, IEnumerable<TResultItem>> work, int initialEstimate = 1)
        {
            if (work == null) return;
            if (initialEstimate < 1) initialEstimate = 1;

            var taskId = Guid.NewGuid();

            LogTools.Logger.Debug($"Ajout d'une tache parallele au Batcher (ID: {taskId}) : {work.Method.Name}"); // Pour le debug

            _tasksStates.TryAdd(taskId, new TaskState { Current = 0, Total = initialEstimate});
            // On ajoute l'estimation initiale au total global
            Interlocked.Add(ref _globalTotal, initialEstimate);
            RecalculateGlobalProgress();

            var taskReporter = new ProgressWrapper(info => HandleTaskReport(taskId, info));

            // On récupère le scheduler actif de manière sécurisée
            TaskScheduler schedulerToUse;
            lock (_lockObject)
            {
                schedulerToUse = _currentScheduler;
            }

            // On utilise Task.Factory.StartNew pour pouvoir spécifier le TaskScheduler limite qui va gérer la concurrence. Important pour éviter de saturer le thread pool et l'UI.
            Task t = Task.Factory.StartNew(() =>
            {
                try
                {
                    // Exécution de la tâche
                    var result = work(taskReporter);

                    if (result != null)
                    {
                        _resultsBag.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    // 1. TRACE : On capture l'erreur immédiate sur le thread secondaire
                    LogTools.Logger.Error(ex, $"Erreur critique dans une tache parallele du Batcher (ID: {taskId})");

                    // 2. RETHROW : Important pour que la Task soit marquée comme "Faulted"
                    // et que l'exception remonte jusqu'au WaitAll.
                    throw;
                }
                finally
                {
                    CompleteTask(taskId);
                }
            },
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            schedulerToUse);

            lock (_lockObject)
            {
                _tasks.Add(t);
            }
        }

        /// <summary>
        /// Attend de maniere Asynchrone la fin de toutes les tâches et retourne les résultats.
        /// Capture les exceptions globales (AggregateException) et logue les erreurs individuelles.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TResultItem>> WaitAllAndGetResultsAsync()
        {
            Task[] tasksToWait;
            lock (_lockObject)
            {
                if (_tasks.Count == 0) return new List<TResultItem>();
                tasksToWait = _tasks.ToArray();
            }

            // On déclare la liste ici
            List<TResultItem> finalResult = new List<TResultItem>();

            try
            {
                await Task.WhenAll(tasksToWait);
            }
            catch (AggregateException ae)
            {
                foreach (var innerEx in ae.Flatten().InnerExceptions)
                {
                    LogTools.Logger.Error(innerEx, "Erreur dans une tâche du Batcher");
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur globale dans l'attente du Batcher");
            }
            finally
            {
                // On peuple la liste finale et on nettoie DANS le finally pour que ce soit garanti
                finalResult = _resultsBag.Where(list => list != null).SelectMany(x => x).ToList();
                Reset();
            }

            // Le return est bien à l'EXTÉRIEUR !
            return finalResult;
        }

        /// <summary>
        /// Attend la fin et capture les exceptions globales (AggregateException).
        /// </summary>
        public List<TResultItem> WaitAllAndGetResults()
        {
            Task[] tasksToWait;
            List<TResultItem> finalResult = new List<TResultItem>();

            // Snapshot thread-safe de la liste des tâches
            lock (_lockObject)
            {
                if (_tasks.Count == 0) return new List<TResultItem>();
                tasksToWait = _tasks.ToArray();
            }

            try
            {
                // On attend que TOUT le monde ait fini (succès ou échec)
                // Task.WaitAll lancera une exception si au moins une tâche a échoué.
                Task.WaitAll(tasksToWait);
            }
            catch (AggregateException ae)
            {
                // 1. On logue toutes les erreurs individuelles
                foreach (var innerEx in ae.Flatten().InnerExceptions)
                {
                    LogTools.Logger.Error(innerEx, "Erreur dans une tâche du Batcher");
                }

                // 2. IMPORTANT : On ne fait pas "return null" ou on ne plante pas tout de suite.
                // On veut peut-être récupérer les résultats des tâches qui ont RÉUSSI (récupération partielle).
                // Si vous préférez que tout échoue, décommentez le throw ci-dessous.

                // throw; // Décommentez pour bloquer tout si une seule erreur survient
            }
            finally
            {
                // 3. Construction de la liste finale
                // Cette étape est très rapide (simple copie de références en mémoire)
                // On utilise ToList() pour figer le résultat.
                finalResult = _resultsBag
                    .Where(list => list != null) // Sécurité contre les nulls
                    .SelectMany(x => x)          // Aplatit les listes de listes
                    .ToList();

                // Nettoyage interne
                Reset();
            }

            return finalResult;
        }

        public bool HasPendingWork
        {
            get { lock (_lockObject) return _tasks.Count > 0; }
        }

        #endregion

        #region METHODES PRIVEES

        /// <summary>
        /// Gestionnaire de callback pour les rapports de progression individuels des tâches.
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="info"></param>
        private void HandleTaskReport(Guid taskId, BatchProgressInfo info)
        {
            if (!_tasksStates.TryGetValue(taskId, out var state)) return;

            // Le lock local est conservé pour la stricte équivalence de comportement
            lock (state)
            {
                if (info.Type == BatchProgressType.Initialization)
                {
                    int diffTotal = info.Value > 0 ? info.Value : 1;
                    int delta = diffTotal - state.Total;
                    state.Total = diffTotal;
                    Interlocked.Add(ref _globalTotal, delta); // Poussée atomique instantanée
                }
                else if (info.Type == BatchProgressType.Progress)
                {
                    int newValue = info.Value;
                    if (newValue > state.Total) newValue = state.Total;
                    int delta = newValue - state.Current;
                    state.Current = newValue;
                    Interlocked.Add(ref _globalCurrent, delta); // Poussée atomique instantanée
                }
            }
            RecalculateGlobalProgress();
        }

        private void CompleteTask(Guid taskId)
        {
            if (_tasksStates.TryGetValue(taskId, out var state))
            {
                lock (state)
                {
                    // On calcule le delta manquant pour fermer la tâche à 100%
                    int delta = state.Total - state.Current;
                    state.Current = state.Total;
                    Interlocked.Add(ref _globalCurrent, delta);
                }
                RecalculateGlobalProgress();
            }
        }

        /// <summary>
        /// Calcul l'avancement global en agrégeant les états individuels de chaque tâche et reporte vers l'UI.
        /// </summary>
        private void RecalculateGlobalProgress()
        {
            if (_globalProgressReporter == null) return;
            var states = _tasksStates.Values.ToList();

            // THROTTLING : Max 1 update toutes les 100ms vers l'UI pour éviter le gel
            long currentTicks = DateTime.UtcNow.Ticks;
            if (currentTicks - Interlocked.Read(ref _lastReportTicks) < _throttlingIntervalTicks) return;
            Interlocked.Exchange(ref _lastReportTicks, currentTicks);

            // LECTURE LOCK-FREE : Plus de boucle, plus de goulot d'étranglement
            long total = Interlocked.Read(ref _globalTotal);
            long current = Interlocked.Read(ref _globalCurrent);

            if (total <= 0) total = 1;
            LogTools.Logger.Debug($"Global progress for # task = '{states.Count}', total = {current}");

            float globalPercent = ((float)current) / total;
            if (globalPercent > 1.0f) globalPercent = 1.0f;

            // LA SÉCURITÉ ANTI-YOYO CENTRALE
            // Le lock ici n'a aucun impact sur les perfs car il est protégé par le throttling (max 10 fois par seconde)
            lock (_reportLock)
            {
                if (globalPercent > _maxReportedPercent)
                {
                    _maxReportedPercent = globalPercent;
                    _globalProgressReporter.Report(_converter(_maxReportedPercent));
                }
            }
        }

        private void Reset()
        {
            lock (_lockObject) _tasks.Clear();
            _tasksStates.Clear();

            while (_resultsBag.TryTake(out _)) { }
            Interlocked.Exchange(ref _globalTotal, 0);
            Interlocked.Exchange(ref _globalCurrent, 0);

            lock (_reportLock)
            {
                _maxReportedPercent = 0f;
            }

            _globalProgressReporter?.Report(_converter(0));
        }

        private class ProgressWrapper : IProgress<BatchProgressInfo>
        {
            private readonly Action<BatchProgressInfo> _handler;
            public ProgressWrapper(Action<BatchProgressInfo> handler) { _handler = handler; }
            public void Report(BatchProgressInfo value) => _handler(value);
        }

        /// <summary>
        /// Met à jour l'instance du TaskScheduler en fonction du niveau demandé.
        /// (Doit être appelé à l'intérieur d'un lock sur _lockObject)
        /// </summary>
        private void UpdateSchedulerConfiguration()
        {
            if (_concurrencyLevel == -1)
            {
                LogTools.Logger.Info("No limit mode: using the default .NET thread pool");
                // Pas de limite : on utilise le pool de threads par défaut de .NET
                _currentScheduler = TaskScheduler.Default;
            }
            else if (_concurrencyLevel == 0)
            {
                // Mode automatique : Processeurs logiques - 1 (pour l'UI)
                int maxConcurrency = Math.Max(1, System.Environment.ProcessorCount - 1);
                LogTools.Logger.Info($"Automatic mode: using a limited concurrency level of {maxConcurrency}");
                _currentScheduler = new LimitedConcurrencyLevel(maxConcurrency);
            }
            else
            {
                // Mode manuel : Valeur stricte
                LogTools.Logger.Info($"Manual mode: using a limited concurrency level of {_concurrencyLevel}");
                _currentScheduler = new LimitedConcurrencyLevel(_concurrencyLevel);
            }
        }

        #endregion
    }
}