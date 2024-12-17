using AppPublication.Export;
using AppPublication.Tools;
using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using Telerik.Windows.Controls;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;
using Tools.Windows;

namespace AppPublication.Controles
{
    /// <summary>
    /// Classe de gestion du Site auto-généré tout au long de la compétition. Il assure la generation du site et contient les objets de publication
    /// locaux et distants.
    /// </summary>
    public class GestionSite : NotificationBase
    {
        #region CONSTANTES
        private const string kSiteLocalInstanceName = "local";
        private const string kSiteDistantInstanceName = "";
        private const string kSiteFranceJudoInstanceName = "ffjudo";
        private const string kSettingEasyConfig = "EasyConfig";
        private const string kSettingURLDistant = "URLDistant";
        private const string kSettingIsolerCompetition = "IsolerCompetition";
        private const string kSettingRepertoireRacineSiteFTPDistant = "RepertoireRacineSiteFTPDistant";
        private const string kSettingPublierProchainsCombats = "PublierProchainsCombats";
        private const string kSettingNbProchainsCombats = "NbProchainsCombats";
        private const string kSettingPublierAffectationTapis = "PublierAffectationTapis";
        private const string kSettingDelaiGenerationSec = "DelaiGenerationSec";
        private const string kSettingDelaiActualisationClientSec = "DelaiActualisationClientSec";
        private const string kSettingMsgProchainsCombats = "MsgProchainsCombats";
        private const string kSettingPouleEnColonnes = "PouleEnColonnes";
        private const string kSettingPouleToujoursEnColonnes = "PouleToujoursEnColonnes";
        private const string kSettingTailleMaxPouleColonnes = "TailleMaxPouleColonnes";
        private const string kSettingRepertoireRacine = "RepertoireRacine";
        private const string kSettingNiveauPublicationFFJudo = "NiveauPublicationFFJudo";
        private const string kSettingEntitePublicationFFJudo = "EntitePublicationFFJudo";
        private const string kSettingSelectedLogo = "SelectedLogo";
        private const string kSettingInterfaceLocalPublication = "InterfaceLocalPublication";
        #endregion

        #region MEMBRES
        private CancellationTokenSource _tokenSource;   // Token pour la gestion de la thread de lecture
        private Task _taskGeneration = null;            // La tache de generation
        private Task _taskNettoyage = null;             // La tache de nettoyage
        private GestionStatistiques _statMgr = null;
        private ExportSiteStructure _structure;         // La structure d'export du site
        private Dictionary<string, EntitePublicationFFJudo> _allEntitePublicationFFJudo = null;
        private Dictionary<string, ObservableCollection<EntitePublicationFFJudo>> _allEntitesPublicationFFJudo = null;

        private string _ftpEasyConfig = string.Empty;   // Le serveur FTP EasyConfig
        private Uri _httpEasyConfig = null;  // Le serveur http EasyConfig

        /// <summary>
        /// Structure interne pour gerer les parametres de generation du site
        /// </summary>
        public class GenereSiteStruct
        {
            public SiteEnum type { get; set; }
            public Phase phase { get; set; }
            public int? tapis { get; set; }
        }
        #endregion

        #region CONSTRUCTEURS
        public GestionSite(GestionStatistiques statMgr)
        {
            try
            {
                // Initialise les objets de gestion des sites Web
                _siteLocal = new MiniSite(true, kSiteLocalInstanceName, true, true);
                _siteDistant = new MiniSite(false, kSiteDistantInstanceName, true, true);           // on utilise un prefix vide pour le site distant pour des questions de retrocompatibilite
                _siteFranceJudo = new MiniSite(false, kSiteFranceJudoInstanceName, false, true);    // On ne garde pas le detail des configuration pour le site FFJudo
                _statMgr = (statMgr != null) ? statMgr : new GestionStatistiques();

                // Initialise la liste des logos
                InitFichiersLogo();

                // Initialise la configuration pour la publication simplifiee France Judo
                InitPublicationFFJudo();

                // Initialise la configuration via le cache de fichier
                InitCacheConfig();

                // Initialise les repertoires d'export - Pas necessaire car en lisant les valeurs en cache le changement se fait a l'initialisation de la propriete
                // FileAndDirectTools.InitExportSiteDirectories();
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        #endregion

        #region PROPRIETES

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
                    NotifyPropertyChanged("EasyConfigDisponible");
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
                if (SiteDistantSelectionne == null || (SiteDistantSelectionne != null || !SiteDistantSelectionne.IsActif))
                {
                    if (_easyConfig != value)
                    {
                        _easyConfig = value;
                        AppSettings.SaveSetting(kSettingEasyConfig, _easyConfig.ToString());
                        NotifyPropertyChanged("EasyConfig");
                        SiteDistantSelectionne = CalculSiteDistantSelectionne();
                    }
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
                NotifyPropertyChanged("AdvancedConfig");
                SiteDistantSelectionne = CalculSiteDistantSelectionne();
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
                NotifyPropertyChanged("SiteDistantSelectionne");
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
                    NotifyPropertyChanged("ListeNiveauxPublicationFFJudo");
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
                    NotifyPropertyChanged("ListeEntitesPublicationFFJudo");
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
                        AppSettings.SaveSetting(kSettingEntitePublicationFFJudo, _entitePublicationFFJudo.Nom);

                        // On Calcul les parametres FTP en fonction de l'entite selectionne
                        GenereConfigFTPFranceJudo(value);
                    }
                    NotifyPropertyChanged("EntitePublicationFFJudo");
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
                    NotifyPropertyChanged("AllEntitePublicationFFJudo");
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
                    _niveauPublicationFFJudo = value;
                    AppSettings.SaveSetting(kSettingNiveauPublicationFFJudo, _niveauPublicationFFJudo);

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

                    NotifyPropertyChanged("NiveauPublicationFFJudo");
                }
            }
        }

        private bool _pouleEnColonnes;
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
                    _pouleEnColonnes = value;
                    AppSettings.SaveSetting(kSettingPouleEnColonnes, _pouleEnColonnes.ToString());
                    NotifyPropertyChanged("PouleEnColonnes");
                }
            }
        }

        private bool _pouleToujoursEnColonnes;
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
                    _pouleToujoursEnColonnes = value;
                    AppSettings.SaveSetting(kSettingPouleToujoursEnColonnes, _pouleToujoursEnColonnes.ToString());
                    NotifyPropertyChanged("PouleToujoursEnColonnes");
                }
            }
        }

        private int _tailleMaxPouleColonnes;
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
                    _tailleMaxPouleColonnes = value;
                    AppSettings.SaveSetting(kSettingTailleMaxPouleColonnes, _tailleMaxPouleColonnes.ToString());
                    NotifyPropertyChanged("TailleMaxPouleColonnes");
                }
            }
        }


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
                                    foreach(string imgFile in op.FileNames)
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
                                        catch(Exception ex)
                                        {
                                            LogTools.Logger.Debug("Fichier '{0}' ignore - Exception lors de la lecture du format", imgFile, ex);
                                            allFileOk = false;
                                        }
                                    }

                                    if(!allFileOk)
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

                                System.Windows.Forms.FolderBrowserDialog dlg = new FolderBrowserDialog();

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

        private string _repertoireRacine;

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
                    _repertoireRacine = value;
                    NotifyPropertyChanged("RepertoireRacine");
                    AppSettings.SaveSetting(kSettingRepertoireRacine, _repertoireRacine);

                    // Met a jour la constante d'export
                    string tmp = OutilsTools.GetExportSiteDir(_repertoireRacine);

                    // Initialise la structure d'export
                    _structure = new ExportSiteStructure(tmp, IdCompetition);

                    // Met a jour les repertoires de l'application
                    InitExportSiteStructure();

                    // Initialise la racine du serveur Web local
                    SiteLocal.ServerHTTP.LocalRootPath = tmp;
                }
            }
        }

        ObservableCollection<FilteredFileInfo> _fichiersLogo = new ObservableCollection<FilteredFileInfo>();
        public ObservableCollection<FilteredFileInfo> FichiersLogo
        {
            get {
                return _fichiersLogo;
            }
            private set {
                if (_fichiersLogo != value)
                {
                    _fichiersLogo = value;
                    NotifyPropertyChanged("FichiersLogo");
                }
            }
        }

        FilteredFileInfo _selectedLogo = null;
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
                    _selectedLogo = value;
                    AppSettings.SaveSetting(kSettingSelectedLogo, _selectedLogo.Name);
                    NotifyPropertyChanged("SelectedLogo");
                }
            }
        }

        StatExecution _statGeneration;
        /// <summary>
        /// Statistique de derniere generation - lecture seule
        /// </summary>
        public StatExecution DerniereGeneration
        {
            get
            {
                return _statGeneration;
            }
            private set
            {
                _statGeneration = value;
                NotifyPropertyChanged("DerniereGeneration");
            }
        }

        StatExecution _statSyncDistant;
        /// <summary>
        /// Statistiques de derniere synchronisation - lecture seule
        /// </summary>
        public StatExecution DerniereSynchronisation
        {
            get
            {
                return _statSyncDistant;
            }
            private set
            {
                _statSyncDistant = value;
                NotifyPropertyChanged("DerniereSynchronisation");
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
                NotifyPropertyChanged("SiteGenere");
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
                NotifyPropertyChanged("SiteSynchronise");
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
                    AppSettings.SaveSetting(kSettingInterfaceLocalPublication, SiteLocal.InterfaceLocalPublication.ToString());
                    NotifyPropertyChanged("InterfaceLocalPublication");
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
                NotifyPropertyChanged("IsGenerationActive");
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
                if (_isolerCompetition != value)
                {
                    _isolerCompetition = value;
                    AppSettings.SaveSetting(kSettingIsolerCompetition, _isolerCompetition.ToString());

                    // Met a jour la structure d'export
                    if (_structure != null)
                    {
                        _structure.CompetitionIsolee = _isolerCompetition;
                    }
                    NotifyPropertyChanged("IsolerCompetition");
                    URLDistantPublication = CalculURLSiteDistant();
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
                    _nbProchainsCombats = value;
                    NotifyPropertyChanged("NbProchainsCombats");
                    AppSettings.SaveSetting(kSettingNbProchainsCombats, _nbProchainsCombats.ToString());
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
                    _delaiGenerationSec = value;
                    AppSettings.SaveSetting(kSettingDelaiGenerationSec, _delaiGenerationSec.ToString());
                    NotifyPropertyChanged("DelaiGenerationSec");
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
                    _delaiActualisationClientSec = value;
                    AppSettings.SaveSetting(kSettingDelaiActualisationClientSec, _delaiActualisationClientSec.ToString());
                    NotifyPropertyChanged("DelaiActualisationClientSec");
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
                    _msgProchainsCombats = value;
                    AppSettings.SaveSetting(kSettingMsgProchainsCombats, _msgProchainsCombats);
                    NotifyPropertyChanged("MsgProchainsCombats");
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
                    _urlDistant = value;
                    AppSettings.SaveSetting(kSettingURLDistant, _urlDistant);
                    NotifyPropertyChanged("URLDistant");
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
                NotifyPropertyChanged("URLDistantPublication");
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
                NotifyPropertyChanged("URLLocalPublication");
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
                    _ftpRepertoireRacineDistant = value;
                    AppSettings.SaveSetting(kSettingRepertoireRacineSiteFTPDistant, _ftpRepertoireRacineDistant);
                    NotifyPropertyChanged("RepertoireRacineSiteFTPDistant");
                    SiteDistant.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();   // Ce parametre ne concerne pas le site FranceJudo
                }
            }
        }


        

        private string _idCompetition;
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
                if (_idCompetition != value)
                {
                    _idCompetition = value;
                    NotifyPropertyChanged("IdCompetition");

                    // Met a jour la structure d'export
                    if (_structure != null)
                    {
                        _structure.IdCompetition = value;
                    }

                    // Recalcul les valeurs des URLs et répertoires distants
                    SiteDistantSelectionne.RepertoireSiteFTPDistant = CalculRepertoireSiteDistant();
                    URLDistantPublication = CalculURLSiteDistant();
                    URLLocalPublication = CalculURLSiteLocal();

                    // On en peut publier que en individuelle
                    CanPublierAffectation = DialogControleur.Instance.ServerData.competition.IsIndividuelle();

                    // Si on est en Shiai, par defaut on met les poules en colonnes
                    if (DialogControleur.Instance.ServerData.competition.IsShiai())
                    {
                        PouleEnColonnes = true;
                        PouleToujoursEnColonnes = true;
                    }
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
                _canPublierAffectation = value;
                NotifyPropertyChanged("CanPublierAffectation");
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
                    _publierProchainsCombats = value;
                    AppSettings.SaveSetting(kSettingPublierProchainsCombats, _publierProchainsCombats.ToString());
                    NotifyPropertyChanged("PublierProchainsCombats");
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
                    _publierAffectationTapis = value;
                    AppSettings.SaveSetting(kSettingPublierAffectationTapis, _publierAffectationTapis.ToString());
                    NotifyPropertyChanged("PublierAffectationTapis");
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
                NotifyPropertyChanged("Status");
                IsGenerationActive = !(_status.State == StateGenerationEnum.Stopped);
            }
        }

        /// <summary>
        /// Nom du fichierd de cache utiliser pour le controle des checksums
        /// </summary>
        public string ChecksumFileName
        {
            get
            {
                return Path.Combine(_structure.RepertoireRacine, ExportTools.getFileName(ExportEnum.Site_Checksum) + ConstantFile.ExtensionXML);
            }
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Assure l'initialisation de la structure du site
        /// </summary>
        private void InitExportSiteStructure()
        {
            if (_structure != null)
            {
                FileAndDirectTools.CreateDirectorie(_structure.RepertoireRacine);
            }
        }

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
            catch(Exception ex)
            {
                // On ne peut pas initialiser le mode EasyConfig
                LogTools.Logger.Error("Desactivation du mode easyConfig - Configuration absente ou incorrecte", ex);
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
                // On lit le repertoire racine en 1er afin de pouvoir initialiser la structure du site
                RepertoireRacine = AppSettings.ReadSetting(kSettingRepertoireRacine, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));


                // Charge les valeurs pour la publication FFJudo
                if (EasyConfigDisponible)
                {
                    EasyConfig = AppSettings.ReadSetting(kSettingEasyConfig, true);

                    // On charge le nom de l'entite en 1er car sinon, en initialisant la liste des niveaux, on fait un reset de la valeur de l'entite a la 1ere de la liste du niveau
                    string tmp = AppSettings.ReadRawSetting(kSettingEntitePublicationFFJudo);

                    // Charge le niveau selectionne
                    NiveauPublicationFFJudo = AppSettings.ReadRawSetting<string>(kSettingNiveauPublicationFFJudo, ListeNiveauxPublicationFFJudo, o => o);

                    // Recherche l'entite a partir de la valeur initiale lue
                    EntitePublicationFFJudo = AppSettings.FindSetting<EntitePublicationFFJudo>(tmp, ListeEntitesPublicationFFJudo, o => o.Nom);                    
                }

                // Les autres parametres peuvent suivre
                URLDistant = AppSettings.ReadSetting(kSettingURLDistant, string.Empty);
                IsolerCompetition = AppSettings.ReadSetting(kSettingIsolerCompetition, false);
                RepertoireRacineSiteFTPDistant = AppSettings.ReadSetting(kSettingRepertoireRacineSiteFTPDistant, string.Empty);
                PublierProchainsCombats = AppSettings.ReadSetting(kSettingPublierProchainsCombats, false);
                NbProchainsCombats = AppSettings.ReadSetting(kSettingNbProchainsCombats, 6);
                PublierAffectationTapis = AppSettings.ReadSetting(kSettingPublierAffectationTapis, true);
                DelaiGenerationSec = AppSettings.ReadSetting(kSettingDelaiGenerationSec, 30);
                DelaiActualisationClientSec = AppSettings.ReadSetting(kSettingDelaiActualisationClientSec, 30);
                MsgProchainsCombats = AppSettings.ReadSetting(kSettingMsgProchainsCombats, string.Empty);
                PouleEnColonnes = AppSettings.ReadSetting(kSettingPouleEnColonnes, false);
                PouleToujoursEnColonnes = AppSettings.ReadSetting(kSettingPouleToujoursEnColonnes, false);
                TailleMaxPouleColonnes = AppSettings.ReadSetting(kSettingTailleMaxPouleColonnes, 5);


                // Recherche le logo dans la liste
                SelectedLogo = AppSettings.ReadRawSetting<FilteredFileInfo>(kSettingSelectedLogo, FichiersLogo, o => o.Name);

                // Recherche l'interface de publication
                InterfaceLocalPublication = AppSettings.ReadRawSetting<IPAddress>(kSettingInterfaceLocalPublication, SiteLocal.InterfacesLocal, o => o.ToString());
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

            // Selectionne en fonction du type de configuration
            if (EasyConfig)
            {
                // Extrait l'URL EasyConfig si possible
                try
                {
                    Uri fullUri = new Uri(_httpEasyConfig, EntitePublicationFFJudo.RacineHttp);
                    urlBase = fullUri.ToString();
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

            if (!String.IsNullOrEmpty(urlBase) && _structure != null && !String.IsNullOrEmpty(_structure.UrlPathIndex))
            {
                // On verifie que le dernier caractere est bien un "/" car sinon la concatenation va ignorer le dernier element du path
                if(urlBase.Last() != '/')
                {
                    urlBase += '/';
                }

                output = (new Uri(new Uri(urlBase), _structure.UrlPathIndex)).ToString();
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

            if (!String.IsNullOrEmpty(IdCompetition) && SiteLocal.ServerHTTP != null && SiteLocal.ServerHTTP.ListeningIpAddress != null && SiteLocal.ServerHTTP.Port > 0 && _structure != null)
            {
                string urlBase = string.Format("http://{0}:{1}/", SiteLocal.ServerHTTP.ListeningIpAddress.ToString(), SiteLocal.ServerHTTP.Port);

                output = (new Uri(new Uri(urlBase), _structure.UrlPathIndex)).ToString();
            }

            return output;
        }

        /// <summary>
        /// Retourne le site distant selectionne
        /// </summary>
        /// <returns></returns>
        private MiniSite CalculSiteDistantSelectionne()
        {
            return (EasyConfig)? SiteFranceJudo : SiteDistant;
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

            if (!String.IsNullOrEmpty(repRoot) && _structure != null)
            {
                output = FileAndDirectTools.PathJoin(repRoot, _structure.UrlPathCompetition);
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
            // Status = new StatusGenerationSite(StateGenerationEnum.Idle);
            Status = StatusGenerationSite.Instance(StateGenerationEnum.Idle);
            DateTime wakeUpTime = DateTime.Now;
            int delaiScrutationMs = 1000;

            // Reset le token d'arret
            if (_tokenSource != null)
            {
                _tokenSource = null;
            }
            _tokenSource = new CancellationTokenSource();

            if (_taskGeneration == null || _taskGeneration.IsCompleted)
            {
                try
                {
                    _taskGeneration = Task.Factory.StartNew(() =>
                    {
                        while (!_tokenSource.Token.IsCancellationRequested)
                        {
                            if (DateTime.Now >= wakeUpTime)
                            {
                                // Pour controler la duree total par rapport au timer
                                Stopwatch watcherTotal = new Stopwatch();
                                watcherTotal.Start();

                                // Pousse les commandes de generation dans le thread de travail
                                // Status = new StatusGenerationSite(StateGenerationEnum.Generating, "Generation du site ...");
                                Status = StatusGenerationSite.Instance(StateGenerationEnum.Generating);

                                StatExecution statGeneration = new StatExecution();
                                Stopwatch watcherGen = new Stopwatch();
                                watcherGen.Start();

                                // Charge le fichier de cache de checksum
                                List<FileWithChecksum> checksumCache = LoadChecksumFichiersGeneres();
                                List<FileWithChecksum> checksumGenere = GenereAll();
                                SiteGenere = (checksumGenere.Count > 0);
                                watcherGen.Stop();
                                statGeneration.DelaiExecutionMs = watcherGen.ElapsedMilliseconds;
                                // Status = new StatusGenerationSite(StateGenerationEnum.Idle, "En attente ...");
                                Status = StatusGenerationSite.Instance(StateGenerationEnum.Idle);

                                _statMgr.EnregistrerGeneration(watcherGen.ElapsedMilliseconds / 1000F);

                                if (SiteGenere)
                                {
                                    // Met a jour la date de generation puisque le site a ete traite
                                    DerniereGeneration = statGeneration;

                                    // Si le site distant est actif, transfere la mise a jour
                                    if ( SiteDistantSelectionne != null &&  SiteDistantSelectionne.IsActif)
                                    {
                                        // string localRoot = Path.Combine(ConstantFile.ExportSite_dir, DialogControleur.Instance.ServerData.competition.remoteId);
                                        string localRoot = _structure.RepertoireCompetition;

                                        // Le site distant sur lequel charger les fichiers selon si on isole ou pas
                                        StatExecution statSync = new StatExecution();
                                        Stopwatch watcherSync = new Stopwatch();
                                        watcherSync.Start();

                                        // Calcul les fichiers a prendre en compte
                                        List<FileInfo> filesToSync = null;
                                        if (checksumCache != null && checksumCache.Count > 0)
                                        {
                                            // Extrait les fichiers generes qui sont differents du cache
                                            List<FileWithChecksum> chkToSync = checksumGenere.Except(checksumCache, new FileWithChecksumComparer()).ToList();
                                            filesToSync = chkToSync.Select(o => o.File).ToList();

                                            // For Debug only
                                            if (filesToSync.Count <= 0)
                                            {
                                                LogTools.Logger.Debug("Fichiers a synchroniser: {0}", string.Join(",", filesToSync.Select(f => f.Name)));
                                            }
                                        }

                                        // Synchronise le site FTP
                                        UploadStatus uploadOut = SiteDistantSelectionne.UploadSite(localRoot, filesToSync);
                                        SiteSynchronise = uploadOut.IsSuccess;

                                        watcherSync.Stop();
                                        statSync.DelaiExecutionMs = watcherSync.ElapsedMilliseconds;

                                        _statMgr.EnregistrerSynchronisation(watcherSync.ElapsedMilliseconds / 1000F, uploadOut);

                                        if (SiteSynchronise)
                                        {
                                            // Enregistre les checksums en cache maintenant qu'on sait que l'etat distant est synchrone
                                            SaveChecksumFichiersGeneres(checksumGenere);
                                            DerniereSynchronisation = statSync;
                                        }
                                    }
                                }
                                else
                                {
                                    _statMgr.EnregistrerErreurGeneration();
                                }

                                watcherTotal.Stop();

                                // Si le transfert a duree plus que le temps d'attente, on attend au plus 5 sec
                                // Sinon, on attend la difference restantes
                                int delaiThread = (int)Math.Max(DelaiGenerationSec * 1000 - watcherTotal.ElapsedMilliseconds, 5000);

                                // Met le thread en attente pour la prochaine generation
                                Status.NextGenerationSec = (int)Math.Round(delaiThread / 1000.0);

                                _statMgr.EnregsitrerDelaiGeneration(delaiThread / 1000F);

                                // prochaine heure de generation
                                wakeUpTime = DateTime.Now.AddMilliseconds(delaiThread);

                                StatExecution tmp = DerniereGeneration;
                                tmp.DateProchaineGeneration = wakeUpTime;
                                DerniereGeneration = tmp;
                            }
                            Thread.Sleep(delaiScrutationMs);
                        }
                    }, _tokenSource.Token);
                }
                catch (Exception ex)
                {
                    // On RAZ l'etat du lecteur
                    throw new Exception("Erreur lors du lancement de la generation du site", ex);
                }
            }
            else
            {
                throw new Exception("Une tache de génération est déjà en cours d'exécution");
            }
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
                    // On RAZ l'etat du lecteur
                    throw new Exception("Erreur lors du lancement du nettoyage du site", ex);
                }
            }
            else
            {
                throw new Exception("Une tache de nettoyage est déjà en cours d'exécution");
            }
        }

        /// <summary>
        /// Arrete le thread de generation du site
        /// </summary>
        public void StopGeneration()
        {
            if (_tokenSource != null)
            {
                // Arrete le thread de generation
                _tokenSource.Cancel();
                _taskGeneration.Wait();
            }

            // Etat de la generation
            // Status = new StatusGenerationSite(StateGenerationEnum.Stopped);
            Status = StatusGenerationSite.Instance(StateGenerationEnum.Stopped);
        }

        /// <summary>
        /// Declenche l'exportation
        /// </summary>
        /// <param name="genere">Type d'exportation</param>
        private List<FileWithChecksum> Exporter(GenereSiteStruct genere)
        {
            List<FileWithChecksum> urls = new List<FileWithChecksum>();
            ConfigurationExportSite cfg = new ConfigurationExportSite(PublierProchainsCombats, PublierAffectationTapis && CanPublierAffectation, DelaiActualisationClientSec, NbProchainsCombats, MsgProchainsCombats, (SelectedLogo != null) ? SelectedLogo.Name : string.Empty, PouleEnColonnes, PouleToujoursEnColonnes, TailleMaxPouleColonnes);

            try
            {
                JudoData DC = DialogControleur.Instance.ServerData;

                switch (genere.type)
                {
                    case SiteEnum.AllTapis:
                        urls = ExportSite.GenereWebSiteAllTapis(DC, cfg, _structure);
                        break;
                    case SiteEnum.Classement:
                        urls = ExportSite.GenereWebSiteClassement(DC, genere.phase.GetVueEpreuve(DC), cfg, _structure);
                        break;
                    case SiteEnum.Index:
                        urls = ExportSite.GenereWebSiteIndex(cfg, _structure);
                        break;
                    case SiteEnum.Menu:
                        urls = ExportSite.GenereWebSiteMenu(DC, cfg, _structure);
                        break;
                    case SiteEnum.Phase:
                        urls = ExportSite.GenereWebSitePhase(DC, genere.phase, cfg, _structure);
                        break;
                    case SiteEnum.AffectationTapis:
                        urls = ExportSite.GenereWebSiteAffectation(DC, cfg, _structure);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error("Erreur rencontree lors de l'export", ex);
            }

            return urls;
        }

        /// <summary>
        /// Ajoute une tache de fond de generation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="phase"></param>
        /// <param name="tapis"></param>
        /// <returns></returns>
        public Task<List<FileWithChecksum>> AddWork(SiteEnum type, Phase phase, int? tapis)
        {
            Task<List<FileWithChecksum>> output = null;

            if (IsGenerationActive)
            {
                GenereSiteStruct export = new GenereSiteStruct
                {
                    type = type,
                    phase = phase,
                    tapis = tapis
                };

                output = OutilsTools.Factory.StartNew(() =>
                {
                    return Exporter(export);
                });
            }

            return output;
        }

        /// <summary>
        /// Genere la totalite du site
        /// </summary>
        /// <returns></returns>
        public List<FileWithChecksum> GenereAll()
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();
            if (IsGenerationActive)
            {
                JudoData DC = DialogControleur.Instance.ServerData;
                if (DC.Organisation.Competitions.Count > 0)
                {
                    List<Task<List<FileWithChecksum>>> listTaskGeneration = new List<Task<List<FileWithChecksum>>>();

                    listTaskGeneration.Add(AddWork(SiteEnum.Index, null, null));
                    listTaskGeneration.Add(AddWork(SiteEnum.Menu, null, null));
                    if (PublierAffectationTapis && CanPublierAffectation)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.AffectationTapis, null, null));
                    }

                    // On ne genere pas les informations de prochains combat si ce n'est pas necessaire
                    if (PublierProchainsCombats)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.AllTapis, null, null));
                    }

                    foreach (Phase phase in DC.Deroulement.Phases)
                    {
                        listTaskGeneration.Add(AddWork(SiteEnum.Phase, phase, null));
                        listTaskGeneration.Add(AddWork(SiteEnum.Classement, phase, null));
                    }

                    try
                    {
                        // Elimine les elements null
                        listTaskGeneration.RemoveAll(item => item == null);
                        if (listTaskGeneration.Count > 0)
                        {
                            Task<List<FileWithChecksum>> t = WaitGenereAll(listTaskGeneration);
                            // Attend la fin de la generation pour rendre la main
                            t.Wait();
                            output = t.Result;

                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Error(ex);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Se met en attente de la fin de tous les travaux de generation. Met a jour l'indicateur de progression en fonction de la fin des taches
        /// </summary>
        /// <param name="listTaskGeneration"></param>
        /// <returns></returns>
        private async Task<List<FileWithChecksum>> WaitGenereAll(List<Task<List<FileWithChecksum>>> listTaskGeneration)
        {
            int totalTask = listTaskGeneration.Count();
            int nTask = 0;
            List<FileWithChecksum> fichiersGeneres = new List<FileWithChecksum>();
            while (listTaskGeneration.Any())
            {
                Task<List<FileWithChecksum>> finishedTask = await Task.WhenAny(listTaskGeneration.ToArray());
                listTaskGeneration.Remove(finishedTask);
                nTask++;
                Status.Progress = (int)Math.Round(100.0 * nTask / totalTask);
                fichiersGeneres = fichiersGeneres.Concat(await finishedTask).ToList();
            }

            return fichiersGeneres;
        }

        /// <summary>
        /// Sauvegarde une liste de fichiers generes dans le cache de checksum (ecrase le precedent)
        /// </summary>
        /// <param name="fichiersGeneres"></param>
        private void SaveChecksumFichiersGeneres(List<FileWithChecksum> fichiersGeneres)
        {
            // Enregistre les checksums des fichiers generes
            XDocument doc = ExportXML.ExportChecksumFichiers(fichiersGeneres);

            if (!File.Exists(ChecksumFileName) || !FileAndDirectTools.IsFileLocked(ChecksumFileName))
            {
                FileAndDirectTools.NeedAccessFile(ChecksumFileName);
                try
                {
                    using (FileStream fs = new FileStream(ChecksumFileName, FileMode.Create))
                    {
                        doc.Save(fs);
                    }
                }
                catch (Exception ex)
                {
                    LogTools.Error(ex);
                }
                finally
                {
                    FileAndDirectTools.ReleaseFile(ChecksumFileName);
                }
            }
        }


        /// <summary>
        /// Charge le fichier de cache de checksum
        /// </summary>
        /// <param name=""></param>
        /// <returns>Liste vide si le fichier n'existe pas</returns>
        private List<FileWithChecksum> LoadChecksumFichiersGeneres()
        {
            List<FileWithChecksum> output = new List<FileWithChecksum>();

            try
            {
                // Charge le fichier
                XDocument doc = XDocument.Load(ChecksumFileName);

                // Recherche la racine
                List<XElement> rootElem = doc.Descendants(ConstantXML.checksums).ToList();

                if (rootElem.Count() >= 1)
                {
                    output = ExportXML.ImportChecksumFichiers(rootElem.First());
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }

            return output;
        }

        #endregion
    }
}
