using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;

namespace Tools.Outils
{
    public class AppSettings
    {
        /// <summary>
        /// Sauvegarde un parametre dans le fichier de configuration de l'application
        /// </summary>
        /// <param name="key">Nom du parametere</param>
        /// <param name="value">Valeur du parametre</param>
        public static void SaveSetting(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[key];
            if (entry == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Sauvegarde un parametre dans le fichier de configuration de l'application en l'encryptant
        /// </summary>
        /// <param name="key">Nom du parametere</param>
        /// <param name="value">Valeur du parametre</param>
        public static void SaveEncryptedSetting(string key, string value)
        {
            string encryptedValue = Encryption.EncryptString( Encryption.ToSecureString(value));
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[key];
            if (entry == null)
            {
                config.AppSettings.Settings.Add(key, encryptedValue);
            }
            else
            {
                config.AppSettings.Settings[key].Value = encryptedValue;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Lit la valeur native d'un parametre depuis le fichier de configuration de l'application
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <returns>La valeur native du parametre, null si absent</returns>
        public static string ReadSetting(string key)
        {
            string output = null;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[key];
            if (entry != null)
            {
                output = config.AppSettings.Settings[key].Value;
            }

            return output;
        }

        /// <summary>
        /// Lit la valeur native d'un parametre depuis le fichier de configuration de l'application
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <returns>La valeur native du parametre, null si absent</returns>
        public static string ReadEncryptedSetting(string key)
        {
            string output = null;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[key];
            if (entry != null)
            {
                output = config.AppSettings.Settings[key].Value;
            }

            return (output == null) ? null : Encryption.ToInsecureString(Encryption.DecryptString(output));
        }

        /// <summary>
        /// Lit la valeur d'un paramtre booleen
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <returns>Valeur du parametre</returns>
        public static bool ReadSetting(string key, bool defaultValue)
        {            
            string valCache = AppSettings.ReadSetting(key);

            bool val = defaultValue;
            bool.TryParse(valCache, out val);
            return (valCache == null) ? defaultValue : val;
        }

        /// <summary>
        /// Lit la valeur d'un paramtre string
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <returns>Valeur du parametre</returns>
        public static string ReadSetting(string key, string defaultValue)
        {
            string valCache = AppSettings.ReadSetting(key);
            return (valCache == null) ? defaultValue : valCache;
        }

        /// <summary>
        /// Lit la valeur d'un parametre string Encrypte
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <returns>Valeur du parametre</returns>
        public static string ReadEncryptedSetting(string key, string defaultValue)
        {
            string valCache = AppSettings.ReadEncryptedSetting(key);
            return (valCache == null) ? defaultValue : valCache;
        }

        /// <summary>
        /// Lit la valeur d'un paramtre int
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <returns>Valeur du parametre</returns>
        public static int ReadSetting(string key, int defaultValue)
        {
            string valCache = AppSettings.ReadSetting(key);

            int val = defaultValue;
            int.TryParse(valCache, out val);
            return (valCache == null) ? defaultValue : val;
        }

        /// <summary>
        /// Lit la valeur d'un parametre devant se trouver dans une liste
        /// </summary>
        /// <typeparam name="T">Type du parametre</typeparam>
        /// <param name="key">Nom du parametre</param>
        /// <param name="sourceList">Liste de valeur dans laquelle doit se trouver le parametre</param>
        /// <param name="predicate">Critere pour identifier une valeur dans la liste</param>
        /// <returns>La valeur du parametre</returns>
        public static T ReadSetting<T>(string key, IEnumerable<T> sourceList, Func<T, string> predicate) where T : class
        {
            T output = null;
            
            // Si la liste est vide, on ne peut rien faire
            if (sourceList != null && sourceList.Count() > 0)
            {
                // lecture de la valeur dans le fichier de configuration
                string valCache = AppSettings.ReadSetting(key);
               
                try
                {
                    // Cherche si la valeur lue existe dans la liste source
                    output = sourceList.Where(o => predicate(o) == valCache).First();
                }
                catch (Exception ex)
                {
                    // la valeur lue n'existe pas dans la liste source, on prend la valeur par defaut (1er element)
                    output = sourceList.First();
                    LogTools.Log(ex);
                }
            }

            return output;
        }
    }
}
