using NLog;
using System;
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
        public const string kConfigSectionName = "PublicationConfigSection";

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
        #endregion

        #region PROPRIETES DE CONFIGURATION

        [ConfigurationProperty(kRepertoireRacine, IsRequired = false)]
        public string RepertoireRacine
        {
            get
            {
                string val = (string)this[kRepertoireRacine];
                if (string.IsNullOrEmpty(val))
                {
                    // Valeur par défaut dynamique conforme à la demande
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                }
                return val;
            }
            set { this[kRepertoireRacine] = value; SaveSection(); }
        }

        [ConfigurationProperty(kDelaiGenerationSec, IsRequired = false)]
        public int DelaiGenerationSec
        {
            get
            {
                object val = this[kDelaiGenerationSec];
                return (val == null) ? 30 : (int)val;
            }
            set { this[kDelaiGenerationSec] = value; SaveSection(); }
        }

        [ConfigurationProperty(kDelaiActualisationClientSec, IsRequired = false)]
        public int DelaiActualisationClientSec
        {
            get
            {
                object val = this[kDelaiActualisationClientSec];
                return (val == null) ? 30 : (int)val;
            }
            set { this[kDelaiActualisationClientSec] = value; SaveSection(); }
        }

        [ConfigurationProperty(kTailleMaxPouleColonnes, IsRequired = false)]
        public int TailleMaxPouleColonnes
        {
            get
            {
                object val = this[kTailleMaxPouleColonnes];
                return (val == null) ? 5 : (int)val;
            }
            set { this[kTailleMaxPouleColonnes] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPouleEnColonnes, IsRequired = false)]
        public bool PouleEnColonnes
        {
            get
            {
                object val = this[kPouleEnColonnes];
                return (val == null) ? false : (bool)val;
            }
            set { this[kPouleEnColonnes] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPouleToujoursEnColonnes, IsRequired = false)]
        public bool PouleToujoursEnColonnes
        {
            get
            {
                object val = this[kPouleToujoursEnColonnes];
                return (val == null) ? false : (bool)val;
            }
            set { this[kPouleToujoursEnColonnes] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPublierProchainsCombats, IsRequired = false)]
        public bool PublierProchainsCombats
        {
            get
            {
                object val = this[kPublierProchainsCombats];
                return (val == null) ? false : (bool)val;
            }
            set { this[kPublierProchainsCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kNbProchainsCombats, IsRequired = false)]
        public int NbProchainsCombats
        {
            get
            {
                object val = this[kNbProchainsCombats];
                return (val == null) ? 6 : (int)val;
            }
            set { this[kNbProchainsCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kMsgProchainsCombats, IsRequired = false)]
        public string MsgProchainsCombats
        {
            get
            {
                string val = (string)this[kMsgProchainsCombats];
                if (string.IsNullOrEmpty(val))
                {
                    return "ATTENTION : Les horaires et numéros de tapis sont donnés à titre indicatif et sont succeptibles d'être modifiés.";
                }
                return val;
            }
            set { this[kMsgProchainsCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPublierAffectationTapis, IsRequired = false)]
        public bool PublierAffectationTapis
        {
            get
            {
                object val = this[kPublierAffectationTapis];
                return (val == null) ? true : (bool)val;
            }
            set { this[kPublierAffectationTapis] = value; SaveSection(); }
        }

        [ConfigurationProperty(kLogo, IsRequired = false)]
        public string Logo
        {
            get
            {
                // Le getter retourne la valeur brute (ou null)
                // La logique de valeur par défaut est gérée dans GestionSite.cs
                return (string)this[kLogo];
            }
            set { this[kLogo] = value; SaveSection(); }
        }

        [ConfigurationProperty(kInterfaceLocalPublication, IsRequired = false)]
        public string InterfaceLocalPublication
        {
            get
            {
                // Le getter retourne la valeur brute (ou null)
                return (string)this[kInterfaceLocalPublication];
            }
            set { this[kInterfaceLocalPublication] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEasyConfig, IsRequired = false)]
        public bool EasyConfig
        {
            get
            {
                object val = this[kEasyConfig];
                return (val == null) ? true : (bool)val;
            }
            set { this[kEasyConfig] = value; SaveSection(); }
        }

        [ConfigurationProperty(kURLDistant, IsRequired = false)]
        public string URLDistant
        {
            get { return (string)this[kURLDistant] ?? string.Empty; }
            set { this[kURLDistant] = value; SaveSection(); }
        }

        [ConfigurationProperty(kIsolerCompetition, IsRequired = false)]
        public bool IsolerCompetition
        {
            get { return (bool?)this[kIsolerCompetition] ?? false; }
            set { this[kIsolerCompetition] = value; SaveSection(); }
        }

        [ConfigurationProperty(kRepertoireRacineSiteFTPDistant, IsRequired = false)]
        public string RepertoireRacineSiteFTPDistant
        {
            get { return (string)this[kRepertoireRacineSiteFTPDistant] ?? string.Empty; }
            set { this[kRepertoireRacineSiteFTPDistant] = value; SaveSection(); }
        }

        [ConfigurationProperty(kPublierEngagements, IsRequired = false)]
        public bool PublierEngagements
        {
            get { return (bool?)this[kPublierEngagements] ?? false; }
            set { this[kPublierEngagements] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEngagementsAbsents, IsRequired = false)]
        public bool EngagementsAbsents
        {
            get { return (bool?)this[kEngagementsAbsents] ?? false; }
            set { this[kEngagementsAbsents] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEngagementsTousCombats, IsRequired = false)]
        public bool EngagementsTousCombats
        {
            get { return (bool?)this[kEngagementsTousCombats] ?? false; }
            set { this[kEngagementsTousCombats] = value; SaveSection(); }
        }

        [ConfigurationProperty(kEffacerAuDemarrage, IsRequired = false)]
        public bool EffacerAuDemarrage
        {
            get { return (bool?)this[kEffacerAuDemarrage] ?? true; }
            set { this[kEffacerAuDemarrage] = value; SaveSection(); }
        }

        [ConfigurationProperty(kUseIntituleCommun, IsRequired = false)]
        public bool UseIntituleCommun
        {
            get { return (bool?)this[kUseIntituleCommun] ?? false; }
            set { this[kUseIntituleCommun] = value; SaveSection(); }
        }

        [ConfigurationProperty(kIntituleCommun, IsRequired = false)]
        public string IntituleCommun
        {
            get { return (string)this[kIntituleCommun] ?? string.Empty; }
            set { this[kIntituleCommun] = value; SaveSection(); }
        }

        [ConfigurationProperty(kScoreEngagesGagnantPerdant, IsRequired = false)]
        public bool ScoreEngagesGagnantPerdant
        {
            get { return (bool?)this[kScoreEngagesGagnantPerdant] ?? false; }
            set { this[kScoreEngagesGagnantPerdant] = value; SaveSection(); }
        }

        [ConfigurationProperty(kAfficherPositionCombat, IsRequired = false)]
        public bool AfficherPositionCombat
        {
            get { return (bool?)this[kAfficherPositionCombat] ?? false; }
            set { this[kAfficherPositionCombat] = value; SaveSection(); }
        }
        #endregion
    }
}