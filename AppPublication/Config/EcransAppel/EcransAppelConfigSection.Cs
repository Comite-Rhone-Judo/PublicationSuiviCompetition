using System;
using System.Configuration;

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
        private const string kLastModifiedTick = "lastModifiedTick";

        // Constructeur privé pour le singleton
        protected EcransAppelConfigSection() : base() { }

        [ConfigurationProperty(kEcrans, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(EcransAppelConfigElementCollection), AddItemName = "ecran")]
        public EcransAppelConfigElementCollection Ecrans
        {
            get { return (EcransAppelConfigElementCollection)this[kEcrans]; }
            set { this[kEcrans] = value; }
        }

        #region Mécanique de Notification

        /// <summary>
        /// Propriété technique utilisée pour marquer la section comme "Dirty" 
        /// lorsque des enfants sont modifiés.
        /// </summary>
        [ConfigurationProperty(kLastModifiedTick, DefaultValue = 0L)]
        internal long LastModifiedTick
        {
            get { return (long)this[kLastModifiedTick]; }
            set { SetValueAndMarkDirty(kLastModifiedTick, value); }
        }

        /// <summary>
        /// Méthode appelée par les enfants (EcranConfigElement) quand ils changent.
        /// Cela met à jour le Tick, ce qui déclenche la sauvegarde via ConfigSectionBase.
        /// </summary>
        internal void NotifyChildModification()
        {
            this.LastModifiedTick = DateTime.Now.Ticks;
        }

        #endregion
    }
}