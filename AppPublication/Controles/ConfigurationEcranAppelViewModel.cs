using AppPublication.Config; // Utilise le namespace Config
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Input;
using Tools.Outils;

namespace AppPublication.Controles
{
    /// <summary>
    /// ViewModel pour la fenêtre de configuration des écrans d'appel.
    /// </summary>
    public class ConfigurationEcranAppelViewModel : NotificationBase
    {
        // Nom de la section dans app.config (utilisation de la constante publique)
        private const string CONFIG_SECTION_NAME = EcransAppelSection.kConfigSectionName;

        // Valeur par défaut pour le nombre de tapis
        private const int DEFAULT_NOMBRE_TOTAL_TAPIS = 8;

        // Membre interne pour stocker le nombre total de tapis
        private readonly int _nombreTotalTapis;

        private ObservableCollection<EcranAppelViewModel> _ecrans;
        public ObservableCollection<EcranAppelViewModel> Ecrans
        {
            get { return _ecrans; }
            set
            {
                if (_ecrans != value)
                {
                    _ecrans = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private EcranAppelViewModel _selectedEcran;
        public EcranAppelViewModel SelectedEcran
        {
            get { return _selectedEcran; }
            set
            {
                if (_selectedEcran != value)
                {
                    _selectedEcran = value;
                    NotifyPropertyChanged();
                    // Mettre à jour l'écran en édition s'il n'est pas déjà en cours d'édition
                    if (!_isEditing)
                    {
                        PrepareEdition(_selectedEcran);
                    }
                }
            }
        }

        private EcranAppelViewModel _ecranEnEdition;
        public EcranAppelViewModel EcranEnEdition
        {
            get { return _ecranEnEdition; }
            set
            {
                if (_ecranEnEdition != value)
                {
                    _ecranEnEdition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<EcranAppelTapisSelectionViewModel> _tapisDisponiblesPourEdition;
        public ObservableCollection<EcranAppelTapisSelectionViewModel> TapisDisponiblesPourEdition
        {
            get { return _tapisDisponiblesPourEdition; }
            set
            {
                if (_tapisDisponiblesPourEdition != value)
                {
                    _tapisDisponiblesPourEdition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _titreBlocEdition;
        public string TitreBlocEdition
        {
            get { return _titreBlocEdition; }
            set
            {
                if (_titreBlocEdition != value)
                {
                    _titreBlocEdition = value;
                    NotifyPropertyChanged();
                }
            }
        }


        // Commandes
        public ICommand CmdAddNewEcran { get; private set; }
        public ICommand CmdEditEcran { get; private set; }
        public ICommand CmdDeleteEcran { get; private set; }
        public ICommand CmdSaveEcran { get; private set; }
        public ICommand CmdCancelEcran { get; private set; }


        public ConfigurationEcranAppelViewModel(int nombreTotalTapis = DEFAULT_NOMBRE_TOTAL_TAPIS)
        {
            _nombreTotalTapis = nombreTotalTapis; // Initialisation du membre interne

            Ecrans = new ObservableCollection<EcranAppelViewModel>();
            TapisDisponiblesPourEdition = new ObservableCollection<EcranAppelTapisSelectionViewModel>();

            // Initialisation des commandes
            CmdAddNewEcran = new RelayCommand(AddNewEcran);
            CmdEditEcran = new RelayCommand(EditEcran, CanEditOrDeleteEcran);
            CmdDeleteEcran = new RelayCommand(DeleteEcran, CanEditOrDeleteEcran);
            CmdSaveEcran = new RelayCommand(SaveEcran, CanSaveEcran);
            CmdCancelEcran = new RelayCommand(CancelEcran, () => IsEditing);

            // Charger la configuration depuis app.config
            LoadConfiguration();

            // Initialiser le panneau d'édition
            PrepareAjout();
        }

        private void RenumeroterEcrans()
        {
            int i = 1;
            foreach (var ecran in Ecrans)
            {
                ecran.Numero = i++;
            }
        }

        #region Logique des Commandes

        private void AddNewEcran(object obj)
        {
            PrepareAjout();
        }

        private bool CanEditOrDeleteEcran(object obj)
        {
            return obj is EcranAppelViewModel;
        }

        private void EditEcran(object obj)
        {
            if (obj is EcranAppelViewModel ecranAEditer)
            {
                PrepareEdition(ecranAEditer);
            }
        }

        private void DeleteEcran(object obj)
        {
            if (obj is EcranAppelViewModel ecranASupprimer)
            {
                // Idéalement, demander confirmation à l'utilisateur ici
                Ecrans.Remove(ecranASupprimer);
                RenumeroterEcrans();
                // Si l'écran supprimé était celui en cours d'édition, repasser en mode ajout
                if (EcranEnEdition?.Numero == ecranASupprimer.Numero)
                {
                    PrepareAjout();
                }
            }
        }

        private bool CanSaveEcran(object obj)
        {
            return IsEditing && EcranEnEdition != null &&
                   (!string.IsNullOrWhiteSpace(EcranEnEdition.HostName) || !string.IsNullOrWhiteSpace(EcranEnEdition.IPAddress));
        }

        private void SaveEcran(object obj)
        {
            // 1. Mettre à jour la liste des tapis dans l'objet en édition
            EcranEnEdition.Tapis = new ObservableCollection<int>(
                TapisDisponiblesPourEdition.Where(t => t.IsSelected).Select(t => t.Numero)
            );

            // 2. Vérifier si c'est un ajout ou une modification
            var existing = Ecrans.FirstOrDefault(e => e.Numero == EcranEnEdition.Numero);
            if (existing != null)
            {
                // Modification
                existing.UpdateFrom(EcranEnEdition);
            }
            else
            {
                // Ajout
                EcranEnEdition.Numero = Ecrans.Any() ? Ecrans.Max(e => e.Numero) + 1 : 1;
                Ecrans.Add(EcranEnEdition);
            }

            RenumeroterEcrans();

            // 3. Réinitialiser le panneau d'édition en mode "Ajout"
            PrepareAjout();
        }

        private void CancelEcran(object obj)
        {
            PrepareAjout();
        }

        #endregion

        #region Préparation Panneau Edition

        private void PrepareAjout()
        {
            TitreBlocEdition = "Ajouter un nouvel écran";
            EcranEnEdition = new EcranAppelViewModel();
            ChargerTapisPourEdition(EcranEnEdition);
            IsEditing = true; // Mode "ajout" est une forme d'édition
        }

        private void PrepareEdition(EcranAppelViewModel ecran)
        {
            if (ecran == null)
            {
                PrepareAjout();
                return;
            }

            TitreBlocEdition = $"Modifier l'écran {ecran.Numero}";
            EcranEnEdition = ecran.Clone(); // Travailler sur une copie
            ChargerTapisPourEdition(EcranEnEdition);
            IsEditing = true;
        }

        /// <summary>
        /// Charge la liste des CheckBox (TapisDisponiblesPourEdition)
        /// en cochant ceux qui sont dans l'écran passé en paramètre.
        /// </summary>
        private void ChargerTapisPourEdition(EcranAppelViewModel ecran)
        {
            TapisDisponiblesPourEdition.Clear();
            var tapisAssocies = new HashSet<int>(ecran.Tapis);

            // Utilise le membre interne _nombreTotalTapis
            for (int i = 1; i <= _nombreTotalTapis; i++)
            {
                TapisDisponiblesPourEdition.Add(new EcranAppelTapisSelectionViewModel
                {
                    Numero = i,
                    IsSelected = tapisAssocies.Contains(i)
                });
            }
        }

        #endregion

        #region Lecture/Ecriture App.config (Section Personnalisée)

        /// <summary>
        /// Charge la configuration des écrans depuis la section personnalisée du app.config
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                Ecrans.Clear();
                EcransAppelSection section = ConfigurationManager.GetSection(CONFIG_SECTION_NAME) as EcransAppelSection;

                if (section != null)
                {
                    foreach (EcranAppelElement element in section.Ecrans)
                    {
                        Ecrans.Add(new EcranAppelViewModel
                        {
                            Numero = element.Numero,
                            HostName = element.HostName,
                            IPAddress = element.IPAddress,
                            Tapis = new ObservableCollection<int>(ConvertTapisStringToList(element.Tapis))
                        });
                    }
                }
                // Si la section est null (absente du .config), Ecrans reste simplement vide (valeur par défaut)

                RenumeroterEcrans();
            }
            catch (Exception ex)
            {
                // Gérer l'erreur de lecture
                Console.WriteLine($"Erreur lors du chargement de la section configuration {CONFIG_SECTION_NAME}: {ex.Message}");
                Ecrans.Clear();
            }
        }

        /// <summary>
        /// Sauvegarde la configuration actuelle des écrans dans la section personnalisée du app.config
        /// </summary>
        public void SaveConfiguration()
        {
            try
            {
                // Ouvre le fichier de configuration de l'application
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Récupère la section. Si elle n'existe pas, la crée.
                EcransAppelSection section = config.GetSection(CONFIG_SECTION_NAME) as EcransAppelSection;
                if (section == null)
                {
                    section = new EcransAppelSection();
                    config.Sections.Add(CONFIG_SECTION_NAME, section);
                }

                // Vide la collection existante et la remplit avec les données du ViewModel
                section.Ecrans.Clear();
                foreach (var vm in Ecrans)
                {
                    section.Ecrans.Add(new EcranAppelElement
                    {
                        Numero = vm.Numero,
                        HostName = vm.HostName,
                        IPAddress = vm.IPAddress,
                        Tapis = string.Join(",", vm.Tapis.OrderBy(t => t)) // Sauvegarde en chaîne CSV
                    });
                }

                // Sauvegarde les modifications dans le fichier app.config
                config.Save(ConfigurationSaveMode.Modified);

                // Rafraîchit les sections pour que ConfigurationManager.GetSection voie les changements
                ConfigurationManager.RefreshSection(CONFIG_SECTION_NAME);
            }
            catch (Exception ex)
            {
                // Gérer l'erreur de sauvegarde
                Console.WriteLine($"Erreur lors de la sauvegarde de la section configuration {CONFIG_SECTION_NAME}: {ex.Message}");
                // Afficher un message à l'utilisateur serait une bonne pratique
            }
        }

        /// <summary>
        /// Utilitaire pour convertir la chaîne "1,2,3" en List<int>
        /// </summary>
        private List<int> ConvertTapisStringToList(string tapisStr)
        {
            if (string.IsNullOrWhiteSpace(tapisStr))
            {
                return new List<int>();
            }

            return tapisStr.Split(',')
                           .Where(s => int.TryParse(s, out _))
                           .Select(int.Parse)
                           .ToList();
        }

        #endregion
    }
}