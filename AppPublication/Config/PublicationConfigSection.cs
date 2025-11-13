using System;
using System.Configuration;

namespace AppPublication.Config
{
    /// <summary>
    /// Gère les paramètres de publication globaux.
    /// Implémenté en Singleton pour un accès facile et une sauvegarde "live".
    /// </summary>
    public class PublicationConfigSection : ConfigurationSection
    {
        // Nom de la section dans app.config
        public const string kConfigSectionName = "PublicationConfigSection";

        // Constantes pour les noms des attributs XML
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

        private static PublicationConfigSection _instance;
        private static readonly object _lock = new object();
        private static Configuration _config;

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

                        if (_instance == null)
                        {
                            _instance = new PublicationConfigSection();
                            _config.Sections.Add(kConfigSectionName, _instance);
                            _config.Save(ConfigurationSaveMode.Modified);
                        }
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Sauvegarde la configuration actuelle dans le fichier app.config
        /// </summary>
        private void Save()
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
                Console.WriteLine($"Erreur sauvegarde PublicationConfigSection: {ex.Message}");
            }
        }

        [ConfigurationProperty(kRepertoireRacine, DefaultValue = @"C:\PublicationSite", IsRequired = false)]
        public string RepertoireRacine
        {
            get { return (string)this[kRepertoireRacine]; }
            set { this[kRepertoireRacine] = value; Save(); }
        }

        [ConfigurationProperty(kDelaiGenerationSec, DefaultValue = 30, IsRequired = false)]
        public int DelaiGenerationSec
        {
            get { return (int)this[kDelaiGenerationSec]; }
            set { this[kDelaiGenerationSec] = value; Save(); }
        }

        [ConfigurationProperty(kDelaiActualisationClientSec, DefaultValue = 20, IsRequired = false)]
        public int DelaiActualisationClientSec
        {
            get { return (int)this[kDelaiActualisationClientSec]; }
            set { this[kDelaiActualisationClientSec] = value; Save(); }
        }

        [ConfigurationProperty(kTailleMaxPouleColonnes, DefaultValue = 7, IsRequired = false)]
        public int TailleMaxPouleColonnes
        {
            get { return (int)this[kTailleMaxPouleColonnes]; }
            set { this[kTailleMaxPouleColonnes] = value; Save(); }
        }

        [ConfigurationProperty(kPouleEnColonnes, DefaultValue = true, IsRequired = false)]
        public bool PouleEnColonnes
        {
            get { return (bool)this[kPouleEnColonnes]; }
            set { this[kPouleEnColonnes] = value; Save(); }
        }

        [ConfigurationProperty(kPouleToujoursEnColonnes, DefaultValue = false, IsRequired = false)]
        public bool PouleToujoursEnColonnes
        {
            get { return (bool)this[kPouleToujoursEnColonnes]; }
            set { this[kPouleToujoursEnColonnes] = value; Save(); }
        }

        [ConfigurationProperty(kPublierProchainsCombats, DefaultValue = true, IsRequired = false)]
        public bool PublierProchainsCombats
        {
            get { return (bool)this[kPublierProchainsCombats]; }
            set { this[kPublierProchainsCombats] = value; Save(); }
        }

        [ConfigurationProperty(kNbProchainsCombats, DefaultValue = 3, IsRequired = false)]
        public int NbProchainsCombats
        {
            get { return (int)this[kNbProchainsCombats]; }
            set { this[kNbProchainsCombats] = value; Save(); }
        }

        [ConfigurationProperty(kMsgProchainsCombats, DefaultValue = "ATTENTION : Les horaires et numéros de tapis sont donnés à titre indicatif et sont succeptibles d'être modifiés.", IsRequired = false)]
        public string MsgProchainsCombats
        {
            get { return (string)this[kMsgProchainsCombats]; }
            set { this[kMsgProchainsCombats] = value; Save(); }
        }

        [ConfigurationProperty(kPublierAffectationTapis, DefaultValue = true, IsRequired = false)]
        public bool PublierAffectationTapis
        {
            get { return (bool)this[kPublierAffectationTapis]; }
            set { this[kPublierAffectationTapis] = value; Save(); }
        }

        [ConfigurationProperty(kLogo, DefaultValue = "logo-France-Judo-Rhone.png", IsRequired = false)]
        public string Logo
        {
            get { return (string)this[kLogo]; }
            set { this[kLogo] = value; Save(); }
        }
    }
}