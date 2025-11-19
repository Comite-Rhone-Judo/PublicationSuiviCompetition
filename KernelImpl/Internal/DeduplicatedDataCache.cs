using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KernelImpl.Internal
{
    /// <summary>
    /// Cache spécialisé pour les listes, avec logique de déduplication intégrée.
    /// </summary>
    internal class DeduplicatedCachedData<TKey, TValue> : AtomicCachedBase<IReadOnlyList<TValue>>
    {
        // Initialisation avec une liste vide
        public DeduplicatedCachedData() : base(new List<TValue>(0)) { }

        /// <summary>
        /// Construit une nouvelle liste dédupliquée et effectue le swap.
        /// </summary>
        public void UpdateSnapshot(IEnumerable<TValue> nouveauxElements, Func<TValue, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var items = nouveauxElements as ICollection<TValue> ?? (nouveauxElements?.ToList());

            if (items == null || items.Count == 0)
            {
                Swap(new List<TValue>(0));
                return;
            }

            // Logique de déduplication (Dictionnaire temporaire)
            var tempDict = new Dictionary<TKey, TValue>(items.Count);
            foreach (var item in items)
            {
                tempDict[keySelector(item)] = item;
            }

            // Swap via la classe de base
            Swap(tempDict.Values.ToList());
        }
    }
}