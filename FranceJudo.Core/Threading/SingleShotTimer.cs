using FranceJudo.Core.Logging;
using System;
using System.Threading;

namespace FranceJudo.Core.Threading
{
    public class SingleShotTimer : IDisposable
    {
        #region MEMBERS
        private TimeSpan _lockTimeout;
        private TimeSpan _disposeTimeout;

        private System.Threading.Timer _timer = null;
        private readonly object _lock = new object();
        private bool _isRunning = false;
        #endregion

        #region CONSTRUCTORS
        public SingleShotTimer(int disposalTimeooutMs = 10000)
        {
            TimeoutMs = disposalTimeooutMs;
        }

        #endregion

        #region PROPERTIES

        public event Action<object> Elapsed;

        public bool IsRunning => _isRunning;

        public int TimeoutMs
        {
            get { return _lockTimeout.Milliseconds; }
            set
            {
                _disposeTimeout = TimeSpan.FromMilliseconds(3 * value);
                _lockTimeout = TimeSpan.FromMilliseconds(value);
            }
        }
        #endregion

        #region METHODES
        public void Start(long durationMs)
        {
            // 1. On arrête le timer de manière sécurisée (gère ses propres verrous)
            Stop();

            // 2. On verrouille juste pour créer la nouvelle instance
            using (TimedLock.Lock(_lock, _lockTimeout))
            {
                if (_timer != null) return; // Sécurité si un autre thread a appelé Start en même temps

                _timer = new System.Threading.Timer(HandleTimerElapsed, null, durationMs, Timeout.Infinite);
                _isRunning = true;
            }
        }

        public void Stop()
        {
            ManualResetEvent waitHandle = null;

            // 1. Verrouillage ultra-court pour récupérer et détruire la référence
            using (TimedLock.Lock(_lock, _lockTimeout))
            {
                if (_timer == null) return;

                waitHandle = new ManualResetEvent(false);

                // Dispose(waitHandle) déclenchera le signal quand le dernier callback sera terminé
                if (_timer.Dispose(waitHandle))
                {
                    _timer = null;
                    _isRunning = false;
                }
                else
                {
                    // Cas rare où le timer était déjà disposé
                    waitHandle.Dispose();
                    waitHandle = null;
                }
            }

            // 2. ATTENTE HORS DU VERROU (La clé de la résolution du problème !)
            // Aucun deadlock croisé possible car le verrou _lock est déjà relâché.
            if (waitHandle != null)
            {
                if (!waitHandle.WaitOne(_disposeTimeout))
                {
                    LogTools.Logger.Warn("Timeout lors de l'attente de l'arrêt complet du timer.");
                }
                waitHandle.Dispose();
            }
        }

        public void Dispose()
        {
            Stop();
        }
        #endregion

        #region METHODES PRIVEES
        private void HandleTimerElapsed(object state)
        {
            Action<object> callbackToFire = null;

            try
            {
                // 1. Verrouillage ultra-court (quelques nanosecondes)
                using (TimedLock.Lock(_lock, _lockTimeout))
                {
                    if (_timer == null) return; // Le timer a été annulé juste avant

                    // On capture le délégué pour l'exécuter HORS du verrou
                    callbackToFire = Elapsed;
                    _isRunning = false;
                }

                // 2. Exécution de la logique métier SANS GARDER LE VERROU.
                // Ainsi, on ne gèle jamais la classe SingleShotTimer.
                callbackToFire?.Invoke(state);
            }
            catch (Exception ex)
            {
                // Un timer de ThreadPool qui lève une exception non gérée fait crasher l'app.
                // Il faut toujours un catch global ici.
                LogTools.Logger.Error(ex, "Erreur lors de l'exécution du callback du SingleShotTimer");
            }
        }

        #endregion
    }
}