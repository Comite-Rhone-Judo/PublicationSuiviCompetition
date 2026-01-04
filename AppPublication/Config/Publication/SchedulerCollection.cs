using Tools.Configuration;

namespace AppPublication.Config.Publication
{
    /// <summary>
    /// Collection d'éléments <miniSite>
    /// </summary>
    public class SchedulerCollection : ConfigCollectionBase<PublicationConfigSection, SchedulerConfigElement>
    {
        protected override object GetElementKey(SchedulerConfigElement element)
        {
            return element.ID;
        }
    }
}
