using FranceJudo.Core.Configuration.Json;
using FranceJudo.Core.Security;
using Newtonsoft.Json;

namespace AppPublication.Config.Publication
{
    public class MiniSiteParams : JsonConfigElement
    {
        private string _id = string.Empty;
        private bool _local = false;

        // --- Propriétés FTP (Distant) ---
        private string _ftpLogin = string.Empty;
        private string _ftpPassword = string.Empty;
        private string _ftpSite = string.Empty;
        private bool _ftpModeActif = false;
        private bool _syncDiff = true; // Oublié précédemment

        // --- Propriétés Serveur HTTP (Local) ---
        private int _portMin = 8080;
        private int _portMax = 8085;
        private string _httpServer = "ServeurHttpBase";
        private string _httpModules = string.Empty;

        public string ID { get => _id; set => SetValue(ref _id, value); }
        public bool Local { get => _local; set => SetValue(ref _local, value); }

        #region CONFIGURATION FTP (Sites Distants)

        public string FtpLogin { get => _ftpLogin; set => SetValue(ref _ftpLogin, value); }
        public string FtpSite { get => _ftpSite; set => SetValue(ref _ftpSite, value); }
        public bool FtpModeActif { get => _ftpModeActif; set => SetValue(ref _ftpModeActif, value); }
        public bool SyncDiff { get => _syncDiff; set => SetValue(ref _syncDiff, value); }

        private string _interfaceLocalPublication = string.Empty;
        public string InterfaceLocalPublication { get => _interfaceLocalPublication; set => SetValue(ref _interfaceLocalPublication, value); }

        [JsonConverter(typeof(EncryptedStringConverter))]
        public string FtpPassword
        {
            get => _ftpPassword;
            set => SetValue(ref _ftpPassword, value);
        }

        #endregion

        #region CONFIGURATION HTTP (Sites Locaux)

        public int PortMin { get => _portMin; set => SetValue(ref _portMin, value); }

        public int PortMax { get => _portMax; set => SetValue(ref _portMax, value); }

        public string HttpServer { get => _httpServer; set => SetValue(ref _httpServer, value); }

        public string HttpModules { get => _httpModules; set => SetValue(ref _httpModules, value); }

        #endregion
    }
}