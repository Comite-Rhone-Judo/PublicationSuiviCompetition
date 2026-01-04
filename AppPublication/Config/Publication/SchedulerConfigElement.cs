using System.Configuration;
using Tools.Configuration;

namespace AppPublication.Config.Publication
{
    /// <summary>
    /// Détail technique d'un accès FTP ou Local.
    /// Hérite maintenant de ConfigElementBase pour la factorisation du code.
    /// </summary>
    public class SchedulerConfigElement : ConfigElementBase<PublicationConfigSection>
    {
        #region CONSTANTES
        // Constantes pour les attributs XML
        private const string kId = "id";
        private const string kDelaiGenerationSec = "delaiGenerationSec";
        #endregion

        #region METHODES INTERNES
        /// <summary>
        /// Implémentation du lien avec le parent Singleton
        /// </summary>
        protected override void NotifyParentOfModification()
        {
            if (PublicationConfigSection.Instance != null)
            {
                PublicationConfigSection.Instance.NotifyChildModification();
            }
        }
        #endregion

        #region PROPRIETES
        [ConfigurationProperty(kId, IsRequired = true, IsKey = true)]
        public string ID
        {
            get { return GetConfigValue<string>(kId, string.Empty); }
            set { SetValueAndMarkDirty(kId, value); }
        }

        [ConfigurationProperty(kDelaiGenerationSec, IsRequired = false)]
        public int DelaiGenerationSec
        {
            get { return GetConfigValue<int>(kDelaiGenerationSec, 30); }
            set { SetValueAndMarkDirty(kDelaiGenerationSec, value); }
        }
        #endregion
    }
}
