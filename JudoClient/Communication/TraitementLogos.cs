using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public class TraitementLogos
    {
        ClientJudo _client = null;

        public TraitementLogos(ClientJudo client)
        {
            _client = client;
        }

        public delegate void OnListeLogosHandler(object sender, XElement xelements);
        public event OnListeLogosHandler OnListeLogos;

        public void ListeLogos(XElement element)
        {
            if (OnListeLogos != null)
            {
                OnListeLogos(_client, element);
            }
        }

        public delegate void OnUpdateLogosHandler(object sender, XElement xelements);
        public event OnUpdateLogosHandler OnUpdateLogos;

        public void UpdateLogos(XElement element)
        {
            if (OnUpdateLogos != null)
            {
                OnUpdateLogos(_client, element);
            }
        }
    }
}
