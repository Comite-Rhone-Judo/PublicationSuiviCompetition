using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementConnexion
    {
        readonly ClientJudo _client = null;

        public TraitementConnexion(ClientJudo client)
        {
            _client = client;
        }



        public delegate void OnAcceptConnectionPeseeHandler(object sender, XElement doc);
        public event OnAcceptConnectionPeseeHandler OnAcceptConnectionPesee;

        public delegate void OnAcceptConnectionCSHandler(object sender, XElement doc);
        public event OnAcceptConnectionCSHandler OnAcceptConnectionCS;

        public delegate void OnAcceptConnectionCOMHandler(object sender, XElement doc);
        public event OnAcceptConnectionCOMHandler OnAcceptConnectionCOM;

        public void AcceptConnectionPesee(XElement doc)
        {
            OnAcceptConnectionPesee?.Invoke(_client, doc);
        }

        public void AcceptConnectionCS(XElement doc)
        {
            OnAcceptConnectionCS?.Invoke(_client, doc);
        }

        public void AcceptConnectionCOM(XElement doc)
        {
            OnAcceptConnectionCOM?.Invoke(_client, doc);
        }


        public delegate void OnAcceptConnectionTestHandler(object sender, XElement doc);
        public event OnAcceptConnectionTestHandler OnAcceptConnectionTest;

        public void AcceptConnectionTest(XElement doc)
        {
            OnAcceptConnectionTest?.Invoke(_client, doc);
        }
    }
}