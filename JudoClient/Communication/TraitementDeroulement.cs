using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public class TraitementDeroulement
    {
        ClientJudo _client = null;

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
            if (OnListePhases != null)
            {
                OnListePhases(_client, element);
            }
        }

        public void ListeCombats(XElement element)
        {
            if (OnListeCombats != null)
            {
                OnListeCombats(_client, element);
            }
        }

        public void CombatReceived(XElement element)
        {
            int combat = int.Parse(element.Element(ConstantXML.Combat).Value);
            if (OnCombatReceived != null)
            {
                OnCombatReceived(_client, combat);
            }
        }

        public void RencontreReceived(XElement element)
        {
            int rencontre = int.Parse(element.Element(ConstantXML.Rencontre).Value);
            if (OnRencontreReceived != null)
            {
                OnRencontreReceived(_client, rencontre);
            }
        }

        public void UpdateRencontreReceived(XElement element)
        {
            if (OnUpdateRencontreReceived != null)
            {
                OnUpdateRencontreReceived(_client, element);
            }
        }

        public delegate void OnUpdatePhasesHandler(object sender, XElement xelements);
        public event OnUpdatePhasesHandler OnUpdatePhases;

        public delegate void OnUpdateCombatsHandler(object sender, XElement xelements);
        public event OnUpdateCombatsHandler OnUpdateCombats;

        public delegate void OnUpdateTapisCombatsHandler(object sender, XElement xelements);
        public event OnUpdateTapisCombatsHandler OnUpdateTapisCombats;

        public void UpdatePhases(XElement element)
        {
            if (OnUpdatePhases != null)
            {
                OnUpdatePhases(_client, element);
            }
        }

        public void UpdateCombats(XElement element)
        {
            if (OnUpdateCombats != null)
            {
                OnUpdateCombats(_client, element);
            }
        }
        public void UpdateTapisCombats(XElement element)
        {
            if (OnUpdateTapisCombats != null)
            {
                OnUpdateTapisCombats(_client, element);
            }
        }
    }
}
