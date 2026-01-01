using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Logging;

namespace Tools.Threading
{
    /// <summary>
    /// Gère l'exécution parallèle de tâches qui produisent des listes de résultats.
    /// </summary>
    /// <typeparam name="TReport">Le type de l'objet de progression vers l'UI.</typeparam>
    /// <typeparam name="TResultItem">Le type des items retournés par les tâches.</typeparam>
    public class ParallelTaskBatcher<TReport, TResultItem>
    {
        private readonly List<Task> _tasks = new List<Task>();
        private readonly object _lockObject = new object();

        private readonly ConcurrentBag<IEnumerable<TResultItem>> _resultsBag
            = new ConcurrentBag<IEnumerable<TResultItem>>();

        private readonly ConcurrentDictionary<Guid, TaskState> _tasksStates
            = new ConcurrentDictionary<Guid, TaskState>();

        private readonly IProgress<TReport> _globalProgressReporter;
        private readonly Func<float, TReport> _converter;

        private class TaskState
        {
            public int Current { get; set; }
            public int Total { get; set; }
        }

        public ParallelTaskBatcher(IProgress<TReport> globalProgressReporter, Func<float, TReport> converter)
        {
            _globalProgressReporter = globalProgressReporter;
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        /// <summary>
        /// Ajoute une tâche. Capture et logue les exceptions internes.
        /// </summary>
        public void AddWork(Func<IProgress<BatchProgressInfo>, IEnumerable<TResultItem>> work, int initialEstimate = 1)
        {
            if (work == null) return;
            if (initialEstimate < 1) initialEstimate = 1;

            var taskId = Guid.NewGuid();

            LogTools.Logger.Debug($"Ajout d'une tâche parallèle au Batcher (ID: {taskId}) : {work.Method.Name}"); // Pour le debug

            _tasksStates.TryAdd(taskId, new TaskState { Current = 0, Total = initialEstimate });
            RecalculateGlobalProgress();

            var taskReporter = new ProgressWrapper(info => HandleTaskReport(taskId, info));

            Task t = Task.Run(() =>
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
                    LogTools.Logger.Error($"Erreur critique dans une tâche parallèle du Batcher (ID: {taskId})", ex);

                    // 2. RETHROW : Important pour que la Task soit marquée comme "Faulted"
                    // et que l'exception remonte jusqu'au WaitAll.
                    throw;
                }
                finally
                {
                    CompleteTask(taskId);
                }
            });

            lock (_lockObject)
            {
                _tasks.Add(t);
            }
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
                    LogTools.Logger.Error(innerEx,"Erreur dans une tâche du Batcher");
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

        // --- Méthodes privées (Inchangées) ---

        private void HandleTaskReport(Guid taskId, BatchProgressInfo info)
        {
            if (!_tasksStates.TryGetValue(taskId, out var state)) return;

            lock (state)
            {
                if (info.Type == BatchProgressType.Initialization)
                {
                    state.Total = info.Value > 0 ? info.Value : 1;
                }
                else if (info.Type == BatchProgressType.Progress)
                {
                    state.Current = info.Value;
                    if (state.Current > state.Total) state.Current = state.Total;
                }
            }
            RecalculateGlobalProgress();
        }

        private void CompleteTask(Guid taskId)
        {
            if (_tasksStates.TryGetValue(taskId, out var state))
            {
                lock (state) state.Current = state.Total;
                RecalculateGlobalProgress();
            }
        }

        private void RecalculateGlobalProgress()
        {
            if (_globalProgressReporter == null) return;

            var states = _tasksStates.Values.ToList();
            long totalGlobal = 0;
            long currentGlobal = 0;

            foreach (var s in states)
            {
                lock (s)
                {
                    totalGlobal += s.Total;
                    currentGlobal += s.Current;
                }
            }

            if (totalGlobal == 0) totalGlobal = 1;

            float globalPercent = ((float) currentGlobal) / totalGlobal;
            if (globalPercent > 1.0) globalPercent = 1.0F;

            _globalProgressReporter.Report(_converter(globalPercent));
        }

        private void Reset()
        {
            lock (_lockObject) _tasks.Clear();
            _tasksStates.Clear();

            while (_resultsBag.TryTake(out _)) { }

            _globalProgressReporter?.Report(_converter(0));
        }

        private class ProgressWrapper : IProgress<BatchProgressInfo>
        {
            private readonly Action<BatchProgressInfo> _handler;
            public ProgressWrapper(Action<BatchProgressInfo> handler) { _handler = handler; }
            public void Report(BatchProgressInfo value) => _handler(value);
        }
    }
}