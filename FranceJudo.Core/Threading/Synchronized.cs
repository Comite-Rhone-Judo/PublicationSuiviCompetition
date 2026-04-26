using System;
using System.Threading;

namespace FranceJudo.Core.Threading
{
    /// <summary>
    /// Encapsule une ressource pour garantir des accès concurrents optimisés
    /// (Lecteurs multiples en parallèle / Écrivain unique) avec Timeouts anti-deadlock.
    /// </summary>
    public class Synchronized<T> : IDisposable
    {
        private readonly T _resource;

        // Optimisé pour les scénarios avec beaucoup de lectures et peu d'écritures
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly TimeSpan _defaultTimeout;
        private bool _isDisposed = false;

        public Synchronized(T resource, int timeoutSeconds = 5)
        {
            _resource = resource ?? throw new ArgumentNullException(nameof(resource));
            _defaultTimeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        /// <summary>
        /// Exécute une opération de LECTURE. 
        /// Plusieurs threads peuvent exécuter cette méthode exactement en même temps.
        /// </summary>
        /// <typeparam name="TResult">Le type de résultat de l'opération de lecture.</typeparam>
        /// <param name="query">La fonction à exécuter en lecture.</param>
        /// <returns>Le résultat de l'opération de lecture.</returns>
        public TResult SafeReadAction<TResult>(Func<T, TResult> query)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Synchronized<T>));

            // Bloque uniquement s'il y a un thread en cours d'ÉCRITURE.
            if (!_lock.TryEnterReadLock(_defaultTimeout))
                throw new TimeoutException($"Impossible d'obtenir le verrou de LECTURE après {_defaultTimeout.TotalSeconds}s.");

            try
            {
                return query(_resource);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Exécute une opération d'ÉCRITURE (Modification).
        /// Bloque tous les autres lecteurs et écrivains pendant l'exécution.
        /// </summary>
        /// <param name="action">L'action à exécuter en écriture.</param>
        public void SafeWriteAction(Action<T> action)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Synchronized<T>));

            // Bloque jusqu'à ce que tous les lecteurs actuels aient fini, puis prend l'exclusivité.
            if (!_lock.TryEnterWriteLock(_defaultTimeout))
                throw new TimeoutException($"Impossible d'obtenir le verrou d'ÉCRITURE après {_defaultTimeout.TotalSeconds}s.");

            try
            {
                action(_resource);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Nettoyage obligatoire pour libérer les handles natifs Windows du verrou.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _lock.Dispose();
                _isDisposed = true;
            }
        }
    }
}