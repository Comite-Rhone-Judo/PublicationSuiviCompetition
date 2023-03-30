using System.Xml.Linq;
using Tools.Enum;

namespace JudoClient.Communication
{
    public class TraitementParticipants
    {
        ClientJudo _client = null;

        public TraitementParticipants(ClientJudo client)
        {
            _client = client;
        }

        public delegate void OnListeJudokasHandler(object sender, XElement xelements);
        public event OnListeJudokasHandler OnListeJudokas;

        public delegate void OnListeEquipesHandler(object sender, XElement xelements);
        public event OnListeEquipesHandler OnListeEquipes;

        public delegate void OnListeLicenciesHandler(object sender, XElement xelements);
        public event OnListeLicenciesHandler OnListeLicencies;

        public delegate void OnInscriptionReceivedHandler(object sender, int inscription);
        public event OnInscriptionReceivedHandler OnInscriptionReceived;

        public void ListeJudokas(XElement element)
        {
            if (OnListeJudokas != null)
            {
                OnListeJudokas(this, element);
            }
        }
        

        public void ListeEquipes(XElement element)
        {
            if (OnListeEquipes != null)
            {
                OnListeEquipes(_client, element);
            }
        }

        public void ListeLicencies(XElement element)
        {
            if (OnListeLicencies != null)
            {
                OnListeLicencies(this, element);
            }
        }

        private void InscriptionReceived(XElement element)
        {
            int judoka = int.Parse(element.Element(ConstantXML.Judoka).Value);
            if (OnInscriptionReceived != null)
            {
                OnInscriptionReceived(_client, judoka);
            }
        }


        public delegate void OnUpdateJudokasHandler(object sender, XElement xelements);
        public event OnUpdateJudokasHandler OnUpdateJudokas;

        public delegate void OnUpdateEquipesHandler(object sender, XElement xelements);
        public event OnUpdateEquipesHandler OnUpdateEquipes;

        public void UpdateJudokas(XElement element)
        {
            if (OnUpdateJudokas != null)
            {
                OnUpdateJudokas(this, element);
            }
        }

        public void UpdateEquipes(XElement element)
        {
            if (OnUpdateEquipes != null)
            {
                OnUpdateEquipes(_client, element);
            }
        }        
    }
}
