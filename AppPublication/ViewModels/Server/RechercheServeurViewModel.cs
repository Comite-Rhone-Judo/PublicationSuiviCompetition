using AppPublication.Models;
using AppPublication.Models.Server;
using FranceJudo.Core.Foundation;
using FranceJudo.Core.Logging;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;
using FranceJudo.UI.Wpf.Foundation;
using JudoClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AppPublication.ViewModels.Server
{
    public class RechercheServeurViewModel : NotificationBase
    {
        #region MEMBRES PRIVÉS
        private CancellationTokenSource _scanCts;

        #endregion

        #region PROPRIETES BINDÉES

        public ObservableCollection<NetworkInterfaceOption> InterfacesReseau { get; } = new ObservableCollection<NetworkInterfaceOption>();

        private NetworkInterfaceOption _selectedInterface;
        public NetworkInterfaceOption SelectedInterface
        {
            get => _selectedInterface;
            set { _selectedInterface = value; NotifyPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        // NOUVELLE PROPRIÉTÉ POUR LA SAISIE MANUELLE
        private string _rechercheManuelle;
        /// <summary>
        /// Obtient ou définit la valeur de la recherche manuelle.
        /// </summary>
        public string RechercheManuelle
        {
            get => _rechercheManuelle;
            set
            {
                _rechercheManuelle = value;
                NotifyPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool _bypassPing;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le ping doit être ignoré.
        /// </summary>
        public bool BypassPing
        {
            get => _bypassPing;
            set { _bypassPing = value; NotifyPropertyChanged(); }
        }

        private bool _isAdvancedMode = false;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le mode avancé est activé.
        /// </summary>
        public bool IsAdvancedMode
        {
            get => _isAdvancedMode;
            set
            {
                if (_isAdvancedMode != value)
                {
                    _isAdvancedMode = value;
                    NotifyPropertyChanged();

                    // Si on sort du mode avancé, on restaure les paramètres par défaut
                    if (!_isAdvancedMode)
                    {
                        BypassPing = false;
                        SelectedInterface = InterfacesReseau.FirstOrDefault(x => x.HasGateway) ?? InterfacesReseau.FirstOrDefault();
                    }
                }
            }
        }

        public ObservableCollection<ServerFoundEventArgs> ServeursTrouves { get; } = new ObservableCollection<ServerFoundEventArgs>();

        private ServerFoundEventArgs _selectedServeur;
        /// <summary>
        /// Obtient ou définit le serveur sélectionné.
        /// </summary>
        public ServerFoundEventArgs SelectedServeur
        {
            get => _selectedServeur;
            set { _selectedServeur = value; NotifyPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        private bool _isScanning;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si un scan est en cours.
        /// </summary>
        public bool IsScanning
        {
            get => _isScanning;
            set { _isScanning = value; NotifyPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        private int _scanProgress;
        /// <summary>
        /// Obtient ou définit la progression du scan.
        /// </summary>
        public int ScanProgress
        {
            get => _scanProgress;
            set { _scanProgress = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region COMMANDES

        public ICommand CmdLancerRecherche { get; }
        public ICommand CmdConnecterServeur { get; }

        #endregion

        public RechercheServeurViewModel()
        {
            ChargerInterfacesReseau();

            CmdLancerRecherche = new RelayCommand(
                async o => await LancerScanAsync(),
                // La commande est valide si un scan n'est pas en cours ET (qu'une interface est choisie OU qu'une saisie manuelle est renseignée)
                o => !IsScanning && (SelectedInterface != null || !string.IsNullOrWhiteSpace(RechercheManuelle))
            );

            CmdConnecterServeur = new RelayCommand(
                            o => ConnecterAuServeur(SelectedServeur),
                            o => SelectedServeur != null
                        );
        }

        #region LOGIQUE METIER

        /// <summary>
        /// Charge les interfaces réseau disponibles.
        /// </summary>
        private void ChargerInterfacesReseau()
        {
            InterfacesReseau.Clear();

            // GetAllNetworkInterfaces() renvoie les interfaces dans l'ordre de priorite defini par Windows
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            var tempList = new List<NetworkInterfaceOption>();

            foreach (var ni in interfaces)
            {
                var ipProps = ni.GetIPProperties();
                bool hasGateway = ipProps.GatewayAddresses.Any(g => g.Address != null && !g.Address.Equals(IPAddress.Any));

                foreach (var addr in ipProps.UnicastAddresses.Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
                {
                    if (addr.Address.GetAddressBytes()[0] == 169) continue; // Ignore APIPA

                    // On ne marque que les reseaux sans Gateway
                    string qualifier = hasGateway ? "" : " (Isolée)";

                    tempList.Add(new NetworkInterfaceOption
                    {
                        IpInfo = addr,
                        HasGateway = hasGateway,
                        DisplayName = $"{ni.Name} - {addr.Address}{qualifier}"
                    });
                }
            }

            // OrderByDescending effectue un tri stable. L'ordre de priorite Windows est conserve pour 
            // les interfaces avec Gateway, qui apparaitront simplement avant celles isolees.
            var triees = tempList.OrderByDescending(x => x.HasGateway).ToList();

            foreach (var item in triees)
            {
                InterfacesReseau.Add(item);
            }

            // Selection par defaut : la premiere interface fonctionnelle non isolee
            SelectedInterface = InterfacesReseau.FirstOrDefault(x => x.HasGateway) ?? InterfacesReseau.FirstOrDefault();
        }

        /// <summary>
        /// Lance le scan des serveurs.
        /// </summary>
        /// <returns></returns>
        private async Task LancerScanAsync()
        {
            // --- TRACE DE DÉBUT EXPLICITE ---
            LogTools.Logger?.Debug("=========================================================================");
            LogTools.Logger?.Debug("=> DÉBUT DE LA SÉQUENCE DE SCAN RÉSEAU");
            LogTools.Logger?.Debug("=========================================================================");

            ServeursTrouves.Clear();
            ScanProgress = 0;
            IsScanning = true;

            var scanner = new ScannerServeurJudo
            {
                BypassPing = this.BypassPing,
                PingTimeoutMs = 1500,
                TcpTimeoutMs = 3000
            };

            // LOGIQUE DE SÉLECTION DE LA CIBLE : 
            // La saisie manuelle (RechercheManuelle) prime sur la sélection de la carte réseau
            if (!string.IsNullOrWhiteSpace(RechercheManuelle))
            {
                string target = RechercheManuelle.Trim();

                // Si l'utilisateur saisit un nom de machine (Hostname) au lieu d'une IP
                if (!IPAddress.TryParse(target, out _))
                {
                    try
                    {
                        // Résolution DNS basique avant de passer l'IP au scanner
                        var hostEntry = await Dns.GetHostEntryAsync(target);
                        var resolvedIp = hostEntry.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                        if (resolvedIp != null)
                        {
                            target = resolvedIp.ToString();
                        }
                    }
                    catch
                    {
                        // Echec DNS, le scan TCP gérera l'erreur
                    }
                }

                scanner.SetExplicitIps(new[] { target });
            }
            else if (SelectedInterface != null)
            {
                scanner.SetNetworkRange(SelectedInterface.IpInfo.Address.ToString(), SelectedInterface.IpInfo.IPv4Mask.ToString());
            }
            else
            {
                IsScanning = false;
                LogTools.Logger?.Debug("=> FIN PREMATUREE : Aucune cible de scan definie.");
                LogTools.Logger?.Debug("=========================================================================");
                return;
            }

            scanner.OnServerFound += Scanner_OnServerFound;

            _scanCts?.Cancel();
            _scanCts = new CancellationTokenSource();
            var reporter = new Progress<int>(percent => ScanProgress = percent);

            try
            {
                int nbTrouves = await scanner.ScanAsync(reporter, _scanCts.Token);
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex, "Erreur lors du scan réseau.");
            }
            finally
            {
                scanner.OnServerFound -= Scanner_OnServerFound;
                IsScanning = false;
                ScanProgress = 100;

                LogTools.Logger?.Debug("=========================================================================");
                LogTools.Logger?.Debug("=> FIN DE LA SEQUENCE DE RECHERCHE DE SERVEURS");
                LogTools.Logger?.Debug("=========================================================================");
            }
        }

        /// <summary>
        /// Gère l'événement de découverte d'un serveur.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement.</param>
        /// <param name="e">Les arguments de l'événement.</param>
        /// <summary>
        /// Gère l'événement de découverte d'un serveur.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement.</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private void Scanner_OnServerFound(object sender, ServerFoundEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!ServeursTrouves.Any(s => s.EndPoint.Equals(e.EndPoint)))
                {
                    ServeursTrouves.Add(e);
                }
            }));
        }

        /// <summary>
        /// Connecte l'application à un serveur trouvé.
        /// </summary>
        /// <param name="serveurInfo">Les informations du serveur à connecter.</param>
        private void ConnecterAuServeur(ServerFoundEventArgs serveurInfo)
        {
            if (serveurInfo == null) return;

            // 1. On coupe proprement le scanner s'il tournait encore
            _scanCts?.Cancel();

            try
            {
                // 2. Extraction sécurisée des informations du site (HTTP/Web) depuis le XML reçu
                string addressSite = serveurInfo.Competition.Attribute(ConstantXML.AddressSite)?.Value ?? serveurInfo.EndPoint.Address.ToString();
                string portSiteStr = serveurInfo.Competition.Attribute(ConstantXML.PortSite)?.Value ?? "80";

                // 3. Configuration du contrôleur pour les requêtes web/API
                Controles.DialogControleur.Instance.Connection.IpAdress = addressSite;
                Controles.DialogControleur.Instance.Connection.Port = portSiteStr;

                // 4. Instanciation du client TCP natif sur le port de découverte (ex: 8480)
                Controles.DialogControleur.Instance.Connection.Client = new ClientJudo(serveurInfo.EndPoint.Address.ToString(), serveurInfo.EndPoint.Port);

                LogTools.Logger?.Debug($"RechercheServeurViewModel: Connexion configuree. TCP -> {serveurInfo.EndPoint} | Web -> {addressSite}:{portSiteStr}");
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex, "Erreur lors de l'extraction des parametres XML de connexion.");
            }

            // 5. Ordre de fermeture de la vue
            OnRequestClose?.Invoke();
        }

        public Action OnRequestClose { get; set; }

        public void CancelSearch()
        {
            _scanCts?.Cancel();
        }

        #endregion
    }
}