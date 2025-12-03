using System;
using System.Configuration;
using Tools.Outils;

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

        // Propriété technique pour forcer le "Dirty" depuis les enfants
        private const string kLastModifiedTick = "lastModifiedTick";

        protected MiniSiteConfigSection() : base() { }

        #region Collection

        [ConfigurationProperty(kCollectionName)]
        [ConfigurationCollection(typeof(MiniSiteCollection), AddItemName = "miniSite")]
        public MiniSiteCollection MiniSites
        {
            get { return (MiniSiteCollection)base[kCollectionName]; }
        }

        #endregion

        #region Mécanique de Notification

        /// <summary>
        /// Propriété cachée utilisée pour déclencher l'événement de modification
        /// depuis les éléments enfants.
        /// </summary>
        [ConfigurationProperty(kLastModifiedTick, DefaultValue = 0L)]
        internal long LastModifiedTick
        {
            // On utilise l'accès direct de ConfigSectionBase ici car on est dans la racine
            get { return (long)this[kLastModifiedTick]; }
            set { SetValueAndMarkDirty(kLastModifiedTick, value); }
        }

        /// <summary>
        /// Méthode appelée par les enfants (MiniSiteConfigElement) quand ils changent.
        /// </summary>
        internal void NotifyChildModification()
        {
            this.LastModifiedTick = DateTime.Now.Ticks;
        }

        #endregion
    }
}