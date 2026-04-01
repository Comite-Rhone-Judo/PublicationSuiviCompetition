
using FranceJudo.Core.XML;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Structures;
using FranceJudo.Metier.XML;
using KernelImpl.Internal;
using System.Collections.Generic;
using System.Xml.Linq;

namespace KernelImpl.Noyau.Structures
{
    /// <summary>
    /// Description des Comites
    /// </summary>
    public class Comite : IComite, IEntityWithKey<string>
    {
        string IEntityWithKey<string>.EntityKey => _idCache;
        private string _idCache;

        private string _id;
        public string id
        {
            get
            {
                if (int.TryParse(_id, out int com))
                {
                    return com.ToString("00");
                }
                else
                {
                    return _id;
                }
            }
            set
            {
                _id = value;
                GetIdCache();
            }
        }
        public string nom { get; set; }
        public string nomCourt { get; set; }

        private string _ligue;
        public string ligue { get { return _ligue; } set { _ligue = value; GetIdCache(); } }
        public string code { get; set; }
        public string secteur { get; set; }


        public void LoadXml(XElement xcomite)
        {
            this._id = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_ID));
            this._ligue = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_Ligue));
            GetIdCache();

            this.nom = XMLTools.LectureString(xcomite.Element(ConstantXML.Comite_Nom));
            this.nomCourt = this.id;
            this.code = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_RemoteID));
            this.secteur = XMLTools.LectureString(xcomite.Attribute(ConstantXML.Comite_Secteur));
        }

        public XElement ToXml(IJudoData DC = null)
        {
            XElement xcomite = new XElement(ConstantXML.Comite);

            if (int.TryParse(id, out int com))
            {
                xcomite.SetAttributeValue(ConstantXML.Comite_ID, com.ToString("00"));
            }
            else
            {
                xcomite.SetAttributeValue(ConstantXML.Comite_ID, id);
            }
            xcomite.SetAttributeValue(ConstantXML.Comite_Ligue, ligue.ToString());
            xcomite.Add(new XElement(ConstantXML.Comite_Nom, nom.ToString()));
            xcomite.Add(new XElement(ConstantXML.Comite_NomCourt, nomCourt.ToString()));

            xcomite.SetAttributeValue(ConstantXML.Comite_RemoteID, code.ToString());
            xcomite.SetAttributeValue(ConstantXML.Comite_Secteur, secteur.ToString());

            return xcomite;
        }

        /// <summary>
        /// Lecture des Comites
        /// </summary>
        /// <param name="xelement">élément décrivant les Comites</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Comites</returns>

        public static ICollection<Comite> LectureComites(XElement xelement)
        {
            ICollection<Comite> comites = new List<Comite>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Comite))
            {
                Comite comite = new Comite();
                comite.LoadXml(xinfo);
                comites.Add(comite);
            }
            return comites;
        }

        private void GetIdCache()
        {
            this._idCache = string.Format("{0}-{1}", id, ligue);
        }
    }
}
