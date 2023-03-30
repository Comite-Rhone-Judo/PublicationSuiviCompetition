using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementStructure
    {
        ClientJudo _client = null;

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
            if (OnListeStructures != null)
            {
                OnListeStructures(_client, element);
            }
        }

        public void ListePays(XElement element)
        {
            if (OnListePays != null)
            {
                OnListePays(_client, element);
            }
        }        

        public void ListeLigues(XElement element)
        {
            if (OnListeLigues != null)
            {
                OnListeLigues(_client, element);
            }
        }

        public void ListeClubs(XElement element)
        {
            if (OnListeClubs != null)
            {
                OnListeClubs(_client, element);
            }
        }

        public void ListeComites(XElement element)
        {
            if (OnListeComites != null)
            {
                OnListeComites(_client, element);
            }
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
            if (OnUpdateStructures != null)
            {
                OnUpdateStructures(_client, element);
            }
        }

        public void UpdatePays(XElement element)
        {
            if (OnUpdatePays != null)
            {
                OnUpdatePays(_client, element);
            }
        }

        public void UpdateLigues(XElement element)
        {
            if (OnUpdateLigues != null)
            {
                OnUpdateLigues(_client, element);
            }
        }

        public void UpdateSecteurs(XElement element)
        {
            if (OnUpdateSecteurs != null)
            {
                OnUpdateSecteurs(_client, element);
            }
        }

        public void UpdateClubs(XElement element)
        {
            if (OnUpdateClubs != null)
            {
                OnUpdateClubs(_client, element);
            }
        }

        public void UpdateComites(XElement element)
        {
            if (OnUpdateComites != null)
            {
                OnUpdateComites(_client, element);
            }
        }
    }
}
