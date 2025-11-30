    using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Config
{
    /// <summary>
    /// Collection d'éléments <miniSite>
    /// </summary>
    public class MiniSiteCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MiniSiteConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MiniSiteConfigElement)element).ID;
        }

        public MiniSiteConfigElement this[int index]
        {
            get { return (MiniSiteConfigElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null) BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public new MiniSiteConfigElement this[string id]
        {
            get { return (MiniSiteConfigElement)BaseGet(id); }
        }

        public void Add(MiniSiteConfigElement element)
        {
            BaseAdd(element);
        }

        public void Remove(string id)
        {
            BaseRemove(id);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}
