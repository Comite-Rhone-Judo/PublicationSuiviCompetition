using System.Configuration;
using System.Windows.Automation;
using Tools.Configuration;
using Tools.Security;

namespace AppPublication.Config.Publication
{
    /// <summary>
    /// Détail technique d'un accès FTP ou Local.
    /// Hérite maintenant de ConfigElementBase pour la factorisation du code.
    /// </summary>
    public class MiniSiteConfigElement : ConfigElementBase<PublicationConfigSection>
    {
        // Constantes pour les attributs XML
        private const string kId = "id";
        private const string kTypeLocal = "local";
        private const string kHttpModules = "httpModules";
        private const string kHttpServer = "httpServer";
        private const string kInterfaceLocalPublication = "interface";
        private const string kSynchroniseDifferences = "syncDiff";
        private const string kFtpLogin = "ftpLogin";
        private const string kFtpPassword = "ftpPassword";
        private const string kFtpSite = "ftpSite";
        private const string kFtpModeActif = "modeActif";

        /// <summary>
        /// Implémentation du lien avec le parent Singleton
        /// </summary>
        protected override void NotifyParentOfModification()
        {
            if (PublicationConfigSection.Instance != null)
            {
                PublicationConfigSection.Instance.NotifyChildModification();
            }
        }

        [ConfigurationProperty(kId, IsRequired = true, IsKey = true)]
        public string ID
        {
            get { return GetConfigValue<string>(kId, string.Empty); }
            set { SetValueAndMarkDirty(kId, value); }
        }

        /// <summary>
        /// Type du minisite
        /// </summary>
        [ConfigurationProperty(kTypeLocal, DefaultValue = true)]
        public bool TypeLocal
        {
            get { return GetConfigValue<bool>(kTypeLocal, false); }
            set { SetValueAndMarkDirty(kTypeLocal, value); }
        }

        [ConfigurationProperty(kHttpModules, IsRequired = false, DefaultValue = "")]
        public string HttpModules
        {
            get { return GetConfigValue<string>(kHttpModules, ""); }
            set { SetValueAndMarkDirty(kHttpModules, value); }
        }

        [ConfigurationProperty(kHttpServer, IsRequired = false, DefaultValue = "ServeurHttpBase")]
        public string HttpServer
        {
            get { return GetConfigValue<string>(kHttpServer, ""); }
            set { SetValueAndMarkDirty(kHttpServer, value); }
        }

        /// <summary>
        /// Adresse IP à utiliser pour la publication locale.
        /// </summary>
        [ConfigurationProperty(kInterfaceLocalPublication, DefaultValue = "")]
        public string InterfaceLocalPublication
        {
            get { return GetConfigValue<string>(kInterfaceLocalPublication, string.Empty); }
            set { SetValueAndMarkDirty(kInterfaceLocalPublication, value); }
        }

        /// <summary>
        /// Option technique spécifique à ce site : n'envoyer que les fichiers modifiés.
        /// </summary>
        [ConfigurationProperty(kSynchroniseDifferences, DefaultValue = false)]
        public bool SynchroniseDifferences
        {
            get { return GetConfigValue<bool>(kSynchroniseDifferences, false); }
            set { SetValueAndMarkDirty(kSynchroniseDifferences, value); }
        }

        [ConfigurationProperty(kFtpLogin, IsRequired = false, DefaultValue = "")]
        public string FtpLogin
        {
            get { return GetConfigValue<string>(kFtpLogin, string.Empty); }
            set { SetValueAndMarkDirty(kFtpLogin, value); }
        }

        [ConfigurationProperty(kFtpPassword, IsRequired = false, DefaultValue = "")]
        public string FtpPassword
        {
            get
            {
                // Lecture brute (encryptée)
                string rawEncrypted = GetConfigValue<string>(kFtpPassword, string.Empty);
                if (string.IsNullOrEmpty(rawEncrypted)) return string.Empty;
                try
                {
                    // Décryptage à la volée
                    return Encryption.ToInsecureString(Encryption.DecryptString(rawEncrypted));
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                string encryptedValue = string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    // Cryptage à la volée avant de passer à la méthode de base
                    encryptedValue = Encryption.EncryptString(Encryption.ToSecureString(value));
                }
                // Sauvegarde de la version cryptée
                SetValueAndMarkDirty(kFtpPassword, encryptedValue);
            }
        }

        [ConfigurationProperty(kFtpSite, IsRequired = false, DefaultValue = "")]
        public string FtpSite
        {
            get { return GetConfigValue<string>(kFtpSite, string.Empty); }
            set { SetValueAndMarkDirty(kFtpSite, value); }
        }

        [ConfigurationProperty(kFtpModeActif, IsRequired = false, DefaultValue = false)]
        public bool FtpModeActif
        {
            get { return GetConfigValue<bool>(kFtpModeActif, false); }
            set { SetValueAndMarkDirty(kFtpModeActif, value); }
        }
    }
}
