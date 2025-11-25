using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using Tools.Outils;
using System.Linq;

namespace AppPublication.Config
{
    /// <summary>
    /// Gère les paramètres de publication globaux.
    /// Implémenté en Singleton pour un accès facile et une sauvegarde "live".
    /// Les 'getters' fournissent des valeurs par défaut dynamiques si la configuration est absente.
    /// </summary>
    public class PublicationConfigSection : ConfigSectionBase
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
        private const string kInterfaceLocalPublication = "interfaceLocalPublication";
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

        #endregion

        #region MEMBRES
        private static PublicationConfigSection _instance;
        private static readonly object _lock = new object();
        #endregion

        #region CONSTRUCTEURS ET SINGLETON

        // Constructeur privé pour le Singleton
        private PublicationConfigSection()
        {
            // Initialisation standard de la ConfigurationSection si elle existe
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = config.GetSection(kConfigSectionName) as PublicationConfigSection;

            // Si la section n'existe pas, la créer (ou la charger du cache si elle existe déjà)
            if (section == null)
            {
                // Tente de créer la section.
                try
                {
                    // Ceci est la seule utilisation du lock original, garantissant l'unicité
                    _instance = new PublicationConfigSection(); // Crée une instance temporaire
                    _instance.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToApplication;
                    config.Sections.Add(kConfigSectionName, _instance);
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(kConfigSectionName);
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, $"Erreur critique : Impossible de créer la section de configuration '{kConfigSectionName}'.");
                    throw;
                }
            }
            else
            {
                _instance = section;
            }

            // Le constructeur doit appeler le constructeur de ConfigSectionBase via le : base() implicite.
            // L'abonnement à l'événement se fait dans ConfigSectionBase.
        }

        /// <summary>
        /// Point d'accès Singleton
        /// </summary>
        public static PublicationConfigSection Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // Forcer l'appel au constructeur privé pour charger ou créer l'instance
                        new PublicationConfigSection();
                    }
                    // Si l'instance a été chargée via ConfigurationManager.GetSection (dans le constructeur), elle est retournée.
                    return _instance;
                }
            }
        }

        #endregion

        #region METHODES
        /// <summary>
        /// Helper générique pour retourner une valeur par défaut si la clé est absente ou non convertible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T GetConfigValue<T>(string key, T defaultValue)
        {
            try
            {
                var raw = this[key];
                if (raw == null) return defaultValue;

                // Si le type attendu est string, faire un cast direct
                if (typeof(T) == typeof(string))
                {
                    return (T)raw;
                }

                // Pour les types nullables (ex: bool?) on gère
                var targetType = typeof(T);
                if (Nullable.GetUnderlyingType(targetType) != null)
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }

                return (T)Convert.ChangeType(raw, targetType);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Retourne l'élément de la collection <c>candidates</c> dont la représentation string
        /// (fourni par <c>valueSelector</c>) correspond à la valeur stockée pour <c>key</c>.
        /// Si aucune correspondance, renvoie le premier élément de la collection (ou default si vide).
        /// Encapsule la logique "valeur présente dans la liste => la retourner, sinon => première valeur".
        /// </summary>
        /// <typeparam name="T">Type des éléments de la collection.</typeparam>
        /// <param name="key">Clé de configuration (attribut dans la section).</param>
        /// <param name="candidates">Collection des éléments valides.</param>
        /// <param name="valueSelector">Fonction qui extrait la représentation string d'un élément (ex: f => f.Name).</param>
        /// <returns>L'élément trouvé ou le premier élément de la collection.</returns>
        public T GetItemFromList<T>(string key, IEnumerable<T> candidates, Func<T, string> valueSelector)
        {
            if (candidates == null) return default(T);
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

            string stored = GetConfigValue<string>(key, null);
            if (!string.IsNullOrWhiteSpace(stored))
            {
                var match = candidates.FirstOrDefault(c => string.Equals(valueSelector(c) ?? string.Empty, stored, StringComparison.OrdinalIgnoreCase));
                if (match != null) return match;
            }

            // fallback : première valeur ou default
            return candidates.FirstOrDefault();
        }
        #endregion

        #region PROPRIETES
        public override string SectionName => kConfigSectionName;
        #endregion

        #region PROPRIETES DE CONFIGURATION



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

        [ConfigurationProperty(kInterfaceLocalPublication, IsRequired = false)]
        public string InterfaceLocalPublication
        {
            get { return GetConfigValue<string>(kInterfaceLocalPublication, null); }
            set { SetValueAndMarkDirty(kInterfaceLocalPublication, value); }
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