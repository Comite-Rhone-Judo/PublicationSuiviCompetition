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
        /// Met à jour le snapshot via un remplacement complet (Snapshot Complet).
        /// Tout ce qui n'est pas dans 'nouveauxElements' disparait.
        /// </summary>
        public void UpdateFullSnapshot(IEnumerable<TValue> nouveauxElements, Func<TValue, TKey> keySelector)
        {
            ApplyUpdate(nouveauxElements, keySelector, isDifferential: false);
        }

        /// <summary>
        /// Met à jour le snapshot via une fusion (Snapshot Différentiel).
        /// Ajoute les nouveaux items et met à jour ceux existants (basé sur la clé).
        /// Les items existants non mentionnés dans le delta sont conservés.
        /// </summary>
        public void UpdateDifferentialSnapshot(IEnumerable<TValue> elementsModifies, Func<TValue, TKey> keySelector)
        {
            ApplyUpdate(elementsModifies, keySelector, isDifferential: true);
        }


        /// <summary>
        /// Logique centralisée de fusion et dédoublonnage.
        /// </summary>
        private void ApplyUpdate(IEnumerable<TValue> incomingData, Func<TValue, TKey> keySelector, bool isDifferential)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var changes = incomingData as ICollection<TValue> ?? (incomingData?.ToList());

            // Optimisation : Si différentiel et aucun changement, on ne fait rien
            if (isDifferential && (changes == null || changes.Count == 0))
            {
                return;
            }

            // 1. Préparation du dictionnaire de travail
            Dictionary<TKey, TValue> workingDict;

            if (isDifferential)
            {
                // Différentiel : On part de l'existant (Copie défensive pour thread-safety)
                var currentSnapshot = Cache;
                workingDict = new Dictionary<TKey, TValue>(currentSnapshot.Count + (changes?.Count ?? 0));

                foreach (var item in currentSnapshot)
                {
                    // On suppose que le cache actuel est déjà propre (clés uniques)
                    workingDict[keySelector(item)] = item;
                }
            }
            else
            {
                // Complet : On part de zéro
                workingDict = new Dictionary<TKey, TValue>(changes?.Count ?? 0);
            }

            // 2. Application des changements (Upsert : Last win)
            if (changes != null)
            {
                foreach (var item in changes)
                {
                    // La clé écrase la valeur existante (mise à jour ou ajout)
                    workingDict[keySelector(item)] = item;
                }
            }

            // 3. Finalisation et Swap atomique
            // On transforme les valeurs du dictionnaire en liste (l'ordre n'est pas garanti par le Dict, 
            // si l'ordre importe, il faudrait trier ici ou utiliser un SortedDictionary)
            Swap(workingDict.Values.ToList());
        }
    }
}