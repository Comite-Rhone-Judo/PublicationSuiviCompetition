using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementArbitrage
    {
        readonly ClientJudo _client = null;

        public TraitementArbitrage(ClientJudo client)
        {
            _client = client;
        }

        public delegate void OnListeArbitrageHandler(object sender, XElement xelements);
        public event OnListeArbitrageHandler OnListeArbitrage;

        public delegate void OnListeArbitreHandler(object sender, XElement xelements);
        public event OnListeArbitreHandler OnListeArbitres;

        public delegate void OnListeCommissairesHandler(object sender, XElement xelements);
        public event OnListeCommissairesHandler OnListeCommissaires;

        public delegate void OnListeDeleguesHandler(object sender, XElement xelements);
        public event OnListeDeleguesHandler OnListeDelegues;

        public void ListeArbitrage(XElement element)
        {
            OnListeArbitrage?.Invoke(_client, element);
        }

        public void ListeArbitres(XElement element)
        {
            OnListeArbitres?.Invoke(_client, element);
        }

        public void ListeCommissaires(XElement element)
        {
            OnListeCommissaires?.Invoke(_client, element);
        }

        public void ListeDelegues(XElement element)
        {
            OnListeDelegues?.Invoke(_client, element);
        }


        public delegate void OnUpdateArbitrageHandler(object sender, XElement xelements);
        public event OnUpdateArbitrageHandler OnUpdateArbitrage;

        public delegate void OnUpdateArbitreHandler(object sender, XElement xelements);
        public event OnUpdateArbitreHandler OnUpdateArbitres;

        public delegate void OnUpdateCommissairesHandler(object sender, XElement xelements);
        public event OnUpdateCommissairesHandler OnUpdateCommissaires;

        public delegate void OnUpdateDeleguesHandler(object sender, XElement xelements);
        public event OnUpdateDeleguesHandler OnUpdateDelegues;

        public void UpdateArbitrage(XElement element)
        {
            OnUpdateArbitrage?.Invoke(_client, element);
        }

        public void UpdateArbitres(XElement element)
        {
            OnUpdateArbitres?.Invoke(_client, element);
        }

        public void UpdateCommissaires(XElement element)
        {
            OnUpdateCommissaires?.Invoke(_client, element);
        }

        public void UpdateDelegues(XElement element)
        {
            OnUpdateDelegues?.Invoke(_client, element);
        }
    }
}