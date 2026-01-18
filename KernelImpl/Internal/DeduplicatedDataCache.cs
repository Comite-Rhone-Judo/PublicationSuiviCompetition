using KernelImpl.Noyau;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
        /// OPTIMISÉ POUR SNAPSHOT COMPLET.
        /// Remplace totalement le cache. Gère la déduplication via le keySelector.
        /// </summary>
        public void UpdateFullSnapshot(IEnumerable<TValue> nouveauxElements, Func<TValue, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            // Optimisation : Gestion rapide des listes vides
            var items = nouveauxElements as ICollection<TValue> ?? (nouveauxElements?.ToList());
            if (items == null || items.Count == 0)
            {
                // Si vide, on swap avec une liste vide sans allouer de Dictionnaire
                Swap(new List<TValue>(0));
                return;
            }

            // 1. Création du dictionnaire (Allocation unique dimensionnée)
            // On écrase les doublons potentiels de l'input (Last wins)
            var workingDict = new Dictionary<TKey, TValue>(items.Count);
            foreach (var item in items)
            {
                workingDict[keySelector(item)] = item;
            }

            // 2. Swap atomique
            Swap(workingDict.Values.ToList());
        }

        /// <summary>
        /// OPTIMISÉ POUR DIFFÉRENTIEL.
        /// Fusionne l'existant avec les modifications.
        /// </summary>
        public void UpdateDifferentialSnapshot(IEnumerable<TValue> elementsModifies, Func<TValue, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var changes = elementsModifies as ICollection<TValue> ?? (elementsModifies?.ToList());
            if (changes == null || changes.Count == 0) return;

            // 1. On part du cache actuel (Lecture O(1))
            var currentSnapshot = Cache;

            // 2. On prépare le dictionnaire avec la taille estimée (Existant + Nouveaux)
            var workingDict = new Dictionary<TKey, TValue>(currentSnapshot.Count + changes.Count);

            // 3. Copie de l'existant
            foreach (var item in currentSnapshot)
            {
                workingDict[keySelector(item)] = item;
            }

            // 4. Application des modifications (Upsert)
            foreach (var item in changes)
            {
                workingDict[keySelector(item)] = item;
            }

            // 5. Swap atomique
            Swap(workingDict.Values.ToList());
        }
    }
}