using AppPublication.Config.Generation;
using AppPublication.Config.Publication;
using AppPublication.Generation;
using AppPublication.Models.EcransAppel;
using AppPublication.Models.Statistiques;
using AppPublication.Publication;
using AppPublication.ViewModels.Configuration;
using KernelImpl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Tools.Export;
using Tools.Files;
using Tools.Logging;
using Tools.Outils;
using Tools.Windows;

namespace AppPublication.Models.Publication
{
    public class GestionSiteInterne : GestionSiteBase
    {
        #region CONSTANTES
        private const string kSiteLocalInstanceName = "internal";
        private const string kSiteRepertoire = "internal-site";
        private const string kCfgSiteLocalInstanceName = "internal";
        #endregion

        #region MEMBRES
        private GenerateurSiteInterne _generateurSite = null;                // Le generateur Site
        private ExportSiteInterneStructure _structureRepertoiresSiteInterne;        // La structure de repertoire d'export du site prive
        private ExportSiteInterneUrls _structureSiteInterne;                        // la structure d'export du site interne
        private EcranCollectionManager _ecransAppel = new EcranCollectionManager(); // La configuration des écrans d'appels
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="dataManager">Le gestionnaire de données</param>
        /// <param name="statMgr">le gestionnaire de statitiques</param>
        public GestionSiteInterne(IJudoDataManager dataManager, GestionStatistiques statMgr)
            : base(dataManager, statMgr)
        {
            try
            {
                // Initialise les objets de gestion des sites Web. Ils chargent automatiquement leur configuration
                _siteLocal = MiniSiteConfigurable.CreateInstance(kSiteLocalInstanceName, true, false);

                // Enregistre les ecrans d'appels en tant que contextes pour les modules HTTP
                _siteLocal.RegisterContext(_ecransAppel);

                // Le generateur de site interne
                _generateurSite = new GenerateurSiteInterne(_judoDataManager, _ecransAppel, _progressHandler);

                // Initialise le scheduler de generation de site interne
                _schedulerSite = new GenerationScheduler(_statMgr.GenerationSiteInterne, null, _generateurSite);
                _schedulerSite.StateChanged += OnSchedulerSiteStateChanged;
            }
            catch (Exception ex)
            {
                LogTools.Logger.Fatal(ex, "Impossible d'initialiser le ViewModel principal. Impossible de continuer");
                AlertWindow win = new AlertWindow("Erreur fatale", "Impossible de démarrer un composant interne, l'application doit s'arrêter. Veuillez contacter le support.");
                win?.ShowDialog();
                // Emergency shutdown
                App.Current.Shutdown();
            }
        }
        #endregion

        #region PROPRIETES SPECIFIQUES

        private ConfigurationEcransViewModel _cfgEcransAppelViewModel = null;
        /// <summary>
        /// Le ViewModel pour les ecrans (doit etre en Properties pour le binding WPF)
        /// </summary>
        public ConfigurationEcransViewModel ConfigurationEcransViewModel
        {
            get
            {
                if (_cfgEcransAppelViewModel == null)
                {
                    _cfgEcransAppelViewModel = new ConfigurationEcransViewModel(_ecransAppel, _nbTapis);
                }
                return _cfgEcransAppelViewModel;
            }
        }

        private string _urlEcransAppelPublication;
        /// <summary>
        /// URL pour le site local des ecrans d'appel
        /// </summary>
        public string URLLocalPublication
        {
            get { return _urlEcransAppelPublication; }
            private set
            {
                _urlEcransAppelPublication = value;
                NotifyPropertyChanged();
            }
        }

        private int _nbTapis = 6;
        public int NbTapis
        {
            get { return _nbTapis; }
            set
            {
                if (_nbTapis != value)
                {
                    _nbTapis = value;
                    // RAZ le viewModel des ecrans d'appel, cela forcera la recreation avec le nouveau nombre de tapis en cas de nouvelle configuration
                    _cfgEcransAppelViewModel = null;
                    _ecransAppel.NbTapis = _nbTapis;    // Propage la valeur au gestionnaire des ecrans d'appel
                    NotifyPropertyChanged();
                }
            }
        }

        private int _delaiDeroulementSec = 10;
        /// <summary>
        /// Delai de deroulement des ecrans d'appel en secondes
        /// </summary>
        public int DelaiDeroulementSec
        {
            get { return _delaiDeroulementSec; }
            set
            {
                if (_delaiDeroulementSec != value)
                {
                    _generateurSite.ConfigurationGeneration.DelaiDeroulementSec = (_delaiDeroulementSec = value);
                    GenerationConfigSection.Instance.GenerateurSiteInterne.DelaiDeroulementSec = _delaiDeroulementSec;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _nbProchainsCombats = 6;
        /// <summary>
        /// Nb de prochains combats a publier pour la chambre d'appel
        /// </summary>
        public int NbProchainsCombats
        {
            get { return _nbProchainsCombats; }
            set
            {
                if (_nbProchainsCombats != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.NbProchainsCombats = (_nbProchainsCombats = value);
                    GenerationConfigSection.Instance.GenerateurSiteInterne.NbProchainsCombats = _nbProchainsCombats;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region IMPLEMENTATION DES HOOKS (Classe de Base)

        public override void InitFromConfigFile()
        {
            try
            {
                // Note: le repertoire racine et le logo sont lus par l'orchestrateur
                // Les autres parametres peuvent suivre
                // Lecture des donnees specifiques de l'instance
                SchedulerConfigElement cfgPriv = PublicationConfigSection.GetInstanceConfigElement(kCfgSiteLocalInstanceName);
                DelaiGenerationSec = cfgPriv.DelaiGenerationSec;

                // L'interface local de publication a ete chargee via la configuration du minisite, il faut juste s'assurer du bon calcul des URLs
                URLLocalPublication = CalculURLSiteLocal();

                // ici on initialise les ecrans d'appel
                InitEcransAppel();
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        protected override void OnRepertoireRacineChanged(string newValue)
        {
            // Met a jour la constante d'export
            string tmp = OutilsTools.GetExportDir(newValue);
            string siteRootInterne = Path.Combine(tmp, kSiteRepertoire);

            // Initialise les structures d'export
            _structureRepertoiresSiteInterne = new ExportSiteInterneStructure(siteRootInterne);
            _structureSiteInterne = new ExportSiteInterneUrls(_structureRepertoiresSiteInterne);

            // Propage la valeur au generateur de site
            if (_generateurSite != null)
                _generateurSite.StructureRepertoire = _structureRepertoiresSiteInterne;

            // Met a jour les repertoires de l'application (Interne)
            if (_structureRepertoiresSiteInterne != null)
            {
                FileAndDirectTools.CreateDirectorie(_structureRepertoiresSiteInterne.RepertoireRacine);
            }

            // Initialise la racine du serveur Web local et On met a jour les contextes pour les modules HTTP
            SiteLocal.ServerHTTP.LocalRootPath = siteRootInterne;
            SiteLocal.RegisterContext(_structureSiteInterne);
        }

        protected override void OnSelectedLogoChanged(string logoName)
        {
            // Propage la valeur au generateur de site interne
            if (_generateurSite != null)
                _generateurSite.ConfigurationGeneration.Logo = logoName;
        }

        protected override void OnInterfaceLocalPublicationChanged()
        {
            URLLocalPublication = CalculURLSiteLocal();
        }

        protected override void UpdateDelaiGenerationConfig(int newValue)
        {
            SchedulerConfigElement cfg = PublicationConfigSection.GetInstanceConfigElement(kCfgSiteLocalInstanceName);
            cfg.DelaiGenerationSec = newValue;
        }

        protected override void OnIdCompetitionChanged(string newValue)
        {
            URLLocalPublication = CalculURLSiteLocal();

            // Note: ici on devrait dans l'absolu utiliser le snapshot mais le traitement est rapide et a peu de chance de changer
            var DC = _judoDataManager.Data;
            if (DC != null && DC.Organisation != null && DC.Organisation.Competition != null)
            {
                // Le nombre de tapis peut avoir changé selon la compétition
                NbTapis = DC.Organisation.Competition.nbTapis;
            }
        }

        #endregion

        #region METHODES SPECIFIQUES

        /// <summary>
        /// Initialise les ecrans d'appel depuis les données en configuration
        /// </summary>
        private void InitEcransAppel()
        {
            try
            {
                // Chargement des Ecrans depuis la Config vers le Modèle Runtime
                if (_ecransAppel == null) throw new ArgumentNullException("La liste des ecrans d'appel est null");

                if (GenerationConfigSection.Instance?.Ecrans != null)
                {
                    foreach (EcransAppelConfigElement cfg in GenerationConfigSection.Instance.Ecrans)
                    {
                        // Parsing des IDs de tapis "1;2;3" -> List<int>
                        List<int> tapisIds = new List<int>();
                        if (!string.IsNullOrEmpty(cfg.TapisIds))
                        {
                            tapisIds = cfg.TapisIds.Split(';')
                                          .Select(s => int.TryParse(s, out int i) ? i : 0)
                                          .Where(i => i > 0)
                                          .ToList();
                        }

                        // On crée le modèle à partir de la config
                        bool ipValid = IPAddress.TryParse(cfg.AdresseIp, out IPAddress ip);
                        var model = new EcranAppelModel
                        {
                            Id = cfg.Id,
                            Description = cfg.Description,
                            Hostname = cfg.Hostname,
                            AdresseIP = ipValid ? ip : IPAddress.None,
                            TapisIds = tapisIds,
                            Groupement = cfg.Groupement,
                            Disposition = cfg.Disposition
                        };

                        // Ajuster le compteur statique pour éviter les doublons d'ID futurs
                        if (model.Id > EcranAppelModel.LastId)
                            EcranAppelModel.LastId = model.Id;

                        _ecransAppel.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        /// <summary>
        /// Calcul l'URL sur le site ecrans en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLSiteLocal()
        {
            string output = "Indefinie";
            try
            {
                if (!string.IsNullOrEmpty(IdCompetition) && SiteLocal.ServerHTTP?.ListeningIpAddress != null && SiteLocal.ServerHTTP.Port > 0 && _structureSiteInterne != null)
                {
                    string urlBase = string.Format("http://{0}:{1}/", SiteLocal.ServerHTTP.ListeningIpAddress.ToString(), SiteLocal.ServerHTTP.Port);
                    output = (new Uri(new Uri(urlBase), _structureSiteInterne.UrlPathEcransAppelRedirecteur)).ToString();
                }
            }
            catch (Exception ex)
            {
                output = string.Empty;
                LogTools.Logger.Error(ex, "Impossible de calculer l'URL du site des ecrans d'appel");
            }
            return output;
        }
        #endregion
    }
}