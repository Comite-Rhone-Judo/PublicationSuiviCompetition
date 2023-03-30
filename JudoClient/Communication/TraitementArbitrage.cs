using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementArbitrage
    {
        ClientJudo _client = null;

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
            if (OnListeArbitrage != null)
            {
                OnListeArbitrage(_client, element);
            }
        }

        public void ListeArbitres(XElement element)
        {
            if (OnListeArbitres != null)
            {
                OnListeArbitres(_client, element);
            }
        }

        public void ListeCommissaires(XElement element)
        {
            if (OnListeCommissaires != null)
            {
                OnListeCommissaires(_client, element);
            }
        }

        public void ListeDelegues(XElement element)
        {
            if (OnListeDelegues != null)
            {
                OnListeDelegues(_client, element);
            }
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
            if (OnUpdateArbitrage != null)
            {
                OnUpdateArbitrage(_client, element);
            }
        }

        public void UpdateArbitres(XElement element)
        {
            if (OnUpdateArbitres != null)
            {
                OnUpdateArbitres(_client, element);
            }
        }

        public void UpdateCommissaires(XElement element)
        {
            if (OnUpdateCommissaires != null)
            {
                OnUpdateCommissaires(_client, element);
            }
        }

        public void UpdateDelegues(XElement element)
        {
            if (OnUpdateDelegues != null)
            {
                OnUpdateDelegues(_client, element);
            }
        }
    }
}