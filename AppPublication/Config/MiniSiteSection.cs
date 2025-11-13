using System.Configuration;
using Tools.Enum;

namespace AppPublication.Config
{
    /// <summary>
    /// Définit la section de configuration personnalisée <MiniSiteSection> dans app.config.
    /// </summary>
    public class MiniSiteSection : ConfigurationSection
    {
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
            return new MiniSiteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            // Utilise 'id' comme clé unique
            return ((MiniSiteElement)element).ID;
        }

        public MiniSiteElement this[int index]
        {
            get { return (MiniSiteElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new MiniSiteElement this[string name]
        {
            get { return (MiniSiteElement)BaseGet(name); }
        }

        public void Add(MiniSiteElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(MiniSiteElement element)
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
    public class MiniSiteElement : ConfigurationElement
    {
        // Constantes pour les attributs XML
        private const string kId = "id";
        private const string kUser = "user";
        private const string kPassword = "password";
        private const string kUrl = "url";
        private const string kSite = "site";
        private const string kIsActif = "isActif";

        [ConfigurationProperty(kId, IsRequired = true, IsKey = true)]
        public string ID
        {
            get { return (string)this[kId]; }
            set { this[kId] = value; }
        }

        [ConfigurationProperty(kUser, IsRequired = false, DefaultValue = "")]
        public string User
        {
            get { return (string)this[kUser]; }
            set { this[kUser] = value; }
        }

        [ConfigurationProperty(kPassword, IsRequired = false, DefaultValue = "")]
        public string Password
        {
            get { return (string)this[kPassword]; }
            set { this[kPassword] = value; }
        }

        [ConfigurationProperty(kUrl, IsRequired = false, DefaultValue = "")]
        public string Url
        {
            get { return (string)this[kUrl]; }
            set { this[kUrl] = value; }
        }

        [ConfigurationProperty(kSite, IsRequired = false, DefaultValue = SiteEnum.Aucun)]
        public SiteEnum Site
        {
            get { return (SiteEnum)this[kSite]; }
            set { this[kSite] = value; }
        }

        [ConfigurationProperty(kIsActif, IsRequired = false, DefaultValue = false)]
        public bool IsActif
        {
            get { return (bool)this[kIsActif]; }
            set { this[kIsActif] = value; }
        }
    }
}