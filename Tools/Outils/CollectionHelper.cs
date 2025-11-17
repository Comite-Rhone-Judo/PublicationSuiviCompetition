using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Outils
{
    public static class CollectionHelper
    {

        /// <summary>
        /// Met à jour de manière générique, atomique et thread-safe une collection
        /// à partir d'un "snapshot" complet des nouvelles données.
        /// Cette méthode remplace l'ancienne collection par une nouvelle.
        /// </summary>
        /// <typeparam name="TKey">Le type de la clé (ex: int)</typeparam>
        /// <typeparam name="TValue">Le type de l'objet (ex: Commissaire, Judoka...)</typeparam>
        /// <param name="collectionActuelle">
        /// La collection ConcurrentDictionary à mettre à jour.
        /// Elle sera remplacée par une nouvelle instance.
        /// </param>
        /// <param name="nouveauxElements">La liste complète des nouveaux éléments reçus.</param>
        /// <param name="keySelector">Une fonction pour extraire la clé de chaque élément (ex: c => c.id).</param>
        public static void UpdateConcurrentCollection<TKey, TValue>(
            ref ConcurrentDictionary<TKey, TValue> collectionActuelle,
            IEnumerable<TValue> nouveauxElements,
            Func<TValue, TKey> keySelector)
        {
            // Si le snapshot est vide ou nul, on vide la collection
            if (nouveauxElements == null || !nouveauxElements.Any())
            {
                collectionActuelle = new ConcurrentDictionary<TKey, TValue>();
                return;
            }

            // 1. Construit un nouveau dictionnaire complet en O(M).
            // Nous utilisons une boucle au lieu de .ToDictionary() pour gérer
            // d'éventuels doublons dans les données sources (le dernier gagne).
            var newDictionary = new Dictionary<TKey, TValue>();
            foreach (var element in nouveauxElements)
            {
                newDictionary[keySelector(element)] = element;
            }

            // 2. Remplace l'ancienne collection par la nouvelle.
            // C'est une assignation de référence atomique et thread-safe.
            // Les threads qui lisaient l'ancienne collection terminent leur travail dessus,
            // les nouveaux threads liront celle-ci.
            collectionActuelle = new ConcurrentDictionary<TKey, TValue>(newDictionary);
        }
    }
}
