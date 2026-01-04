using Tools.Configuration;

namespace AppPublication.Config.Publication
{
    /// <summary>
    /// Collection d'éléments <miniSite>
    /// </summary>
    public class MiniSiteCollection : ConfigCollectionBase<PublicationConfigSection, MiniSiteConfigElement>
    {
        protected override object GetElementKey(MiniSiteConfigElement element)
        {
            return element.ID;
        }
    }
}
