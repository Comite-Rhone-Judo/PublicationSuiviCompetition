using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Net;

namespace FranceJudo.Core.Reflection

{
    public static class AssemblyResourceHelper
    {
        private static readonly Assembly appAssembly = Assembly.GetEntryAssembly();

        /// <summary>
        /// Renvoit une resource de l'assembly sous forme de Stream
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Stream GetAssembyResource(string name, bool useApp = false)
        {
            Assembly assembly = Assembly.GetCallingAssembly();  // On utilise le calling assembly car on n'est pas forcément dans la meme Lib
            Stream output = null;
            output = (useApp) ? appAssembly.GetManifestResourceStream(name) : assembly.GetManifestResourceStream(name);

            return  output;
        }

        /// <summary>
        /// Renvoit la premiere resource de l'assembly dont le nom contient contain
        /// </summary>
        /// <param name="contain"></param>
        /// <returns></returns>
        public static Stream SearchAssemblyResource(string contain)
        {
            Stream output = null;
            string[] resList = GetAssembyResourceName();

            List<string> filtered = resList.Where(o => o.Contains(contain)).ToList();

            if(filtered.Count > 0)
            {
                output = GetAssembyResource(filtered.First());
            }

            return output;
        }

        /// <summary>
        /// Renvoit les noms de toutes les resources disponible dans l'assembly
        /// </summary>
        /// <returns></returns>
        public static string[] GetAssembyResourceName(Assembly targetAssembly = null)
        {
            Assembly assembly = Assembly.GetCallingAssembly();  // On utilise le calling assembly car on n'est pas forcément dans la meme Lib

            // Si targetAssembly n'est pas spécifié, on utilise l'assembly local par défaut
            Assembly asm = targetAssembly ?? assembly;
            return asm.GetManifestResourceNames();
        }
    }
}
