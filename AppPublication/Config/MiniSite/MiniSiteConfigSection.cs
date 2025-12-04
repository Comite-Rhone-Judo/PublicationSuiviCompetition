using System;
using System.Configuration;
using Tools.Configuration;

namespace AppPublication.Config.MiniSite
{
    /// <summary>
    /// Section de configuration pour la gestion des paramètres techniques des MiniSites.
    /// </summary>
    [SectionName(MiniSiteConfigSection.kConfigSectionName)]
    public class MiniSiteConfigSection : ConfigSectionBase<MiniSiteConfigSection>
    {
        public const string kConfigSectionName = "MiniSiteSection";
        private const string kCollectionName = "miniSites";

        protected MiniSiteConfigSection() : base() { }

        #region Collection

        [ConfigurationProperty(kCollectionName)]
        [ConfigurationCollection(typeof(MiniSiteCollection), AddItemName = "miniSite")]
        public MiniSiteCollection MiniSites
        {
            get { return (MiniSiteCollection)base[kCollectionName]; }
        }

        #endregion
    }
}