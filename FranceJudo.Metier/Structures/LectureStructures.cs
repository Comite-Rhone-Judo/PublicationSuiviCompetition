using System.Collections.Generic;
using System.Xml;
using FranceJudo.Core.Reflection;
using FranceJudo.Metier.Resources;
using FranceJudo.Metier.XML;

namespace FranceJudo.Metier.Structures
{
    public static class LectureStructures
    {
        public static IList<Structure> GetStructures()
        {
            IList<Structure> result = new List<Structure>();

            XmlReader structureReader = XmlReader.Create(AssemblyResourceHelper.GetAssembyResource(ResourceDictionnay.Referentiels_Structures));

            XmlDocument doc = new XmlDocument();
            doc.Load(structureReader);

            Structure item1 = new Structure
            {
                Nom = "FRANCE JUDO",
                Id = "FRANCE JUDO",
                Ordre = 1,
                Type = TypeStructureEnum.National
            };
            result.Add(item1);

            XmlNodeList xligues = doc.DocumentElement.SelectNodes("descendant::ligue");
            foreach (XmlNode xligue in xligues)
            {
                Structure item = new Structure
                {
                    Nom = "LIGUE " + xligue.Attributes[ConstantXML.Structure_Nom].Value,
                    Id = xligue.Attributes[ConstantXML.Structure_ID].Value,
                    Ordre = 2,
                    Type = TypeStructureEnum.Ligue
                };
                result.Add(item);
            }

            XmlNodeList xcomites = doc.DocumentElement.SelectNodes("descendant::comite");
            foreach (XmlNode xcomite in xcomites)
            {
                Structure item = new Structure
                {
                    Nom = "COMITE " + xcomite.Attributes[ConstantXML.Structure_Nom].Value,
                    Id = xcomite.Attributes[ConstantXML.Structure_ID].Value,
                    Ordre = 3,
                    Type = TypeStructureEnum.Comite
                };
                result.Add(item);
            }

            return result;
        }

    }

    public class Structure
    {
        public string Nom { get; set; }
        public string Id { get; set; }
        public int Ordre { get; set; }
        public TypeStructureEnum Type { get; set; }
    }

    public enum TypeStructureEnum
    {
        National = 1,
        Ligue = 2,
        Comite = 3,
        Club = 4,

    }
}
