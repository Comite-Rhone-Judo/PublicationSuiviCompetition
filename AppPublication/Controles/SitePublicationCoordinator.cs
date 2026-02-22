using AppPublication.Config.Publication;
using AppPublication.Controles;
using AppPublication.Tools.Files;
using KernelImpl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Tools.Enum;
using Tools.Export;
using Tools.Framework;
using Tools.Logging;
using Tools.Outils;
using Tools.Windows;
using AppPublication.Models.Publication;
using AppPublication.Models.Statistiques;
using System.ComponentModel;

namespace AppPublication.Controles
{
    public class SitePublicationCoordinator : NotificationBase
    {
        #region MEMBERS
        GestionSitePublique _gestionSite;
        GestionSiteInterne _gestionSiteInterne;

        #endregion

        #region CONSTRUCTEUR
        public SitePublicationCoordinator(IJudoDataManager dataManager, GestionStatistiques statMgr)
        {
            // Initialise la liste des logos
            InitFichiersLogo();

            // Creation des instances de gestion de site
            _gestionSite = new GestionSitePublique(dataManager, statMgr);
            _gestionSiteInterne = new GestionSiteInterne(dataManager, statMgr);

            // Lance l'initialisation depuis le cache sur disque
            InitFromConfigFile();
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Indique si toutes les générations sont inactives
        /// </summary>
        public bool IsAllGenerationInactive
        {
            get
            {
               return !(GestionnaireSitePublique.IsGenerationActive || GestionnaireSiteInterne.IsGenerationActive);
            }
        }

        /// <summary>
        /// Indique si au moins une génération est active
        /// </summary>
        public bool IsGenerationActiveOne
        {
            get
            {
                return GestionnaireSitePublique.IsGenerationActive || GestionnaireSiteInterne.IsGenerationActive;
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

                // Propage l'ID de competition dans les gestionnaires de site
                GestionnaireSitePublique.IdCompetition = value;
                GestionnaireSiteInterne.IdCompetition = value;
            }
        }

        /// <summary>
        /// Le gestionnaire de site public
        /// </summary>
        public GestionSitePublique GestionnaireSitePublique
        {
            get { return _gestionSite; }
        }

        /// <summary>
        /// Le gestionnaire de site interne
        /// </summary>
        public GestionSiteInterne GestionnaireSiteInterne
        {
            get { return _gestionSiteInterne; }
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
                    _selectedLogo = value;

                    // Sauvegarde la valeur
                    string logoName = (value != null) ? value.Name : string.Empty;
                    PublicationConfigSection.Instance.General.Logo = logoName;

                    // Propage le logo selectionne dans les gestionnaires de site
                    _gestionSite.SelectedLogo = value;
                    _gestionSiteInterne.SelectedLogo = value;

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
                    PublicationConfigSection.Instance.General.RepertoireRacine = (_repertoireRacine = value);
                    NotifyPropertyChanged();

                    // Propage le repertoire racine dans les gestionnaires de site
                    _gestionSite.RepertoireRacine = value;
                    _gestionSiteInterne.RepertoireRacine = value;
                }
            }
        }


        private bool _effacerAuDemarrage = true;
        /// <summary>
        /// Indique si on doit faire un RAZ du contenu du répertoire au demarrage de la generation
        /// </summary>
        public bool EffacerAuDemarrage
        {
            get { return _effacerAuDemarrage; }
            set
            {
                if (_effacerAuDemarrage != value)
                {
                    _effacerAuDemarrage = value;
                    PublicationConfigSection.Instance.General.EffacerAuDemarrage = value;
                    NotifyPropertyChanged();

                    GestionnaireSiteInterne.EffacerAuDemarrage = value;
                    GestionnaireSitePublique.EffacerAuDemarrage = value;
                }
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
                                return GestionnaireSiteInterne.CanChangeProperties && GestionnaireSitePublique.CanChangeProperties;
                            });
                }
                return _cmdGetRepertoireRacine;
            }
        }

        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// Initialise les données à partir du fichier de configuration
        /// </summary>
        public void InitFromConfigFile()
        {
            // Recupere les donnees mutualisee
            RepertoireRacine = PublicationConfigSection.Instance.General.RepertoireRacine;
            SelectedLogo = PublicationConfigSection.Instance.General.GetLogo(FichiersLogo.ToList(), o => o.Name);

            // Propage la lecture du fichier de configuration dans les gestionnaires de site
            _gestionSite.InitFromConfigFile();
            _gestionSiteInterne.InitFromConfigFile();
        }

        #endregion

        #region METHODES PRIVEES

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
        #endregion
    }
}
