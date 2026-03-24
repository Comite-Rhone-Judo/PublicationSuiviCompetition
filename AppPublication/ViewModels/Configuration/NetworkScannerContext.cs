using System.Collections.ObjectModel;
using Tools.Net.Scanner;

namespace AppPublication.ViewModels.Configuration
{
    /// <summary>
    /// Contient l'état du scanner (Cache des appareils et préférences).
    /// Il n'est PAS statique, il est porté par le ViewModel principal.
    /// </summary>
    public class NetworkScannerContext
    {
        public ObservableCollection<NetworkDevice> Devices { get; } = new ObservableCollection<NetworkDevice>();

        // On sauvegarde l'ID de l'interface réseau pour la resélectionner automatiquement
        public string LastSelectedInterfaceId { get; set; }
    }
}