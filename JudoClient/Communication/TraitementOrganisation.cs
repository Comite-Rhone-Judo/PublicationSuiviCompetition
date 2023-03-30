using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementOrganisation
    {
        ClientJudo _client = null;

        public TraitementOrganisation(ClientJudo client)
        {
            _client = client;
        }


        public delegate void OnListeOrganisationHandler(object sender, XElement xelements);
        public event OnListeOrganisationHandler OnListeOrganisation;

        public delegate void OnListeCompetitionsHandler(object sender, XElement xelements);
        public event OnListeCompetitionsHandler OnListeCompetitions;

        public delegate void OnListeEpreuvesHandler(object sender, XElement xelements);
        public event OnListeEpreuvesHandler OnListeEpreuves;

        public delegate void OnListeTapisHandler(object sender, XElement xelements);
        public event OnListeTapisHandler OnListeTapis;

        public void ListeOrganisation(XElement element)
        {
            if (OnListeOrganisation != null)
            {
                OnListeOrganisation(_client, element);
            }
        }

        public void ListeCompetitions(XElement element)
        {
            if (OnListeCompetitions != null)
            {
                OnListeCompetitions(_client, element);
            }
        }

        public void ListeEpreuves(XElement element)
        {
            if (OnListeEpreuves != null)
            {
                OnListeEpreuves(_client, element);
            }
        }

        public void ListeTapis(XElement element)
        {
            if (OnListeTapis != null)
            {
                OnListeTapis(_client, element);
            }
        }



        public delegate void OnUpdateOrganisationHandler(object sender, XElement xelements);
        public event OnUpdateOrganisationHandler OnUpdateOrganisation;

        public delegate void OnUpdateCompetitionsHandler(object sender, XElement xelements);
        public event OnUpdateCompetitionsHandler OnUpdateCompetitions;

        public delegate void OnUpdateEpreuvesHandler(object sender, XElement xelements);
        public event OnUpdateEpreuvesHandler OnUpdateEpreuves;

        public delegate void OnUpdateTapisHandler(object sender, XElement xelements);
        public event OnUpdateTapisHandler OnUpdateTapis;

        public void UpdateOrganisation(XElement element)
        {
            if (OnUpdateOrganisation != null)
            {
                OnUpdateOrganisation(_client, element);
            }
        }

        public void UpdateCompetitions(XElement element)
        {
            if (OnUpdateCompetitions != null)
            {
                OnUpdateCompetitions(_client, element);
            }
        }

        public void UpdateEpreuves(XElement element)
        {
            if (OnUpdateEpreuves != null)
            {
                OnUpdateEpreuves(_client, element);
            }
        }

        public void UpdateTapis(XElement element)
        {
            if (OnUpdateTapis != null)
            {
                OnUpdateTapis(_client, element);
            }
        }
    }
}
