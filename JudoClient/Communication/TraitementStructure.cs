
using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementStructure
    {
        readonly ClientJudo _client = null;

        public TraitementStructure(ClientJudo client)
        {
            _client = client;
        }


        public delegate void OnListeStructuresHandler(object sender, XElement xelements);
        public event OnListeStructuresHandler OnListeStructures;

        public delegate void OnListePaysHandler(object sender, XElement xelements);
        public event OnListePaysHandler OnListePays;

        public delegate void OnListeLiguesHandler(object sender, XElement xelements);
        public event OnListeLiguesHandler OnListeLigues;

        public delegate void OnListeClubsHandler(object sender, XElement xelements);
        public event OnListeClubsHandler OnListeClubs;

        public delegate void OnListeComitesHandler(object sender, XElement xelements);
        public event OnListeComitesHandler OnListeComites;

        public void ListeStructures(XElement element)
        {
            OnListeStructures?.Invoke(_client, element);
        }

        public void ListePays(XElement element)
        {
            OnListePays?.Invoke(_client, element);
        }

        public void ListeLigues(XElement element)
        {
            OnListeLigues?.Invoke(_client, element);
        }

        public void ListeClubs(XElement element)
        {
            OnListeClubs?.Invoke(_client, element);
        }

        public void ListeComites(XElement element)
        {
            OnListeComites?.Invoke(_client, element);
        }



        public delegate void OnUpdateStructuresHandler(object sender, XElement xelements);
        public event OnUpdateStructuresHandler OnUpdateStructures;

        public delegate void OnUpdatePaysHandler(object sender, XElement xelements);
        public event OnUpdatePaysHandler OnUpdatePays;

        public delegate void OnUpdateLiguesHandler(object sender, XElement xelements);
        public event OnUpdateLiguesHandler OnUpdateLigues;

        public delegate void OnUpdateClubsHandler(object sender, XElement xelements);
        public event OnUpdateClubsHandler OnUpdateClubs;

        public delegate void OnUpdateComitesHandler(object sender, XElement xelements);
        public event OnUpdateComitesHandler OnUpdateComites;

        public delegate void OnUpdateSecteursHandler(object sender, XElement xelements);
        public event OnUpdateSecteursHandler OnUpdateSecteurs;

        public void UpdateStructures(XElement element)
        {
            OnUpdateStructures?.Invoke(_client, element);
        }

        public void UpdatePays(XElement element)
        {
            OnUpdatePays?.Invoke(_client, element);
        }

        public void UpdateLigues(XElement element)
        {
            OnUpdateLigues?.Invoke(_client, element);
        }

        public void UpdateSecteurs(XElement element)
        {
            OnUpdateSecteurs?.Invoke(_client, element);
        }

        public void UpdateClubs(XElement element)
        {
            OnUpdateClubs?.Invoke(_client, element);
        }

        public void UpdateComites(XElement element)
        {
            OnUpdateComites?.Invoke(_client, element);
        }
    }
}
