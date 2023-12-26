using System.IO;
using System.Reflection;

namespace Tools.Outils
{
    public static class ResourcesTools
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();

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
        public static Stream GetAssembyResource(string name)
        {
            return assembly.GetManifestResourceStream(name);
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
