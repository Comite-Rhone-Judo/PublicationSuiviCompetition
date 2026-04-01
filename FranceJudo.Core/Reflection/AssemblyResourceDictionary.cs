using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FranceJudo.Core.Reflection
{
    /// <summary>
    /// Fournit un accès simplifié et sous forme de dictionnaire aux ressources incorporées d'un Assembly.
    /// </summary>
    public class AssemblyResourceDictionary
    {
        private readonly Assembly _assembly;
        private readonly string _rootNamespace;
        private readonly HashSet<string> _resourceNames;

        /// <summary>
        /// Initialise le dictionnaire sur l'assembly spécifié.
        /// </summary>
        /// <param name="assembly">L'assembly contenant les ressources</param>
        /// <param name="rootNamespace">Le namespace de base (ex: "FranceJudo.Metier.Resources")</param>
        public AssemblyResourceDictionary(Assembly assembly, string rootNamespace = null)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _rootNamespace = rootNamespace ?? _assembly.GetName().Name;

            // Stockage dans un HashSet pour une vérification ultra-rapide (O(1))
            _resourceNames = new HashSet<string>(_assembly.GetManifestResourceNames());
        }

        /// <summary>
        /// Retourne la liste complète de toutes les ressources de l'Assembly.
        /// </summary>
        public IReadOnlyCollection<string> AllResources => _resourceNames.ToList();

        /// <summary>
        /// Formate un "chemin relatif" en nom complet de ressource.
        /// (ex: "Site.img.logo.png" devient "FranceJudo.Metier.Resources.Site.img.logo.png")
        /// </summary>
        public string GetFullName(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return _rootNamespace;
            if (relativePath.StartsWith(_rootNamespace)) return relativePath; // Déjà complet

            return $"{_rootNamespace}.{relativePath}";
        }

        /// <summary>
        /// Vérifie si une ressource existe.
        /// </summary>
        public bool Exists(string relativePath)
        {
            return _resourceNames.Contains(GetFullName(relativePath));
        }

        /// <summary>
        /// Obtient le flux (Stream) d'une ressource spécifique.
        /// Retourne null si la ressource n'existe pas.
        /// </summary>
        public Stream GetStream(string relativePath)
        {
            string fullName = GetFullName(relativePath);
            if (!_resourceNames.Contains(fullName))
                return null;

            return _assembly.GetManifestResourceStream(fullName);
        }

        /// <summary>
        /// Cherche toutes les ressources contenues dans un "dossier" spécifique (préfixe).
        /// Exemple : FindByFolder("Site.img")
        /// </summary>
        public IEnumerable<string> FindByFolder(string folderRelativePath)
        {
            string prefix = GetFullName(folderRelativePath);
            if (!prefix.EndsWith(".")) prefix += ".";

            return _resourceNames.Where(name => name.StartsWith(prefix));
        }
    }
}