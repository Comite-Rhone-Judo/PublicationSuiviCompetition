using System.Collections.Generic;
using System.Xml;
using Tools.Enum;
using Tools.Outils;

namespace AppPublication.Tools.LectureFile
{
    public static class LectureStructures
    {
        public static IList<Structure> GetStructures()
        {
            IList<Structure> result = new List<Structure>();

            XmlReader structureReader = XmlReader.Create(ResourcesTools.GetAssembyResource(ConstantResource.Structures));

            XmlDocument doc = new XmlDocument();
            doc.Load(structureReader);

            Structure item1 = new Structure();
            item1.nom = "FRANCE JUDO";
            item1.id = "FRANCE JUDO";
            item1.ordre = 1;
            item1.type = TypeStructureEnum.National;
            result.Add(item1);

            XmlNodeList xligues = doc.DocumentElement.SelectNodes("descendant::ligue");
            foreach (XmlNode xligue in xligues)
            {
                Structure item = new Structure();
                item.nom = "LIGUE " + xligue.Attributes[ConstantXML.Structure_Nom].Value;
                item.id = xligue.Attributes[ConstantXML.Structure_ID].Value;
                item.ordre = 2;
                item.type = TypeStructureEnum.Ligue;
                result.Add(item);
            }

            XmlNodeList xcomites = doc.DocumentElement.SelectNodes("descendant::comite");
            foreach (XmlNode xcomite in xcomites)
            {
                Structure item = new Structure();
                item.nom = "COMITE " + xcomite.Attributes[ConstantXML.Structure_Nom].Value;
                item.id = xcomite.Attributes[ConstantXML.Structure_ID].Value;
                item.ordre = 3;
                item.type = TypeStructureEnum.Comite;
                result.Add(item);
            }

            return result;
        }

    }

    public class Structure
    {
        public string nom { get; set; }
        public string id { get; set; }
        public int ordre { get; set; }
        public TypeStructureEnum type { get; set; }
    }

    public enum TypeStructureEnum
    {
        National = 1,
        Ligue = 2,
        Comite = 3,
        Club = 4,

    }
}
