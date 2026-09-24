using FranceJudo.Metier.XML;
using System.Xml.Linq;



namespace JudoClient.Communication
{
    public class TraitementParticipants
    {
        readonly ClientJudo _client = null;

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
            OnListeJudokas?.Invoke(_client, element);
        }


        public void ListeEquipes(XElement element)
        {
            OnListeEquipes?.Invoke(_client, element);
        }

        public void ListeLicencies(XElement element)
        {
            OnListeLicencies?.Invoke(_client, element);
        }

        private void InscriptionReceived(XElement element)
        {
            int judoka = int.Parse(element.Element(ConstantXML.Judoka).Value);
            OnInscriptionReceived?.Invoke(_client, judoka);
        }


        public delegate void OnUpdateJudokasHandler(object sender, XElement xelements);
        public event OnUpdateJudokasHandler OnUpdateJudokas;

        public delegate void OnUpdateEquipesHandler(object sender, XElement xelements);
        public event OnUpdateEquipesHandler OnUpdateEquipes;

        public void UpdateJudokas(XElement element)
        {
            OnUpdateJudokas?.Invoke(_client, element);
        }

        public void UpdateEquipes(XElement element)
        {
            OnUpdateEquipes?.Invoke(_client, element);
        }
    }
}
