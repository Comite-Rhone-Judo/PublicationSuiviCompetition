using AppPublication.Config.Publication;
using System.Net;
using Tools.Net;
using Tools.Logging;
using Tools.Core;
using System.Windows.Media;
using System;

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
        protected MiniSiteConfigurable(bool local, IServeurHttp httpInstance, string instanceName = "", bool cacheCfg = true, bool cachePwd = true) : base(local, httpInstance)
        {
            // Initialise les caracteristiques du MiniSite
            InstanceName = instanceName;
            CacheConfig = cacheCfg;
            CachePassword = cachePwd;

            // Chargement initial de la configuration
            LoadConfiguration();
        }

        /// <summary>
        /// Factory de création d'une instance de MiniSiteConfigurable
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="cacheCfg"></param>
        /// <param name="cachePwd"></param>
        /// <returns></returns>
        public static MiniSiteConfigurable CreateInstance(string instanceName = "", bool cacheCfg = true, bool cachePwd = true)
        {
            IServeurHttp httpInstance = null;

            // Ici on force le nom de l'instance car on n'a pas encore d'instance initialisé donc InstanceName n'existe pas
            MiniSiteConfigElement cfg = GetInstanceConfigElement(instanceName);

            if (cfg.TypeLocal)
            {
                // On cherche le type d'instance Htttp
                try
                {
                    httpInstance = ClassFactory.CreateInstance<IServeurHttp>(cfg.HttpServerClass);
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error($"Erreur lors de la création de l'instance du serveur HTTP '{cfg.HttpServerClass}' pour le minisite '{instanceName}' : {ex.Message}");
                    throw new NullReferenceException($"Impossible de créer l'instance du serveur HTTP '{cfg.HttpServerClass}' pour le minisite '{instanceName}'", ex);
                }
            }
            
            // On appel le constructeur maintenant que l'on connait le type d'instance
            return new MiniSiteConfigurable(cfg.TypeLocal, httpInstance, instanceName, cacheCfg, cachePwd);
        }


        #endregion

        #region PROPERTIES

        private string _instanceName = string.Empty;
        /// <summary>
        /// Indique le nom de l'instance du Minisite
        /// </summary>
        public string InstanceName
        {
            get
            {
                return _instanceName;
            }
            private set
            {
                _instanceName = value;
                NotifyPropertyChanged();
            }
        }

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
                        MiniSiteConfigElement cfg = GetCurrentInstanceConfigElement();
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
                        MiniSiteConfigElement cfg = GetCurrentInstanceConfigElement();
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
                        MiniSiteConfigElement cfg = GetCurrentInstanceConfigElement();
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
                        MiniSiteConfigElement cfg = GetCurrentInstanceConfigElement();
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
                        MiniSiteConfigElement cfg = GetCurrentInstanceConfigElement();
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
                        MiniSiteConfigElement cfg = GetCurrentInstanceConfigElement();
                        cfg.SynchroniseDifferences = value;
                    }
                }
            }
        }

        #region METHODES

        /// <summary>
        /// Recherche l'element de sauvegarde de la configuration pour l'instance en cours
        /// </summary>
        private MiniSiteConfigElement GetCurrentInstanceConfigElement()
        {
            return MiniSiteConfigurable.GetInstanceConfigElement(InstanceName);
        }

        /// <summary>
        /// Recherche l'element de sauvegarde de la configuration pour l'instance donnee, ou l'ajoute s'il n'existe pas
        /// </summary>
        private static MiniSiteConfigElement GetInstanceConfigElement(string instanceName)
        {
            // Sauvegarde de la config
            MiniSiteConfigElement cfg = PublicationConfigSection.Instance.MiniSites[instanceName];
            if (cfg == null)
            {
                // Pas de config trouvée, on crée une config vide par défaut
                cfg = new MiniSiteConfigElement();
                PublicationConfigSection.Instance.MiniSites.Add(cfg);
            }

            return cfg;
        }

        /// <summary>
        /// Charge la configuration de l'instance courante
        /// </summary>
        private void LoadConfiguration()
        {
            MiniSiteConfigElement cfg = GetCurrentInstanceConfigElement();

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
