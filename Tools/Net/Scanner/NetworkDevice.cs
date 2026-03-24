namespace Tools.Net.Scanner
{
    public class NetworkDevice
    {
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public DeviceType Category { get; set; }

        public override string ToString() => $"{IpAddress,-15} | {Category,-20} | MAC: {MacAddress}";
    }
}