using FranceJudo.Core.Configuration;
using System.Configuration;

namespace AppPublication.Config.Generation
{
    /// <summary>
    /// Section de configuration pour la gestion des paramètres techniques des generateurs.
    /// </summary>
    [SectionName(GenerationConfigSection.kConfigSectionName)]
    public class GenerationConfigSection : ConfigSectionBase<GenerationConfigSection>
    {
        #region CONSTANTES
        public const string kConfigSectionName = "GenerationConfigSection";
        private const string kEcranCollectionName = "ecrans";
        private const string kEcranCollectionItemName = "ecran";
        private const string kGenerateurSiteElement = "generateurSite";
        private const string kGenerateurSiteInterneElement = "generateurSiteInterne";
        #endregion

        #region CONSTRUCTEURS
        protected GenerationConfigSection() : base() { }
        #endregion

        #region Collection

        [ConfigurationProperty(kEcranCollectionName, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(EcransAppelConfigElementCollection), AddItemName = kEcranCollectionItemName)]
        public EcransAppelConfigElementCollection Ecrans
        {
            get { return (EcransAppelConfigElementCollection)this[kEcranCollectionName]; }
            set { this[kEcranCollectionName] = value; }
        }

        #endregion

        #region PROPRIETES DE CONFIGURATION

        [ConfigurationProperty(kGenerateurSiteElement, IsRequired = true)]
        public GenerateurSiteConfigElement GenerateurSite
        {
            get { return (GenerateurSiteConfigElement)this[kGenerateurSiteElement]; }
            set { this[kGenerateurSiteElement] = value; }
        }

        [ConfigurationProperty(kGenerateurSiteInterneElement, IsRequired = true)]
        public GenerateurSiteInterneConfigElement GenerateurSiteInterne
        {
            get { return (GenerateurSiteInterneConfigElement)this[kGenerateurSiteInterneElement]; }
            set { this[kGenerateurSiteInterneElement] = value; }
        }

        #endregion
    }
}