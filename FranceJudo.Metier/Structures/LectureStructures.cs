using FranceJudo.Metier.Resources;
using FranceJudo.Metier.XML;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FranceJudo.Metier.Structures
{
    public static class LectureStructures
    {
        public static IList<Structure> GetStructures()
        {
            // 1. Utilisation de l'initialiseur de collection pour plus de lisibilité
            IList<Structure> result = new List<Structure>
    {
        new Structure
        {
            Nom = "FRANCE JUDO",
            Id = "FRANCE JUDO",
            Ordre = 1,
            Type = TypeStructureEnum.National
        }
    };

            // 2. On récupère le flux via notre méthode raccourcie, et on le place dans un bloc 'using' !
            // Cela garantit que la mémoire est libérée instantanément à la fin de la lecture.
            using (Stream stream = MetierResources.GetStructuresXml())
            {
                if (stream == null) return result; // Sécurité si le fichier est introuvable

                XmlDocument doc = new XmlDocument();

                // 3. Pas besoin de XmlReader : XmlDocument.Load() sait lire un Stream nativement !
                doc.Load(stream);

                XmlNodeList xligues = doc.DocumentElement.SelectNodes("descendant::ligue");
                foreach (XmlNode xligue in xligues)
                {
                    result.Add(new Structure
                    {
                        // Ajout de '?.' par sécurité au cas où un nœud XML n'aurait pas l'attribut
                        Nom = "LIGUE " + xligue.Attributes[ConstantXML.Structure_Nom]?.Value,
                        Id = xligue.Attributes[ConstantXML.Structure_ID]?.Value,
                        Ordre = 2,
                        Type = TypeStructureEnum.Ligue
                    });
                }

                XmlNodeList xcomites = doc.DocumentElement.SelectNodes("descendant::comite");
                foreach (XmlNode xcomite in xcomites)
                {
                    result.Add(new Structure
                    {
                        Nom = "COMITE " + xcomite.Attributes[ConstantXML.Structure_Nom]?.Value,
                        Id = xcomite.Attributes[ConstantXML.Structure_ID]?.Value,
                        Ordre = 3,
                        Type = TypeStructureEnum.Comite
                    });
                }
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
