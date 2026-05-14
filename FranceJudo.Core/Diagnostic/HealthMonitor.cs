using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using FranceJudo.Core.Logging; // Ajustez selon votre espace de noms

namespace FranceJudo.Core.Diagnostic
{
    public static class HealthMonitor
    {
        private static Timer _systemTimer;
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

            LogTools.Logger?.Info($"Monitoring systeme demarre (Intervalle : {intervalSeconds}s).");
        }

        /// <summary>
        /// Arrête tous les monitorings (Système et Heartbeats).
        /// </summary>
        public static void StopAllMonitoring()
        {
            if (_systemTimer != null)
            {
                // On crée un handle d'attente
                using (var waitHandle = new ManualResetEvent(false))
                {
                    // Dispose(waitHandle) signale le handle quand TOUS les callbacks sont finis
                    if (_systemTimer.Dispose(waitHandle))
                    {
                        waitHandle.WaitOne(TimeSpan.FromSeconds(2));
                    }
                }
                _systemTimer = null;
                _isMonitoringSystem = false;
            }
            LogTools.Logger?.Info("[HEALTH] Tous les monitorings ont ete arretes.");
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
                LogTools.Logger?.Error(ex, "[HEALTH] Erreur lors de la lecture des compteurs systeme.");
            }
        }
    }
}