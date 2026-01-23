using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Tools.Core
{
    public static class ClassFactory
    {
        // Liste pour ajouter manuellement d'autres assemblies si necessaire
        public static List<Assembly> AssembliesExternes { get; } = new List<Assembly>();

        /// <summary>
        /// Instancie une classe situee dans l'assembly appelant, l'assembly en cours, ou ceux enregistres.
        /// </summary>
        public static T CreateInstance<T>(string nomClasse)
        {
            // 1. Construction de la liste de recherche par priorite
            var assembliesARerchercher = new List<Assembly>();

            // A. Priorite 1 : L'application qui appelle
            assembliesARerchercher.Add(Assembly.GetCallingAssembly());

            // B. Priorite 2 : La librairie elle-meme
            assembliesARerchercher.Add(Assembly.GetExecutingAssembly());

            // C. Priorite 3 : Autres assemblies ajoutes manuellement
            if (AssembliesExternes.Count > 0)
            {
                assembliesARerchercher.AddRange(AssembliesExternes);
            }

            // On retire les doublons pour eviter de chercher deux fois au meme endroit
            var listeFinale = assembliesARerchercher.Distinct().ToList();

            // 2. Boucle de recherche
            foreach (var assembly in listeFinale)
            {
                Type typeTrouve = ChercherTypeDansAssembly(assembly, nomClasse);

                if (typeTrouve != null)
                {
                    // Verification de compatibilite (T)
                    if (!typeof(T).IsAssignableFrom(typeTrouve))
                    {
                        // Message sans accent pour les logs
                        throw new InvalidCastException($"La classe '{typeTrouve.FullName}' trouvee dans '{assembly.GetName().Name}' n'herite pas de '{typeof(T).Name}'.");
                    }

                    // Instanciation
                    return (T)Activator.CreateInstance(typeTrouve);
                }
            }

            // Si on arrive ici, c'est qu'aucun assembly n'a le type
            // Message sans accent pour les logs
            string listeNoms = string.Join(", ", listeFinale.Select(a => a.GetName().Name));
            throw new TypeLoadException($"La classe '{nomClasse}' est introuvable dans les assemblies analyses ({listeNoms}).");
        }

        /// <summary>
        /// Logique de recherche
        /// </summary>
        private static Type ChercherTypeDansAssembly(Assembly assembly, string nomClasse)
        {
            // 1. Recherche exacte
            Type type = assembly.GetType(nomClasse);
            if (type != null) return type;

            // 2. Recherche souple (nom de classe seul)
            var resultats = assembly.GetTypes()
                .Where(t => t.Name.Equals(nomClasse, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (resultats.Count > 1)
            {
                // Message sans accent pour les logs
                throw new AmbiguousMatchException($"Plusieurs classes portent le nom '{nomClasse}' dans l'assembly '{assembly.GetName().Name}' (ex: {resultats[0].FullName}, {resultats[1].FullName}). Utilisez le namespace complet.");
            }

            return resultats.FirstOrDefault();
        }
    }

}
