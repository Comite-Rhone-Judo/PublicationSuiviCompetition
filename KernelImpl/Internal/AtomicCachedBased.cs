using System.Threading;

namespace KernelImpl.Internal
{
    /// <summary>
    /// Classe de base gérant le stockage atomique d'une référence.
    /// Garantit des lectures O(1) non-bloquantes et des mises à jour thread-safe.
    /// </summary>
    /// <typeparam name="T">Le type de la donnée stockée (List, Dictionary, etc.)</typeparam>
    internal abstract class AtomicCachedBase<T> where T : class
    {
        // Le champ volatile conceptuel (manipulé par Interlocked)
        private T _cache;

        protected AtomicCachedBase(T initialValue)
        {
            _cache = initialValue;
        }

        /// <summary>
        /// Accès public en lecture seule (O(1)).
        /// </summary>
        public T Cache => _cache;

        /// <summary>
        /// Remplace atomiquement la référence stockée.
        /// </summary>
        protected void Swap(T newValue)
        {
            Interlocked.Exchange(ref _cache, newValue);
        }
    }
}