using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tools.Logging;
using Tools.Security;

namespace Tools.Configuration
{
    /// <summary>
    /// Classe utilitaire pour la gestion des paramètres d'application dans le fichier de configuration.
    /// plutot recommandé d'utiliser ConfigurationService et des ConfigSectionBase dédiées.
    /// </summary>
    public class AppSettings
    {
        #region ECRITURE
        /// <param name="value">Valeur du parametre</param>
        public static void SaveSetting(string key, string value, string prefix = "")
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string internalKey = (string.IsNullOrEmpty(prefix) ? key : prefix + "_" + key);

            var entry = config.AppSettings.Settings[internalKey];
            if (entry == null)
            {
                config.AppSettings.Settings.Add(internalKey, value);
            }
            else
            {
                config.AppSettings.Settings[internalKey].Value = value;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Sauvegarde un parametre dans le fichier de configuration de l'application en l'encryptant
        /// </summary>
        /// <param name="key">Nom du parametere</param>
        /// <param name="value">Valeur du parametre</param>
        public static void SaveEncryptedSetting(string key, string value, string prefix = "")
        {
            string encryptedValue = Encryption.EncryptString( Encryption.ToSecureString(value));
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string internalKey = (string.IsNullOrEmpty(prefix) ? key : prefix + "_" + key);

            var entry = config.AppSettings.Settings[internalKey];
            if (entry == null)
            {
                config.AppSettings.Settings.Add(internalKey, encryptedValue);
            }
            else
            {
                config.AppSettings.Settings[internalKey].Value = encryptedValue;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        #endregion

        #region LECTURE NATIVE
        /// <summary>
        /// Lit la valeur native d'un parametre depuis le fichier de configuration de l'application
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <returns>La valeur native du parametre, null si absent</returns>
        public static string ReadRawSetting(string key, string prefix = null)
        {
            string output = null;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string internalKey = (string.IsNullOrEmpty(prefix) ? key : prefix + "_" + key);

            var entry = config.AppSettings.Settings[internalKey];
            if (entry != null)
            {
                output = config.AppSettings.Settings[internalKey].Value;
            }

            return output;
        }

        /// <summary>
        /// Lit la valeur native d'un parametre depuis le fichier de configuration de l'application
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <returns>La valeur native du parametre, null si absent</returns>
        public static string ReadRawEncryptedSetting(string key, string prefix = null)
        {
            string output = null;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string internalKey = (string.IsNullOrEmpty(prefix) ? key : prefix + "_" + key);

            var entry = config.AppSettings.Settings[internalKey];
            if (entry != null)
            {
                output = config.AppSettings.Settings[internalKey].Value;
            }

            return (output == null) ? null : Encryption.ToInsecureString(Encryption.DecryptString(output));
        }

        /// <summary>
        /// Lit la valeur d'un parametre devant se trouver dans une liste
        /// </summary>
        /// <typeparam name="T">Type du parametre</typeparam>
        /// <param name="key">Nom du parametre</param>
        /// <param name="sourceList">Liste de valeur dans laquelle doit se trouver le parametre</param>
        /// <param name="predicate">Critere pour identifier une valeur dans la liste</param>
        /// <returns>La valeur du parametre</returns>
        public static T ReadRawSetting<T>(string key, IEnumerable<T> sourceList, Func<T, string> predicate, string prefix = "") where T : class
        {
            T output = null;

            // Si la liste est vide, on ne peut rien faire
            if (sourceList != null && sourceList.Count() > 0)
            {
                // lecture de la valeur dans le fichier de configuration
                string valCache = AppSettings.ReadRawSetting(key, prefix);

                output = AppSettings.FindSetting<T>(valCache, sourceList, predicate);
            }

            return output;
        }

        /// <summary>
        /// Recherche une valeur de parametre dans une liste predefinie. Retourne le 1er de la liste si la valeur du parametre n'existe pas dans la liste
        /// </summary>
        /// <typeparam name="T">Type du parametre</typeparam>
        /// <param name="value">Valeur du parametre</param>
        /// <param name="sourceList">Liste source</param>
        /// <param name="predicate">Fonction permettant de mapper T >> String </param>
        /// <returns></returns>
        public static T FindSetting<T>(string value, IEnumerable<T> sourceList, Func<T, string> predicate) where T : class
        {
            T output = null;

            // Si la liste est vide, on ne peut rien faire
            if (sourceList != null && sourceList.Count() > 0)
            {
                try
                {
                    // Cherche si la valeur lue existe dans la liste source
                    output = sourceList.Where(o => predicate(o) == value).First();
                }
                catch (Exception ex)
                {
                    // la valeur lue n'existe pas dans la liste source, on prend la valeur par defaut (1er element)
                    output = sourceList.First();
                    LogTools.Debug(ex);
                }
            }

            return output;
        }

        #endregion

        #region LECTURE avec valeur par defaut

        /// <summary>
        /// Lit la valeur d'un parametre booleen
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <returns>Valeur du parametre</returns>
        public static bool ReadSetting(string key, bool defaultValue, string prefix = null)
        {            
            string valCache = AppSettings.ReadRawSetting(key, prefix);

            bool val = defaultValue;
            bool.TryParse(valCache, out val);
            return (valCache == null) ? defaultValue : val;
        }


        /// <summary>
        /// Lit la valeur d'un parametre string
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <returns>Valeur du parametre</returns>
        public static string ReadSetting(string key, string defaultValue, string prefix = null)
        {
            string valCache = AppSettings.ReadRawSetting(key, prefix);
            return (valCache == null) ? defaultValue : valCache;
        }

        /// <summary>
        /// Lit la valeur d'un parametre string Encrypte
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <returns>Valeur du parametre</returns>
        public static string ReadEncryptedSetting(string key, string defaultValue, string prefix = null)
        {
            string valCache = AppSettings.ReadRawEncryptedSetting(key, prefix);
            return (valCache == null) ? defaultValue : valCache;
        }

        /// <summary>
        /// Lit la valeur d'un parametre int
        /// </summary>
        /// <param name="key">Nom du parametre</param>
        /// <param name="defaultValue">Valeur par defaut si le parametre est absent</param>
        /// <param name="prefix">Prefix du nom du parametre</param>
        /// <returns>Valeur du parametre</returns>
        public static int ReadSetting(string key, int defaultValue, string prefix = null)
        {
            string valCache = AppSettings.ReadRawSetting(key, prefix);

            int val = defaultValue;
            int.TryParse(valCache, out val);
            return (valCache == null) ? defaultValue : val;
        }

        #endregion

    }
}
