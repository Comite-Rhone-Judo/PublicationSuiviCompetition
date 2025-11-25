using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using Tools.Outils;

namespace AppPublication.Config
{
    /// <summary>
    /// Gère les paramètres de publication globaux.
    /// Implémenté en Singleton pour un accès facile et une sauvegarde "live".
    /// Les 'getters' fournissent des valeurs par défaut dynamiques si la configuration est absente.
    /// </summary>
    public class PublicationConfigSection : ConfigurationSection
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
        private static Configuration _config;
        #endregion

        #region CONSTRUCTEURS ET SINGLETON

        // Constructeur privé pour le Singleton
        private PublicationConfigSection() { }

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
                        // Ouvre le fichier de configuration de l'application
                        _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                        // Récupère la section. Si elle n'existe pas, la crée.
                        _instance = _config.GetSection(kConfigSectionName) as PublicationConfigSection;

                        try
                        {
                            _instance = new PublicationConfigSection();
                            _instance.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToApplication;
                            _config.Sections.Add(kConfigSectionName, _instance);
                            _config.Save(ConfigurationSaveMode.Modified);
                        }
                        catch (Exception ex)
                        {
                            // Propage l'erreur si la section ne peut pas être créée
                            LogTools.Logger.Error(ex, $"Erreur critique : Impossible de créer la section de configuration '{kConfigSectionName}'.");
                            throw new ConfigurationErrorsException($"Erreur critique : Impossible de créer la section de configuration '{kConfigSectionName}'.", ex);
                        }
                    }
                    return _instance;
                }
            }
        }

        #endregion

        #region METHODES
        /// <summary>
        /// Sauvegarde la configuration actuelle dans le fichier app.config
        /// </summary>
        private void SaveSection()
        {
            try
            {
                lock (_lock)
                {
                    if (_config != null)
                    {
                        _config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection(kConfigSectionName);
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer l'erreur de sauvegarde (ex: log)
                // Console.WriteLine($"Erreur sauvegarde PublicationConfigSection: {ex.Message}");
                LogTools.Logger.Error(ex, $"Erreur critique : Impossible de sauvegarder la section de configuration '{kConfigSectionName}'.");
                throw ex; // Propage l'exception
            }
        }

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

        #region PROPRIETES DE CONFIGURATION

        [ConfigurationProperty(kRepertoireRacine, IsRequired = false)]
        public string RepertoireRacine
        {
            get { return GetConfigValue<string>(kRepertoireRacine, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));}
            set { this[kRepertoireRacine] = value; SaveSection(); }
        }

        [ConfigurationProperty(kDelaiGenerationSec, IsRequired = false)]
        public int DelaiGenerationSec
        {
            get { return GetConfigValue<int>(kDelaiGenerationSec, 30); }
            set { this[kDelaiGenerationSec] = value; SaveSection(); }
        }

        [ConfigurationProperty(kDelaiActualisationClientSec, IsRequired = false)]
        public int DelaiActualisationClientSec
        {
            get { return GetConfigValue<int>(kDelaiActualisationClientSec, 30); }
            set { this[kDelaiActualisationClientSec] = value; SaveSection(); }
        }

        [ConfigurationProperty(kTailleMaxPouleColonnes, IsRequired = false)]
        public int TailleMaxPouleColonnes
        {
            get { return GetConfigValue<int>(kTailleMaxPouleColonnes, 5); }
            set { this[kTailleMaxPouleColonnes] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPouleEnColonnes, IsRequired = false)]
        public bool PouleEnColonnes
        {
            get { return GetConfigValue<bool>(kPouleEnColonnes, false); }
            set { this[kPouleEnColonnes] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPouleToujoursEnColonnes, IsRequired = false)]
        public bool PouleToujoursEnColonnes
        {
            get { return GetConfigValue<bool>(kPouleToujoursEnColonnes, false); }
            set { this[kPouleToujoursEnColonnes] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPublierProchainsCombats, IsRequired = false)]
        public bool PublierProchainsCombats
        {
            get { return GetConfigValue<bool>(kPublierProchainsCombats, false); }
            set { this[kPublierProchainsCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kNbProchainsCombats, IsRequired = false)]
        public int NbProchainsCombats
        {
            get { return GetConfigValue<int>(kNbProchainsCombats, 6); }
            set { this[kNbProchainsCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kMsgProchainsCombats, IsRequired = false)]
        public string MsgProchainsCombats
        {
            get { return GetConfigValue<string>(kMsgProchainsCombats, String.Empty); }
            set { this[kMsgProchainsCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPublierAffectationTapis, IsRequired = false)]
        public bool PublierAffectationTapis
        {
            get { return GetConfigValue<bool>(kPublierAffectationTapis, true); }
            set { this[kPublierAffectationTapis] = value; SaveSection(); }
        }

        [ConfigurationProperty(kLogo, IsRequired = false)]
        public string Logo
        {
            get { return GetConfigValue<string>(kLogo, null); }
            set { this[kLogo] = value; SaveSection(); }
        }

        [ConfigurationProperty(kInterfaceLocalPublication, IsRequired = false)]
        public string InterfaceLocalPublication
        {
            get { return GetConfigValue<string>(kInterfaceLocalPublication, null); }
            set { this[kInterfaceLocalPublication] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEasyConfig, IsRequired = false)]
        public bool EasyConfig
        {
            get { return GetConfigValue<bool>(kEasyConfig, true); }
            set { this[kEasyConfig] = value; SaveSection(); }
        }

        [ConfigurationProperty(kURLDistant, IsRequired = false)]
        public string URLDistant
        {
            get { return GetConfigValue<string>(kURLDistant, string.Empty); }
            set { this[kURLDistant] = value; SaveSection(); }
        }

        [ConfigurationProperty(kIsolerCompetition, IsRequired = false)]
        public bool IsolerCompetition
        {
            get { return GetConfigValue<bool>(kIsolerCompetition, false); }
            set { this[kIsolerCompetition] = value; SaveSection(); }
        }

        [ConfigurationProperty(kRepertoireRacineSiteFTPDistant, IsRequired = false)]
        public string RepertoireRacineSiteFTPDistant
        {
            get { return GetConfigValue<string>(kRepertoireRacineSiteFTPDistant, string.Empty); }
            set { this[kRepertoireRacineSiteFTPDistant] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPublierEngagements, IsRequired = false)]
        public bool PublierEngagements
        {
            get { return GetConfigValue<bool>(kPublierEngagements, true); }
            set { this[kPublierEngagements] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEngagementsAbsents, IsRequired = false)]
        public bool EngagementsAbsents
        {
            get { return GetConfigValue<bool>(kEngagementsAbsents, false); }
            set { this[kEngagementsAbsents] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEngagementsTousCombats, IsRequired = false)]
        public bool EngagementsTousCombats
        {
            get { return GetConfigValue<bool>(kEngagementsTousCombats, false); }
            set { this[kEngagementsTousCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEffacerAuDemarrage, IsRequired = false)]
        public bool EffacerAuDemarrage
        {
            get { return GetConfigValue<bool>(kEffacerAuDemarrage, true); }
            set { this[kEffacerAuDemarrage] = value; SaveSection(); }
        }

        [ConfigurationProperty(kUseIntituleCommun, IsRequired = false)]
        public bool UseIntituleCommun
        {
            get { return GetConfigValue<bool>(kUseIntituleCommun, false); }
            set { this[kUseIntituleCommun] = value; SaveSection(); }
        }

        [ConfigurationProperty(kIntituleCommun, IsRequired = false)]
        public string IntituleCommun
        {
            get { return GetConfigValue<string>(kIntituleCommun, string.Empty); }
            set { this[kIntituleCommun] = value; SaveSection(); }
        }

        [ConfigurationProperty(kScoreEngagesGagnantPerdant, IsRequired = false)]
        public bool ScoreEngagesGagnantPerdant
        {
            get { return GetConfigValue<bool>(kScoreEngagesGagnantPerdant, false); }
            set { this[kScoreEngagesGagnantPerdant] = value; SaveSection(); }
        }

        [ConfigurationProperty(kAfficherPositionCombat, IsRequired = false)]
        public bool AfficherPositionCombat
        {
            get { return GetConfigValue<bool>(kAfficherPositionCombat, true); }
            set { this[kAfficherPositionCombat] = value; SaveSection(); }
        }
        #endregion
    }
}