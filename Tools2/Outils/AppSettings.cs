using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace Tools.Outils
{
    public class AppSettings
    {
        public static void SaveSettings(string key, string value)
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

        public static string ReadSettings(string key)
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
    }
}
