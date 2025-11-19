using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KernelImpl.Internal
{
    /// <summary>
    /// Wrapper thread-safe optimisé pour les listes lues fréquemment (O(1)).
    /// Cette version effectue un remplacement complet (Snapshot) SANS déduplication.
    /// </summary>
    internal class ConcurrentCachedData<TValue> // Note: TKey n'est plus nécessaire ici
    {
        // Stockage de la référence vers la liste.
        private List<TValue> _listCache = new List<TValue>(0);

        /// <summary>
        /// Accès thread-safe, non-bloquant et O(1) à la version actuelle de la liste.
        /// </summary>
        public IReadOnlyList<TValue> ListCache => _listCache;

        /// <summary>
        /// Met à jour atomiquement la collection à partir d'un snapshot complet.
        /// </summary>
        public void UpdateSnapshot(IEnumerable<TValue> nouveauxElements)
        {
            // 1. Matérialisation (évite double énumération et copie la source)
            // Si nouveauxElements est déjà une List<T>, ToList() en crée une copie superficielle,
            // ce qui est nécessaire pour garantir que la liste stockée n'est pas modifiée de l'extérieur.
            var nouvelleListe = nouveauxElements?.ToList();

            if (nouvelleListe == null || nouvelleListe.Count == 0)
            {
                // Remplacement atomique par une liste vide
                Interlocked.Exchange(ref _listCache, new List<TValue>(0));
                return;
            }

            // 2. Échange atomique de la référence (O(1) + Barrière Mémoire)
            Interlocked.Exchange(ref _listCache, nouvelleListe);
        }
    }
}
