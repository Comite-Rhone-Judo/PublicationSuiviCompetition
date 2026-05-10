using FranceJudo.Core.Foundation;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Network.Scanner;
using FranceJudo.UI.Wpf.Foundation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppPublication.ViewModels.Configuration
{
    public class NetworkInterfaceDisplay
    {
        public NetworkInterface Interface { get; set; }
        public string DisplayName { get; set; }
        public string IpAddress { get; set; }
        public string NetworkInfo { get; set; } // NOUVEAU : Propriété pour les infos réseau
    }

    public class NetworkScannerViewModel : NotificationBase
    {
        #region MEMBRES
        private CancellationTokenSource _cts;                   // Pour l'arret de la recherche Async
        private readonly NetworkScannerContext _context;        // Le contexte de recherche pour le scanner
        #endregion

        #region PROPRIETES

        // La liste pointe directement sur celle du contexte partagé
        public ObservableCollection<NetworkDevice> Devices => _context.Devices;

        public ObservableCollection<NetworkInterfaceDisplay> Interfaces { get; } = new ObservableCollection<NetworkInterfaceDisplay>();

        private NetworkInterfaceDisplay _selectedInterface;
        public NetworkInterfaceDisplay SelectedInterface
        {
            get { return _selectedInterface; }
            set
            {
                _selectedInterface = value;
                // Mémorisation du choix de l'utilisateur dans le contexte
                if (value != null) _context.LastSelectedInterfaceId = value.Interface.Id;
                NotifyPropertyChanged();
            }
        }

        private NetworkDevice _selectedDevice;
        public NetworkDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set { _selectedDevice = value; NotifyPropertyChanged(); }
        }

        private bool _isScanning;
        public bool IsScanning
        {
            get { return _isScanning; }
            set
            {
                if (_isScanning != value)
                {
                    _isScanning = value;
                    NotifyPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        #endregion


        #region COMMANDES

        private ICommand _cmdLancerRecherche;
        public ICommand CmdLancerRecherche {
            get
            {
                _cmdLancerRecherche ??= new RelayCommand(async (o) =>
                {
                    await LancerRechercheAsync();
                },
                (o) =>
                {
                    return !IsScanning && SelectedInterface != null;
                });

                return _cmdLancerRecherche;
            }
        }
        
        private ICommand _cmdAnnulerRecherche;
        public ICommand CmdAnnulerRecherche
        {
            get
            {
                _cmdAnnulerRecherche ??= new RelayCommand( (o) =>
                {
                    AnnulerRecherche();
                },
                (o) =>
                {
                    return IsScanning;
                });

                return _cmdAnnulerRecherche;
            }
        }

        private ICommand _cmdValider;
        public ICommand CmdValider
        {
            get
            {
                _cmdValider ??= new RelayCommand(o =>
                {
                    if (o is Window window)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                },
                o =>
                {
                    return SelectedDevice != null;
                });

                return _cmdValider;
            }
        }
        private ICommand _cmdFermer; 
        public ICommand CmdFermer
        {
            get
            {
                _cmdFermer ??= new RelayCommand(o =>
                {
                    if (o is Window window)
                    {
                        window.DialogResult = false;
                        window.Close();
                    }
                },
                o =>
                {
                    return true;
                });

                return _cmdFermer;
            }
        }
        public ICommand CmdWindowClosing { get; }

        #endregion

        #region CONSTRUCTEURS
        public NetworkScannerViewModel(NetworkScannerContext context)
        {
            _context = context;
            // Nouvelles commandes
            CmdWindowClosing = new RelayCommand((o) => AnnulerRecherche());

            ChargerInterfacesReseau();
        }
        #endregion

        #region METHODES PUBLIQUES

        public void AnnulerRecherche()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }

        #endregion

        #region METHODES PRIVEES

        /// <summary>
        /// Charge les interfaces reseau de la machine
        /// </summary>
        private void ChargerInterfacesReseau()
        {
            try
            {
                var netInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => i.OperationalStatus == OperationalStatus.Up &&
                                (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet || i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211));

                foreach (var ni in netInterfaces)
                {
                    var ipInfo = ni.GetIPProperties()?.UnicastAddresses?.FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);

                    // MODIFICATION : On vérifie aussi que le masque n'est pas null
                    if (ipInfo != null && ipInfo.IPv4Mask != null)
                    {
                        // NOUVEAU : Calcul du réseau, du masque CIDR et du nombre d'appareils
                        string networkAddress = GetNetworkAddress(ipInfo.Address, ipInfo.IPv4Mask);
                        int prefixLength = GetPrefixLength(ipInfo.IPv4Mask);
                        int maxDevices = GetUsableHostCount(prefixLength);

                        Interfaces.Add(new NetworkInterfaceDisplay
                        {
                            Interface = ni,
                            DisplayName = $"{ni.Description}",
                            IpAddress = ipInfo.Address.ToString(),
                            NetworkInfo = $"{networkAddress}/{prefixLength} ({maxDevices} appareils max)" // NOUVEAU
                        });
                    }
                }

                // Restauration intelligente de la sélection précédente
                if (!string.IsNullOrEmpty(_context.LastSelectedInterfaceId))
                {
                    SelectedInterface = Interfaces.FirstOrDefault(i => i.Interface.Id == _context.LastSelectedInterfaceId) ?? Interfaces.FirstOrDefault();
                }
                else
                {
                    SelectedInterface = Interfaces.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de la lecture des cartes reseau.");
            }
        }

        /// <summary>
        /// Execute en Async la tache de scan du reseau via le Network Scanner
        /// </summary>
        /// <returns></returns>
        private async Task LancerRechercheAsync()
        {
            if (IsScanning || SelectedInterface == null) return;

            IsScanning = true;
            // On vide le cache UNIQUEMENT quand l'utilisateur clique sur "Rechercher", 
            // pas quand il ouvre simplement la fenêtre.
            Devices.Clear();

            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            try
            {
                var progress = new Progress<NetworkDevice>(device =>
                {
                    if (device != null && !string.IsNullOrWhiteSpace(device.IpAddress))
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (!Devices.Any(d => d.IpAddress == device.IpAddress))
                            {
                                Devices.Add(device);
                            }
                        });
                    }
                });

                await NetworkScanner.ScanNetworkAsync(SelectedInterface.Interface, progress, _cts.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur inattendue lors du scan reseau.");
            }
            finally
            {
                IsScanning = false;
            }
        }



        /// <summary>
        /// Calcul les informations du réseau en fonction de son adresse et de son masque
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        private string GetNetworkAddress(IPAddress ip, IPAddress mask)
        {
            byte[] ipBytes = ip.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();
            byte[] networkBytes = new byte[ipBytes.Length];

            for (int i = 0; i < ipBytes.Length; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes).ToString();
        }

        private int GetPrefixLength(IPAddress mask)
        {
            byte[] maskBytes = mask.GetAddressBytes();
            int prefixLength = 0;

            foreach (byte b in maskBytes)
            {
                int v = b;
                while (v > 0)
                {
                    prefixLength += (v & 1);
                    v >>= 1;
                }
            }
            return prefixLength;
        }

        private int GetUsableHostCount(int prefixLength)
        {
            if (prefixLength >= 31) return 0;
            return (int)Math.Pow(2, 32 - prefixLength) - 2;
        }

        #endregion
    }
}