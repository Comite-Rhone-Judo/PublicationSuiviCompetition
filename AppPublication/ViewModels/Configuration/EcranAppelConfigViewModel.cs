using AppPublication.Config.Generation;
using AppPublication.Models.EcransAppel;
using FranceJudo.Core.Foundation;
using FranceJudo.Core.Logging;
using FranceJudo.UI.Wpf.Dialogs;
using FranceJudo.UI.Wpf.Foundation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static AppPublication.Models.EcransAppel.EcranAppelModel;

namespace AppPublication.ViewModels.Configuration
{
    public class EcranAppelConfigViewModel : NotificationBase
    {
        private enum TypeSaisieEnum
        {
            AddressIP,
            Hostname,
            Inconnu
        }

        #region CONSTANTES
        private const int kMaxTapisSelection = 8; // Constante pour la limite
        private const int kMaxCombatsPage = 12; // Constante pour le nombre max de combats par page 
        #endregion

        #region MEMBERS
        private readonly EcranAppelModel _model; // Référence vers l'objet dans GestionSite

        // Champs visuels (non stockés)
        private string _rawUserInput;
        private bool _isRechercheIpEnCours;
        private bool _isRechercheHostnameEnCours;
        private CancellationTokenSource _searchCts;
        private readonly NetworkScannerContext _scannerContext;
        #endregion

        #region COMMANDES

        private ICommand _cmdOuvrirScanner;
        public ICommand CmdOuvrirScanner
        {
            get
            {
                _cmdOuvrirScanner ??= new RelayCommand(OuvrirScannerAction);
                return _cmdOuvrirScanner;
            }
        }

        public ICommand DeleteCommand { get; set; }

        #endregion

        #region CONSTRUCTEUR
        /// <summary>
        /// Construit une View a partir d'un model et de la liste des tapis disponibles
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tousLesTapis"></param>
        public EcranAppelConfigViewModel(EcranAppelModel model, List<int> tousLesTapis, NetworkScannerContext scannerContext)
        {
            _model = model;
            _scannerContext = scannerContext; // Sauvegarde du contexte

            // Initialisation visuelle
            Hostname = string.IsNullOrEmpty(model.Hostname) ? string.Empty : model.Hostname;
            AdresseIP = (model.AdresseIP == null || model.AdresseIP.Equals(IPAddress.None)) ? string.Empty : model.AdresseIP.ToString();

            // On pré-remplit la saisie avec le Hostname ou l'IP existant
            _rawUserInput = (model.AdresseIP != null && !model.AdresseIP.Equals(IPAddress.None) ? model.AdresseIP.ToString()
                                : !string.IsNullOrEmpty(model.Hostname) ? model.Hostname : string.Empty);

            // Création des CheckBoxes pour les tapis
            var _tmpList = new ObservableCollection<EcranAppelTapisSelectionViewModel>();
            foreach (var idTapis in tousLesTapis)
            {
                var vmTapis = new EcranAppelTapisSelectionViewModel
                {
                    Numero = idTapis,
                    IsSelected = model.TapisIds.Contains(idTapis)
                };
                // Abonnement pour sauvegarde immédiate
                vmTapis.PropertyChanged += (s, e) => { if (e.PropertyName == "IsSelected") OnTapisSelectionChanged(vmTapis); };
                _tmpList.Add(vmTapis);
            }

            ListeTapisViewModels = _tmpList;
        }

        #endregion

        #region PROPRIETES

        /// <summary>
        /// Indique le nb de combat par page a afficher (de 1 à 12)
        /// </summary>
        public int NbCombatsPage
        {
            get => _model.NbCombatsPage;
            set
            {
                if (_model.NbCombatsPage != value)
                {
                    // Controle la valeur pour éviter les erreurs de configuration*
                    if (value >= 1 && value <= kMaxCombatsPage)
                    {
                        // On ne tient pas compte d'une valeur hors range
                        _model.NbCombatsPage = value;
                        NotifyPropertyChanged();
                        var cfg = GetConfigElement();
                        cfg?.NbCombatsPage = value;
                    }
                }
            }
        }

        /// <summary>
        /// Les options de nb de combats par page
        /// </summary>
        public List<int> NbCombatsPageOptions
        {
            get
            {
                // Options de base toujours disponibles
                var options = Enumerable.Range(1, kMaxCombatsPage).ToList();
                return options;
            }
        }

        /// <summary>
        /// Indique si on doit ajuster automatiquement la taille du texte
        /// </summary>
        public bool AjusteTailleTexte
        {
            get => _model.AjusteTailleTexte;
            set
            {
                if (_model.AjusteTailleTexte != value)
                {
                    _model.AjusteTailleTexte = value;
                    NotifyPropertyChanged();
                    var cfg = GetConfigElement();
                    cfg?.AjusteTexteAuto = value;
                }
            }
        }


        /// <summary>
        /// Options pour la Dropdown de disposition (extraites dynamiquement de l'enum)
        /// </summary>
        public IEnumerable<DispositionAffichage> DispositionOptions => Enum.GetValues(typeof(DispositionAffichage)).Cast<DispositionAffichage>();

        /// <summary>
        /// Disposition de l'écran (Ligne ou Colonne)
        /// </summary>
        public DispositionAffichage Disposition
        {
            get
            {
                return _model.Disposition;
            }
            set
            {
                if (_model.Disposition != value)
                {
                    _model.Disposition = value;
                    NotifyPropertyChanged();

                    // On doit notifier pour assurer le changement
                    NotifyPropertyChanged(nameof(Groupement));

                    // SAUVEGARDE IMMEDIATE
                    var cfg = GetConfigElement();
                    cfg?.Disposition = value;
                }
            }
        }

        /// <summary>
        /// Disposition de l'écran (Ligne ou Colonne)
        /// </summary>
        public DispositionAffichage DispositionCombat
        {
            get
            {
                return _model.DispositionCombat;
            }
            set
            {
                if (_model.DispositionCombat != value)
                {
                    _model.DispositionCombat = value;
                    NotifyPropertyChanged();

                    // SAUVEGARDE IMMEDIATE
                    var cfg = GetConfigElement();
                    cfg?.DispositionCombat = value;
                }
            }
        }

        /// <summary>
        /// Les options de groupement des tapis (1, 2, 4 ou 8)
        /// </summary>
        public List<int> GroupementOptions => new List<int> { 1, 2, 4, 6, 8 };

        /// <summary>
        /// Nombre de tapis par groupe (1, 2, 4, 6 ou 8)
        /// </summary>
        public int Groupement
        {
            get
            {
                return _model.Groupement;
            }
            set
            {
                if (_model.Groupement != value)
                {
                    _model.Groupement = value;
                    NotifyPropertyChanged();

                    // SAUVEGARDE IMMEDIATE
                    var cfg = GetConfigElement();
                    cfg?.Groupement = value; // Déclenche le IsDirty automatique
                }
            }
        }

        private string _listeTapisSelectionnesAffiche;
        public string ListeTapisSelectionnesAffiche
        {
            get
            {
                return _listeTapisSelectionnesAffiche;
            }

            private set
            {
                _listeTapisSelectionnesAffiche = value;
                NotifyPropertyChanged();
            }
        }

        public int Id => _model.Id;

        public string Description
        {
            get { return _model.Description; }
            set
            {
                if (_model.Description != value)
                {
                    _model.Description = value;
                    NotifyPropertyChanged();

                    // SAUVEGARDE IMMEDIATE
                    var cfg = GetConfigElement();
                    cfg?.Description = value; // Déclenche le IsDirty automatique
                }
            }
        }

        private string _hostname = string.Empty;
        public string Hostname
        {
            get { return _hostname; }
            set
            {
                if (_hostname != value)
                {
                    _hostname = value;
                    NotifyPropertyChanged();

                    // SAUVEGARDE IMMEDIATE
                    _model.Hostname = value;
                    var cfg = GetConfigElement();
                    cfg?.Hostname = value;
                }
            }
        }

        private string _ipAdresse = string.Empty;
        public string AdresseIP
        {
            get { return _ipAdresse.ToString(); }
            set
            {
                if (_ipAdresse != value)
                {
                    _ipAdresse = value;
                    NotifyPropertyChanged();

                    // 2. Tentative de parsing
                    if (IPAddress.TryParse(value, out IPAddress ip))
                    {
                        // On ne met à jour le modèle QUE si l'IP est valide et n'est pas "None"
                        if (!ip.Equals(IPAddress.None))
                        {
                            _model.AdresseIP = ip;

                            // SAUVEGARDE IMMEDIATE
                            var cfg = GetConfigElement();
                            cfg?.AdresseIp = value;
                        }
                    }
                }
            }
        }

        // Champ de saisie pour la recherche (non stocké)
        public string RawUserInput
        {
            get { return _rawUserInput; }
            set
            {
                if (_rawUserInput != value)
                {
                    _rawUserInput = value;
                    NotifyPropertyChanged();
                    var typeSaisie = DeterminerTypeSaisie(value);
                    LancerRechercheComplementaire(value, typeSaisie);
                }
            }
        }

        private ObservableCollection<EcranAppelTapisSelectionViewModel> _listeTapisViewModels;

        /// <summary>
        /// La lisye des ViewModels de sélection des tapis
        /// </summary>
        public ObservableCollection<EcranAppelTapisSelectionViewModel> ListeTapisViewModels
        {
            get
            {
                return _listeTapisViewModels;
            }
            set
            {
                _listeTapisViewModels = value;
                NotifyPropertyChanged();
                ListeTapisSelectionnesAffiche = GetListeTapisAffiche();
            }
        }
        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// True si une recherche asynchrone d'IP est en cours
        /// </summary>
        public bool IsRechercheIpEnCours
        {
            get { return _isRechercheIpEnCours; }
            set
            {
                if (_isRechercheIpEnCours != value)
                {
                    _isRechercheIpEnCours = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsRechercheEnCours));
                }
            }
        }

        /// <summary>
        /// True si une recherche asynchrone de hostname est en cours
        /// </summary>
        public bool IsRechercheHostnameEnCours
        {
            get { return _isRechercheHostnameEnCours; }
            set
            {
                if (_isRechercheHostnameEnCours != value)
                {
                    _isRechercheHostnameEnCours = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsRechercheEnCours));
                }
            }
        }

        /// <summary>
        /// True si une recherche asynchrone est en cours
        /// </summary>
        public bool IsRechercheEnCours
        {
            get { return IsRechercheIpEnCours || IsRechercheHostnameEnCours; }
        }

        /// <summary>
        /// Annule la recherche en cours
        /// </summary>
        public void CancelSearch()
        {
            _searchCts?.Cancel();
        }


        #endregion

        #region METHODES PRIVEES

        /// <summary>
        /// Ouvre la fenetre de scanner reseau
        /// </summary>
        /// <param name="obj"></param>
        private void OuvrirScannerAction(object obj)
        {
            // On passe le contexte au ViewModel du scanner
            var vm = new NetworkScannerViewModel(_scannerContext);
            var win = new AppPublication.Views.Configuration.NetworkScannerView { DataContext = vm };

            // CORRECTION DU BUG DE LA FENÊTRE QUI DISPARAÎT :
            // On attache la fenêtre modale à la fenêtre actuellement active (votre fenêtre de config),
            // et non à Application.Current.MainWindow qui pourrait être cachée en arrière-plan.
            var activeWindow = System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (activeWindow != null)
            {
                win.Owner = activeWindow;
            }

            if (win.ShowDialog() == true && vm.SelectedDevice != null)
            {
                if (!string.IsNullOrWhiteSpace(vm.SelectedDevice.IpAddress))
                {
                    RawUserInput = vm.SelectedDevice.IpAddress; // Déclenche la recherche Hostname
                }
            }
        }

        // --- Helpers Configuration ---
        private EcransAppelConfigElement GetConfigElement()
        {
            // On va chercher l'élément correspondant dans la config globale
            if (GenerationConfigSection.Instance != null && GenerationConfigSection.Instance.Ecrans != null)
            {
                return GenerationConfigSection.Instance.Ecrans.GetElementById(Id);
            }
            return null;
        }

        /// <summary>
        /// Nouvelle méthode pour gérer la restriction du nombre de tapis
        /// </summary>
        private void OnTapisSelectionChanged(EcranAppelTapisSelectionViewModel changedItem)
        {
            // Si l'utilisateur vient de cocher une case
            if (changedItem.IsSelected)
            {
                int count = ListeTapisViewModels.Count(t => t.IsSelected);
                if (count > kMaxTapisSelection)
                {
                    // On annule la sélection (ceci va déclencher récursivement OnTapisSelectionChanged avec IsSelected=false)
                    changedItem.IsSelected = false;

                    // Optionnel : Afficher un message à l'utilisateur
                    AlertWindow win = new AlertWindow("Limite atteinte", $"Vous ne pouvez sélectionner que {kMaxTapisSelection} tapis maximum par écran.");
                    win.ShowDialog();

                    return; // On sort pour ne pas sauvegarder l'état invalide
                }
            }

            // On pense a changer la chaine affichée
            ListeTapisSelectionnesAffiche = GetListeTapisAffiche();

            // Si tout est OK (ou si on vient de décocher suite à l'annulation), on sauvegarde
            UpdateTapisAndSave();
        }

        /// <summary>
        /// Met a jour la chaine des tapis sélectionnés dans le modèle et la configuration
        /// </summary>
        private void UpdateTapisAndSave()
        {
            var ids = ListeTapisViewModels.Where(t => t.IsSelected).Select(t => t.Numero).ToList();

            // Mise à jour Modèle Runtime
            _model.TapisIds = ids;

            // Mise à jour Configuration
            var cfg = GetConfigElement();
            cfg?.TapisIds = string.Join(";", ids);
        }

        /// <summary>
        /// Determine si la saisie utilisateur est une adresse IP ou un hostname, et met à jour les propriétés en conséquence
        /// </summary>
        /// <param name="saisie"></param>
        private TypeSaisieEnum DeterminerTypeSaisie(string saisie)
        {
            if (string.IsNullOrWhiteSpace(saisie))
            {
                return TypeSaisieEnum.Inconnu;
            }

            if (IPAddress.TryParse(saisie, out _))
            {
                AdresseIP = saisie;
                return TypeSaisieEnum.AddressIP;
            }
            else
            {
                Hostname = saisie;
                return TypeSaisieEnum.Hostname;
            }
        }

        /// <summary>
        /// Lance une recherche asynchrone pour compléter l'adresse IP ou le hostname en fonction de la saisie utilisateur
        /// </summary>
        /// <param name="saisie"></param>
        private async void LancerRechercheComplementaire(string saisie, TypeSaisieEnum type)
        {
            if (_searchCts != null)
            {
                _searchCts.Cancel();
                _searchCts.Dispose();
                IsRechercheHostnameEnCours = false;
                IsRechercheIpEnCours = false;
            }

            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            if (string.IsNullOrWhiteSpace(saisie) || type == TypeSaisieEnum.Inconnu)
            {
                LogTools.Logger.Debug("LancerRechercheComplementaire: saisie vide ou inconnue, pas de recherche lancee.");
                return;
            }

            // Determine le type de recherche a effectuer et Vide le champ que l'on va rechercher
            switch (type)
            {
                case TypeSaisieEnum.AddressIP:
                    IsRechercheHostnameEnCours = true;
                    Hostname = String.Empty;
                    break;
                case TypeSaisieEnum.Hostname:
                    IsRechercheIpEnCours = true;
                    AdresseIP = String.Empty;
                    break;
                default:
                    break;
            }

            try
            {
                await Task.Delay(500, token);
                string res = "";
                bool isIp = type == TypeSaisieEnum.AddressIP;

                await Task.Run(async () =>
                {
                    try
                    {
                        if (isIp)
                        {
                            // var e = await Dns.GetHostEntryAsync(IPAddress.Parse(saisie));
                            var e = Dns.GetHostEntry(IPAddress.Parse(saisie));
                            res = e.HostName;
                        }
                        else
                        {
                            // var e = await Dns.GetHostEntryAsync(saisie);
                            var e = Dns.GetHostEntry(saisie);
                            var i = e.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                            if (i != null)
                            {
                                res = i.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTools.Logger.Warn($"LancerRechercheComplementaire: Erreur lors de la recherche DNS pour '{saisie}': {ex.Message}");
                    }
                }, token);

                if (!token.IsCancellationRequested && !string.IsNullOrEmpty(res))
                {
                    if (isIp)
                    {
                        Hostname = res; // Setter -> Sauvegarde
                    }
                    else
                    {
                        AdresseIP = res;     // Setter -> Sauvegarde
                    }
                }
            }
            catch (OperationCanceledException) { /* Ignoré lors de l'annulation */ }
            catch (Exception ex)
            {
                LogTools.Logger.Warn(ex, $"Erreur DNS : {ex.Message}");
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    IsRechercheIpEnCours = false;
                    IsRechercheHostnameEnCours = false;
                }
            }
        }

        /// <summary>
        /// Calcul une chaîne affichant les tapis sélectionnés de manière lisible
        /// </summary>
        /// <returns></returns>
        private string GetListeTapisAffiche()
        {
            // 1. Récupération des IDs sélectionnés
            // On suppose que ListeTapisViewModels contient des objets avec 'Id' et 'IsSelected'
            var idsSelectionnes = ListeTapisViewModels
                                    .Where(vm => vm.IsSelected)
                                    .Select(vm => vm.Numero)
                                    .OrderBy(id => id)
                                    .ToList();

            // 2. Gestion des cas simples
            if (idsSelectionnes.Count == 0)
                return "Aucun tapis";

            if (idsSelectionnes.Count == 1)
                return $"Tapis {idsSelectionnes[0]}";

            // 3. Formatage complexe : "Tapis 1, 2 et 5"
            // On prend tous les éléments sauf le dernier pour les joindre par une virgule
            string partieVirgule = string.Join(", ", idsSelectionnes.Take(idsSelectionnes.Count - 1));

            // On récupère le dernier pour le préfixer par " et "
            string dernier = idsSelectionnes.Last().ToString();

            return $"Tapis {partieVirgule} et {dernier}";
        }

        #endregion
    }
}