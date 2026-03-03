
using AppPublication.Tools.Files;
using AppPublication.Tools.FranceJudo;
using System;
using System.Collections.Generic;
using System.Configuration;
using Tools.Configuration;

namespace AppPublication.Config.Generation
{
    /// <summary>
    /// Gère les paramètres de publication globaux.
    /// Implémenté en Singleton pour un accès facile et une sauvegarde "live".
    /// Les 'getters' fournissent des valeurs par défaut dynamiques si la configuration est absente.
    /// </summary>

    public class GenerateurSiteInterneConfigElement : ConfigElementBase<GenerationConfigSection>
    {
        #region CONSTANTES

        // Nom des clefs de configuration
        private const string kDelaiDeroulementSec = "delaiDeroulementSec";
        private const string kNbProchainsCombats = "nbProchainsCombats";
        #endregion

        #region METHODES
        /// <summary>
        /// Méthode héritée de ConfigElementBase.
        /// Notifie la section parente qu'une propriété a changé pour déclencher le mécanisme de sauvegarde différée.
        /// </summary>
        protected override void NotifyParentOfModification()
        {
            if (GenerationConfigSection.Instance != null)
            {
                GenerationConfigSection.Instance.NotifyChildModification();
            }
        }
        #endregion

        #region PROPRIETES DE CONFIGURATION

        [ConfigurationProperty(kDelaiDeroulementSec, IsRequired = false)]
        public int DelaiDeroulementSec
        {
            get { return GetConfigValue<int>(kDelaiDeroulementSec, 30); }
            set { SetValueAndMarkDirty(kDelaiDeroulementSec, value); }
        }

        [ConfigurationProperty(kNbProchainsCombats, IsRequired = false)]
        public int NbProchainsCombats
        {
            get { return GetConfigValue<int>(kNbProchainsCombats, 6); }
            set { SetValueAndMarkDirty(kNbProchainsCombats, value); }
        }

        #endregion
    }
}