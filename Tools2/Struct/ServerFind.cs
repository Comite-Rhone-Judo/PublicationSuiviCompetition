namespace Tools.Struct
{
    public struct ServerFind
    {
        public System.Net.IPEndPoint IEP { get; set; }
        public string machine { get; set; }
        public string user { get; set; }
        public string competition { get; set; }
        public string addressSite { get; set; }
        public int portSite { get; set; }
    }
}
