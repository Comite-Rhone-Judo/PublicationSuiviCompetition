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
        public static T CreateInstance<T>(string nomClasse)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 1. Essai rapide : on suppose que c'est le nom complet (Namespace.Classe)
            Type type = assembly.GetType(nomClasse);

            // 2. Si non trouvé, on cherche par le nom court (Classe uniquement)
            if (type == null)
            {
                // On cherche tous les types dont la propriété .Name correspond
                var resultats = assembly.GetTypes()
                                        .Where(t => t.Name.Equals(nomClasse, StringComparison.OrdinalIgnoreCase))
                                        .ToList();

                if (resultats.Count == 0)
                {
                    throw new TypeLoadException($"Aucune classe nommée '{nomClasse}' n'a été trouvée.");
                }
                else if (resultats.Count > 1)
                {
                    // Cas rare : deux namespaces différents contiennent la même classe (ex: Reseau.Server et UI.Server)
                    throw new AmbiguousMatchException($"Plusieurs classes portent le nom '{nomClasse}'. Veuillez utiliser le namespace complet.");
                }

                type = resultats[0];
            }

            // 3. Instanciation
            return (T)Activator.CreateInstance(type);
        }
    }
}
