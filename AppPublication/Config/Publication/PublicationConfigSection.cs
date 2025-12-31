using System;
using System.Collections.Generic;
using System.Configuration;
using AppPublication.Tools.Files;
using AppPublication.Tools.FranceJudo;
using Tools.Configuration;

namespace AppPublication.Config.Publication
{
    // TODO A voir comment on fait si on separe les parametres de generation Site et SitePrivate

    /// <summary>
    /// Gère les paramètres de publication globaux.
    /// Implémenté en Singleton pour un accès facile et une sauvegarde "live".
    /// Les 'getters' fournissent des valeurs par défaut dynamiques si la configuration est absente.
    /// </summary>
    [SectionName(kConfigSectionName)]
    public class PublicationConfigSection : ConfigSectionBase<PublicationConfigSection>
    {
        #region CONSTANTES
        // Nom de la section dans app.config
        public const string kConfigSectionName = "PublicationSection";

        // Nom des clefs de configuration
        private const string kRepertoireRacine = "repertoireRacine";
        private const string kDelaiGenerationSec = "delaiGenerationSec";
        private const string kDelaiActualisationClientSec = "delaiActualisationClientSec";
        private const string kTailleMaxPouleColonnes = "tailleMaxPouleColonnes";
        private const string kPouleEnColonnes = "pouleEnColonnes";
        private const string kPouleToujoursEnColonnes = "pouleToujoursEnColonnes";
        private const string kPublierProchainsCombats = "publierProchainsCombats";
        private const string kNbProchainsCombats = "nbProchainsCombats";
        private const string kMsgProchainsCombats = "msgProchainsCombats";
        private const string kPublierAffectationTapis = "publierAffectationTapis";
        private const string kLogo = "logo";
        private const string kEasyConfig = "easyConfig";
        private const string kURLDistant = "urlDistant";
        private const string kIsolerCompetition = "isolerCompetition";
        private const string kRepertoireRacineSiteFTPDistant = "repertoireRacineSiteFTPDistant";
        private const string kPublierEngagements = "publierEngagements";
        private const string kEngagementsAbsents = "engagementsAbsents";
        private const string kEngagementsTousCombats = "engagementsTousCombats";
        private const string kEffacerAuDemarrage = "effacerAuDemarrage";
        private const string kUseIntituleCommun = "useIntituleCommun";
        private const string kIntituleCommun = "intituleCommun";
        private const string kScoreEngagesGagnantPerdant = "scoreEngagesGagnantPerdant";
        private const string kAfficherPositionCombat = "afficherPositionCombat";
        private const string kNiveauPublicationFFJudo = "NiveauPublicationFFJudo";
        private const string kEntitePublicationFFJudo = "EntitePublicationFFJudo";

        #endregion

        #region CONSTRUCTEURS

        // Constructeur privé pour le Singleton
        protected PublicationConfigSection() : base() {}

        #endregion

        #region PROPRIETES DE CONFIGURATION

        [ConfigurationProperty(kNiveauPublicationFFJudo, IsRequired = false)]
        public string NiveauPublicationFFJudo
        {
            get { return GetConfigValue<string>(kNiveauPublicationFFJudo, string.Empty); }
            set { SetValueAndMarkDirty(kNiveauPublicationFFJudo, value); }
        }

        /// <summary>
        /// Retourne la propriété NiveauPublicationFFJudo si elle est dans la liste des candidats, sinon retourne le 1er de la liste
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public string GetNiveauPublicationFFJudo(IEnumerable<string> candidates, Func<string, string> valueSelector)
        {
            return GetItemFromList(kNiveauPublicationFFJudo, candidates, valueSelector);
        }

        [ConfigurationProperty(kEntitePublicationFFJudo, IsRequired = false)]
        public string EntitePublicationFFJudo
        {
            get { return GetConfigValue<string>(kEntitePublicationFFJudo, string.Empty); }
            set { SetValueAndMarkDirty(kEntitePublicationFFJudo, value); }
        }

        /// <summary>
        /// Retourne la propriété EntitePublicationFFJudo si elle est dans la liste des candidats, sinon retourne le 1er de la liste
        /// si val n'est pas null, retourne l'élément correspondant à val dans la liste ou le 1er de la liste
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public EntitePublicationFFJudo GetEntitePublicationFFJudo(IEnumerable<EntitePublicationFFJudo> candidates, Func<EntitePublicationFFJudo, string> valueSelector, string val = null)
        {
            if(val != null)
            {
                return FindItemFromList(candidates, valueSelector, val);
            }

            return GetItemFromList(kEntitePublicationFFJudo, candidates, e => e.Nom);
        }

        [ConfigurationProperty(kRepertoireRacine, IsRequired = false)]
        public string RepertoireRacine
        {
            get { return GetConfigValue<string>(kRepertoireRacine, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));}
            set { SetValueAndMarkDirty(kRepertoireRacine, value); }
        }

        [ConfigurationProperty(kDelaiGenerationSec, IsRequired = false)]
        public int DelaiGenerationSec
        {
            get { return GetConfigValue<int>(kDelaiGenerationSec, 30); }
            set { SetValueAndMarkDirty(kDelaiGenerationSec, value); }
        }

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
            set { SetValueAndMarkDirty(kTailleMaxPouleColonnes, value);  }
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
            set { SetValueAndMarkDirty(kPouleToujoursEnColonnes, value);  }
        }

        [ConfigurationProperty(kPublierProchainsCombats, IsRequired = false)]
        public bool PublierProchainsCombats
        {
            get { return GetConfigValue<bool>(kPublierProchainsCombats, false); }
            set { SetValueAndMarkDirty(kPublierProchainsCombats, value);  }
        }

        [ConfigurationProperty(kNbProchainsCombats, IsRequired = false)]
        public int NbProchainsCombats
        {
            get { return GetConfigValue<int>(kNbProchainsCombats, 6); }
            set { SetValueAndMarkDirty(kNbProchainsCombats, value);  }
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
            set { SetValueAndMarkDirty(kPublierAffectationTapis, value);}
        }

        [ConfigurationProperty(kLogo, IsRequired = false)]
        public string Logo
        {
            get { return GetConfigValue<string>(kLogo, null); }
            set { SetValueAndMarkDirty(kLogo, value); }
        }

        /// <summary>
        /// Retourne le fichier Logo si il est dans la liste, sinon le 1er de la liste
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public FilteredFileInfo GetLogo(IEnumerable<FilteredFileInfo> candidates, Func<FilteredFileInfo, string> valueSelector)
        {
            return GetItemFromList(kLogo, candidates, valueSelector);
        }

        [ConfigurationProperty(kEasyConfig, IsRequired = false)]
        public bool EasyConfig
        {
            get { return GetConfigValue<bool>(kEasyConfig, true); }
            set { SetValueAndMarkDirty(kEasyConfig, value);  }
        }

        [ConfigurationProperty(kURLDistant, IsRequired = false)]
        public string URLDistant
        {
            get { return GetConfigValue<string>(kURLDistant, string.Empty); }
            set { SetValueAndMarkDirty(kURLDistant, value);  }
        }

        [ConfigurationProperty(kIsolerCompetition, IsRequired = false)]
        public bool IsolerCompetition
        {
            get { return GetConfigValue<bool>(kIsolerCompetition, false); }
            set { SetValueAndMarkDirty(kIsolerCompetition, value); }
        }

        [ConfigurationProperty(kRepertoireRacineSiteFTPDistant, IsRequired = false)]
        public string RepertoireRacineSiteFTPDistant
        {
            get { return GetConfigValue<string>(kRepertoireRacineSiteFTPDistant, string.Empty); }
            set { SetValueAndMarkDirty(kRepertoireRacineSiteFTPDistant, value);  }
        }

        [ConfigurationProperty(kPublierEngagements, IsRequired = false)]
        public bool PublierEngagements
        {
            get { return GetConfigValue<bool>(kPublierEngagements, true); }
            set { SetValueAndMarkDirty(kPublierEngagements, value);  }
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
            set { SetValueAndMarkDirty(kEngagementsTousCombats, value);  }
        }

        [ConfigurationProperty(kEffacerAuDemarrage, IsRequired = false)]
        public bool EffacerAuDemarrage
        {
            get { return GetConfigValue<bool>(kEffacerAuDemarrage, true); }
            set { SetValueAndMarkDirty(kEffacerAuDemarrage, value); }
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
            set { SetValueAndMarkDirty(kIntituleCommun, value);  }
        }

        [ConfigurationProperty(kScoreEngagesGagnantPerdant, IsRequired = false)]
        public bool ScoreEngagesGagnantPerdant
        {
            get { return GetConfigValue<bool>(kScoreEngagesGagnantPerdant, false); }
            set { SetValueAndMarkDirty(kScoreEngagesGagnantPerdant, value);  }
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