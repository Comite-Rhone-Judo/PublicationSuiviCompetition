using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Tools.Outils
{
    public static class ResourcesTools
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static Assembly appAssembly = Assembly.GetEntryAssembly();

        /// <summary>
        /// Renvoit le nom de l'assembly courrante
        /// </summary>
        /// <returns></returns>
        public static string GetAssembyName()
        {
            return assembly.GetName().Name;
        }

        /// <summary>
        /// Renvoit une resource de l'assembly sous forme de Stream
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Stream GetAssembyResource(string name, bool useApp = false)
        {
            return (useApp) ? appAssembly.GetManifestResourceStream(name) : assembly.GetManifestResourceStream(name);
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
        public static string[] GetAssembyResourceName()
        {
            return assembly.GetManifestResourceNames();
        }
    }
}
