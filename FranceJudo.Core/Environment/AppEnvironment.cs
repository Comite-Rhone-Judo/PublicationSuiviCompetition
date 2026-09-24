using System;
using System.Linq;
using System.Reflection;

namespace FranceJudo.Core.Environment
{
    public class AppEnvironment
    {
        /// <summary>
        /// Retourne la version de l'APP
        /// </summary>
        /// <returns>Version</returns>

        public static String GetVersionInformation()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            String version = assembly.GetName().Version?.ToString() ?? "";

            // On cherche la métadonnée "VersionBeta" injectée par Directory.Build.props
            var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
            var betaAttr = metadataAttributes.FirstOrDefault(a => a.Key == "VersionBeta");

            if (betaAttr != null && int.TryParse(betaAttr.Value, out int betaValue) && betaValue > 0)
            {
                version += String.Format("-beta{0:00}", betaValue);
            }

            return version;
        }

        public static string GetCompanyInformation()
        {
            string output = string.Empty;

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;

                output = myAttr.Company;
            }
            catch { }

            return output;
        }

        public static string GetCopyrightInformation()
        {
            string output = string.Empty;

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;

                output = myAttr.Copyright;
            }
            catch { }

            return output;
        }

        public static string GetTrademarkInformation()
        {
            string output = string.Empty;

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyTrademarkAttribute)) as AssemblyTrademarkAttribute;

                output = myAttr.Trademark;
            }
            catch { }

            return output;
        }

        /// <summary>
        /// Répertoire des DATA
        /// </summary>
        /// <returns>PATH</returns>

        public static string GetDataDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.Replace(@"\", "/");
        }

        /// <summary>
        /// Répertoire de l'APP
        /// </summary>
        /// <returns>PATH</returns>

        public static string GetAppDirectory()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory);
            return uri.LocalPath;// AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
