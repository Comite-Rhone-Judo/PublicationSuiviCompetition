using System;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Threading
{
    /// <summary>
    /// Un verrou asynchrone avec Timeout, conçu pour protéger les zones de code contenant 'await'.
    /// Remplace le mot-clé 'lock' qui est interdit en asynchrone.
    /// </summary>
    public sealed class AsyncTimedLock : IDisposable
    {
        // Semaphore initialisé à 1 jeton (comportement d'un Mutex/Lock classique)
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Tente d'obtenir le verrou de manière asynchrone.
        /// Utilisation : using(await _monCadenas.LockAsync(TimeSpan.FromSeconds(5))) { ... }
        /// </summary>
        public async Task<Releaser> LockAsync(TimeSpan timeout)
        {
            // ConfigureAwait(false) est une bonne pratique dans les bibliothèques Core
            // pour ne pas forcer le retour sur le Thread UI s'il n'est pas nécessaire.
            bool isLocked = await _semaphore.WaitAsync(timeout).ConfigureAwait(false);

            if (!isLocked)
            {
                throw new TimeoutException($"Impossible d'obtenir le verrou asynchrone après {timeout.TotalSeconds}s. Deadlock potentiel évité.");
            }

            return new Releaser(_semaphore);
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }

        /// <summary>
        /// Structure légère (0 allocation mémoire) chargée de relâcher le sémaphore lors du Dispose()
        /// </summary>
        public readonly struct Releaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            internal Releaser(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                _semaphore?.Release();
            }
        }
    }
}