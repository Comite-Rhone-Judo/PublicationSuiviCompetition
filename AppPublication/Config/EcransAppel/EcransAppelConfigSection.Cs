using System;
using System.Configuration;
using Tools.Configuration;

namespace AppPublication.Config.EcransAppel
{
    /// <summary>
    /// Section de configuration pour les écrans d'appel.
    /// Singleton géré par ConfigSectionBase.
    /// </summary>
    [SectionName("EcransAppelSection")]
    public class EcransAppelConfigSection : ConfigSectionBase<EcransAppelConfigSection>
    {
        private const string kEcrans = "ecrans";

        // Constructeur privé pour le singleton
        protected EcransAppelConfigSection() : base() { }

        [ConfigurationProperty(kEcrans, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(EcransAppelConfigElementCollection), AddItemName = "ecran")]
        public EcransAppelConfigElementCollection Ecrans
        {
            get { return (EcransAppelConfigElementCollection)this[kEcrans]; }
            set { this[kEcrans] = value; }
        }
    }
}