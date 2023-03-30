using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Noyau;

namespace Tools.Outils
{
    /// <summary>
    /// Outils de lecture des fichiers XML
    /// </summary>

    public static class LectureXMLTools
    {
        public delegate void MontreInformations(int index, int maximum, string info1, string info2);

        public static int GetCategorie(int annee, ICollection<CategorieAge> cateages)
        {
            try
            {
                return cateages.First(o => o.anneeMin <= annee && annee <= o.anneeMax).id;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Lecture des compétition
        /// </summary>
        /// <param name="xelement">élément décrivant les compétitions</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les compétition</returns>

        public static ICollection<Competition> LectureCompetitions(XElement xelement, MontreInformations MI)
        {
            ICollection<Competition> competitions = new List<Competition>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Competition))
            {
                Competition compet = new Competition();
                compet.LoadXml(xinfo);
                competitions.Add(compet);
            }
            return competitions;
        }

        /// <summary>
        /// Lecture des Ceintures
        /// </summary>
        /// <param name="xelement">élément décrivant les Ceintures</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Ceintures</returns>

        public static ICollection<Ceintures> LectureCeintures(XElement xelement, MontreInformations MI)
        {
            ICollection<Ceintures> ceintures = new List<Ceintures>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Ceinture))
            {
                Ceintures compet = new Ceintures();
                compet.LoadXml(xinfo);
                ceintures.Add(compet);
            }
            return ceintures;
        }

        /// <summary>
        /// Lecture des Epreuves
        /// </summary>
        /// <param name="xelement">élément décrivant les Epreuves</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Epreuves</returns>
        
        public static ICollection<Epreuve_Equipe> LectureEpreuveEquipes(XElement xelement, MontreInformations MI)
        {
            ICollection<Epreuve_Equipe> epreuves = new List<Epreuve_Equipe>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Epreuve_Equipe))
            {
                Epreuve_Equipe epreuve = new Epreuve_Equipe();
                epreuve.LoadXml(xinfo);
                epreuves.Add(epreuve);
            }
            return epreuves;
        }

        /// <summary>
        /// Lecture des Epreuves
        /// </summary>
        /// <param name="xelement">élément décrivant les Epreuves</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Epreuves</returns>

        public static ICollection<Epreuve> LectureEpreuves(XElement xelement, MontreInformations MI)
        {
            ICollection<Epreuve> epreuves = new List<Epreuve>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Epreuve))
            {
                Epreuve epreuve = new Epreuve();
                epreuve.LoadXml(xinfo);
                epreuves.Add(epreuve);
            }
            return epreuves;
        }

        /// <summary>
        /// Lecture des Phases
        /// </summary>
        /// <param name="xelement">élément décrivant les Phases</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Phases</returns>

        public static ICollection<Phase> LecturePhases(XElement xelement, MontreInformations MI)
        {
            ICollection<Phase> phases = new List<Phase>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Phase))
            {
                Phase phase = new Phase();
                phase.LoadXml(xinfo);
                phases.Add(phase);
            }
            return phases;
        }

        /// <summary>
        /// Lecture des Epreuve des Judoka
        /// </summary>
        /// <param name="xelement">élément décrivant les Epreuve des Judoka</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Epreuve des Judoka</returns>

        public static ICollection<EpreuveJudoka> LectureEpreuveJudokas(XElement xelement, MontreInformations MI)
        {
            ICollection<EpreuveJudoka> ejs = new List<EpreuveJudoka>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.EpreuveJudoka))
            {
                EpreuveJudoka ej = new EpreuveJudoka();
                ej.LoadXml(xinfo);
                ejs.Add(ej);
            }
            return ejs;
        }

        /// <summary>
        /// Lecture des Categories Age
        /// </summary>
        /// <param name="xelement">élément décrivant les Categories Age</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Categories Age</returns>

        public static ICollection<CategorieAge> LectureCategorieAge(XElement xelement, MontreInformations MI)
        {
            ICollection<CategorieAge> cateages = new List<CategorieAge>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.CateAge))
            {
                CategorieAge cateage = new CategorieAge();
                cateage.LoadXml(xinfo);
                cateages.Add(cateage);
            }
            return cateages;
        }

        /// <summary>
        /// Lecture des Categories Poids
        /// </summary>
        /// <param name="xelement">élément décrivant les Categories Poids</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Categories Poids</returns>

        public static ICollection<CategoriePoids> LectureCategoriePoids(XElement xelement, MontreInformations MI)
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

        /// <summary>
        /// Lecture des Participants
        /// </summary>
        /// <param name="xelement">élément décrivant les Participants</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Participants</returns>

        public static ICollection<Participant> LectureParticipant(XElement xelement, MontreInformations MI)
        {
            ICollection<Participant> participants = new List<Participant>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Participant))
            {
                Participant participant = new Participant();
                participant.LoadXml(xinfo);
                participants.Add(participant);
            }
            return participants;
        }

        /// <summary>
        /// Lecture des Judokas
        /// </summary>
        /// <param name="xelement">élément décrivant les Judokas</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Judokas</returns>

        public static ICollection<Judoka> LectureJudoka(XElement xelement, MontreInformations MI)
        {
            ICollection<Judoka> judokas = new List<Judoka>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Judoka))
            {
                Judoka judoka = new Judoka();
                judoka.LoadXml(xinfo);
                judokas.Add(judoka);
            }
            return judokas;
        }

        /// <summary>
        /// Lecture des Combats
        /// </summary>
        /// <param name="xelement">élément décrivant les Combats</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Combats</returns>

        public static ICollection<Combat> LectureCombats(XElement xelement, int tapis, MontreInformations MI)
        {
            ICollection<Combat> combats = new List<Combat>();
            foreach (XElement xcombat in xelement.Descendants(ConstantXML.Combat))
            {
                Combat combat = new Combat();
                combat.LoadXml(xcombat);
                if (combat.tapis == tapis)
                {
                    combats.Add(combat);
                }

            }

            return combats;
        }

        /// <summary>
        /// Lecture des Rencontres
        /// </summary>
        /// <param name="xelement">élément décrivant les Rencontres</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Rencontres</returns>

        public static ICollection<Rencontre> LectureRencontres(XElement xelement, int tapis, MontreInformations MI)
        {
            ICollection<Rencontre> rencontres = new List<Rencontre>();

            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Tapis))
            {
                if (int.Parse(xinfo.Attribute(ConstantXML.Tapis).Value) != tapis)
                {
                    continue;
                }
                int maximum = xelement.Descendants(ConstantXML.Rencontre).Count();
                int index = 0;

                Parallel.ForEach(xelement.Descendants(ConstantXML.Rencontre), xrencontre =>
                {
                    if (MI != null)
                    {
                        lock ((rencontres as ICollection).SyncRoot)
                        {
                            Interlocked.Add(ref index, 1);
                        }
                        MI(index, maximum, "Importation des judokas", "");
                    }

                    Rencontre rencontre = new Rencontre();
                    rencontre.LoadXml(xrencontre);
                    lock ((rencontres as ICollection).SyncRoot)
                    {
                        rencontres.Add(rencontre);
                    }
                });
            }


            //foreach (XElement xinfo in xelement.Descendants(ConstantXML.Tapis))
            //{
            //    if (int.Parse(xinfo.Attribute(ConstantXML.Tapis).Value) != tapis)
            //    {
            //        continue;
            //    }

            //    foreach (XElement xrencontre in xelement.Descendants(ConstantXML.Rencontre))
            //    {
            //        Rencontre rencontre = new Rencontre();
            //        rencontre.LoadXml(xrencontre);
            //        rencontres.Add(rencontre);
            //    }
            //}
            return rencontres;
        }

        /// <summary>
        /// Lecture des Equipes
        /// </summary>
        /// <param name="xelement">élément décrivant les Equipes</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Equipes</returns>

        public static ICollection<Equipe> LectureEquipes(XElement xelement, MontreInformations MI)
        {
            ICollection<Equipe> equipes = new List<Equipe>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Equipe))
            {
                Equipe equipe = new Equipe();
                equipe.LoadXml(xinfo);
                equipes.Add(equipe);
            }
            return equipes;
        }

        /// <summary>
        /// Lecture des Feuilles
        /// </summary>
        /// <param name="xelement">élément décrivant les Feuilles</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Feuilles</returns>

        public static ICollection<Feuille> LectureFeuilles(XElement xelement, MontreInformations MI)
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

        /// <summary>
        /// Lecture des Clubs
        /// </summary>
        /// <param name="xelement">élément décrivant les Clubs</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Clubs</returns>

        public static ICollection<Club> LectureClubs(XElement xelement, MontreInformations MI)
        {
            ICollection<Club> clubs = new List<Club>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Club))
            {
                Club club = new Club();
                club.LoadXml(xinfo);
                clubs.Add(club);
            }
            return clubs;
        }

        /// <summary>
        /// Lecture des Comites
        /// </summary>
        /// <param name="xelement">élément décrivant les Comites</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Comites</returns>

        public static ICollection<Comite> LectureComites(XElement xelement, MontreInformations MI)
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

        /// <summary>
        /// Lecture des Ligues
        /// </summary>
        /// <param name="xelement">élément décrivant les Ligues</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Ligues</returns>

        public static ICollection<Ligue> LectureLigues(XElement xelement, MontreInformations MI)
        {
            ICollection<Ligue> ligues = new List<Ligue>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Ligue))
            {
                Ligue ligue = new Ligue();
                ligue.LoadXml(xinfo);
                ligues.Add(ligue);
            }
            return ligues;
        }

        /// <summary>
        /// Lecture des Ligues
        /// </summary>
        /// <param name="xelement">élément décrivant les Ligues</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Ligues</returns>

        public static ICollection<string> LectureLogosCommissaire(XElement xelement, MontreInformations MI)
        {
            ICollection<string> urls = new List<string>();

            try
            {
                OutilsTools.DeleteDirectory(ConstantFile.LogoCom_dir);

                Directory.CreateDirectory(ConstantFile.LogoCom_dir);
            }
            catch(Exception ex)
            {
                LogTools.Trace(ex, LogTools.Level.ERROR);
            }
            finally
            {
                foreach (XElement xinfo in xelement.Descendants(ConstantXML.Logo))
                {
                    string val = xinfo.Element(ConstantXML.Logo_Valeur) != null ? xinfo.Element(ConstantXML.Logo_Valeur).Value : "";
                    string nom = xinfo.Element(ConstantXML.Logo_Nom) != null ? xinfo.Element(ConstantXML.Logo_Nom).Value : "";
                    if (!String.IsNullOrWhiteSpace(val))
                    {
                        Image img = OutilsTools.StringToImage(val);

                        int index = 0;
                        while (File.Exists(ConstantFile.LogoCom_dir + nom))
                        {
                            string filename = Path.GetFileNameWithoutExtension(ConstantFile.LogoCom_dir + nom);
                            string extension = Path.GetExtension(ConstantFile.LogoCom_dir + nom);

                            nom = filename + "_" + ++index + extension;
                        }

                        img.Save(ConstantFile.LogoCom_dir + nom);
                        urls.Add(ConstantFile.LogoCom_dir + nom);
                    }                }
            }
            
            return urls;
        }

       /// <summary>
       /// Lecture d'une date
       /// </summary>
       /// <param name="ladate">attribut date</param>
       /// <param name="format">format de la date</param>
       /// <param name="default_value">date par défaut si mauvaise lecture</param>
       /// <returns>la date</returns>

        public static DateTime LectureDate(XAttribute ladate, string format, DateTime default_value)
        {
            try
            {
                return DateTime.ParseExact(ladate.Value, format, null);
            }
            catch
            {
                return default_value;
            }
        }

        /// <summary>
        /// Lecture d'une date
        /// </summary>
        /// <param name="ladate">attribut date</param>
        /// <param name="format">format de la date</param>
        /// <returns>la date</returns>

        public static DateTime LectureDate(XmlAttribute ladate, string format)
        {
            try
            {
                return DateTime.ParseExact(ladate.Value, format, null);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Lecture d'une date
        /// </summary>
        /// <param name="ladate">date</param>
        /// <param name="format">format de la date</param>
        /// <returns>la date</returns>

        public static DateTime LectureDate(string ladate, string format)
        {
            try
            {
                return DateTime.ParseExact(ladate, format, null);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Lecture d'un time
        /// </summary>
        /// <param name="ladate">date</param>
        /// <param name="format">format de la date</param>
        /// <returns>time</returns>

        public static TimeSpan LectureTime(XAttribute ladate, string format)
        {
            try
            {
                return DateTime.ParseExact(ladate.Value, format, null).TimeOfDay;
            }
            catch
            {
                return DateTime.Now.TimeOfDay;
            }
        }

        /// <summary>
        /// Lecture d'un poids
        /// </summary>
        /// <param name="lepoids"></param>
        /// <returns>poids</returns>

        public static int LecturePoids(string lepoids)
        {
            int poid = 0;
            try
            {
                if (!String.IsNullOrWhiteSpace(lepoids))
                {
                    int.TryParse(lepoids, out poid);
                }
            }
            catch
            {
            }
            return poid;
        }

        /// <summary>
        /// Lecture d'un Int32
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static int LectureInt(XAttribute xattribute)
        {
            int result = 549;
            try
            {
                if (!String.IsNullOrWhiteSpace(xattribute.Value))
                {
                    int.TryParse(xattribute.Value, out result);
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Lecture d'un Int32
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static int LectureInt(XmlAttribute xattribute)
        {
            int result = 0;
            try
            {
                if (!String.IsNullOrWhiteSpace(xattribute.Value))
                {
                    int.TryParse(xattribute.Value, out result);
                }
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Lecture d'un Nullable<Int32>
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static int? LectureNullableInt(XAttribute xattribute)
        {
            int result = 0;
            try
            {
                if (!String.IsNullOrWhiteSpace(xattribute.Value))
                {
                    if (int.TryParse(xattribute.Value, out result))
                    {
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Lecture d'un String
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static string LectureString(XAttribute xattribute)
        {
            try
            {
                return xattribute.Value;               
            }
            catch
            {
                return "";
            }
        }

        public static string LectureString(XmlAttribute xattribute)
        {
            try
            {
                return xattribute.Value;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// XDocument -> XmlDocument
        /// </summary>
        /// <param name="xDocument">XDocument</param>
        /// <returns>XmlDocument</returns>

        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        /// <summary>
        /// XmlDocument -> XDocument
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>XDocument</returns>

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
