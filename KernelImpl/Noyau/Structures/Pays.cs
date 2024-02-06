
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Structures
{
    public class Pays
    {
        public int id { get; set; }
        public int code { get; set; }
        public string abr2 { get; set; }
        public string abr3 { get; set; }
        public string nom { get; set; }
        public string AbrF { get; set; }

        public BitmapImage GetFlag()
        {
            string uri_flag = ConstantFile.MediaFlags_dir + @"" + this.abr3 + ".svg_800.png";
            if (File.Exists(uri_flag))
            {
                return new BitmapImage(new Uri(uri_flag));
            }

            return null;

        }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Pays_ID));
            this.nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Pays_Nom));
            this.code = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Pays_Code));
            this.abr2 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Pays_Abr2));
            this.abr3 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Pays_Abr3));
            this.AbrF = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Pays_AbrF));
        }

        public System.Xml.Linq.XElement ToXml()
        {

            XElement xpays = new System.Xml.Linq.XElement(ConstantXML.Pays);
            xpays.SetAttributeValue(ConstantXML.Pays_ID, id);
            xpays.SetAttributeValue(ConstantXML.Pays_Nom, nom.ToUpper());
            xpays.SetAttributeValue(ConstantXML.Pays_Code, code);
            xpays.SetAttributeValue(ConstantXML.Pays_Abr2, abr2);
            xpays.SetAttributeValue(ConstantXML.Pays_Abr3, abr3);
            xpays.SetAttributeValue(ConstantXML.Pays_AbrF, AbrF);
            return xpays;

        }

        /// <summary>
        /// Lecture des Pays
        /// </summary>
        /// <param name="xelement">élément décrivant les Pays</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Ligues</returns>

        public static ICollection<Pays> LecturePays(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Pays> _pays = new List<Pays>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Pays))
            {
                Pays pays = new Pays();
                pays.LoadXml(xinfo);
                _pays.Add(pays);
            }
            return _pays;
        }
    }
}
