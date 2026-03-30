using System.Linq;

namespace FranceJudo.Core.Reflection
{
    /// <summary>
    /// Utilitaire pour manipuler les chemins de ressources incorporées (séparés par des points).
    /// </summary>
    public static class ResourcePath
    {
        /// <summary>
        /// Combine un tableau de chaînes en un seul chemin de ressource.
        /// </summary>
        public static string Combine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            // On ignore les chaînes vides, et on nettoie les points en début/fin de chaque segment 
            // pour éviter la création de doubles points (ex: "Dossier." + ".Fichier")
            var cleanedSegments = paths
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim('.'));

            // On joint le tout avec un seul point
            return string.Join(".", cleanedSegments);
        }
    }
}