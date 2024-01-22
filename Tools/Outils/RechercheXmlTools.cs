using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Tools.Enum;

namespace Tools.Outils
{
    public static class RechercheXmlTools
    {
        public class Personne
        {
            public string licence { get; set; }
            public string nom { get; set; }
            public string prenom { get; set; }
            public string remoteId { get; set; }
            public bool sexe { get; set; }
            public DateTime naissance { get; set; }
            public int categorie { get; set; }
            public string club { get; set; }
            public string grade { get; set; }
        }

        public static string FormatPrenom(string chaine)
        {
            string result = "";
            bool maj = true;
            foreach (char ch in chaine.ToList())
            {
                result += (maj ? Char.ToUpper(ch) : Char.ToLower(ch));
                if (ch == '-' || ch == ' ')
                {
                    maj = true;
                }
                else
                {
                    maj = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Lecture d'une personne
        /// </summary>
        /// <param name="xjudoka"></param>
        /// <returns></returns>

        public static Personne LecturePersonne(XmlNode xjudoka)
        {
            DateTime naissance = XMLTools.LectureDate(xjudoka.Attributes[ConstantXML.Judoka_Naissance].Value, "dd/MM/yyyy");

            Personne personne = new Personne();

            personne.licence = xjudoka.Attributes[ConstantXML.Judoka_Licence].Value;
            personne.nom = xjudoka.Attributes[ConstantXML.Judoka_Nom].Value.ToUpper();
            personne.prenom = RechercheXmlTools.FormatPrenom(xjudoka.Attributes[ConstantXML.Judoka_Prenom].Value);
            personne.remoteId = xjudoka.Attributes[ConstantXML.Judoka_ID].Value;
            personne.sexe = xjudoka.Attributes[ConstantXML.Judoka_Sexe].Value == "F";
            personne.naissance = naissance;
            personne.categorie = 0;// Outils.GetCategorie(naissance.Year);
            personne.grade = xjudoka.Attributes[ConstantXML.Judoka_Grade].Value;
            personne.club = xjudoka.Attributes[ConstantXML.Judoka_Club].Value;

            return personne;
        }

        //public static DateTime LectureDate(string ladate, string format)
        //{
        //    try
        //    {
        //        return DateTime.ParseExact(ladate, format, null);
        //    }
        //    catch
        //    {
        //        return DateTime.Now;
        //    }
        //}

        /// <summary>
        /// Traite chaine XPATH
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>

        public static string traiteChaine(string chaine)
        {
            string result = chaine;

            string chaine1 = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
            string chaine2 = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

            int length = chaine1.ToCharArray().Length;

            for (int i = 0; i < length; i++)
            {
                result = result.Replace(chaine1.ToCharArray().ElementAt(i), chaine2.ToCharArray().ElementAt(i));
            }

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            result = rgx.Replace(result, " ");

            return result;
        }

        /// <summary>
        /// Construit un requete XPATH
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>

        public static string getRequeteXpath(string text)
        {
            List<string> listWord = new List<string>();

            string translate1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
            string translate2 = "abcdefghijklmnopqrstuvwxyzaaaaaaaaaaaaooooooooooooeeeeeeeeiiiiiiiiuuuuuuuuynncc";

            string xpath = "(contains(translate(@nom, '{1}', '{2}'),'{0}') or contains(translate(@prenom, '{1}', '{2}'),'{0}') or contains(translate(@licence, '{1}', '{2}'),'{0}'))";

            string chaine = traiteChaine(text);

            foreach (string word in chaine.ToLower().Split(' '))
            {
                listWord.Add(String.Format(xpath, word, translate1, translate2));
            }

            xpath = String.Join(" and ", listWord);

            return xpath;
        }

        /// <summary>
        /// Recherche un text dans un document XML
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requete"></param>
        /// <returns></returns>

        public static XmlNodeList RechercheXml(XmlDocument document, string requete)
        {
            XmlNode root = document.DocumentElement;
            return root.SelectNodes(String.Format("descendant::judoka[{0}]", requete));
        }


        /// <summary>
        /// Compte le nombre de judokas dans un fichier XML
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static int CountJudokaXml(XmlDocument document)
        {
            try
            {
                if (document == null)
                {
                    return 0;
                }

                XmlNode root = document.DocumentElement;
                return root.SelectNodes("descendant::judoka").Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}
