using FranceJudo.Metier.XML;
using System.Xml.Linq;


namespace JudoClient.Communication
{
    public class TraitementDeroulement
    {
        readonly ClientJudo _client = null;

        public TraitementDeroulement(ClientJudo client)
        {
            _client = client;
        }


        public delegate void OnListePhasesHandler(object sender, XElement xelements);
        public event OnListePhasesHandler OnListePhases;

        public delegate void OnListeCombatsHandler(object sender, XElement xelements);
        public event OnListeCombatsHandler OnListeCombats;

        public delegate void OnCombatReceivedHandler(object sender, int combat);
        public event OnCombatReceivedHandler OnCombatReceived;

        public delegate void OnRencontreReceivedHandler(object sender, int rencontre);
        public event OnRencontreReceivedHandler OnRencontreReceived;

        public delegate void OnUpdateRencontreReceivedHandler(object sender, XElement xelements);
        public event OnUpdateRencontreReceivedHandler OnUpdateRencontreReceived;


        public void ListePhases(XElement element)
        {
            OnListePhases?.Invoke(_client, element);
        }

        public void ListeCombats(XElement element)
        {
            OnListeCombats?.Invoke(_client, element);
        }

        public void CombatReceived(XElement element)
        {
            int combat = int.Parse(element.Element(ConstantXML.Combat).Value);
            OnCombatReceived?.Invoke(_client, combat);
        }

        public void RencontreReceived(XElement element)
        {
            int rencontre = int.Parse(element.Element(ConstantXML.Rencontre).Value);
            OnRencontreReceived?.Invoke(_client, rencontre);
        }

        public void UpdateRencontreReceived(XElement element)
        {
            OnUpdateRencontreReceived?.Invoke(_client, element);
        }

        public delegate void OnUpdatePhasesHandler(object sender, XElement xelements);
        public event OnUpdatePhasesHandler OnUpdatePhases;

        public delegate void OnUpdateCombatsHandler(object sender, XElement xelements);
        public event OnUpdateCombatsHandler OnUpdateCombats;

        public delegate void OnUpdateTapisCombatsHandler(object sender, XElement xelements);
        public event OnUpdateTapisCombatsHandler OnUpdateTapisCombats;

        public void UpdatePhases(XElement element)
        {
            OnUpdatePhases?.Invoke(_client, element);
        }

        public void UpdateCombats(XElement element)
        {
            OnUpdateCombats?.Invoke(_client, element);
        }
        public void UpdateTapisCombats(XElement element)
        {
            OnUpdateTapisCombats?.Invoke(_client, element);
        }
    }
}
