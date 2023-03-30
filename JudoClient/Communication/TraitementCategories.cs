using System.Xml.Linq;

namespace JudoClient.Communication
{
    public class TraitementCategories
    {
        ClientJudo _client = null;

        public TraitementCategories(ClientJudo client)
        {
            _client = client;
        }

        public delegate void OnListeCategoriesHandler(object sender, XElement xelements);
        public event OnListeCategoriesHandler OnListeCategories;

        public delegate void OnListeCateAgeHandler(object sender, XElement xelements);
        public event OnListeCateAgeHandler OnListeCateAge;

        public delegate void OnListeCatePoidsHandler(object sender, XElement xelements);
        public event OnListeCatePoidsHandler OnListeCatePoids;

        public delegate void OnListeCeinturesHandler(object sender, XElement xelements);
        public event OnListeCeinturesHandler OnListeCeintures;

        public void ListeCategories(XElement element)
        {
            if (OnListeCategories != null)
            {
                OnListeCategories(_client, element);
            }
        }

        public void ListeCateAge(XElement element)
        {
            if (OnListeCateAge != null)
            {
                OnListeCateAge(_client, element);
            }
        }

        public void ListeCatePoids(XElement element)
        {
            if (OnListeCatePoids != null)
            {
                OnListeCatePoids(_client, element);
            }
        }

        public void ListeCeintures(XElement element)
        {
            if (OnListeCeintures != null)
            {
                OnListeCeintures(_client, element);
            }
        }


        public delegate void OnUpdateCategoriesHandler(object sender, XElement xelements);
        public event OnUpdateCategoriesHandler OnUpdateCategories;

        public delegate void OnUpdateCateAgeHandler(object sender, XElement xelements);
        public event OnUpdateCateAgeHandler OnUpdateCateAge;

        public delegate void OnUpdateCatePoidsHandler(object sender, XElement xelements);
        public event OnUpdateCatePoidsHandler OnUpdateCatePoids;

        public delegate void OnUpdateCeinturesHandler(object sender, XElement xelements);
        public event OnUpdateCeinturesHandler OnUpdateCeintures;

        public void UpdateCategories(XElement element)
        {
            if (OnUpdateCategories != null)
            {
                OnUpdateCategories(_client, element);
            }
        }

        public void UpdateCateAge(XElement element)
        {
            if (OnUpdateCateAge != null)
            {
                OnUpdateCateAge(_client, element);
            }
        }

        public void UpdateCatePoids(XElement element)
        {
            if (OnUpdateCatePoids != null)
            {
                OnUpdateCatePoids(_client, element);
            }
        }

        public void UpdateCeintures(XElement element)
        {
            if (OnUpdateCeintures != null)
            {
                OnUpdateCeintures(_client, element);
            }
        }
    }
}
