using KernelImpl.Internal;

namespace KernelImpl.Internal
{
    /// <summary>
    /// Cache générique pour tout type d'objet (Dictionnaire, Objet complexe).
    /// Effectue un remplacement simple sans transformation.
    /// </summary>
    internal class SimpleCachedData<T> : AtomicCachedBase<T> where T : class, new()
    {
        // Initialisation avec 'new T()' (ex: dictionnaire vide)
        public SimpleCachedData() : base(new T()) { }

        /// <summary>
        /// Met à jour le snapshot directement avec l'objet fourni.
        /// </summary>
        public void UpdateSnapshot(T newValue)
        {
            // On s'assure de ne jamais stocker null pour éviter les crashs en lecture
            if (newValue == null)
            {
                Swap(new T());
            }
            else
            {
                Swap(newValue);
            }
        }
    }
}