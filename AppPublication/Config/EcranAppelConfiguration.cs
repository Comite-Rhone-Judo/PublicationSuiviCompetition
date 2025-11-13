using System;
using System.Configuration;
using System.Linq;

namespace AppPublication.Config
{
    /// <summary>
    /// Définit la section de configuration personnalisée <EcransAppelSection> dans app.config.
    /// </summary>
    public class EcransAppelSection : ConfigurationSection
    {
        // Constantes pour les noms XML
        public const string kConfigSectionName = "EcransAppelSection";
        private const string kCollectionName = "ecrans";
        private const string kElementName = "ecran";

        [ConfigurationProperty(kCollectionName)]
        [ConfigurationCollection(typeof(EcranAppelCollection), AddItemName = kElementName)]
        public EcranAppelCollection Ecrans
        {
            get { return (EcranAppelCollection)base[kCollectionName]; }
        }
    }

    /// <summary>
    /// Définit la collection d'éléments <ecran>
    /// </summary>
    public class EcranAppelCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EcranAppelElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            // Utilise le 'numero' comme clé unique pour chaque élément
            return ((EcranAppelElement)element).Numero;
        }

        public EcranAppelElement this[int index]
        {
            get { return (EcranAppelElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(EcranAppelElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(EcranAppelElement element)
        {
            BaseRemove(GetElementKey(element));
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
    }

    /// <summary>
    /// Définit un élément <ecran> avec ses attributs.
    /// </summary>
    public class EcranAppelElement : ConfigurationElement
    {
        // Constantes pour les attributs XML
        private const string kNumero = "numero";
        private const string kHostName = "hostName";
        private const string kIpAddress = "ipAddress";
        private const string kTapis = "tapis";

        [ConfigurationProperty(kNumero, IsRequired = true, IsKey = true)]
        public int Numero
        {
            get { return (int)this[kNumero]; }
            set { this[kNumero] = value; }
        }

        [ConfigurationProperty(kHostName, IsRequired = false)]
        public string HostName
        {
            get { return (string)this[kHostName]; }
            set { this[kHostName] = value; }
        }

        [ConfigurationProperty(kIpAddress, IsRequired = false)]
        public string IPAddress
        {
            get { return (string)this[kIpAddress]; }
            set { this[kIpAddress] = value; }
        }

        // Les listes sont stockées sous forme de chaîne séparée par des virgules
        [ConfigurationProperty(kTapis, IsRequired = false, DefaultValue = "")]
        public string Tapis
        {
            get { return (string)this[kTapis]; }
            set { this[kTapis] = value; }
        }
    }
}