using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tools.Framework;
using Tools.Logging;
using Tools.Net.Scanner;

namespace AppPublication.ViewModels.Configuration
{
    public class NetworkInterfaceDisplay
    {
        public NetworkInterface Interface { get; set; }
        public string DisplayName { get; set; }
        public string IpAddress { get; set; }
    }

    public class NetworkScannerViewModel : NotificationBase
    {
        private readonly NetworkScannerContext _context;

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

        private CancellationTokenSource _cts;

        public ICommand CmdLancerRecherche { get; }
        public ICommand CmdAnnulerRecherche { get; }

        public NetworkScannerViewModel(NetworkScannerContext context)
        {
            _context = context;

            CmdLancerRecherche = new RelayCommand(async (o) => await LancerRechercheAsync(), (o) => !IsScanning && SelectedInterface != null);
            CmdAnnulerRecherche = new RelayCommand((o) => AnnulerRecherche(), (o) => IsScanning);

            ChargerInterfacesReseau();
        }

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
                    if (ipInfo != null)
                    {
                        Interfaces.Add(new NetworkInterfaceDisplay
                        {
                            Interface = ni,
                            DisplayName = $"{ni.Name} ({ni.Description})",
                            IpAddress = ipInfo.Address.ToString()
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

        public void AnnulerRecherche()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }
    }
}