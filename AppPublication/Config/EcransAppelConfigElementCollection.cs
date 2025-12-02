using System.Configuration;
using System.Linq;

namespace AppPublication.Config
{
    /// <summary>
    /// Collection d'éléments de configuration pour les écrans.
    /// </summary>
    [ConfigurationCollection(typeof(EcransAppelConfigElement), AddItemName = "ecran", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class EcransAppelConfigElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EcransAppelConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EcransAppelConfigElement)element).Id;
        }

        public void Add(EcransAppelConfigElement element)
        {
            BaseAdd(element);
        }

        public void Remove(EcransAppelConfigElement element)
        {
            BaseRemove(element.Id);
        }

        public void Remove(int id)
        {
            BaseRemove(id);
        }

        public new int Count
        {
            get { return base.Count; }
        }

        public EcransAppelConfigElement this[int index]
        {
            get { return (EcransAppelConfigElement)BaseGet(index); }
        }
        public EcransAppelConfigElement GetElementById(int id)
        {
            return (EcransAppelConfigElement) this.Cast<EcransAppelConfigElement>().FirstOrDefault(e => e.Id == id);
        }
    }
}