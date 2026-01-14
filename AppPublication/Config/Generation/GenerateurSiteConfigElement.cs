
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

    public class GenerateurSiteConfigElement : ConfigElementBase<GenerationConfigSection>
    {
        #region CONSTANTES

        // Nom des clefs de configuration
        private const string kDelaiActualisationClientSec = "delaiActualisationClientSec";
        private const string kTailleMaxPouleColonnes = "tailleMaxPouleColonnes";
        private const string kPouleEnColonnes = "pouleEnColonnes";
        private const string kPouleToujoursEnColonnes = "pouleToujoursEnColonnes";
        private const string kPublierProchainsCombats = "publierProchainsCombats";
        private const string kNbProchainsCombats = "nbProchainsCombats";
        private const string kMsgProchainsCombats = "msgProchainsCombats";
        private const string kPublierAffectationTapis = "publierAffectationTapis";
        private const string kPublierEngagements = "publierEngagements";
        private const string kEngagementsAbsents = "engagementsAbsents";
        private const string kEngagementsTousCombats = "engagementsTousCombats";
        private const string kUseIntituleCommun = "useIntituleCommun";
        private const string kIntituleCommun = "intituleCommun";
        private const string kScoreEngagesGagnantPerdant = "scoreEngagesGagnantPerdant";
        private const string kAfficherPositionCombat = "afficherPositionCombat";

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

        [ConfigurationProperty(kDelaiActualisationClientSec, IsRequired = false)]
        public int DelaiActualisationClientSec
        {
            get { return GetConfigValue<int>(kDelaiActualisationClientSec, 30); }
            set { SetValueAndMarkDirty(kDelaiActualisationClientSec, value); }
        }

        [ConfigurationProperty(kTailleMaxPouleColonnes, IsRequired = false)]
        public int TailleMaxPouleColonnes
        {
            get { return GetConfigValue<int>(kTailleMaxPouleColonnes, 5); }
            set { SetValueAndMarkDirty(kTailleMaxPouleColonnes, value); }
        }

        [ConfigurationProperty(kPouleEnColonnes, IsRequired = false)]
        public bool PouleEnColonnes
        {
            get { return GetConfigValue<bool>(kPouleEnColonnes, false); }
            set { SetValueAndMarkDirty(kPouleEnColonnes, value); }
        }

        [ConfigurationProperty(kPouleToujoursEnColonnes, IsRequired = false)]
        public bool PouleToujoursEnColonnes
        {
            get { return GetConfigValue<bool>(kPouleToujoursEnColonnes, false); }
            set { SetValueAndMarkDirty(kPouleToujoursEnColonnes, value); }
        }

        [ConfigurationProperty(kPublierProchainsCombats, IsRequired = false)]
        public bool PublierProchainsCombats
        {
            get { return GetConfigValue<bool>(kPublierProchainsCombats, false); }
            set { SetValueAndMarkDirty(kPublierProchainsCombats, value); }
        }

        [ConfigurationProperty(kNbProchainsCombats, IsRequired = false)]
        public int NbProchainsCombats
        {
            get { return GetConfigValue<int>(kNbProchainsCombats, 6); }
            set { SetValueAndMarkDirty(kNbProchainsCombats, value); }
        }

        [ConfigurationProperty(kMsgProchainsCombats, IsRequired = false)]
        public string MsgProchainsCombats
        {
            get { return GetConfigValue<string>(kMsgProchainsCombats, String.Empty); }
            set { SetValueAndMarkDirty(kMsgProchainsCombats, value); }
        }

        [ConfigurationProperty(kPublierAffectationTapis, IsRequired = false)]
        public bool PublierAffectationTapis
        {
            get { return GetConfigValue<bool>(kPublierAffectationTapis, true); }
            set { SetValueAndMarkDirty(kPublierAffectationTapis, value); }
        }

        [ConfigurationProperty(kPublierEngagements, IsRequired = false)]
        public bool PublierEngagements
        {
            get { return GetConfigValue<bool>(kPublierEngagements, true); }
            set { SetValueAndMarkDirty(kPublierEngagements, value); }
        }

        [ConfigurationProperty(kEngagementsAbsents, IsRequired = false)]
        public bool EngagementsAbsents
        {
            get { return GetConfigValue<bool>(kEngagementsAbsents, false); }
            set { SetValueAndMarkDirty(kEngagementsAbsents, value); }
        }

        [ConfigurationProperty(kEngagementsTousCombats, IsRequired = false)]
        public bool EngagementsTousCombats
        {
            get { return GetConfigValue<bool>(kEngagementsTousCombats, false); }
            set { SetValueAndMarkDirty(kEngagementsTousCombats, value); }
        }

        [ConfigurationProperty(kUseIntituleCommun, IsRequired = false)]
        public bool UseIntituleCommun
        {
            get { return GetConfigValue<bool>(kUseIntituleCommun, false); }
            set { SetValueAndMarkDirty(kUseIntituleCommun, value); }
        }

        [ConfigurationProperty(kIntituleCommun, IsRequired = false)]
        public string IntituleCommun
        {
            get { return GetConfigValue<string>(kIntituleCommun, string.Empty); }
            set { SetValueAndMarkDirty(kIntituleCommun, value); }
        }

        [ConfigurationProperty(kScoreEngagesGagnantPerdant, IsRequired = false)]
        public bool ScoreEngagesGagnantPerdant
        {
            get { return GetConfigValue<bool>(kScoreEngagesGagnantPerdant, false); }
            set { SetValueAndMarkDirty(kScoreEngagesGagnantPerdant, value); }
        }

        [ConfigurationProperty(kAfficherPositionCombat, IsRequired = false)]
        public bool AfficherPositionCombat
        {
            get { return GetConfigValue<bool>(kAfficherPositionCombat, true); }
            set { SetValueAndMarkDirty(kAfficherPositionCombat, value); }
        }
        #endregion
    }
}