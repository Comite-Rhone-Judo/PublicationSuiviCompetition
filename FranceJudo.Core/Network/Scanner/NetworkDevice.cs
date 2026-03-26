namespace FranceJudo.Core.Network.Scanner
{
    public class NetworkDevice
    {
        public string IpAddress { get; set; }
        public string Hostname { get; set; } 
        public string MacAddress { get; set; }
        public DeviceType Category { get; set; }

        public override string ToString() => $"{IpAddress,-15} | {Hostname,-20} | {Category,-20} | MAC: {MacAddress}";
    }
}