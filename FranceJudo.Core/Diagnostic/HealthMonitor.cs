using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Concurrent;
using FranceJudo.Core.Logging; // Ajustez selon votre espace de noms

namespace FranceJudo.Core.Diagnostic
{
    public static class HealthMonitor
    {
        private static Timer _systemTimer;

        // Stocke les timers de heartbeat pour chaque Dispatcher surveillé, pour pouvoir les arrêter proprement
        private static readonly ConcurrentDictionary<int, DispatcherTimer> _heartbeatTimers = new ConcurrentDictionary<int, DispatcherTimer>();

        private static bool _isMonitoringSystem = false;

        /// <summary>
        /// Démarre le monitoring global du système (Mémoire, Threads natifs, GC).
        /// À n'appeler qu'une seule fois au démarrage de l'application.
        /// </summary>
        /// <param name="intervalSeconds">L'intervalle de log en secondes (défaut: 60s)</param>
        public static void StartSystemMonitoring(int intervalSeconds = 60)
        {
            if (_isMonitoringSystem) return;

            // Timer de fond (utilise le ThreadPool)
            _systemTimer = new Timer(LogSystemHealth, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalSeconds));
            _isMonitoringSystem = true;

            LogTools.Logger.Info($"Monitoring systeme demarre (Intervalle : {intervalSeconds}s).");
        }

        /// <summary>
        /// Ajoute un "Heartbeat" sur un Dispatcher spécifique pour surveiller ses temps de blocage (Freeze).
        /// Utile pour le Thread UI principal, mais aussi pour tout Dispatcher secondaire.
        /// </summary>
        /// <param name="dispatcher">Le dispatcher à surveiller</param>
        /// <param name="name">Le nom logique du thread (ex: "MainUI", "NetworkListener") pour les logs</param>
        /// <param name="maxFreezeThresholdMs">Le temps de blocage toléré avant d'émettre un avertissement (défaut: 3000ms)</param>
        public static void MonitorDispatcher(Dispatcher dispatcher, string name, int maxFreezeThresholdMs = 3000)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

            // Identifiant unique du thread pour le dictionnaire
            int threadId = dispatcher.Thread.ManagedThreadId;

            if (_heartbeatTimers.ContainsKey(threadId))
            {
                LogTools.Logger.Warn($"Le dispatcher '{name}' (Thread {threadId}) est deja surveille.");
                return;
            }

            // Variables capturées par la lambda du tick
            DateTime lastTick = DateTime.Now;

            // On utilise DispatcherPriority.Background pour s'assurer que le tick 
            // n'est traité QUE si le thread a le temps. S'il est surchargé, le tick sera retardé.
            var heartbeatTimer = new DispatcherTimer(DispatcherPriority.Background, dispatcher)
            {
                Interval = TimeSpan.FromSeconds(1) // On bat la mesure chaque seconde
            };

            heartbeatTimer.Tick += (s, e) =>
            {
                var now = DateTime.Now;
                var delay = (now - lastTick).TotalMilliseconds;

                // Si le délai entre deux battements dépasse le seuil, c'est un freeze.
                if (delay > maxFreezeThresholdMs)
                {
                    LogTools.HealthLogger.Warn($"[HEALTH] FREEZE Le Dispatcher '{name}' (Thread {threadId}) a ete bloque pendant {delay:F0} ms !");
                }

                lastTick = now;
            };

            if (_heartbeatTimers.TryAdd(threadId, heartbeatTimer))
            {
                heartbeatTimer.Start();
                LogTools.Logger.Info($"[HEALTH] Monitoring du Dispatcher '{name}' demarre (Seuil : {maxFreezeThresholdMs}ms).");
            }
        }

        /// <summary>
        /// Arrête tous les monitorings (Système et Heartbeats).
        /// </summary>
        public static void StopAllMonitoring()
        {
            if (_systemTimer != null)
            {
                _systemTimer.Dispose();
                _systemTimer = null;
                _isMonitoringSystem = false;
            }

            foreach (var kvp in _heartbeatTimers)
            {
                kvp.Value.Stop();
            }
            _heartbeatTimers.Clear();

            LogTools.Logger.Info("[HEALTH] Tous les monitorings ont ete arretes.");
        }

        /// <summary>
        /// La méthode de log appelée périodiquement par le System Timer.
        /// </summary>
        private static void LogSystemHealth(object state)
        {
            try
            {
                using (var process = Process.GetCurrentProcess())
                {
                    long ramUsageMb = process.WorkingSet64 / (1024 * 1024);
                    int gc0 = GC.CollectionCount(0);
                    int gc1 = GC.CollectionCount(1);
                    int gc2 = GC.CollectionCount(2);

                    LogTools.HealthLogger.Info($"[HEALTH] SYS RAM: {ramUsageMb} MB | Threads: {process.Threads.Count} | GC (0/1/2): {gc0}/{gc1}/{gc2}");
                }
            }
            catch (Exception ex)
            {
                // Ne jamais faire crasher l'appli depuis un timer de fond
                LogTools.Logger.Error(ex, "[HEALTH] Erreur lors de la lecture des compteurs systeme.");
            }
        }
    }
}