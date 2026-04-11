using System;
using System.Linq;

namespace FranceJudo.Core.Reflection
{
    /// <summary>
    /// Utilitaire pour manipuler les chemins de ressources incorporées (séparés par des points).
    /// </summary>
    public static class ResourcePath
    {
        /// <summary>
        /// Combine un tableau de chaînes en un seul chemin de ressource (ex: "Dossier", "Fichier.xml" -> "Dossier.Fichier.xml").
        /// </summary>
        public static string Combine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            var cleanedSegments = paths
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim('.'));

            return string.Join(".", cleanedSegments);
        }

        /// <summary>
        /// Extrait la fin d'un chemin de ressource en retirant le chemin de base (dossier parent).
        /// Idéal pour retrouver le vrai nom de fichier d'une ressource.
        /// </summary>
        /// <param name="fullResourcePath">Le chemin complet (ex: "FranceJudo.Metier.Resources.Site.img.logo.png")</param>
        /// <param name="basePath">Le dossier à retirer (ex: "FranceJudo.Metier.Resources.Site.img")</param>
        /// <returns>Le nom relatif (ex: "logo.png")</returns>
        public static string GetRelativePath(string fullResourcePath, string basePath)
        {
            if (string.IsNullOrWhiteSpace(fullResourcePath)) return string.Empty;
            if (string.IsNullOrWhiteSpace(basePath)) return fullResourcePath;

            // On s'assure que le préfixe se termine bien par un point
            string prefix = basePath.EndsWith(".") ? basePath : basePath + ".";

            // On vérifie que la ressource commence bien par ce dossier
            if (fullResourcePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                // On extrait uniquement ce qui vient APRÈS le préfixe
                return fullResourcePath.Substring(prefix.Length);
            }

            return fullResourcePath;
        }

        /// <summary>
        /// Tente de deviner le nom du fichier d'une ressource en se basant sur la position des points.
        /// Attention : Cette méthode échouera et tronquera le nom si le fichier contient 
        /// lui-même des points (comme "jquery.min.js" ou "version.1.2.pdf").
        /// À utiliser UNIQUEMENT si vous êtes certain du format simple du fichier (ex: "logo.png").
        /// </summary>
        /// <param name="resourcePath">Le chemin complet de la ressource</param>
        /// <param name="hasExtension">Indique si le fichier possède une extension (vrai par défaut)</param>
        public static string GuessFileName(string resourcePath, bool hasExtension = true)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
                return string.Empty;

            // On trouve le tout dernier point
            int lastDotIndex = resourcePath.LastIndexOf('.');

            if (lastDotIndex == -1)
                return resourcePath;

            // S'il n'y a pas d'extension, le nom commence après le dernier point
            if (!hasExtension)
            {
                return resourcePath.Substring(lastDotIndex + 1);
            }

            // S'il y a une extension, on cherche le point encore avant (l'avant-dernier)
            int secondToLastDotIndex = resourcePath.LastIndexOf('.', lastDotIndex - 1);

            if (secondToLastDotIndex == -1)
                return resourcePath;

            // On coupe après l'avant-dernier point
            return resourcePath.Substring(secondToLastDotIndex + 1);
        }
    }
}