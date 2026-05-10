using AppPublication.Config;
using AppPublication.Config.Generation;
using AppPublication.Config.Publication;
using AppPublication.Generation;
using AppPublication.Models.Statistiques;
using AppPublication.Publication;
using AppPublication.Statistiques;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Network;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Resources;
using FranceJudo.Metier.Structures;
using FranceJudo.Metier.XML;
using FranceJudo.UI.Wpf.Dialogs;
using FranceJudo.UI.Wpf.Foundation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AppPublication.Models.Publication
{
    public class GestionSitePublique : GestionSiteBase
    {
        #region CONSTANTES
        private const string kSiteLocalInstanceName = "local";
        private const string kSiteDistantInstanceName = "distant";
        private const string kSiteFranceJudoInstanceName = "ffjudo";
        private const string kSiteRepertoire = "site";
        private const string kCfgSitePublicInstanceName = "public";
        #endregion

        #region MEMBRES
        readonly private GenerateurSite _generateurSite = null;                  // Le generateur Site
        private SitePhysicalStructure _structureRepertoiresSite;          // La structure de repertoire d'export du site
        private SiteUrlGenerator _siteLocalUrlGenerator;                     // la structure d'export du site local
        private SiteUrlGenerator _siteDistantUrlGenerator;                   // la structure d'export du site distant

        private Dictionary<string, EntitePublicationFFJudo> _allEntitePublicationFFJudo = null;
        private Dictionary<string, ObservableCollection<EntitePublicationFFJudo>> _allEntitesPublicationFFJudo = null;

        private string _ftpEasyConfig = string.Empty;                   // Le serveur FTP EasyConfig
        private Uri _httpEasyConfig = null;                             // Le serveur http EasyConfig

        readonly private MiniSite _siteDistant = null;                           // Le site distant de base
        readonly private MiniSite _siteFranceJudo = null;                        // Le site distant France Judo

        private string _localServerBaseUri = string.Empty;
        readonly private string _distantServerBaseUri = string.Empty;
        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="dataManager">Le gestionnaire de données</param>
        /// <param name="statMgr">le gestionnaire de statitiques</param>
        public GestionSitePublique(IJudoDataManager dataManager, GestionStatistiques statMgr)
            : base(dataManager, statMgr)
        {
            try
            {
                // Initialise les objets de gestion des sites Web. Ils chargent automatiquement leur configuration
                _siteLocal = MiniSiteConfigurable.CreateInstance(kSiteLocalInstanceName, true, false);
                _siteDistant = MiniSiteConfigurable.CreateInstance(kSiteDistantInstanceName, true, true);           // on utilise un prefix vide pour le site distant pour des questions de retrocompatibilite
                _siteFranceJudo = MiniSiteConfigurable.CreateInstance(kSiteFranceJudoInstanceName, false, true);    // On ne garde pas le detail des configuration pour le site FFJudo

                // Le generateur de site
                _generateurSite = new GenerateurSite(_judoDataManager, SiteDistantSelectionne, _progressHandler);

                // Initialise le scheduler de generation de site
                _schedulerSite = new GenerationScheduler(_statMgr.GenerationSite, _statMgr.Synchronisation, _generateurSite);
                _schedulerSite.StateChanged += OnSchedulerSiteStateChanged;

                // Initialise la configuration pour la publication simplifiee France Judo
                InitPublicationFFJudo();

                // Initialise la configuration via le cache de fichier
                InitFromConfigFile();
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

        public override bool CanChangeProperties
        {
            get
            {
                return SiteDistantSelectionne == null || !SiteDistantSelectionne.IsActif && !SiteLocal.IsActif && !IsGenerationActive;
            }
        }

        private bool _easyConfigDisponible;
        /// <summary>
        /// Flag indiquant si le mode de configuration simplifie est disponible
        /// </summary>
        public bool EasyConfigDisponible
        {
            get { return _easyConfigDisponible; }
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
            get { return _easyConfig; }
            set
            {
                // On ne peut changer la valeur que si le site en cours n'est pas actif
                if (SiteDistantSelectionne == null || !SiteDistantSelectionne.IsActif)
                {
                    // Enregistre la valeur en cache
                    AppConfigRoot.Instance.Publication.General.EasyConfig = (_easyConfig = value);
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
            get { return !EasyConfig; }
            set
            {
                EasyConfig = !value;
                NotifyPropertyChanged();
                // Inutile, le fait de faire le set sur EasyConfig suffit a mettre a jour le site selectionne
            }
        }

        private MiniSite _siteDistantSelectionne;
        /// <summary>
        /// Le MiniSite selectionne en fonction du mode de configuration
        /// </summary>
        public MiniSite SiteDistantSelectionne
        {
            get { return _siteDistantSelectionne; }
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
            get { return _listeNiveauxPublicationFFJudo; }
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
            get { return _listeEntitesPublicationFFJudo; }
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
            get { return _entitePublicationFFJudo; }
            set
            {
                if (_entitePublicationFFJudo != value)
                {
                    _entitePublicationFFJudo = value;
                    if (value != null)
                    {
                        // Garde en memoire la derniere valeur sauvegardee pour ce niveau
                        _allEntitePublicationFFJudo[_niveauPublicationFFJudo] = value;
                        AppConfigRoot.Instance.Publication.General.EntitePublicationFFJudo = _entitePublicationFFJudo.Nom;
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
            get { return _allEntitePublicationFFJudo; }
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
            get { return _niveauPublicationFFJudo; }
            set
            {
                if (_niveauPublicationFFJudo != value)
                {
                    AppConfigRoot.Instance.Publication.General.NiveauPublicationFFJudo = (_niveauPublicationFFJudo = value);
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
            get { return _pouleEnColonnes; }
            set
            {
                if (_pouleEnColonnes != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.PouleEnColonnes = value;
                    });
                    AppConfigRoot.Instance.Generation.GenerateurSite.PouleEnColonnes = (_pouleEnColonnes = value);
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
            get { return _pouleToujoursEnColonnes; }
            set
            {
                if (_pouleToujoursEnColonnes != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.PouleToujoursEnColonnes = value;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.PouleToujoursEnColonnes = (_pouleToujoursEnColonnes = value);
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
            get { return _tailleMaxPouleColonnes; }
            set
            {
                if (_tailleMaxPouleColonnes != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier( c => { c.TailleMaxPouleColonnes = value; });
                    AppConfigRoot.Instance.Generation.GenerateurSite.TailleMaxPouleColonnes = (_tailleMaxPouleColonnes = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private TaskExecutionInformation _statSyncDistant;
        /// <summary>
        /// Statistiques de derniere synchronisation - lecture seule
        /// </summary>
        public TaskExecutionInformation DerniereSynchronisation
        {
            get { return _statSyncDistant; }
            private set
            {
                _statSyncDistant = value;
                NotifyPropertyChanged();
            }
        }

        private bool _siteSynchronise = false;
        /// <summary>
        /// Indique si le site a bien ete synchronnise - lecture seule
        /// </summary>
        public bool SiteSynchronise
        {
            get { return _siteSynchronise; }
            private set
            {
                _siteSynchronise = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Le site de publication distant
        /// </summary>
        public MiniSite SiteDistant => _siteDistant;

        /// <summary>
        /// Le site de publication distant sur les serveurs de France Judo
        /// </summary>
        public MiniSite SiteFranceJudo => _siteFranceJudo;

        private bool _isolerCompetition = false;
        /// <summary>
        /// Isole les competitions avec leur ID lors de l'upload sur le site distant
        /// </summary>
        public bool IsolerCompetition
        {
            get { return _isolerCompetition; }
            set
            {
                AppConfigRoot.Instance.Publication.General.IsolerCompetition = (_isolerCompetition = value);
                // Met a jour la structure d'export
                _siteDistantUrlGenerator?.CompetitionIsolee = _isolerCompetition;
                NotifyPropertyChanged();
                URLDistantPublication = CalculURLSiteDistant();

                // On met a jour les 2 Sites (en cas de bascule)
                SiteDistant?.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant(useEasyConfig: false);
                SiteFranceJudo?.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant(useEasyConfig: true);
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
                    _generateurSite?.ExportConfigurationManager?.Modifier(c => { c.NbProchainsCombats = value; });
                    AppConfigRoot.Instance.Generation.GenerateurSite.NbProchainsCombats = (_nbProchainsCombats = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private int _delaiActualisationClientSec = 30;
        /// <summary>
        /// Delai d'actualisation cote client en secondes
        /// </summary>
        public int DelaiActualisationClientSec
        {
            get { return _delaiActualisationClientSec; }
            set
            {
                if (_delaiActualisationClientSec != value)
                {
                    // Propage au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.DelaiActualisationClientSec = value;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.DelaiActualisationClientSec = (_delaiActualisationClientSec = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private string _msgProchainsCombats = string.Empty;
        /// <summary>
        /// Message optionnel pour les prochains combats
        /// </summary>
        public string MsgProchainsCombats
        {
            get { return _msgProchainsCombats; }
            set
            {
                if (_msgProchainsCombats != value)
                {
                    // propage au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.MsgProchainsCombats = value;
                    });
                    AppConfigRoot.Instance.Generation.GenerateurSite.MsgProchainsCombats = (_msgProchainsCombats = value);
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
            get { return _urlDistant; }
            set
            {
                if (_urlDistant != value)
                {
                    AppConfigRoot.Instance.Publication.General.URLDistant = (_urlDistant = value);
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
            get { return _urlDistantPublication; }
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
            get { return _urlLocalPublication; }
            private set
            {
                _urlLocalPublication = value;
                NotifyPropertyChanged();
            }
        }

        private string _ftpRepertoireRacineDistant;
        /// <summary>
        /// Repertoire racine cible sur le site distant
        /// </summary>
        public string RepertoireRacineSiteFTPDistant
        {
            get { return _ftpRepertoireRacineDistant; }
            set
            {
                if (_ftpRepertoireRacineDistant != value)
                {
                    AppConfigRoot.Instance.Publication.General.RepertoireRacineSiteFTPDistant = (_ftpRepertoireRacineDistant = value);
                    NotifyPropertyChanged();
                    SiteDistant?.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant(useEasyConfig: false);   // Ce parametre ne concerne pas le site FranceJudo
                }
            }
        }

        private bool _canPublierAffectation = true;
        /// <summary>
        /// Indique si on peut publier l'affectation des tapis ou non
        /// </summary>
        public bool CanPublierAffectation
        {
            get { return _canPublierAffectation; }
            private set
            {
                // Propage la valeur au generateur de site
                _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                {
                    c.PublierAffectationTapis = value && PublierAffectationTapis;
                });
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
            get { return _canPublierEngagements; }
            private set
            {
                // Propage la valeur au generateur de site
                _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                {
                    c.PublierEngagements = value && PublierEngagements;
                } );
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
            get { return _publierProchainsCombats; }
            set
            {
                if (_publierProchainsCombats != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c => { c.PublierProchainsCombats = value; });
                    AppConfigRoot.Instance.Generation.GenerateurSite.PublierProchainsCombats = (_publierProchainsCombats = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _publierAffectationTapis = false;
        /// <summary>
        /// Indique si on doit publier l'affectation des tapis ou non
        /// </summary>
        public bool PublierAffectationTapis
        {
            get { return _publierAffectationTapis; }
            set
            {
                if (_publierAffectationTapis != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.PublierAffectationTapis = value && CanPublierAffectation;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.PublierAffectationTapis = (_publierAffectationTapis = value);
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
            get { return _publierEngagements; }
            set
            {
                if (_publierEngagements != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.PublierEngagements = value && CanPublierEngagements;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.PublierEngagements = (_publierEngagements = value);
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
            get { return _engagementsAbsents; }
            set
            {
                if (_engagementsAbsents != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.EngagementsAbsents = value;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.EngagementsAbsents = (_engagementsAbsents = value);
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
            get { return _engagementsTousCombats; }
            set
            {
                if (_engagementsTousCombats != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.EngagementsTousCombats = value;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.EngagementsTousCombats = (_engagementsTousCombats = value);
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
                    _generateurSite?.ExportConfigurationManager?.Modifier(c => { c.UseIntituleCommun = value; }) ;
                    AppConfigRoot.Instance.Generation.GenerateurSite.UseIntituleCommun = (_useIntituleCommun = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private string _intituleCommun;
        /// <summary>
        /// intitule commun en cas de poly competition
        /// </summary>
        public string IntituleCommun
        {
            get { return _intituleCommun; }
            set
            {
                if (_intituleCommun != value)
                {
                    // propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.IntituleCommun = value;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.IntituleCommun = (_intituleCommun = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _scoreEngagesGagnantPerdant;
        public bool ScoreEngagesGagnantPerdant
        {
            get { return _scoreEngagesGagnantPerdant; }
            set
            {
                if (_scoreEngagesGagnantPerdant != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.EngagementsScoreGP = value;
                    }   );
                    AppConfigRoot.Instance.Generation.GenerateurSite.ScoreEngagesGagnantPerdant = (_scoreEngagesGagnantPerdant = value);
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _afficherPositionCombat;
        public bool AfficherPositionCombat
        {
            get { return _afficherPositionCombat; }
            set
            {
                if (_afficherPositionCombat != value)
                {
                    // Propage la valeur au generateur de site
                    _generateurSite?.ExportConfigurationManager?.Modifier(c =>
                    {
                        c.AfficherPositionCombat = value;
                    } );
                    AppConfigRoot.Instance.Generation.GenerateurSite.AfficherPositionCombat = (_afficherPositionCombat = value);
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
                // Note: Le repertoire racine est lue via le coordinateur ainsi que le Logo selectionne

                // Charge les valeurs pour la publication FFJudo
                if (EasyConfigDisponible)
                {
                    EasyConfig = AppConfigRoot.Instance.Publication.General.EasyConfig;

                    // On charge le nom de l'entite en 1er car sinon, en initialisant la liste des niveaux, on fait un reset de la valeur de l'entite a la 1ere de la liste du niveau
                    string tmp = AppConfigRoot.Instance.Publication.General.EntitePublicationFFJudo;

                    // Charge le niveau selectionne
                    NiveauPublicationFFJudo = AppConfigRoot.Instance.Publication.General.GetNiveauPublicationFFJudo(ListeNiveauxPublicationFFJudo, o => o);

                    // Recherche l'entite a partir de la valeur initiale lue
                    EntitePublicationFFJudo = AppConfigRoot.Instance.Publication.General.GetEntitePublicationFFJudo(ListeEntitesPublicationFFJudo, o => o.Nom, tmp);
                }

                // Les autres parametres peuvent suivre
                URLDistant = AppConfigRoot.Instance.Publication.General.URLDistant;
                IsolerCompetition = AppConfigRoot.Instance.Publication.General.IsolerCompetition;
                RepertoireRacineSiteFTPDistant = AppConfigRoot.Instance.Publication.General.RepertoireRacineSiteFTPDistant;

                var cfgPub = AppConfigRoot.Instance.Publication.GetScheduler(kCfgSitePublicInstanceName);
                DelaiGenerationSec = cfgPub.DelaiGenerationSec;

                PublierProchainsCombats = AppConfigRoot.Instance.Generation.GenerateurSite.PublierProchainsCombats;
                NbProchainsCombats = AppConfigRoot.Instance.Generation.GenerateurSite.NbProchainsCombats;
                PublierAffectationTapis = AppConfigRoot.Instance.Generation.GenerateurSite.PublierAffectationTapis;
                PublierEngagements = AppConfigRoot.Instance.Generation.GenerateurSite.PublierEngagements;
                EngagementsAbsents = AppConfigRoot.Instance.Generation.GenerateurSite.EngagementsAbsents;
                EngagementsTousCombats = AppConfigRoot.Instance.Generation.GenerateurSite.EngagementsTousCombats;
                DelaiActualisationClientSec = AppConfigRoot.Instance.Generation.GenerateurSite.DelaiActualisationClientSec;
                MsgProchainsCombats = AppConfigRoot.Instance.Generation.GenerateurSite.MsgProchainsCombats;
                PouleEnColonnes = AppConfigRoot.Instance.Generation.GenerateurSite.PouleEnColonnes;
                PouleToujoursEnColonnes = AppConfigRoot.Instance.Generation.GenerateurSite.PouleToujoursEnColonnes;
                TailleMaxPouleColonnes = AppConfigRoot.Instance.Generation.GenerateurSite.TailleMaxPouleColonnes;
                UseIntituleCommun = AppConfigRoot.Instance.Generation.GenerateurSite.UseIntituleCommun;
                IntituleCommun = AppConfigRoot.Instance.Generation.GenerateurSite.IntituleCommun;
                ScoreEngagesGagnantPerdant = AppConfigRoot.Instance.Generation.GenerateurSite.ScoreEngagesGagnantPerdant;
                AfficherPositionCombat = AppConfigRoot.Instance.Generation.GenerateurSite.AfficherPositionCombat;
                // L'interface local de publication a ete chargee via la configuration du minisite, il faut juste s'assurer du bon calcul des URLs
                URLLocalPublication = CalculURLSiteLocal();
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        protected override void OnRepertoireRacineChanged(string newValue)
        {
            // Met a jour la constante d'export
            string tmp = AppDirectoryManager.GetExportDir(newValue);
            string siteRoot = Path.Combine(tmp, kSiteRepertoire);

            // Initialise les structures d'export (si l'ID de competition n'est pas encore connue, on passe NULL pour indiquer qu'on ne connait pas l'ID de competition
            _structureRepertoiresSite = new SitePhysicalStructure(siteRoot, IsIdCompetitionValide ? IdCompetition : null);
            _siteDistantUrlGenerator = new SiteUrlGenerator(_structureRepertoiresSite, _localServerBaseUri);
            _siteLocalUrlGenerator = new SiteUrlGenerator(_structureRepertoiresSite, _distantServerBaseUri);

            // Propage la valeur au generateur de site
            _generateurSite?.StructureSiteGenerator = _siteDistantUrlGenerator;

            // Met a jour les repertoires de l'application si on peut
            if (_structureRepertoiresSite != null && _structureRepertoiresSite.IsFullyConfigured)
            {
                FileSystemHelper.CreateDirectory(_structureRepertoiresSite.RepertoireRacine);
            }

            // Initialise la racine du serveur Web local
            SiteLocal.ServerHTTP.LocalRootPath = siteRoot;
        }

        protected override void OnSelectedLogoChanged(string logoName)
        {
            // Propage la valeur au generateur de site
            _generateurSite?.ExportConfigurationManager?.Modifier(c =>
            {
                c.Logo = logoName;
            } );
        }

        protected override void OnInterfaceLocalPublicationChanged()
        {
            URLLocalPublication = CalculURLSiteLocal();
        }

        protected override void UpdateDelaiGenerationConfig(int newValue)
        {
            // On cible le scheduler "public" dans le JSON
            var cfg = AppConfigRoot.Instance.Publication.GetScheduler(kCfgSitePublicInstanceName);
            cfg.DelaiGenerationSec = newValue;
        }

        protected override void OnIdCompetitionChanged(string newValue)
        {
            // Met a jour la structure d'export
            _structureRepertoiresSite?.IdCompetition = newValue;

            // Appel de la méthode centralisée
            ForceRefreshUrls();

            // Note: ici on devrait dans l'absolu utiliser le snapshot mais le traitement est rapide et a peu de chance de changer
            var DC = _judoDataManager.Data;
            CanPublierAffectation = DC.Organisation.Competition.IsIndividuelle();
            CanPublierEngagements = DC.Organisation.Competition.IsIndividuelle() || DC.Organisation.Competition.IsShiai();

            // Si on est en Shiai, par defaut on met les poules en colonnes
            if (DC.Organisation.Competition.IsShiai())
            {
                PouleEnColonnes = true;
                PouleToujoursEnColonnes = true;
            }
        }

        /// <summary>
        /// Force le recalcul explicite des URLs (Remplace le hack de réassignation d'ID)
        /// </summary>
        public override void ForceRefreshUrls()
        {
            // Recalcul les valeurs des URLs et répertoires distants
            // Ici on ne peut pas se contenter du site selectionne, les 2 sites utilisent la competition dans le calcul de leur URL et repertoire distant, il faut donc forcer le recalcul pour les 2

            SiteDistant?.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant(useEasyConfig: false);
            SiteFranceJudo?.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant(useEasyConfig: true);

            URLDistantPublication = CalculURLSiteDistant();
            URLLocalPublication = CalculURLSiteLocal();
        }

        protected override void OnSchedulerSiteStateChanged(object sender, SchedulerStateEventArgs evt)
        {
            // Appel à la logique de base pour State, SiteGenere, DerniereGeneration et DelaiNextSec
            base.OnSchedulerSiteStateChanged(sender, evt);

            // Traitement spécifique de la synchronisation (Syncing)
            System.Windows.Application.Current.ExecOnUiThread(() =>
            {
                if (evt.InfosExecution != null && evt.State == StateGenerationEnum.Syncing)
                {
                    SiteSynchronise = evt.InfosExecution.IsSuccess;
                    DerniereSynchronisation = evt.InfosExecution;
                }
            });
        }

        #endregion

        #region METHODES SPECIFIQUES

        /// <summary>
        /// Calcul les parametres FTP pour le MiniSite France Judo
        /// </summary>
        /// <param name="entite">Entite selectionnee</param>
        private void GenereConfigFTPFranceJudo(EntitePublicationFFJudo entite)
        {
            // Configure le site France Judo
            SiteFranceJudo?.LoginSiteFTPDistant = entite.Login;
            SiteFranceJudo?.ModeActifFTPDistant = false;
            SiteFranceJudo?.SiteFTPDistant = _ftpEasyConfig;
            SiteFranceJudo?.SynchroniseDifferences = true;
            SiteFranceJudo?.MaxRetryFTP = 10;

            // Calcul le repertoire distant en fonction de la competition
            SiteFranceJudo?.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant(useEasyConfig: true);

            // Recalcul l'URL distante
            URLDistantPublication = CalculURLSiteDistant();
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
                // 1. Chargement avec LINQ to XML
                XDocument doc;
                using (var stream = MetierResources.GetPublicationFFJudoXml())
                {
                    doc = XDocument.Load(stream);
                }

                XElement root = doc.Root;

                // 2. Vérification de la racine
                if (root == null || root.Name != ConstantXML.EasyConfig_Racine)
                {
                    throw new InvalidOperationException("Racine du fichier de configuration inconnue ou manquante");
                }

                // 3. Extraction des attributs FTP/HTTP avec cast direct
                _ftpEasyConfig = (string)root.Attribute(ConstantXML.EasyConfig_Racine_Ftp);
                string httpVal = (string)root.Attribute(ConstantXML.EasyConfig_Racine_Http);

                if (string.IsNullOrEmpty(_ftpEasyConfig) || string.IsNullOrEmpty(httpVal))
                {
                    throw new InvalidOperationException("Attributs FTP ou HTTP manquants à la racine");
                }
                _httpEasyConfig = new Uri(httpVal);

                // 4. Parcours des éléments (Niveaux / Echelons)
                if (root.HasElements)
                {
                    var tmpNiveaux = new ObservableCollection<string>();
                    _allEntitePublicationFFJudo = new Dictionary<string, EntitePublicationFFJudo>();
                    _allEntitesPublicationFFJudo = new Dictionary<string, ObservableCollection<EntitePublicationFFJudo>>();

                    foreach (XElement niveauNode in root.Elements())
                    {
                        var tmpEntitesNiveau = new ObservableCollection<EntitePublicationFFJudo>();

                        // Récupération de l'échelon (attribut du noeud parent ex: <Ligues echelon="2">)
                        string echStr = (string)niveauNode.Attribute(ConstantXML.EasyConfig_Entite_Echelon);

                        if (niveauNode.HasElements && !string.IsNullOrEmpty(echStr))
                        {
                            int ech = int.Parse(echStr);

                            foreach (XElement childNode in niveauNode.Elements())
                            {
                                // Extraction sécurisée des attributs de l'entité
                                string nom = (string)childNode.Attribute(ConstantXML.EasyConfig_Entite_Nom);
                                string libelle = (string)childNode.Attribute(ConstantXML.EasyConfig_Entite_Libelle);
                                string login = (string)childNode.Attribute(ConstantXML.EasyConfig_Entite_Login);
                                string rFtp = (string)childNode.Attribute(ConstantXML.EasyConfig_Entite_RacineFtp);
                                string rHttp = (string)childNode.Attribute(ConstantXML.EasyConfig_Entite_RacineHttp);

                                if (!string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(libelle) &&
                                    !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(rFtp) &&
                                    !string.IsNullOrEmpty(rHttp))
                                {
                                    tmpEntitesNiveau.Add(new EntitePublicationFFJudo(nom, libelle, ech, login, rFtp, rHttp));
                                }
                            }

                            // On ne tient compte d'un niveau que s'il a des entites valides
                            if (tmpEntitesNiveau.Count > 0)
                            {
                                string niveauName = niveauNode.Name.LocalName;
                                tmpNiveaux.Add(niveauName);
                                _allEntitePublicationFFJudo.Add(niveauName, tmpEntitesNiveau.First());
                                _allEntitesPublicationFFJudo.Add(niveauName, tmpEntitesNiveau);
                            }
                        }
                    }
                    ListeNiveauxPublicationFFJudo = tmpNiveaux;
                    EasyConfigDisponible = true;
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Désactivation du mode EasyConfig - Configuration absente ou incorrecte");
                EasyConfig = false;
                EasyConfigDisponible = false;
            }
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
                if (IsIdCompetitionValide && SiteLocal.ServerHTTP?.ListeningIpAddress != null && SiteLocal.ServerHTTP.Port > 0 && _siteLocalUrlGenerator != null)
                {
                    _localServerBaseUri = string.Format("http://{0}:{1}/", SiteLocal.ServerHTTP.ListeningIpAddress.ToString(), SiteLocal.ServerHTTP.Port);

                    // On doit mettre a jour le gestionnaire d'URL
                    _siteLocalUrlGenerator.RootDomain = _localServerBaseUri;

                    output = _siteLocalUrlGenerator.UrlIndex.AbsoluteUri;
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
        /// Calcul l'URL sur le site distant en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculURLSiteDistant()
        {
            string output = "Indefinie";
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

                if (!string.IsNullOrEmpty(urlBase) && _siteDistantUrlGenerator != null)
                {
                    _siteDistantUrlGenerator.RootDomain = urlBase;
                    output = _siteDistantUrlGenerator.UrlIndex.AbsoluteUri;
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
        /// Calcul le repertoire sur le site distant en fonction de la configuration
        /// </summary>
        /// <returns></returns>
        private string CalculRepertoireSiteDistant(bool useEasyConfig)
        {
            string output = string.Empty;
            string repRoot;

            try
            {
                repRoot = (useEasyConfig) ?  EntitePublicationFFJudo.RacineFtp : RepertoireRacineSiteFTPDistant;
            }
            catch
            {
                repRoot = string.Empty;
            }

            if (!string.IsNullOrEmpty(repRoot) && _siteDistantUrlGenerator != null)
            {
                try
                {
                    // L'URL de la competition n'existe pas forcement encore si pas connecte
                    if (string.IsNullOrEmpty(_siteDistantUrlGenerator.UrlPathCompetition))
                    {
                        throw new ArgumentNullException("Le path de la competition ne doit pas etre null ou vide");
                    }

                    // Repertoire distant donc, c'est unix style
                    output = FileSystemHelper.PathJoin(repRoot, _siteDistantUrlGenerator.UrlPathCompetition, unixStyle: true);
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Debug(ex, "Erreur lors du calcul UrlPathCompetition");
                    // on a essayer de traiter une structure non configuree sans doute
                    output = repRoot;   // par défaut, on reste sur le répertoire racine configuré
                }
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
        /// Execute le nettoyage du site (synchrone)
        /// </summary>
        public void StartNettoyage()
        {
            // Nettoyer le site distant
            SiteDistantSelectionne?.NettoyerSite();
        }
        #endregion
    }
}