using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Outils
{
    public class SingleShotTimer : IDisposable
    {
        #region MEMBRES

        private TimeSpan _lockTimeout;       // Temps max pour acquerir un verrou
        private TimeSpan _disposeTimeout;     // Temps max pour liberer le timer

        private System.Threading.Timer _timer = null;   // Le timer en lui meme
        private object _lock = null;                    // Verrour interne pour la synchronisation
        private bool _isRunning = false;                // Indique si le timer est actif

        #endregion

        #region CONSTRUCTEURS

        public SingleShotTimer(int disposalTimeooutMs = 10000)
        {
            TimeoutMs = disposalTimeooutMs;
            _lock = new object();
            _timer = null;
            _isRunning = false;
        }

        #endregion

        #region PROPRIETES

        /// <summary>
        /// La methode appelee lors de l'execution du timer
        /// </summary>
        public event Action<object> Elapsed;

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        /// <summary>
        /// Timeout unitaire pour l'acquisition du verrou
        /// </summary>
        public int TimeoutMs
        {
            get { return _lockTimeout.Milliseconds; }
            set
            {
                _disposeTimeout = new TimeSpan(0, 0, 0, 0, 3 * value);
                _lockTimeout = new TimeSpan(0, 0, 0, 0, value);
            }
        }

        #endregion

        /// <summary>
        /// Demarre le timer
        /// </summary>
        /// <param name="durationMs">Delai avant le declenchement</param>
        /// <param name="state">objet d'etat a passer en parametre au callback</param>
        public void Start(long durationMs, object state = null)
        {
            lock (_lock)
            {
                try
                {
                    UnsafeStop();     // Pour le cas ou le timer serait deja actif
                }
                catch (TimeoutException)
                {
                    LogTools.Logger.Error("TimeoutException lors de la tentative d'arret du timer");
                }

                // Si on n'a pas reussi a le stopper, on ne peut pas le redemarrer
                if (null != _timer)
                {
                    return;
                }

                // Active le timer (sans recurrence)
                _timer = new System.Threading.Timer(HandleTimerElapsed, null, durationMs, Timeout.Infinite);
                _isRunning = true;
            }
        }

        /// <summary>
        /// Appel lors de l'execution du timer
        /// <param name="state"></param>
        private void HandleTimerElapsed(object state)
        {
            // Verifie l'etat du Timer, si ce dernier a ete supprime juste avant l'appel, on ignore tout simplement l'evenement
            // Utilise un verrou avec timeout pour eviter un Deadlock sur le Dispose
            try
            {
                if (Monitor.TryEnter(_lock, _lockTimeout))
                {
                    if (null == _timer)
                    {
                        return;
                    }

                    // Appel de l'evenement
                    Elapsed?.Invoke(state);

                    // On ne stop pas le timer car
                    // 1 - il est prevu pour un single shot donc il ne se relancera pas
                    // 2 - cela cree une situation de deadlock si appeler depuis le handler
                    _isRunning = false;
                }
            }
            finally {
                try
                {
                    Monitor.Exit(_lock);
                }
                catch(Exception ex)
                {
                    LogTools.Logger.Debug("Erreur lors de la liberation du verrou interne", ex);
                }
            }
        }

        /// <summary>
        /// Arrete le timer
        /// </summary>
        /// <exception cref="TimeoutException">Impossible d'arreter le timer dans le temps imparti</exception>
        public void Stop()
        {
            lock(_lock)
            {
                UnsafeStop();
            }
        }

        /// <summary>
        /// Arrete le timer (sans verrou)
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        private void UnsafeStop()
        {
            if (null == _timer)
            {
                return;
            }

            var waitHandle = new ManualResetEvent(false);
            // Supprimer le timer
            if (_timer.Dispose(waitHandle))
            {
                // Attend l'arret des eventuels appels en cours lances avant le Stop
                if (waitHandle.WaitOne(_disposeTimeout))
                {
                    _timer = null;
                    waitHandle.Dispose();
                }
                else
                {
                    // don't dispose the wait handle, because the timer might still use it.
                    // Disposing it might cause an ObjectDisposedException on 
                    // the timer thread - whereas not disposing it will 
                    // result in the GC cleaning up the resources later
                    throw new TimeoutException("Timeout waiting for timer to stop");
                }
            }

            _isRunning = false;

        }

        public void Dispose()
        {
            try
            {
                UnsafeStop();
            }
            catch (TimeoutException)
            {
                LogTools.Logger.Error("TimeoutException lors de la tentative d'arret du timer");
                throw;
            }
            finally
            {
                _timer = null;
            }
        }
    }
}
