

using KernelImpl.Noyau.Arbitrage;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Organisation
{
    /// <summary>
    /// Description des Competitions
    /// </summary>
    public class Competition
    {
        public Competition()
        {
            this.id = 0;
            this.nom = "";
            this.date = DateTime.Now;
            this.lieu = "";
            this.siteInternet = "";
            this.remoteId = "";
            this.codeAcces = "";
            this.type = 2;
            this.type2 = 2;
            this.discipline = CompetitionDisciplineEnum.Judo.ToString();
            this.nbTapis = 6;
            this.tempsCombat = 600;
            this.niveau = 0;
            this.couleur1 = "";
            this.couleur2 = "";
            this.version = "";
            this.afficheCSA = (int)TypeCSAEnum.Aucun;
            this.afficheKinzas = false;
            this.afficheAnimationVainqueur = false;
            this.tempsMedical = 120;
            this.isRandomCombat = false;
            this.couleur1 = ConstantCouleur.Rouge.ToString();
            this.couleur2 = ConstantCouleur.Blanc.ToString();
        }


        public int id { get; set; }
        public string nom { get; set; }
        public DateTime date { get; set; }
        public string lieu { get; set; }
        public string siteInternet { get; set; }

        public string remoteId { get; set; }
        public string codeAcces { get; set; }
        public int type { get; set; }
        public int type2 { get; set; }
        public string discipline { get; set; }
        public int nbTapis { get; set; }
        public int tempsCombat { get; set; }
        public int niveau { get; set; }
        public string couleur1 { get; set; }
        public string couleur2 { get; set; }
        public string version { get; set; }

        public int afficheCSA { get; set; }

        public bool afficheKinzas { get; set; }

        public bool afficheAnimationVainqueur { get; set; }

        public int tempsMedical { get; set; }
         public bool isRandomCombat { get; set; }


        public void LoadXml(XElement xinfo)
        {
            this.nom = XMLTools.LectureString(xinfo.Element(ConstantXML.Competition_Titre));
            this.lieu = XMLTools.LectureString(xinfo.Element(ConstantXML.Competition_Lieu));

            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_ID));

            this.date = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Competition_Date), "ddMMyyyy", DateTime.Now);
            this.remoteId = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_RemoteID));
            this.type = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Type));
            this.type2 = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Type2));

            this.discipline = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Discipline));

            this.nbTapis = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Tapis));

            this.niveau = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Niveau));
            this.version = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Version));
            this.couleur1 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Couleur1));
            this.couleur2 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Couleur2));
            this.afficheCSA = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_AfficheCSA));

            this.afficheKinzas = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_AfficheKinzas)) == "Oui" ? true : false;
            this.afficheAnimationVainqueur = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_AfficheAnimationVainqueur)) == "Oui" ? true : false;

            this.tempsMedical = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_TempsMedical));
            this.isRandomCombat = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_RandomCombat)) == "Oui" ? true : false;
        }

        public XElement ToXmlInformations()
        {
            XElement xcompetition = new XElement(ConstantXML.Competition);
            xcompetition.SetAttributeValue(ConstantXML.Competition_ID, id.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_RemoteID, remoteId.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Date, date.ToString("ddMMyyyy"));
            xcompetition.SetAttributeValue(ConstantXML.Competition_Type, type.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Type2, type2.ToString());

            xcompetition.SetAttributeValue(ConstantXML.Competition_Discipline, discipline.ToString());

            xcompetition.SetAttributeValue(ConstantXML.Competition_Discipline, discipline);

            xcompetition.SetAttributeValue(ConstantXML.Competition_Niveau, niveau.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Version, version.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Couleur1, couleur1.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Couleur2, couleur2.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_AfficheCSA, afficheCSA.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_AfficheKinzas, afficheKinzas ? "Oui" : "Non");
            xcompetition.SetAttributeValue(ConstantXML.Competition_AfficheAnimationVainqueur, afficheAnimationVainqueur ? "Oui" : "Non");

            xcompetition.SetAttributeValue(ConstantXML.Competition_RandomCombat, isRandomCombat ? "Oui" : "Non");
            xcompetition.SetAttributeValue(ConstantXML.Competition_TempsMedical, tempsMedical.ToString());

            xcompetition.Add(new XElement(ConstantXML.Competition_Titre, nom));
            xcompetition.Add(new XElement(ConstantXML.Competition_Lieu, lieu));
            return xcompetition;
        }


        public bool IsOfficielle()
        {
            return this.type2 == 2;
        }

        public bool IsProLeague()
        {
            return this.type2 == 3;
        }
        public bool IsIndividuelle()
        {
            return this.type == (int)CompetitionTypeEnum.Individuel;
        }

        public bool IsShiai()
        {
            return this.type == (int)CompetitionTypeEnum.Shiai;
        }

        public bool IsEquipe()
        {
            return this.type == (int)CompetitionTypeEnum.Equipe;
        }


        /// <summary>
        /// Lecture des compétition
        /// </summary>
        /// <param name="xelement">élément décrivant les compétitions</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les compétition</returns>

        public static ICollection<Competition> LectureCompetitions(XElement xelement, OutilsTools.MontreInformation1 MI)
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
    }
}
