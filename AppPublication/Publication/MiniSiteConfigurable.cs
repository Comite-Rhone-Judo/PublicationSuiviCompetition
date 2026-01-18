using AppPublication.Config.Publication;
using System.Net;
using Tools.Net;
using Tools.Logging;

namespace AppPublication.Publication
{
    public class MiniSiteConfigurable : MiniSite
    {
        // TODO voir pour faire que MiniSite soit WPF agnostic et mettre les properties bindable dans cette classe

        #region CONSTRUCTEURS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="local">Mode du minisite (local = true, distant = false)</param>
        /// <param name="instanceName">Nom de l'instance</param>
        /// <param name="cacheCfg">True pour activer la mise en cache de la configuration</param>
        /// <param name="cachePwd">True pour activer la mise en cache du mot de passe</param>
        public MiniSiteConfigurable(bool local, string instanceName = "", bool cacheCfg = true, bool cachePwd = true) : base(local, instanceName)
        {
            CacheConfig = cacheCfg;
            CachePassword = cachePwd;

            // Chargement initial de la configuration
            LoadConfiguration();

        }
        #endregion

        #region PROPERTIES

        bool _cacheConfig = false;
        /// <summary>
        /// Indique si les donnees de configuration doivent etre gardees en cache
        /// </summary>
        public bool CacheConfig
        {
            get
            {
                return _cacheConfig;
            }
            private set
            {
                _cacheConfig = value;
                NotifyPropertyChanged();
            }
        }

        bool _cachePassword = false;
        /// <summary>
        /// Indique si le mot de passe doit etre gardees en cache
        /// </summary>
        public bool CachePassword
        {
            get
            {
                return _cachePassword;
            }
            private set
            {
                _cachePassword = value;
                NotifyPropertyChanged();
            }
        }
        #endregion


        /// <summary>
        /// Interface (@IP) utilisée pour la publication du site en mode local
        /// doit etre presente dans la liste InterfacesLocal
        /// </summary>
        public override IPAddress InterfaceLocalPublication
        {
            get
            {
                return base.InterfaceLocalPublication;
            }
            set
            {
                if (!Equals(base.InterfaceLocalPublication, value))
                {
                    // Mise à jour de la valeur en mémoire
                    base.InterfaceLocalPublication = value;

                    // Sauvegarde de la config si besoin
                    if (CacheConfig && value != null)
                    {
                        MiniSiteConfigElement cfg = GetInstanceConfigElement();
                        cfg.InterfaceLocalPublication = value.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// L'adresse du site FTP distant
        /// </summary>
        public override string SiteFTPDistant
        {
            get
            {
                return base.SiteFTPDistant;
            }
            set
            {
                if(base.SiteFTPDistant != value)
                {
                    // Mise à jour de la valeur en mémoire
                    base.SiteFTPDistant = value;
                    // Sauvegarde de la config si besoin
                    if (CacheConfig)
                    {
                        MiniSiteConfigElement cfg = GetInstanceConfigElement();
                        cfg.FtpSite = value;
                    }
                }
            }
        }

        /// <summary>
        /// Login sur le site FTP distant
        /// </summary>
        public override string LoginSiteFTPDistant
        {
            get
            {
                return base.LoginSiteFTPDistant;
            }
            set
            {
                if(base.LoginSiteFTPDistant != value)
                {
                    // Mise à jour de la valeur en mémoire
                    base.LoginSiteFTPDistant = value;
                    // Sauvegarde de la config si besoin
                    if (CacheConfig)
                    {
                        MiniSiteConfigElement cfg = GetInstanceConfigElement();
                        cfg.FtpLogin = value;
                    }
                }
            }
        }

        /// <summary>
        /// Mode de fonctionnement FTP Actif (true) ou passif (false)
        /// </summary>
        public override bool ModeActifFTPDistant
        {
            get
            {
                return base.ModeActifFTPDistant;
            }
            set
            {
                if(base.ModeActifFTPDistant != value)
                {
                    // Mise à jour de la valeur en mémoire
                    base.ModeActifFTPDistant = value;
                    // Sauvegarde de la config si besoin
                    if (CacheConfig)
                    {
                        MiniSiteConfigElement cfg = GetInstanceConfigElement();
                        cfg.FtpModeActif = value;
                    }
                }
            }
        }

        /// <summary>
        /// Mot de passe FTP au site FTP Distant
        /// </summary>
        public override string PasswordSiteFTPDistant
        {
            get
            {
                return base.PasswordSiteFTPDistant;
            }
            set
            {
                if (base.PasswordSiteFTPDistant != value)
                {
                    // Mise à jour de la valeur en mémoire
                    base.PasswordSiteFTPDistant = value;
                    // Sauvegarde de la config si besoin
                    if (CacheConfig)
                    {
                        MiniSiteConfigElement cfg = GetInstanceConfigElement();
                        cfg.FtpPassword = CachePassword ? value : string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Synchronise uniquement les différences (True) sinon, transfere tous les fichier (False)
        /// </summary>
        public override bool SynchroniseDifferences
        {
            get
            {
                return base.SynchroniseDifferences;
            }
            set
            {
                if(base.SynchroniseDifferences != value)
                {
                    // Mise à jour de la valeur en mémoire
                    base.SynchroniseDifferences = value;
                    // Sauvegarde de la config si besoin
                    if (CacheConfig)
                    {
                        MiniSiteConfigElement cfg = GetInstanceConfigElement();
                        cfg.SynchroniseDifferences = value;
                    }
                }
            }
        }

        #region METHODES

        /// <summary>
        /// Recherche l'element de sauvegarde de la configuration, ou l'ajoute s'il n'existe pas
        /// </summary>
        private MiniSiteConfigElement GetInstanceConfigElement()
        {
            // Sauvegarde de la config
                MiniSiteConfigElement cfg = PublicationConfigSection.Instance.MiniSites[InstanceName];
                if (cfg == null)
                {
                    // Pas de config trouvée, on crée une config vide par défaut
                    cfg = new MiniSiteConfigElement();
                    PublicationConfigSection.Instance.MiniSites.Add(cfg);
                }

            return cfg;
        }

        private void LoadConfiguration()
        {
            // TODO Ajouter ici la gestion des types de serveurs HTTP
            MiniSiteConfigElement cfg = PublicationConfigSection.Instance.MiniSites[InstanceName];
            if (cfg == null)
            {
                // Pas de config trouvée, on crée une config vide par défaut
                cfg = new MiniSiteConfigElement();
                PublicationConfigSection.Instance.MiniSites.Add(cfg);
            }

            if (IsLocal)
            {
                // En local seul le parametre de l'interface réseau est pertinent

                // Lecture de l'interface réseau préférée
                if (InterfacesLocal != null && InterfacesLocal.Count > 0)
                {
                    SelectInterfaceOrDefault(cfg.InterfaceLocalPublication);
                }
            }
            else
            {
                // Pour le mode distant, on ne tient compte que des paramètres FTP

                // Lecture de la config FTP
                try
                {
                    // On set les propriétés de base directement
                    SiteFTPDistant = cfg.FtpSite;
                    LoginSiteFTPDistant = cfg.FtpLogin;
                    PasswordSiteFTPDistant = cfg.FtpPassword;
                    ModeActifFTPDistant = cfg.FtpModeActif;
                    SynchroniseDifferences = cfg.SynchroniseDifferences;
                }
                catch
                {
                    LogTools.Logger.Error($"Erreur lors du chargement de la configuration FTP pour le minisite distant '{InstanceName}'");
                }
            }
        }
        #endregion
    }
}
