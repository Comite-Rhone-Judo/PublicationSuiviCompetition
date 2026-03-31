using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementLogos
    {
        readonly ClientJudo _client = null;

        public TraitementLogos(ClientJudo client)
        {
            _client = client;
        }

        public delegate void OnListeLogosHandler(object sender, XElement xelements);
        public event OnListeLogosHandler OnListeLogos;

        public void ListeLogos(XElement element)
        {
            OnListeLogos?.Invoke(_client, element);
        }

        public delegate void OnUpdateLogosHandler(object sender, XElement xelements);
        public event OnUpdateLogosHandler OnUpdateLogos;

        public void UpdateLogos(XElement element)
        {
            OnUpdateLogos?.Invoke(_client, element);
        }
    }
}
