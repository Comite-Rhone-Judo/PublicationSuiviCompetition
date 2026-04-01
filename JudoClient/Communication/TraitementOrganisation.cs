using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementOrganisation
    {
        readonly ClientJudo _client = null;

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
            OnListeOrganisation?.Invoke(_client, element);
        }

        public void ListeCompetitions(XElement element)
        {
            OnListeCompetitions?.Invoke(_client, element);
        }

        public void ListeEpreuves(XElement element)
        {
            OnListeEpreuves?.Invoke(_client, element);
        }

        public void ListeTapis(XElement element)
        {
            OnListeTapis?.Invoke(_client, element);
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
            OnUpdateOrganisation?.Invoke(_client, element);
        }

        public void UpdateCompetitions(XElement element)
        {
            OnUpdateCompetitions?.Invoke(_client, element);
        }

        public void UpdateEpreuves(XElement element)
        {
            OnUpdateEpreuves?.Invoke(_client, element);
        }

        public void UpdateTapis(XElement element)
        {
            OnUpdateTapis?.Invoke(_client, element);
        }
    }
}
