using System;
using System.Deployment.Application;
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
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                String version = assembly.GetName().Version.ToString();

                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyVersionBeta)) as AssemblyVersionBeta;
                if (myAttr.Value > 0)
                {
                    version += String.Format("-beta{0:00}", myAttr.Value);
                }

                return version; // assembly.GetName().Version;
            }
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
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.DataDirectory + "/";
            }
            else
            {
                return AppDomain.CurrentDomain.BaseDirectory.Replace(@"\", "/");
            }
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
