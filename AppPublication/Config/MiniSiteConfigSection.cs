using System.Configuration;
using Tools.Enum;

namespace AppPublication.Config
{
    /// <summary>
    /// Définit la section de configuration personnalisée <MiniSiteSection> dans app.config.
    /// </summary>
    public class MiniSiteConfigSection : ConfigurationSection
    {
        // TODO Ajouter le mode local/distant, reprendre la liste de tous les parametres nécessaires.

        // Constantes pour les noms XML
        public const string kConfigSectionName = "MiniSiteSection";
        private const string kCollectionName = "miniSites";
        private const string kElementName = "miniSite";

        [ConfigurationProperty(kCollectionName)]
        [ConfigurationCollection(typeof(MiniSiteCollection), AddItemName = kElementName)]
        public MiniSiteCollection MiniSites
        {
            get { return (MiniSiteCollection)base[kCollectionName]; }
        }
    }

    /// <summary>
    /// Définit la collection d'éléments <miniSite>
    /// </summary>
    public class MiniSiteCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MiniSiteConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            // Utilise 'id' comme clé unique
            return ((MiniSiteConfigElement)element).ID;
        }

        public MiniSiteConfigElement this[int index]
        {
            get { return (MiniSiteConfigElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new MiniSiteConfigElement this[string name]
        {
            get { return (MiniSiteConfigElement)BaseGet(name); }
        }

        public void Add(MiniSiteConfigElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(MiniSiteConfigElement element)
        {
            BaseRemove(GetElementKey(element));
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
    }

    /// <summary>
    /// Définit un élément <miniSite> avec ses attributs.
    /// </summary>
    public class MiniSiteConfigElement : ConfigurationElement
    {
        // Constantes pour les attributs XML
        private const string kId = "id";                // Equivalent du prefix dans l'ancienne config
        private const string kFtpLogin = "ftpLogin";            
        private const string kFtpPassword = "ftpPassword";
        private const string kFtpSite = "ftpSite";
        private const string kFtpModeActif = "modeActif";
        private const string ksyncDiff = "syncDiff";


        /*
            kSettingInterfaceLocalPublication   Mode local 
            kSettingSiteFTPDistant              Mode Distant
            kSettingLoginSiteFTPDistant         Mode Distant
            kSettingModeActifFTPDistant         Mode Distant
            kSettingSynchroniseDifferences      Mode Distant
        */

        [ConfigurationProperty(kId, IsRequired = true, IsKey = true)]
        public string ID
        {
            get { return (string)this[kId]; }
            set { this[kId] = value; }
        }

        [ConfigurationProperty(kFtpLogin, IsRequired = false, DefaultValue = "")]
        public string FtpLogin
        {
            get { return (string)this[kFtpLogin]; }
            set { this[kFtpLogin] = value; }
        }

        [ConfigurationProperty(kFtpPassword, IsRequired = false, DefaultValue = "")]
        public string FtpPassword
        {
            // TODO Transferer la logique de cryptage/décryptage ici
            get { return (string)this[kFtpPassword]; }
            set { this[kFtpPassword] = value; }
        }

        [ConfigurationProperty(kFtpSite, IsRequired = false, DefaultValue = "")]
        public string FtpSite
        {
            get { return (string)this[kFtpSite]; }
            set { this[kFtpSite] = value; }
        }

        [ConfigurationProperty(kFtpModeActif, IsRequired = false, DefaultValue = false)]
        public bool FtpModeActif
        {
            get { return (bool)this[kFtpModeActif]; }
            set { this[kFtpModeActif] = value; }
        }
    }
}