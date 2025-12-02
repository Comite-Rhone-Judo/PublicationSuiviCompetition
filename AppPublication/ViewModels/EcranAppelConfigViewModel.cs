using AppPublication.Config;
using AppPublication.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Tools.Outils;

namespace AppPublication.ViewModels
{
    public class EcranAppelConfigViewModel : NotificationBase
    {
        #region CONSTANTES
        private const string kNotFoundPlaceholder = "<...>";
        #endregion

        #region MEMBERS
        private readonly EcranAppelModel _model; // Référence vers l'objet dans GestionSite

        // Champs visuels (non stockés)
        private string _rawUserInput;
        private bool _isRechercheEnCours;
        private CancellationTokenSource _searchCts;
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
            if (!string.IsNullOrEmpty(model.Hostname)) _rawUserInput = model.Hostname;
            else _rawUserInput = model.AdresseIP.ToString();

            // Création des CheckBoxes pour les tapis
            ListeTapisVM = new ObservableCollection<EcranAppelTapisSelectionViewModel>();
            foreach (var idTapis in tousLesTapis)
            {
                var vmTapis = new EcranAppelTapisSelectionViewModel
                {
                    Numero = idTapis,
                    IsSelected = model.TapisIds.Contains(idTapis)
                };
                // Abonnement pour sauvegarde immédiate
                vmTapis.PropertyChanged += (s, e) => { if (e.PropertyName == "IsSelected") UpdateTapisAndSave(); };
                ListeTapisVM.Add(vmTapis);
            }
        }

        #endregion

        #region PROPRIETES
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
                    if (cfg != null) cfg.Nom = value; // Déclenche le IsDirty automatique
                }
            }
        }

        private string _hostname;
        public string Hostname
        {
            get { return _hostname; }
            set
            {
                if (_hostname != value)
                {
                    _hostname = value;
                    NotifyPropertyChanged();

                    // On ne sauvegarde pas le placeholder dans le modele car c'est juste de l'affichage
                    if (value != kNotFoundPlaceholder)
                    {
                        _model.Hostname = value;

                        // SAUVEGARDE IMMEDIATE
                        var cfg = GetConfigElement();
                        if (cfg != null) cfg.Hostname = value;
                    }
                }
            }
        }

        private string _ipAdresse;
        public string AdresseIP
        {
            get { return _ipAdresse.ToString(); }
            set
            {
                if (_ipAdresse != value)
                {
                    _ipAdresse = value;
                    NotifyPropertyChanged();

                    // On ne sauvegarde pas le placeholder dans le modele car c'est juste de l'affichage
                    if (value != kNotFoundPlaceholder)
                    {
                        IPAddress ip = IPAddress.None;
                        bool ipValid = IPAddress.TryParse(value, out ip);

                        if (ipValid)
                        {
                            {
                                // SAUVEGARDE IMMEDIATE
                                var cfg = GetConfigElement();
                                if (cfg != null) cfg.AdresseIp = value;
                            }
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
                    DeterminerTypeSaisie(value);
                    LancerRechercheComplementaire(value);
                }
            }
        }
        /// <summary>
        /// La lisye des ViewModels de sélection des tapis
        /// </summary>
        public ObservableCollection<EcranAppelTapisSelectionViewModel> ListeTapisVM { get; set; }
        #endregion

        #region METHODES PUBLIQUES
        /// <summary>
        /// True si une recherche asynchrone est en cours
        /// </summary>
        public bool IsRechercheEnCours
        {
            get { return _isRechercheEnCours; }
            set { if (_isRechercheEnCours != value) { _isRechercheEnCours = value; NotifyPropertyChanged(); } }
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
        private EcranConfigElement GetConfigElement()
        {
            // On va chercher l'élément correspondant dans la config globale
            if (EcransConfigSection.Instance != null && EcransConfigSection.Instance.Ecrans != null)
            {
                return EcransConfigSection.Instance.Ecrans.Cast<EcranConfigElement>()
                                          .FirstOrDefault(e => e.Id == this.Id);
            }
            return null;
        }

        /// <summary>
        /// Met a jour la chaine des tapis sélectionnés dans le modèle et la configuration
        /// </summary>
        private void UpdateTapisAndSave()
        {
            var ids = ListeTapisVM.Where(t => t.IsSelected).Select(t => t.Numero).ToList();

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
        private void DeterminerTypeSaisie(string saisie)
        {
            if (string.IsNullOrWhiteSpace(saisie)) { Hostname = ""; AdresseIP = ""; return; }
            if (IPAddress.TryParse(saisie, out _)) { AdresseIP = saisie; if (string.IsNullOrEmpty(Hostname)) Hostname = kNotFoundPlaceholder; }
            else { Hostname = saisie; if (string.IsNullOrEmpty(AdresseIP)) AdresseIP = kNotFoundPlaceholder; }
        }

        /// <summary>
        /// Lance une recherche asynchrone pour compléter l'adresse IP ou le hostname en fonction de la saisie utilisateur
        /// </summary>
        /// <param name="saisie"></param>
        private async void LancerRechercheComplementaire(string saisie)
        {
            if (_searchCts != null) { _searchCts.Cancel(); _searchCts.Dispose(); }
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            if (string.IsNullOrWhiteSpace(saisie)) { IsRechercheEnCours = false; return; }

            IsRechercheEnCours = true;
            try
            {
                await Task.Delay(500, token);
                string res = "";
                bool isIp = IPAddress.TryParse(saisie, out IPAddress ipAddr);

                await Task.Run(async () => {
                    try
                    {
                        if (isIp) { var e = await Dns.GetHostEntryAsync(ipAddr); res = e.HostName; }
                        else { var e = await Dns.GetHostEntryAsync(saisie); var i = e.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork); if (i != null) res = i.ToString(); }
                    }
                    catch { }
                }, token);

                if (!token.IsCancellationRequested && !string.IsNullOrEmpty(res))
                {
                    if (isIp) Hostname = res; // Setter -> Sauvegarde
                    else AdresseIP = res;     // Setter -> Sauvegarde
                }
            }
            catch { }
            finally { if (!token.IsCancellationRequested) IsRechercheEnCours = false; }
        }

        #endregion
    }
}