using AppPublication.Config.EcransAppel;
using AppPublication.Config.Publication;
using AppPublication.Controles;
using AppPublication.Generation;
using AppPublication.Models.EcransAppel;
using AppPublication.Publication;
using AppPublication.Statistiques;
using AppPublication.Tools.Files;
using AppPublication.Tools.FranceJudo;
using AppPublication.ViewModels.Configuration;
using AppPublication.Views.Configuration;
using KernelImpl;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using Tools.Enum;
using Tools.Export;
using Tools.Files;
using Tools.Framework;
using Tools.Logging;
using Tools.Net;
using Tools.Outils;
using Tools.Threading;
using Tools.Windows;

namespace AppPublication.Controles
{
    public class GestionSite : NotificationBase
    {
        #region CONSTANTES
        private const string kSiteLocalInstanceName = "local";
        private const string kSiteDistantInstanceName = "distant";
        private const string kSiteFranceJudoInstanceName = "ffjudo";
        private const string kSitePrivateInstanceName = "private";

        private const string kSiteRepertoire = "site";
        private const string kPrivateSiteRepertoire = "private-site";

        #endregion

        #region MEMBRES
        private Task _taskNettoyage = null;             // La tache de nettoyage
        private GestionStatistiques _statMgr = null;
        private GenerationScheduler _schedulerSite = null;  // Le scheduler de generation Site
        private GenerateurSite _generateurSite = null;  // Le generateur Site
        private IProgress<OperationProgress> _progressHandler = null;

        private ExportSiteStructure _structureRepertoiresSite;                      // La structure de repertoire d'export du site
        private ExportSitePrivateStructure _structureRepertoiresPrivateSite;        // La structure de repertoire d'export du site prive

        private ExportSiteUrls _structureSiteLocal;                 // la structure d'export du site local
        private ExportSiteUrls _structureSiteDistant;                 // la structure d'export du site distant

        private Dictionary<string, EntitePublicationFFJudo> _allEntitePublicationFFJudo = null;
        private Dictionary<string, ObservableCollection<EntitePublicationFFJudo>> _allEntitesPublicationFFJudo = null;

        private string _ftpEasyConfig = string.Empty;   // Le serveur FTP EasyConfig
        private Uri _httpEasyConfig = null;  // Le serveur http EasyConfig

        private IJudoDataManager _judoDataManager;                  // Le gestionnaire de données interne

        private EcranCollectionManager _ecransAppel = new EcranCollectionManager(); // La configuration des écrans d'appels


        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="dataManager">Le gestionnaire de données</param>
        /// <param name="statMgr">le gestionnaire de statitiques</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GestionSite(IJudoDataManager dataManager, GestionStatistiques statMgr)
        {
            // Impossible d'etre null
            if (dataManager == null) throw new ArgumentNullException();

            try
            {
                // Initialise les objets de gestion des sites Web. Ils chargent automatiquement leur configuration
                _siteLocal = new MiniSiteConfigurable(true, kSiteLocalInstanceName, true, false);
                _sitePrivate = new MiniSiteConfigurable(true, kSitePrivateInstanceName, true, false);
                _siteDistant = new MiniSiteConfigurable(false, kSiteDistantInstanceName, true, true);           // on utilise un prefix vide pour le site distant pour des questions de retrocompatibilite
                _siteFranceJudo = new MiniSiteConfigurable(false, kSiteFranceJudoInstanceName, false, true);    // On ne garde pas le detail des configuration pour le site FFJudo
                _statMgr = (statMgr != null) ? statMgr : new GestionStatistiques();
                _judoDataManager = dataManager;

                // Initialise le progress handler pour la generation de site
                _progressHandler = new Progress<OperationProgress>(onGenerationSiteProgressReport);

                // Le generateur de site
                _generateurSite = new GenerateurSite(_judoDataManager, SiteDistantSelectionne, _progressHandler);

                // Initialise le scheduler de generation de site
                _schedulerSite = new GenerationScheduler(_statMgr, _generateurSite);
                _schedulerSite.StateChanged += onSchedulerStateChanged;

                // Initialise la liste des logos
                InitFichiersLogo();

                // Initialise la configuration pour la publication simplifiee France Judo
                InitPublicationFFJudo();

                // Initialise la configuration via le cache de fichier
                InitCacheConfig();
            }
            catch (Exception ex)
            {
                LogTools.Logger.Fatal(ex, "Impossible d'initialiser le ViewModel principal. Impossible de continuer");
                AlertWindow win = new AlertWindow("Erreur fatale", "Impossible de démarrer un composant interne, l'application doit s'arrêter. Veuillez contacter le support.");
                if (win != null)
                {
                    win.ShowDialog();
                }

                // Emergency shutdown
                App.Current.Shutdown();
            }
        }

        #endregion

        #region PROPRIETES

        
        private ConfigurationEcransViewModel _cfgEcransAppelViewModel = null;
        /// <summary>
        /// Le ViewModel pour les ecrans (doit etre en Properties pour le binding WPF
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

        private bool _easyConfigDisponible;

        /// <summary>
        /// Flag indiquant si le mode de configuration simplifie est disponible
        /// </summary>
        public bool EasyConfigDisponible
        {
            get
            {
                return _easyConfigDisponible;
            }
            private set
            {
                if (_easyConfigDisponible != value)
                {
                    _easyConfigDisponible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _easyConfig;

        /// <summary>
        /// Flag indiquant si le mode de configuration simplifie est selectionne (True). == !AdvancedConfig
        /// </summary>
        public bool EasyConfig
        {
            get
            {
                return _easyConfig;
            }
            set
            {
                // On ne peut changer la valeur que si le site en cours n'est pas actif
                if (SiteDistantSelectionne == null || !SiteDistantSelectionne.IsActif)
                {
                    // Enregistre la valeur en cache
                    PublicationConfigSection.Instance.EasyConfig = (_easyConfig = value);

                    NotifyPropertyChanged();

                    // Met a jour le site distant selectionne
                    SiteDistantSelectionne = CalculSiteDistantSelectionne();
                }
            }
        }

        /// <summary>
        /// Flag indiquant si le mode de configuration avance est selectionne (True). == !EasyConfig
        /// </summary>
        public bool AdvancedConfig
        {
            get
            {
                return !EasyConfig;
            }
            set
            {
                EasyConfig = !value;
                NotifyPropertyChanged();
                // Inutile, le fait de faire le set sur EasyConfig suffit a mettre a jour le site selectionne
                // SiteDistantSelectionne = CalculSiteDistantSelectionne();
            }
        }


        /// <summary>
        /// Le MiniSite selectionne en fonction du mode de configuration
        /// </summary>

        private MiniSite _siteDistantSelectionne;
        public MiniSite SiteDistantSelectionne
        {
            get
            {
                return _siteDistantSelectionne;
            }
            private set
            {
                _siteDistantSelectionne = value;

                // Met a jour le SiteProvider du generateur de site
                _generateurSite.SiteProvider = _siteDistantSelectionne;

                // Il faut recalculer l'URL du site de publication car on vient de changer de site
                URLDistantPublication = CalculURLSiteDistant();
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<string> _listeNiveauxPublicationFFJudo;
        /// <summary>
        /// La liste des niveaux de publication
        /// </summary>
        public ObservableCollection<string> ListeNiveauxPublicationFFJudo
        {
            get
            {
                return _listeNiveauxPublicationFFJudo;
            }
            set
            {
                if (_listeNiveauxPublicationFFJudo != value)
                {
                    _listeNiveauxPublicationFFJudo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<EntitePublicationFFJudo> _listeEntitesPublicationFFJudo;

        /// <summary>
        /// La liste de toutes les entites de publication existantes pour le niveau de publication selectionne
        /// </summary>
        public ObservableCollection<EntitePublicationFFJudo> ListeEntitesPublicationFFJudo
        {
            get
            {
                return _listeEntitesPublicationFFJudo;
            }
            set
            {
                if (_listeEntitesPublicationFFJudo != value)
                {
                    _listeEntitesPublicationFFJudo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private EntitePublicationFFJudo _entitePublicationFFJudo;

        /// <summary>
        /// Entite de publication selectionnee
        /// </summary>
        public EntitePublicationFFJudo EntitePublicationFFJudo
        {
            get
            {
                return _entitePublicationFFJudo;
            }
            set
            {
                if (_entitePublicationFFJudo != value)
                {
                    _entitePublicationFFJudo = value;
                    if (value != null)
                    {
                        // Garde en memoire la derniere valeur sauvegardee pour ce niveau
                        _allEntitePublicationFFJudo[_niveauPublicationFFJudo] = value;
                        PublicationConfigSection.Instance.EntitePublicationFFJudo = _entitePublicationFFJudo.Nom;

                        // On Calcul les parametres FTP en fonction de l'entite selectionne
                        GenereConfigFTPFranceJudo(value);
                    }
                    NotifyPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Les entites de publication selectionnees par niveau
        /// </summary>
        public Dictionary<string, EntitePublicationFFJudo> AllEntitePublicationFFJudo
        {
            get
            {
                return _allEntitePublicationFFJudo;
            }
            set
            {
                if (_allEntitePublicationFFJudo != value)
                {
                    _allEntitePublicationFFJudo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _niveauPublicationFFJudo;
        /// <summary>
        /// Le niveau de publication selectionne
        /// </summary>
        public string NiveauPublicationFFJudo
        {
            get
            {
                return _niveauPublicationFFJudo;
            }
            set
            {
                if (_niveauPublicationFFJudo != value)
                {
                    PublicationConfigSection.Instance.NiveauPublicationFFJudo = (_niveauPublicationFFJudo = value);

                    // Ajuste la liste des entites et restaure le dernier element selectionne pour ce niveau
                    ObservableCollection<EntitePublicationFFJudo> ent = null;
                    try
                    {
                        ent = _allEntitesPublicationFFJudo[_niveauPublicationFFJudo];
                    }
                    catch
                    {
                        ent = null;
                    }
                    finally
                    {
                        ListeEntitesPublicationFFJudo = ent;
                        // La selection de l'entite provoque automatiquement la mise a jour des parametres EasyConfig
                        EntitePublicationFFJudo = _allEntitePublicationFFJudo[_niveauPublicationFFJudo];
                    }

                    NotifyPropertyChanged();
                }
            }
        }

        private bool _pouleEnColonnes;
        /// <summary>
        /// Type d'affichage des Poules
        /// </summary>
        public bool PouleEnColonnes
        {
            get
            {
                return _pouleEnColonnes;
            }
            set
            {
                if (_pouleEnColonnes != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.PouleEnColonnes = value;

                    PublicationConfigSection.Instance.PouleEnColonnes = (_pouleEnColonnes = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _pouleToujoursEnColonnes;
        /// <summary>
        /// Force l'affichage des poules en colonnes
        /// </summary>
        public bool PouleToujoursEnColonnes
        {
            get
            {
                return _pouleToujoursEnColonnes;
            }
            set
            {
                if (_pouleToujoursEnColonnes != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.PouleToujoursEnColonnes = value;

                    PublicationConfigSection.Instance.PouleToujoursEnColonnes = (_pouleToujoursEnColonnes = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private int _tailleMaxPouleColonnes;
        /// <summary>
        /// Taille max d'une poule pour l'affichage en colonnes
        /// </summary>
        public int TailleMaxPouleColonnes
        {
            get
            {
                return _tailleMaxPouleColonnes;
            }
            set
            {
                if (_tailleMaxPouleColonnes != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.TailleMaxPouleColonnes = value;
                    
                    PublicationConfigSection.Instance.TailleMaxPouleColonnes = (_tailleMaxPouleColonnes = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private string _repertoireRacine;
        /// <summary>
        /// Le répertoire Racine configuré oar l'utilisateur
        /// </summary>
        public string RepertoireRacine
        {
            get
            {
                return _repertoireRacine;
            }
            set
            {
                if (value != _repertoireRacine)
                {
                    PublicationConfigSection.Instance.RepertoireRacine = (_repertoireRacine = value);
                    NotifyPropertyChanged();

                    // Met a jour la constante d'export
                    string tmp = OutilsTools.GetExportDir(_repertoireRacine);
                    string siteRoot = Path.Combine(tmp, kSiteRepertoire);

                    // Initialise les structures d'export
                    _structureRepertoiresSite = new ExportSiteStructure(siteRoot, IdCompetition);
                    _structureRepertoiresPrivateSite = new ExportSitePrivateStructure(Path.Combine(tmp, kPrivateSiteRepertoire));

                    _structureSiteDistant = new ExportSiteUrls(_structureRepertoiresSite);
                    _structureSiteLocal = new ExportSiteUrls(_structureRepertoiresSite);

                    // Propage la valeur au generateur de site
                    _generateurSite.StructureRepertoire = _structureRepertoiresSite;

                    // Met a jour les repertoires de l'application
                    InitExportSiteStructure();

                    // Initialise la racine du serveur Web local
                    SiteLocal.ServerHTTP.LocalRootPath = siteRoot;
                    // TODO initialiser le repoertoire racine du site prive
                }
            }
        }

        ObservableCollection<FilteredFileInfo> _fichiersLogo = new ObservableCollection<FilteredFileInfo>();
        /// <summary>
        /// La liste des fichiers Logos disponibles
        /// </summary>
        public ObservableCollection<FilteredFileInfo> FichiersLogo
        {
            get
            {
                return _fichiersLogo;
            }
            private set
            {
                if (_fichiersLogo != value)
                {
                    _fichiersLogo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        FilteredFileInfo _selectedLogo = null;
        /// <summary>
        /// Le fichier logo sélectionné
        /// </summary>
        public FilteredFileInfo SelectedLogo
        {
            get
            {
                return _selectedLogo;
            }
            set
            {
                if (_selectedLogo != value)
                {
                    string logoName = (value != null) ? value.Name : string.Empty;
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.Logo = logoName;

                    _selectedLogo = value;
                    PublicationConfigSection.Instance.Logo = logoName;
                    NotifyPropertyChanged();
                }
            }
        }

        TaskExecutionInformation _statGeneration;
        /// <summary>
        /// Statistique de derniere generation - lecture seule
        /// </summary>
        public TaskExecutionInformation DerniereGeneration
        {
            get
            {
                return _statGeneration;
            }
            private set
            {
                _statGeneration = value;
                NotifyPropertyChanged();
            }
        }

        TaskExecutionInformation _statSyncDistant;
        /// <summary>
        /// Statistiques de derniere synchronisation - lecture seule
        /// </summary>
        public TaskExecutionInformation DerniereSynchronisation
        {
            get
            {
                return _statSyncDistant;
            }
            private set
            {
                _statSyncDistant = value;
                NotifyPropertyChanged();
            }
        }

        bool _siteGenere = false;
        /// <summary>
        /// Indique si le site a ete bien genere (true) - lecture seule
        /// </summary>
        public bool SiteGenere
        {
            get
            {
                return _siteGenere;
            }
            private set
            {
                _siteGenere = value;
                NotifyPropertyChanged();
            }
        }

        bool _siteSynchronise = false;
        /// <summary>
        /// Indique si le site a bien ete synchronnise - lecture seule
        /// </summary>
        public bool SiteSynchronise
        {
            get
            {
                return _siteSynchronise;
            }
            private set
            {
                _siteSynchronise = value;
                NotifyPropertyChanged();
            }
        }

        private MiniSite _siteLocal = null;
        /// <summary>
        /// Le site de publication local
        /// </summary>
        public MiniSite SiteLocal
        {
            get
            {
                return _siteLocal;
            }
        }

        private MiniSite _sitePrivate = null;
        /// <summary>
        /// Le site de publication local des ecrans d'appel
        /// </summary>
        public MiniSite SitePrivate
        {
            get
            {
                return _sitePrivate;
            }
        }

        /// <summary>
        /// Propriete passerelle pour selectionner l'interface de publication du site ecrans
        /// Permet de tenir a jour le QR code de l'URL de publication
        /// </summary>
        public IPAddress InterfacePrivateSite
        {
            get
            {
                return SitePrivate.InterfaceLocalPublication;
            }
            set
            {
                // Verifie que la valeur selectionnee est bien dans la liste des interfaces
                try
                {
                    SitePrivate.InterfaceLocalPublication = value;
                    NotifyPropertyChanged();
                    URLEcransAppelPublication = CalculURLPrivateSite();
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        /// <summary>
        /// Propriete passerelle pour selectionner l'interface de publication du site local
        /// Permet de tenir a jour le QR code de l'URL de publication
        /// </summary>
        public IPAddress InterfaceLocalPublication
        {
            get
            {
                return SiteLocal.InterfaceLocalPublication;
            }
            set
            {
                // Verifie que la valeur selectionnee est bien dans la liste des interfaces
                try
                {
                    SiteLocal.InterfaceLocalPublication = value;
                    NotifyPropertyChanged();
                    URLLocalPublication = CalculURLSiteLocal();
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        private MiniSite _siteDistant = null;
        /// <summary>
        /// Le site de publication distant
        /// </summary>
        public MiniSite SiteDistant
        {
            get
            {
                return _siteDistant;
            }
        }

        private MiniSite _siteFranceJudo = null;
        /// <summary>
        /// Le site de publication distant sur les serveurs de France Judo
        /// </summary>
        public MiniSite SiteFranceJudo
        {
            get
            {
                return _siteFranceJudo;
            }
        }

        bool _generationActive = false;
        /// <summary>
        /// Etat de la generation du site
        /// </summary>
        public bool IsGenerationActive
        {
            get
            {
                return _generationActive;
            }
            private set
            {
                _generationActive = value;
                NotifyPropertyChanged();
            }
        }

        bool _isolerCompetition = false;
        /// <summary>
        /// Isole les competitions avec leur ID lors de l'upload sur le site distant
        /// </summary>
        public bool IsolerCompetition
        {
            get
            {
                return _isolerCompetition;
            }
            set
            {
                PublicationConfigSection.Instance.IsolerCompetition = (_isolerCompetition = value);

                // Met a jour la structure d'export
                if (_structureSiteDistant != null)
                {
                    _structureSiteDistant.CompetitionIsolee = _isolerCompetition;
                }
                NotifyPropertyChanged();
                URLDistantPublication = CalculURLSiteDistant();
                if (SiteDistantSelectionne != null)
                {
                    SiteDistantSelectionne.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();
                }
            }
        }

        private int _nbProchainsCombats = 6;
        /// <summary>
        /// Nb de prochains combats a publier pour la chambre d'appel
        /// </summary>
        public int NbProchainsCombats
        {
            get
            {
                return _nbProchainsCombats;
            }
            set
            {
                if (_nbProchainsCombats != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.NbProchainsCombats = value;

                    PublicationConfigSection.Instance.NbProchainsCombats = (_nbProchainsCombats = value);
                    NotifyPropertyChanged();
                }
            }
        }


        int _delaiGenerationSec = 30;
        /// <summary>
        /// Delai entre 2 generations du site
        /// </summary>
        public int DelaiGenerationSec
        {
            get
            {
                return _delaiGenerationSec;
            }
            set
            {
                if (_delaiGenerationSec != value)
                {
                    // Configure le scheduler
                    _schedulerSite.DelaiGenerationSec = value;

                    PublicationConfigSection.Instance.DelaiGenerationSec = (_delaiGenerationSec = value);
                    NotifyPropertyChanged();
                }
            }
        }

        bool _effacerAuDemarrage = true;
        /// <summary>
        /// Indique si on doit faire un RAZ du contenu du répertoire au demarrage de la generation
        /// </summary>
        public bool EffacerAuDemarrage
        {
            get
            {
                return _effacerAuDemarrage;
            }
            set
            {
                if (_effacerAuDemarrage != value)
                {
                    // Configure le scheduler
                    _schedulerSite.EffacerAuDemarrage = value;

                    PublicationConfigSection.Instance.EffacerAuDemarrage = (_effacerAuDemarrage = value);
                    NotifyPropertyChanged();
                }
            }
        }


        int _delaiActualisationClientSec = 30;
        /// <summary>
        /// Delai entre 2 generations du site
        /// </summary>
        public int DelaiActualisationClientSec
        {
            get
            {
                return _delaiActualisationClientSec;
            }
            set
            {
                if (_delaiActualisationClientSec != value)
                {
                    // Propage au generateur de site
                    _generateurSite.ConfigurationGeneration.DelaiActualisationClientSec = value;

                    PublicationConfigSection.Instance.DelaiActualisationClientSec = (_delaiActualisationClientSec = value);
                    NotifyPropertyChanged();
                }
            }
        }

        string _msgProchainsCombats = string.Empty;
        /// <summary>
        /// Message optionnel pour les prochains coùbats
        /// </summary>
        public string MsgProchainsCombats
        {
            get
            {
                return _msgProchainsCombats;
            }
            set
            {
                if (_msgProchainsCombats != value)
                {
                    // propage au generateur de site
                    _generateurSite.ConfigurationGeneration.MsgProchainsCombats = value;

                    PublicationConfigSection.Instance.MsgProchainsCombats = (_msgProchainsCombats = value);
                    NotifyPropertyChanged();
                }
            }
        }


        private string _urlDistant;
        /// <summary>
        /// URL racine du site distant de publication
        /// </summary>
        public string URLDistant
        {
            get
            {
                return _urlDistant;
            }
            set
            {
                if (_urlDistant != value)
                {
                    PublicationConfigSection.Instance.URLDistant = (_urlDistant = value);
                    NotifyPropertyChanged();
                    URLDistantPublication = CalculURLSiteDistant();
                }
            }
        }

        private string _urlDistantPublication;
        /// <summary>
        /// URL Complete sur le site distant de publication
        /// </summary>
        public string URLDistantPublication
        {
            get
            {
                return _urlDistantPublication;
            }
            private set
            {
                _urlDistantPublication = value;
                NotifyPropertyChanged();
            }
        }

        private string _urlLocalPublication;
        /// <summary>
        /// URL sur le site local
        /// </summary>
        public string URLLocalPublication
        {
            get
            {
                return _urlLocalPublication;
            }
            private set
            {
                _urlLocalPublication = value;
                NotifyPropertyChanged();
            }
        }

        private string _urlEcransAppelPublication;
        /// <summary>
        /// URL pour le site local des ecrans d'appel
        /// </summary>
        public string URLEcransAppelPublication
        {
            get
            {
                return _urlEcransAppelPublication;
            }
            private set
            {
                _urlEcransAppelPublication = value;
                NotifyPropertyChanged();
            }
        }

        private string _ftpRepertoireRacineDistant;
        /// <summary>
        /// Repertoire racine cible sur le site distant
        /// </summary>
        public string RepertoireRacineSiteFTPDistant
        {
            get
            {
                return _ftpRepertoireRacineDistant;
            }
            set
            {
                if (_ftpRepertoireRacineDistant != value)
                {
                    PublicationConfigSection.Instance.RepertoireRacineSiteFTPDistant = (_ftpRepertoireRacineDistant = value);
                    NotifyPropertyChanged();
                    SiteDistant.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();   // Ce parametre ne concerne pas le site FranceJudo
                }
            }
        }




        private string _idCompetition = string.Empty;
        /// <summary>
        /// ID de la competition en cours
        /// </summary>
        public string IdCompetition
        {
            get
            {
                return _idCompetition;
            }
            set
            {
                _idCompetition = value;
                NotifyPropertyChanged();

                // Met a jour la structure d'export
                if (_structureRepertoiresSite != null)
                {
                    _structureRepertoiresSite.IdCompetition = value;
                }

                // Recalcul les valeurs des URLs et répertoires distants
                if (SiteDistantSelectionne != null)
                {
                    SiteDistantSelectionne.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();
                }
                URLDistantPublication = CalculURLSiteDistant();
                URLLocalPublication = CalculURLSiteLocal();
                // Note: ici on devrait dans l'absolu utiliser le snapshot mais le traitement est rapide et a peu de chance de changer
                var DC = _judoDataManager.Data;
                CanPublierAffectation = DC.Organisation.Competition.IsIndividuelle();
                CanPublierEngagements = DC.Organisation.Competition.IsIndividuelle() || DC.Organisation.Competition.IsShiai();

                // Le nombre de tapis peut avoir changer selon la compétition
                NbTapis = DC.Organisation.Competition.nbTapis;

                // Si on est en Shiai, par defaut on met les poules en colonnes
                if (DC.Organisation.Competition.IsShiai())
                {
                    PouleEnColonnes = true;
                    PouleToujoursEnColonnes = true;
                }
            }
        }

        private int _nbTapis = 6;
        public int NbTapis
        {
            get
            {
                return _nbTapis;
            }
            set
            {
                if (_nbTapis != value)
                {
                    _nbTapis = value;
                    // RAZ le viewModel des ecrans d'appel, cela forcera la recreation avec le nouveau nombre de tapis en cas de nouvelle configuration
                    _cfgEcransAppelViewModel = null;

                    NotifyPropertyChanged();
                }
            }
        }

        private bool _canPublierAffectation = true;
        /// <summary>
        /// Indique si on peut publier l'affectation des tapis ou nnon
        /// </summary>
        public bool CanPublierAffectation
        {
            get
            {
                return _canPublierAffectation;
            }
            private set
            {
                // Propage la valeur au generateur de site
                _generateurSite.ConfigurationGeneration.PublierAffectationTapis = value && PublierAffectationTapis;

                _canPublierAffectation = value;
                NotifyPropertyChanged();
            }
        }

        private bool _canPublierEngagements = true;
        /// <summary>
        /// Indique si on peut publier les engages ou non
        /// </summary>
        public bool CanPublierEngagements
        {
            get
            {
                return _canPublierEngagements;
            }
            private set
            {
                // Propage la valeur au generateur de site
                _generateurSite.ConfigurationGeneration.PublierEngagements = value && PublierEngagements;

                _canPublierEngagements = value;
                NotifyPropertyChanged();
            }
        }

        private bool _publierProchainsCombats = false;
        /// <summary>
        /// Indique si on doit publier la liste des prochains combats ou non
        /// </summary>
        public bool PublierProchainsCombats
        {
            get
            {
                return _publierProchainsCombats;
            }
            set
            {
                if (_publierProchainsCombats != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.PublierProchainsCombats = value;

                    PublicationConfigSection.Instance.PublierProchainsCombats = (_publierProchainsCombats = value);
                    NotifyPropertyChanged();
                }
            }
        }



        private bool _publierAffectationTapis = false;
        /// <summary>
        /// Indique si on doit publier la liste des prochains combats ou non
        /// </summary>
        public bool PublierAffectationTapis
        {
            get
            {
                return _publierAffectationTapis;
            }
            set
            {
                if (_publierAffectationTapis != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.PublierAffectationTapis = value && CanPublierAffectation;

                    PublicationConfigSection.Instance.PublierAffectationTapis = (_publierAffectationTapis = value);
                    NotifyPropertyChanged();
                }
            }
        }



        private bool _publierEngagements = false;
        /// <summary>
        /// Indique si on doit publier la liste des engages
        /// </summary>
        public bool PublierEngagements
        {
            get
            {
                return _publierEngagements;
            }
            set
            {
                if (_publierEngagements != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.PublierEngagements = value && CanPublierEngagements;

                    PublicationConfigSection.Instance.PublierEngagements = (_publierEngagements = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _engagementsAbsents = false;

        /// <summary>
        /// Indique si on doit publier les judokas absents
        /// </summary>
        public bool EngagementsAbsents
        {
            get
            {
                return _engagementsAbsents;
            }
            set
            {
                if (_engagementsAbsents != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.EngagementsAbsents = value;

                    PublicationConfigSection.Instance.EngagementsAbsents = (_engagementsAbsents = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _engagementsTousCombats = false;

        /// <summary>
        /// Indique si on doit publier tous les combats des judokas, finis ou non
        /// </summary>
        public bool EngagementsTousCombats
        {
            get
            {
                return _engagementsTousCombats;
            }
            set
            {
                if (_engagementsTousCombats != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.EngagementsTousCombats = value;

                    PublicationConfigSection.Instance.EngagementsTousCombats = (_engagementsTousCombats = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _useIntituleCommun;
        /// <summary>
        /// Flag indiquant si on doit utiliser un intitule commun en cas de poly competition
        /// </summary>
        public bool UseIntituleCommun
        {
            get { return _useIntituleCommun; }
            set
            {
                if (_useIntituleCommun != value)
                {
                    // propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.UseIntituleCommun = value;

                    PublicationConfigSection.Instance.UseIntituleCommun = (_useIntituleCommun = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private string _intituleCommun;

        /// <summary>
        /// intitule commun en cas de poly competition
        /// </summary>
        public String IntituleCommun
        {
            get { return _intituleCommun; }
            set
            {
                if (_intituleCommun != value)
                {
                    // propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.IntituleCommun = value;

                    PublicationConfigSection.Instance.IntituleCommun = (_intituleCommun = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _scoreEngagesGagnantPerdant;
        public bool ScoreEngagesGagnantPerdant
        {
            get
            {
                return _scoreEngagesGagnantPerdant;
            }
            set
            {
                if (_scoreEngagesGagnantPerdant != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.EngagementsScoreGP = value;

                    PublicationConfigSection.Instance.ScoreEngagesGagnantPerdant = (_scoreEngagesGagnantPerdant = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _afficherPositionCombat;
        public bool AfficherPositionCombat
        {
            get
            {
                return _afficherPositionCombat;
            }
            set
            {
                if (_afficherPositionCombat != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite.ConfigurationGeneration.AfficherPositionCombat = value;

                    PublicationConfigSection.Instance.AfficherPositionCombat = (_afficherPositionCombat = value);
                    NotifyPropertyChanged();
                }
            }
        }



        private StatusGenerationSite _status;
        /// <summary>
        /// Le statut de generation du site
        /// </summary>
        public StatusGenerationSite Status
        {
            get
            {
                if (null == _status)
                {
                    _status = new StatusGenerationSite();
                }
                return _status;
            }
            set
            {
                _status = value;
                NotifyPropertyChanged();
                IsGenerationActive = !(_status.State == StateGenerationEnum.Stopped);
            }
        }
        #endregion

        #region COMMANDES

        private ICommand _cmdAjouterLogo;

        /// <summary>
        /// Commande permettant d'ajouter un logo dans la liste
        /// </summary>
        public ICommand CmdAjouterLogo
        {
            get
            {
                if (_cmdAjouterLogo == null)
                {
                    _cmdAjouterLogo = new RelayCommand(
                            o =>
                            {
                                bool allFileOk = true;

                                OpenFileDialog op = new OpenFileDialog();
                                op.Title = "Sélectionner une image";
                                op.Filter = "Portable Network Graphic (*.png)|*.png";
                                op.Multiselect = true;
                                op.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                                op.RestoreDirectory = true;
                                if (op.ShowDialog() == DialogResult.OK)
                                {
                                    foreach (string imgFile in op.FileNames)
                                    {
                                        try
                                        {
                                            if (imgFile.ToLower().Contains("logo"))
                                            {
                                                int w, h;

                                                using (var stream = new FileStream(imgFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                                                {
                                                    var bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                                                    w = bitmapFrame.PixelWidth;
                                                    h = bitmapFrame.PixelHeight;

                                                    // Verifie la taille de l'image
                                                    if (w <= 200 && h <= 200)
                                                    {
                                                        FilteredFileInfo newItem = new FilteredFileInfo(new FileInfo(imgFile));

                                                        // Copy le fichier dans le répertoire de travail de l'application
                                                        File.Copy(newItem.FullName, Path.Combine(ConstantFile.ExportStyle_dir, newItem.Name));

                                                        // Actualise la liste des logos
                                                        FichiersLogo.Add(newItem);
                                                    }
                                                    else
                                                    {
                                                        LogTools.Logger.Debug("Fichier '{0}' ignore - taille {1}x{2} incorrecte", imgFile, w, h);
                                                        allFileOk = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LogTools.Logger.Debug("Fichier '{0}' ignore - Nom ne contient pas 'logo'", imgFile);
                                                allFileOk = false;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LogTools.Logger.Debug("Fichier '{0}' ignore - Exception lors de la lecture du format", imgFile, ex);
                                            allFileOk = false;
                                        }
                                    }

                                    if (!allFileOk)
                                    {
                                        AlertWindow win = new AlertWindow("Infomation", "Certains fichiers n'ont pas put être chargé. Veuillez vérifier les noms, formats et dimensions");
                                        if (win != null)
                                        {
                                            win.ShowDialog();
                                        }
                                    }
                                }

                            },
                            o =>
                            {
                                // Meme si le site est demarre on peut ajouter un logo, il n'est pas pris automatiquement enc compte
                                return true;
                            });
                }
                return _cmdAjouterLogo;
            }
        }

        private ICommand _cmdGetRepertoireRacine;
        /// <summary>
        /// Commande pour gérer la selection du repertoire Racine
        /// </summary>
        public ICommand CmdGetRepertoireRacine
        {
            get
            {
                if (_cmdGetRepertoireRacine == null)
                {
                    _cmdGetRepertoireRacine = new RelayCommand(
                            o =>
                            {
                                string output = string.Empty;

                                FolderBrowserDialog dlg = new FolderBrowserDialog();
                                dlg.Description = "Sélectionner le répertoire à utiliser pour les exports";
                                dlg.ShowNewFolderButton = true;
                                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    output = dlg.SelectedPath;
                                }
                                RepertoireRacine = output;
                            },
                            o =>
                            {
                                // On ne peut modifier le repertoire racine que si tous les processus sont arretes
                                return (SiteDistantSelectionne != null) ? !SiteDistantSelectionne.IsActif && !SiteLocal.IsActif && !IsGenerationActive : true;
                            });
                }
                return _cmdGetRepertoireRacine;
            }
        }

        #endregion

        #region METHODES

        private void onGenerationSiteProgressReport(OperationProgress valueReported)
        {
            LogTools.Logger.Debug($"Progress {valueReported} signale par le generateur");

            // on doit juste s'assurer que tout est bien execute dans le UI Thread
            System.Windows.Application.Current.ExecOnUiThread(() =>
            {
                if (valueReported != null && valueReported.Etape == EtapeGenerateurSiteEnum.ExecuteGeneration)
                {
                    // Clone le status courant
                    StatusGenerationSite cpy = Status.Clone();

                    // Met a jour le status avec la nouvelle progression et notifie les changements
                    cpy.Progress = (int)Math.Round(valueReported.ProgressPercent * 100);
                    Status = cpy;
                }
            });
        }

        /// <summary>
        /// Gestionnaire d'evenement pour les changements d'etat du scheduler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evt"></param>
        private void onSchedulerStateChanged(object sender, SchedulerStateEventArgs evt)
        {
            LogTools.Logger.Debug($"Event {evt.State} signale par le scheduler");

            // on doit juste s'assurer que tout est bien execute dans le UI Thread
            System.Windows.Application.Current.ExecOnUiThread(() =>
                {
                    // Clone le status courant
                    StatusGenerationSite cpy = Status.Clone();

                    // Met a jour l'etat avec celui reçu s'il est documente, notifie les changements en assignant la propriete
                    if (evt.State != StateGenerationEnum.None) { cpy.State = evt.State; }
                    Status = cpy;

                    // Verifie si on a des infos d'exécution signalées
                    if(evt.InfosExecution != null)
                    {
                        switch(evt.State)
                        {
                            case StateGenerationEnum.Syncing:
                                SiteSynchronise = evt.InfosExecution.IsSuccess;
                                DerniereSynchronisation = evt.InfosExecution;
                                break;
                            default:
                                SiteGenere = evt.InfosExecution.IsSuccess;
                                DerniereGeneration = evt.InfosExecution;
                                break;
                        }
                    }

                    // Met a jour le delai avant la prochaine generation s'il est documente
                    if (evt.DelaiNextSec != long.MinValue)
                    {
                        // On n'a pas de delai, on met a zero
                        cpy.NextGenerationSec = (int)evt.DelaiNextSec;
                    }
                });
        }

        /// <summary>
        /// Assure l'initialisation de la structure du site
        /// </summary>
        private void InitExportSiteStructure()
        {
            // TODO Ajouter ici la strucuture specifique au Ecrans d'appel

            if (_structureRepertoiresSite != null)
            {
                FileAndDirectTools.CreateDirectorie(_structureRepertoiresSite.RepertoireRacine);
            }
        }

        /// <summary>
        /// Initialise la liste des fichiers de logos
        /// </summary>
        private void InitFichiersLogo()
        {
            // Recupere le repertoire des images du site
            IEnumerable<FilteredFileInfo> files = ExportTools.EnumerateCustomLogoFiles().Select(o => new FilteredFileInfo(o)).OrderBy(o => o.Name);

            // Liste les fichiers logos
            FichiersLogo = new ObservableCollection<FilteredFileInfo>(files);
        }

        /// <summary>
        /// Initialise la liste des comites et ligues pour la publication sur les serveurs France Judo
        /// Une adresse Web sera definie par http://{Attribut "http" de <Publication>}/{Attribut "racineHttp" de <Entite>}/{ID competition ou "courante"}/...
        /// L'adresse de destination FTP sera definie par ftp://{Attribut "ftp" de <Publication>}/{Attribut "racineFtp" de <Entite>}/{ID competition ou "courante"}/...
        /// </summary>
        private void InitPublicationFFJudo()
        {
            try
            {
                // Charge la structure XML en memoire depuis les resources
                XmlReader structureReader = XmlReader.Create(ResourcesTools.GetAssembyResource(ConstantResource.PublicationFFJUDO));

                XmlDocument doc = new XmlDocument();
                doc.Load(structureReader);

                // Charge les informations sur le serveur de publication depuis l'element racine
                // <Publication ftp="ftp.ffjudo.com" http="http://ftp.ffjudo.com">
                if (doc.DocumentElement == null || doc.DocumentElement.Name != ConstantXML.EasyConfig_Racine)
                {
                    throw new NullReferenceException("Racine du fichier de configuration inconnue ou manquante");
                }
                XmlElement root = doc.DocumentElement;

                if (root.Attributes == null || root.Attributes[ConstantXML.EasyConfig_Racine_Ftp] == null
                    || string.IsNullOrEmpty(root.Attributes[ConstantXML.EasyConfig_Racine_Ftp].Value)
                    || root.Attributes[ConstantXML.EasyConfig_Racine_Http] == null || string.IsNullOrEmpty(root.Attributes[ConstantXML.EasyConfig_Racine_Http].Value))
                {
                    throw new NullReferenceException("Attributs manquants a la racine");
                }
                _ftpEasyConfig = root.Attributes[ConstantXML.EasyConfig_Racine_Ftp].Value;
                _httpEasyConfig = new Uri(root.Attributes[ConstantXML.EasyConfig_Racine_Http].Value);

                // Parcours les elements
                if (doc.DocumentElement.HasChildNodes)
                {
                    ObservableCollection<string> tmp = new ObservableCollection<string>();
                    _allEntitePublicationFFJudo = new Dictionary<string, EntitePublicationFFJudo>();
                    _allEntitesPublicationFFJudo = new Dictionary<string, ObservableCollection<EntitePublicationFFJudo>>();

                    // Extrait chaque element comme une entite si les attributs necessaires sont presents
                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        ObservableCollection<EntitePublicationFFJudo> tmpNiveau = new ObservableCollection<EntitePublicationFFJudo>();
                        if (node.HasChildNodes && node.Attributes != null && node.Attributes[ConstantXML.EasyConfig_Entite_Echelon] != null)
                        {
                            // Element commun pour la suite
                            int ech = int.Parse(node.Attributes[ConstantXML.EasyConfig_Entite_Echelon].Value);

                            foreach (XmlNode childNode in node.ChildNodes)
                            {
                                // Parcours les differentes entites
                                if (childNode.Attributes != null && childNode.Attributes[ConstantXML.EasyConfig_Entite_Nom] != null
                                    && childNode.Attributes[ConstantXML.EasyConfig_Entite_Libelle] != null
                                    && childNode.Attributes[ConstantXML.EasyConfig_Entite_Login] != null
                                    && childNode.Attributes[ConstantXML.EasyConfig_Entite_RacineFtp] != null
                                    && childNode.Attributes[ConstantXML.EasyConfig_Entite_RacineHttp] != null)
                                {
                                    tmpNiveau.Add(new EntitePublicationFFJudo(childNode.Attributes[ConstantXML.EasyConfig_Entite_Nom].Value,
                                                                                childNode.Attributes[ConstantXML.EasyConfig_Entite_Libelle].Value,
                                                                                ech,
                                                                                childNode.Attributes[ConstantXML.EasyConfig_Entite_Login].Value,
                                                                                childNode.Attributes[ConstantXML.EasyConfig_Entite_RacineFtp].Value,
                                                                                childNode.Attributes[ConstantXML.EasyConfig_Entite_RacineHttp].Value));
                                }
                            }

                            // On ne tient compte d'un niveau que s'il a des entites en dessous
                            if (tmpNiveau.Count > 0)
                            {
                                tmp.Add(node.Name);
                                _allEntitePublicationFFJudo.Add(node.Name, tmpNiveau.First());
                                _allEntitesPublicationFFJudo.Add(node.Name, tmpNiveau);
                            }
                        }
                    }
                    ListeNiveauxPublicationFFJudo = tmp;
                    EasyConfigDisponible = true;
                }
            }
            catch (Exception ex)
            {
                // On ne peut pas initialiser le mode EasyConfig
                LogTools.Logger.Error(ex, "Desactivation du mode easyConfig - Configuration absente ou incorrecte");
                EasyConfig = false;
                EasyConfigDisponible = false;
            }
        }

        /// <summary>
        /// Initialise les donnees a partir du cache de fichier AppConfig
        /// </summary>
        private void InitCacheConfig()
        {
            try
            {
                // La lecture de la config ne permet pas d'initialiser la structure du site correctement

                // On lit le repertoire racine en 1er afin de pouvoir initialiser la structure du site
                RepertoireRacine = PublicationConfigSection.Instance.RepertoireRacine;

                // Charge les valeurs pour la publication FFJudo
                if (EasyConfigDisponible)
                {
                    EasyConfig = PublicationConfigSection.Instance.EasyConfig;

                    // On charge le nom de l'entite en 1er car sinon, en initialisant la liste des niveaux, on fait un reset de la valeur de l'entite a la 1ere de la liste du niveau
                    string tmp = PublicationConfigSection.Instance.EntitePublicationFFJudo;

                    // Charge le niveau selectionne
                    NiveauPublicationFFJudo = PublicationConfigSection.Instance.GetNiveauPublicationFFJudo(ListeNiveauxPublicationFFJudo, o => o);

                    // Recherche l'entite a partir de la valeur initiale lue
                    EntitePublicationFFJudo = PublicationConfigSection.Instance.GetEntitePublicationFFJudo(ListeEntitesPublicationFFJudo, o => o.Nom, tmp);
                }

                // Les autres parametres peuvent suivre
                URLDistant = PublicationConfigSection.Instance.URLDistant;
                IsolerCompetition = PublicationConfigSection.Instance.IsolerCompetition;
                RepertoireRacineSiteFTPDistant = PublicationConfigSection.Instance.RepertoireRacineSiteFTPDistant;
                PublierProchainsCombats = PublicationConfigSection.Instance.PublierProchainsCombats;
                NbProchainsCombats = PublicationConfigSection.Instance.NbProchainsCombats;
                PublierAffectationTapis = PublicationConfigSection.Instance.PublierAffectationTapis;
                PublierEngagements = PublicationConfigSection.Instance.PublierEngagements;
                EngagementsAbsents = PublicationConfigSection.Instance.EngagementsAbsents;
                EngagementsTousCombats = PublicationConfigSection.Instance.EngagementsTousCombats;
                DelaiGenerationSec = PublicationConfigSection.Instance.DelaiGenerationSec;
                EffacerAuDemarrage = PublicationConfigSection.Instance.EffacerAuDemarrage;
                DelaiActualisationClientSec = PublicationConfigSection.Instance.DelaiActualisationClientSec;
                MsgProchainsCombats = PublicationConfigSection.Instance.MsgProchainsCombats;
                PouleEnColonnes = PublicationConfigSection.Instance.PouleEnColonnes;
                PouleToujoursEnColonnes = PublicationConfigSection.Instance.PouleToujoursEnColonnes;
                TailleMaxPouleColonnes = PublicationConfigSection.Instance.TailleMaxPouleColonnes;
                UseIntituleCommun = PublicationConfigSection.Instance.UseIntituleCommun;
                IntituleCommun = PublicationConfigSection.Instance.IntituleCommun;
                ScoreEngagesGagnantPerdant = PublicationConfigSection.Instance.ScoreEngagesGagnantPerdant;
                AfficherPositionCombat = PublicationConfigSection.Instance.AfficherPositionCombat;

                // Recherche le logo dans la liste
                SelectedLogo = PublicationConfigSection.Instance.GetLogo(FichiersLogo.ToList(), o => o.Name);

                // L'interface local de publication a ete chargee via la configuration du minisite, il faut juste s'assurer du bon calcul des URLs
                URLLocalPublication = CalculURLSiteLocal();

                URLEcransAppelPublication = CalculURLPrivateSite();

                // ici on initialise les ecrans d'appel
                InitEcransAppel();
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        /// <summary>
        /// Initialise les ecrans d'appel depuis les données en configuration
        /// </summary>
        private void InitEcransAppel()
        {
            try
            {
                // Chargement des Ecrans depuis la Config vers le Modèle Runtime
                _ecransAppel = new EcranCollectionManager();

                if (EcransAppelConfigSection.Instance != null && EcransAppelConfigSection.Instance.Ecrans != null)
                {
                    foreach (EcransAppelConfigElement cfg in EcransAppelConfigSection.Instance.Ecrans)
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
                        IPAddress ip = IPAddress.None;
                        bool ipValid = IPAddress.TryParse(cfg.AdresseIp, out ip);
                        var model = new EcranAppelModel
                        {
                            Id = cfg.Id,
                            Description = cfg.Description,
                            Hostname = cfg.Hostname,
                            AdresseIP = ipValid ? ip : IPAddress.None,
                            TapisIds = tapisIds
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
        /// Calcul l'URL sur le site distant en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLSiteDistant()
        {
            string output = "Indefinie";
            string easyConfigUrl = string.Empty;
            string urlBase = string.Empty;

            try
            {
                // Selectionne en fonction du type de configuration
                if (EasyConfig)
                {
                    // Extrait l'URL EasyConfig si possible
                    try
                    {
                        if (EntitePublicationFFJudo != null)
                        {
                            Uri fullUri = new Uri(_httpEasyConfig, EntitePublicationFFJudo.RacineHttp);
                            urlBase = fullUri.ToString();
                        }
                    }
                    catch
                    {
                        urlBase = string.Empty;
                    }
                }
                else
                {
                    urlBase = URLDistant;
                }

                if (!String.IsNullOrEmpty(urlBase) && _structureSiteDistant != null && !String.IsNullOrEmpty(_structureSiteDistant.UrlPathIndex))
                {
                    // On verifie que le dernier caractere est bien un "/" car sinon la concatenation va ignorer le dernier element du path
                    if (urlBase.Last() != '/')
                    {
                        urlBase += '/';
                    }

                    output = (new Uri(new Uri(urlBase), _structureSiteDistant.UrlPathIndex)).ToString();
                }
            }
            catch (Exception ex)
            {
                output = string.Empty;
                LogTools.Logger.Debug(ex, "Impossible de calculer l'URL du site distant");
            }
            return output;
        }

        /// <summary>
        /// Calcul l'URL sur le site local en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLSiteLocal()
        {
            string output = "Indefinie";

            try
            {
                if (!String.IsNullOrEmpty(IdCompetition) && SiteLocal.ServerHTTP != null && SiteLocal.ServerHTTP.ListeningIpAddress != null && SiteLocal.ServerHTTP.Port > 0 && _structureSiteLocal != null)
                {
                    string urlBase = string.Format("http://{0}:{1}/", SiteLocal.ServerHTTP.ListeningIpAddress.ToString(), SiteLocal.ServerHTTP.Port);

                    output = (new Uri(new Uri(urlBase), _structureSiteLocal.UrlPathIndex)).ToString();
                }
            }
            catch (Exception ex)
            {
                output = string.Empty;
                LogTools.Logger.Error(ex, "Impossible de calculer l'URL du site local");
            }
            return output;
        }

        /// <summary>
        /// Calcul l'URL sur le site ecrans en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLPrivateSite()
        {
            string output = "Indefinie";

            try
            {
                if (!String.IsNullOrEmpty(IdCompetition) && SitePrivate.ServerHTTP != null && SitePrivate.ServerHTTP.ListeningIpAddress != null && SitePrivate.ServerHTTP.Port > 0)
                {
                    string urlBase = string.Format("http://{0}:{1}/", SitePrivate.ServerHTTP.ListeningIpAddress.ToString(), SitePrivate.ServerHTTP.Port);

                    // TODO Ajouter la structure des ecrans d'appel
                    // output = (new Uri(new Uri(urlBase), _structureSiteLocal.UrlPathIndex)).ToString();
                }
            }
            catch (Exception ex)
            {
                output = string.Empty;
                LogTools.Logger.Error(ex, "Impossible de calculer l'URL du site des ecrans d'appel");
            }
            return output;
        }

        /// <summary>
        /// Retourne le site distant selectionne
        /// </summary>
        /// <returns></returns>
        private MiniSite CalculSiteDistantSelectionne()
        {
            return (EasyConfig) ? SiteFranceJudo : SiteDistant;
        }

        /// <summary>
        /// Calcul le repertoire sur le site distant en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculRepertoireSiteDistant()
        {
            string output = string.Empty;
            string repRoot = string.Empty;

            try
            {
                repRoot = (AdvancedConfig) ? RepertoireRacineSiteFTPDistant : EntitePublicationFFJudo.RacineFtp;
            }
            catch
            {
                repRoot = string.Empty;
            }

            if (!String.IsNullOrEmpty(repRoot) && _structureSiteDistant != null)
            {
                try
                {
                    output = FileAndDirectTools.PathJoin(repRoot, _structureSiteDistant.UrlPathCompetition);
                }
                catch(Exception ex)
                {
                    LogTools.Logger.Debug(ex, "Erreur lors du calcul UrlPathCompetition");
                    // on a essayer de traiter une structure non configuree sans doute
                    output = repRoot;   // par défaut, on reste sur le répertoire racine configuré
                }
            }

            return output;
        }

        /// <summary>
        /// Calcul les parametres FTP pour le MiniSite France Judo
        /// </summary>
        /// <param name="entite">Entite selectionnee</param>
        private void GenereConfigFTPFranceJudo(EntitePublicationFFJudo entite)
        {
            // Configure le site France Judo
            SiteFranceJudo.LoginSiteFTPDistant = entite.Login;
            SiteFranceJudo.ModeActifFTPDistant = false;
            SiteFranceJudo.SiteFTPDistant = _ftpEasyConfig;
            SiteFranceJudo.SynchroniseDifferences = true;
            SiteFranceJudo.MaxRetryFTP = 10;

            // Calcul le repertoire distant en fonction de la competition
            SiteFranceJudo.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();

            // Recalcul l'URL distante
            URLDistantPublication = CalculURLSiteDistant();
        }

        /// <summary>
        /// Demarre le thread de generation du site
        /// </summary>
        public void StartGeneration()
        {
            // TODO Ajouter la génération des écrans d'appel

            // TODO a voir si on garde un seule boucle de generation ou si on en fait une separee
            // TODO cela pourrait permettre de separer les instances en ayant des generations dediees

            _schedulerSite.StartGeneration();
        }

        /// <summary>
        /// Arrete le thread de generation du site
        /// </summary>
        public void StopGeneration()
        {
            _schedulerSite.StopGeneration();
        }

        /// <summary>
        /// Demarre un thread pour traiter le nettoyage du site
        /// </summary>
        public void StartNettoyage()
        {
            if (_taskNettoyage == null || _taskNettoyage.IsCompleted)
            {
                try
                {
                    _taskNettoyage = Task.Factory.StartNew(() =>
                    {
                        if (SiteDistantSelectionne != null)
                        {
                            // Nettoyer le site distant
                            SiteDistantSelectionne.NettoyerSite();
                        }
                    });
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Error(ex, "Erreur lors du lancement du nettoyage du site");
                    throw new Exception("Erreur lors du lancement du nettoyage du site", ex);
                }
            }
            else
            {
                LogTools.Logger.Error("Une tache de nettoyage est deja en cours d'execution");
                throw new Exception("Une tache de nettoyage est deja en cours d'execution");
            }
        }

        #endregion
    }
}
