
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    /// <summary>
    /// Description des Feuilles (construction d'un tableau)
    /// </summary>
    public class Feuille
    {
        public int id { get; set; }
        public bool repechage { get; set; }
        public int source1 { get; set; }
        public int source2 { get; set; }
        public string reference { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public bool typeSource { get; set; }
        public int numero { get; set; }
        public int ordre { get; set; }
        public int pere { get; set; }
        public int classement1 { get; set; }
        public int classement2 { get; set; }
        public int niveau { get; set; }
        public Nullable<int> combat { get; set; }
        public int phase { get; set; }



        public Combat Combat1(JudoData DC)
        {
            return DC.Deroulement.Combats.FirstOrDefault(o => o.id == this.combat);
        }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_ID));
            this.repechage = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Feuille_Repechage));
            this.source1 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Source1));
            this.source2 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Source2));
            this.reference = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Feuille_Reference));
            this.ref1 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Feuille_Ref1));
            this.ref2 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Feuille_Ref2));
            this.typeSource = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Feuille_TypeSource));
            this.numero = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Numero));
            this.ordre = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Ordre));
            this.pere = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Pere));
            this.classement1 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Classement1));
            this.classement2 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Classement2));
            this.niveau = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Niveau));
            this.combat = XMLTools.LectureNullableInt(xinfo.Attribute(ConstantXML.Feuille_Combat));
            this.phase = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Feuille_Phase));
        }

        public XElement ToXml()
        {
            XElement xfeuille = new XElement(ConstantXML.Feuille);

            xfeuille.SetAttributeValue(ConstantXML.Feuille_ID, id);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Numero, numero);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Repechage, repechage);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Source1, source1);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Source2, source2);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Reference, reference);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Ref1, ref1);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Ref2, ref2);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Ordre, ordre);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Niveau, niveau);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Combat, combat);

            xfeuille.SetAttributeValue(ConstantXML.Feuille_TypeSource, typeSource);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Pere, pere);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Classement1, classement1);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Classement2, classement2);
            xfeuille.SetAttributeValue(ConstantXML.Feuille_Phase, phase);

            return xfeuille;
        }

        /// <summary>
        /// Lecture des Feuilles
        /// </summary>
        /// <param name="xelement">élément décrivant les Feuilles</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Feuilles</returns>

        public static ICollection<Feuille> LectureFeuilles(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Feuille> feuilles = new List<Feuille>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Feuille))
            {
                Feuille feuille = new Feuille();
                feuille.LoadXml(xinfo);
                feuilles.Add(feuille);
            }
            return feuilles;
        }
    }
}
