using System.Configuration;

namespace AppPublication.Config
{
    /// <summary>
    /// Collection d'éléments de configuration pour les écrans.
    /// </summary>
    [ConfigurationCollection(typeof(EcranConfigElement), AddItemName = "ecran", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class EcranConfigElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EcranConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EcranConfigElement)element).Id;
        }

        public void Add(EcranConfigElement element)
        {
            BaseAdd(element);
        }

        public void Remove(EcranConfigElement element)
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

        public EcranConfigElement this[int index]
        {
            get { return (EcranConfigElement)BaseGet(index); }
        }
    }
}