using AppPublication.Config.Generation;
using AppPublication.Models.EcransAppel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tools.Logging;
using Tools.Windows;
using Tools.Framework;

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
        private const int kMaxTapisSelection = 4; // Constante pour la limite
        #endregion

        #region MEMBERS
        private readonly EcranAppelModel _model; // Référence vers l'objet dans GestionSite

        // Champs visuels (non stockés)
        private string _rawUserInput;
        private bool _isRechercheIpEnCours;
        private bool _isRechercheHostnameEnCours;
        private CancellationTokenSource _searchCts;
        private static List<int> _groupementOptionsStatic { get; } = new List<int> { 1, 2, 4 };

        #endregion

        #region COMMANDES
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region CONSTRUCTEUR
        /// <summary>
        /// Construit une View a partir d'un model et de la liste des tapis disponibles
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tousLesTapis"></param>
        public EcranAppelConfigViewModel(EcranAppelModel model, List<int> tousLesTapis)
        {
            _model = model;

            // Initialisation visuelle
            Hostname = string.IsNullOrEmpty(model.Hostname) ? string.Empty : model.Hostname;
            AdresseIP = (model.AdresseIP == null || model.AdresseIP.Equals(IPAddress.None))  ? string.Empty : model.AdresseIP.ToString();

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
        /// Les options de groupement des tapis (1, 2 ou 4)
        /// </summary>
        public List<int> GroupementOptions => _groupementOptionsStatic;

        /// <summary>
        /// Nombre de tapis par groupe (1, 2 ou 4)
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
                    if (cfg != null) cfg.Groupement = value; // Déclenche le IsDirty automatique

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
                    if (cfg != null) cfg.Description = value; // Déclenche le IsDirty automatique
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
                    if (cfg != null) cfg.Hostname = value;
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

                    IPAddress ip = IPAddress.None;
                    bool ipValid = IPAddress.TryParse(value, out ip);

                    // Mise à jour Modèle Runtime
                    _model.AdresseIP = ipValid ? ip : IPAddress.None;

                    // SAUVEGARDE IMMEDIATE
                    var cfg = GetConfigElement();
                    if (cfg != null) cfg.AdresseIp = ipValid ? value : string.Empty;
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
            if (cfg != null)
            {
                cfg.TapisIds = string.Join(";", ids);
            }
        }

        /// <summary>
        /// Determine si la saisie utilisateur est une adresse IP ou un hostname, et met à jour les propriétés en conséquence
        /// </summary>
        /// <param name="saisie"></param>
        private TypeSaisieEnum DeterminerTypeSaisie(string saisie)
        {
            if (string.IsNullOrWhiteSpace(saisie))
            {
                Hostname = ""; AdresseIP = ""; return TypeSaisieEnum.Inconnu;
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
                LogTools.Logger.Debug("LancerRechercheComplementaire: saisie vide ou inconnue, pas de recherche lancée.");
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
                            var e = await Dns.GetHostEntryAsync(IPAddress.Parse(saisie));
                            res = e.HostName;
                        }
                        else
                        {
                            var e = await Dns.GetHostEntryAsync(saisie);
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
            catch { }
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