
using KernelImpl.Internal;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;
using Tools.XML;

namespace KernelImpl.Noyau.Categories
{
    /// <summary>
    /// Description des Categorie Poids
    /// </summary>
    public class CategoriePoids : IEntityWithKey<int>
    {
        int IEntityWithKey<int>.EntityKey => id;

        public int id { get; set; }
        public string nom { get; set; }
        public string remoteId { get; set; }
        public int poidsMin { get; set; }
        public int poidsMax { get; set; }
        public string ordre { get; set; }
        public int categorieAge { get; set; }
        private int _sexe;
        public int sexe
        {
            get
            {
                return _sexe;
            }
            set
            {
                _sexe = value;
                _sexeEnum = new EpreuveSexe(_sexe);
            }
        }

        private EpreuveSexe _sexeEnum;
        public EpreuveSexe sexeEnum
        {
            get
            {
                return _sexeEnum;
            }
            set
            {
                _sexeEnum = value;
                _sexe = (int)_sexeEnum;
            }
        }
        public bool equipe { get; set; }
        public string discipline { get; set; }


        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CatePoids_id));
            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.CatePoids_nom));
            this.poidsMin = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CatePoids_poidsMin));
            this.poidsMax = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CatePoids_poidsMax));
            this.ordre = XMLTools.LectureString(xinfo.Attribute(ConstantXML.CatePoids_ordre));
            this.categorieAge = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CatePoids_cateage));
            this.sexe = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.CatePoids_sexe));
            this.remoteId = XMLTools.LectureString(xinfo.Attribute(ConstantXML.CatePoids_remoteId));
            this.equipe = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.CatePoids_equipe));
            this.discipline = XMLTools.LectureString(xinfo.Attribute(ConstantXML.CatePoids_discipline));
        }

        public XElement ToXml()
        {
            XElement xcatepoids = new XElement(ConstantXML.CatePoids);
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_id, id.ToString());
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_nom, nom.ToString());
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_ordre, ordre.ToString());
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_remoteId, remoteId);
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_poidsMin, poidsMin);
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_poidsMax, poidsMax);
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_sexe, sexeEnum.ToString());
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_cateage, categorieAge);
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_equipe, equipe.ToString().ToLower());
            xcatepoids.SetAttributeValue(ConstantXML.CatePoids_discipline, discipline);
            return xcatepoids;
        }


        /// <summary>
        /// Lecture des Categories Poids
        /// </summary>
        /// <param name="xelement">élément décrivant les Categories Poids</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Categories Poids</returns>

        public static ICollection<CategoriePoids> LectureCategoriePoids(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<CategoriePoids> catepoids = new List<CategoriePoids>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.CatePoids))
            {
                CategoriePoids catepoid = new CategoriePoids();
                catepoid.LoadXml(xinfo);
                catepoids.Add(catepoid);
            }
            return catepoids;
        }
    }
}
