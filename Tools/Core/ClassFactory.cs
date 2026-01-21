using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class ClassFactory
    {
        /// <summary>
        /// Instancie une classe située dans l'assembly appelant.
        /// Accepte le nom complet (Namespace.Classe) ou le nom court (Classe).
        /// </summary>
        public static T CreateInstance<T>(string nomClasse)
        {
            // Récupère l'assembly du code qui appelle cette méthode (votre Application)
            // et non l'assembly de la librairie elle-même.
            Assembly assemblyAppelant = Assembly.GetCallingAssembly();

            // 1. Recherche exacte (Nom complet avec Namespace)
            // C'est le plus rapide si la config est précise.
            Type type = assemblyAppelant.GetType(nomClasse);

            // 2. Recherche souple (Nom de classe seul)
            if (type == null)
            {
                // On cherche parmi tous les types de l'assembly appelant
                var resultats = assemblyAppelant.GetTypes()
                    .Where(t => t.Name.Equals(nomClasse, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (resultats.Count == 0)
                {
                    throw new TypeLoadException($"La classe '{nomClasse}' est introuvable dans l'assembly '{assemblyAppelant.GetName().Name}'.");
                }

                if (resultats.Count > 1)
                {
                    throw new AmbiguousMatchException($"Plusieurs classes portent le nom '{nomClasse}' (ex: {resultats[0].FullName}, {resultats[1].FullName}). Veuillez utiliser le namespace complet.");
                }

                type = resultats[0];
            }

            // 3. Vérification de compatibilité avant instanciation
            // (Optionnel mais recommandé pour avoir une erreur claire)
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new InvalidCastException($"La classe '{type.Name}' n'hérite pas de '{typeof(T).Name}'.");
            }

            // 4. Instanciation
            return (T)Activator.CreateInstance(type);
        }
    }
}
