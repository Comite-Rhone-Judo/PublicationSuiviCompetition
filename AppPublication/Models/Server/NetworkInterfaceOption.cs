using System.Net;
using System.Net.NetworkInformation;

namespace AppPublication.Models.Server
{
    /// <summary>
    /// Définit une option de configuration pour une interface réseau, incluant son nom d'affichage, les informations d'adresse IP unicast et la présence d'une passerelle.
    /// </summary>
    public class NetworkInterfaceOption
    {
        public string DisplayName { get; set; }
        public UnicastIPAddressInformation IpInfo { get; set; }
        public bool HasGateway { get; set; }
    }
}