using AppPublication.Config.Generation;
using AppPublication.Tools.Files;
using AppPublication.Tools.FranceJudo;
using System;
using System.Collections.Generic;
using System.Configuration;
using Tools.Configuration;

namespace AppPublication.Config.Publication
{
    // TODO A voir comment on fait si on separe les parametres de generation Site et SitePrivate

    /// <summary>
    /// Gère les paramètres de publication globaux.
    /// Implémenté en Singleton pour un accès facile et une sauvegarde "live".
    /// Les 'getters' fournissent des valeurs par défaut dynamiques si la configuration est absente.
    /// </summary>

    public class GeneralConfigElement : ConfigElementBase<PublicationConfigSection>
    {
        #region CONSTANTES

        // Nom des clefs de configuration
        private const string kRepertoireRacine = "repertoireRacine";
        private const string kLogo = "logo";
        private const string kEasyConfig = "easyConfig";
        private const string kURLDistant = "urlDistant";
        private const string kIsolerCompetition = "isolerCompetition";
        private const string kRepertoireRacineSiteFTPDistant = "repertoireRacineSiteFTPDistant";
        private const string kEffacerAuDemarrage = "effacerAuDemarrage";

        private const string kNiveauPublicationFFJudo = "NiveauPublicationFFJudo";
        private const string kEntitePublicationFFJudo = "EntitePublicationFFJudo";

        #endregion

        #region METHODES
        /// <summary>
        /// Méthode héritée de ConfigElementBase.
        /// Notifie la section parente qu'une propriété a changé pour déclencher le mécanisme de sauvegarde différée.
        /// </summary>
        protected override void NotifyParentOfModification()
        {
            if (PublicationConfigSection.Instance != null)
            {
                PublicationConfigSection.Instance.NotifyChildModification();
            }
        }
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
            if (val != null)
            {
                return FindItemFromList(candidates, valueSelector, val);
            }

            return GetItemFromList(kEntitePublicationFFJudo, candidates, e => e.Nom);
        }

        [ConfigurationProperty(kRepertoireRacine, IsRequired = false)]
        public string RepertoireRacine
        {
            get { return GetConfigValue<string>(kRepertoireRacine, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)); }
            set { SetValueAndMarkDirty(kRepertoireRacine, value); }
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
            set { SetValueAndMarkDirty(kEasyConfig, value); }
        }

        [ConfigurationProperty(kURLDistant, IsRequired = false)]
        public string URLDistant
        {
            get { return GetConfigValue<string>(kURLDistant, string.Empty); }
            set { SetValueAndMarkDirty(kURLDistant, value); }
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
            set { SetValueAndMarkDirty(kRepertoireRacineSiteFTPDistant, value); }
        }

        [ConfigurationProperty(kEffacerAuDemarrage, IsRequired = false)]
        public bool EffacerAuDemarrage
        {
            get { return GetConfigValue<bool>(kEffacerAuDemarrage, true); }
            set { SetValueAndMarkDirty(kEffacerAuDemarrage, value); }
        }
        #endregion
    }
}