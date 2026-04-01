namespace FranceJudo.Metier.Network
{
    public struct ServerFind
    {
        public System.Net.IPEndPoint IEP { get; set; }
        public string Machine { get; set; }
        public string User { get; set; }
        public string Competition { get; set; }
        public string AddresseSite { get; set; }
        public int PortSite { get; set; }
    }
}