using System.Configuration;
using Tools.Configuration;

namespace AppPublication.Config.MiniSite
{
    /// <summary>
    /// Collection d'éléments <miniSite>
    /// </summary>
    public class MiniSiteCollection : ConfigCollectionBase<MiniSiteConfigSection, MiniSiteConfigElement>
    {
        protected override object GetElementKey(MiniSiteConfigElement element)
        {
            return element.ID;
        }
    }
}
