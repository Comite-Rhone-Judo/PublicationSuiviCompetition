using System;
using System.Collections.Generic;

namespace FranceJudo.Core.Utils
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Découpe une collection en sous-listes (chunks) d'une taille spécifiée.
        /// Rétro-portage du comportement de .NET 6, optimisé pour le multithreading.
        /// </summary>
        public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize), "La taille du lot doit etre > 0.");

            // Pré-allocation de la capacité pour éviter les redimensionnements mémoires
            List<T> currentChunk = new List<T>(chunkSize);

            foreach (var item in source)
            {
                currentChunk.Add(item);

                if (currentChunk.Count == chunkSize)
                {
                    yield return currentChunk;
                    currentChunk = new List<T>(chunkSize); // On repart sur une nouvelle référence propre
                }
            }

            // On n'oublie pas le reste de la division (le dernier paquet)
            if (currentChunk.Count > 0)
            {
                yield return currentChunk;
            }
        }
    }
}