using FranceJudo.Core.XML;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.XML;
using KernelImpl.Internal;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace KernelImpl.Noyau.Organisation
{
    /// <summary>
    /// Description des Competitions
    /// </summary>
    public class Competition : ICompetition, IEntityWithKey<int>
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
            this.type = CompetitionTypeEnum.Individuel;
            this.type2 = CompetitionType2Enum.Officielle;
            this.discipline = CompetitionDisciplineEnum.Judo.ToString2();
            this.nbTapis = 6;
            this.tempsCombat = 600;
            this.niveau = (int)EchelonEnum.Club;
            this.couleur1 = "";
            this.couleur2 = "";
            this.version = "";
            this.afficheCSA = (int)TypeCSAEnum.Aucun;
            this.afficheKinzas = false;
            this.afficheAutoTempsRecuperation = true;
            this.afficheAnimationVainqueur = false;
            this.tempsMedical = 120;
            this.isRandomCombat = false;
            this.couleur1 = ConstantCouleur.Rouge.ToString();
            this.couleur2 = ConstantCouleur.Blanc.ToString();
            this.reglementEquipe = ReglementEquipeEnum.FFJDA;
        }

        int IEntityWithKey<int>.EntityKey => id;

        public int id { get; set; }
        public string nom { get; set; }
        public DateTime date { get; set; }
        public string lieu { get; set; }
        public string siteInternet { get; set; }

        public string remoteId { get; set; }
        public string codeAcces { get; set; }
        public CompetitionTypeEnum type { get; set; }
        public CompetitionType2Enum type2 { get; set; }

        private string _discipline;
        private CompetitionDisciplineEnum _disciplineEnum;
        public string discipline
        {
            get { return _discipline; }
            set
            {
                _discipline = value;
                _disciplineEnum = _discipline.ByString2();
            }
        }

        public CompetitionDisciplineEnum disciplineId
        {
            get
            {
                return _disciplineEnum;
            }
            private set
            {
                _disciplineEnum = value;
            }
        }
        public int nbTapis { get; set; }
        public int tempsCombat { get; set; }
        public int niveau { get; set; }
        public string couleur1 { get; set; }
        public string couleur2 { get; set; }
        public string version { get; set; }

        public int afficheCSA { get; set; }

        public bool afficheKinzas { get; set; }
        public bool afficheAutoTempsRecuperation { get; set; }

        public bool afficheAnimationVainqueur { get; set; }

        public int tempsMedical { get; set; }
        public bool isRandomCombat { get; set; }
        public ReglementEquipeEnum reglementEquipe { get; set; }


        public void LoadXml(XElement xinfo)
        {
            this.nom = XMLTools.LectureString(xinfo.Element(ConstantXML.Competition_Titre));
            this.lieu = XMLTools.LectureString(xinfo.Element(ConstantXML.Competition_Lieu));

            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_ID));

            this.tempsCombat = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_TempsCombat));
            this.siteInternet = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_SiteInternet));
            this.codeAcces = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_CodeAcces));

            this.date = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Competition_Date), "ddMMyyyy", DateTime.Now);
            this.remoteId = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_RemoteID));
            this.type = (CompetitionTypeEnum)XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Type));
            this.type2 = (CompetitionType2Enum)XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Type2));

            this.discipline = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Discipline));

            this.nbTapis = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Tapis));

            this.niveau = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_Niveau));
            this.version = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Version));
            this.couleur1 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Couleur1));
            this.couleur2 = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_Couleur2));
            this.afficheCSA = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_AfficheCSA));

            this.afficheKinzas = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_AfficheKinzas)) == "Oui";
            this.afficheAutoTempsRecuperation = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_AfficheAutoTempsRecuperation)) == "Oui";
            this.afficheAnimationVainqueur = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_AfficheAnimationVainqueur)) == "Oui";

            this.tempsMedical = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_TempsMedical));
            this.isRandomCombat = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Competition_RandomCombat)) == "Oui";
            this.reglementEquipe = (ReglementEquipeEnum)XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Competition_ReglementEquipe));
        }

        public XElement ToXml(IJudoData DC = null)
        {
            XElement xcompetition = new XElement(ConstantXML.Competition);
            xcompetition.SetAttributeValue(ConstantXML.Competition_ID, id.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_RemoteID, remoteId.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Date, date.ToString("ddMMyyyy"));
            xcompetition.SetAttributeValue(ConstantXML.Competition_Type, (int) type);
            xcompetition.SetAttributeValue(ConstantXML.Competition_Type2, (int) type2);

            xcompetition.SetAttributeValue(ConstantXML.Competition_Discipline, discipline);
            xcompetition.SetAttributeValue(ConstantXML.Competition_DisciplineId, (int)disciplineId);

            xcompetition.SetAttributeValue(ConstantXML.Competition_Niveau, niveau.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Version, version.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Couleur1, couleur1.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_Couleur2, couleur2.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_AfficheCSA, afficheCSA.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_AfficheKinzas, afficheKinzas ? "Oui" : "Non");
            xcompetition.SetAttributeValue(ConstantXML.Competition_AfficheAutoTempsRecuperation, afficheAutoTempsRecuperation ? "Oui" : "Non");
            xcompetition.SetAttributeValue(ConstantXML.Competition_AfficheAnimationVainqueur, afficheAnimationVainqueur ? "Oui" : "Non");

            xcompetition.SetAttributeValue(ConstantXML.Competition_RandomCombat, isRandomCombat ? "Oui" : "Non");
            xcompetition.SetAttributeValue(ConstantXML.Competition_Tapis, nbTapis.ToString());
            xcompetition.SetAttributeValue(ConstantXML.Competition_TempsMedical, tempsMedical.ToString());

            xcompetition.SetAttributeValue(ConstantXML.Competition_TempsCombat, tempsCombat);
            xcompetition.SetAttributeValue(ConstantXML.Competition_SiteInternet, siteInternet ?? string.Empty);
            xcompetition.SetAttributeValue(ConstantXML.Competition_CodeAcces, codeAcces ?? string.Empty);

            xcompetition.Add(new XElement(ConstantXML.Competition_Titre, nom));
            xcompetition.Add(new XElement(ConstantXML.Competition_Lieu, lieu));
            xcompetition.SetAttributeValue(ConstantXML.Competition_ReglementEquipe, (int) reglementEquipe);
            return xcompetition;
        }


        public bool IsOfficielle()
        {
            return this.type2 == CompetitionType2Enum.Officielle;
        }

        public bool IsProLeague()
        {
            return this.type2 == CompetitionType2Enum.ProLeague;
        }
        public bool IsIndividuelle()
        {
            return this.type == CompetitionTypeEnum.Individuel;
        }

        public bool IsShiai()
        {
            return this.type == CompetitionTypeEnum.Shiai;
        }

        public bool IsEquipe()
        {
            return this.type == CompetitionTypeEnum.Equipe;
        }


        /// <summary>
        /// Lecture des compétition
        /// </summary>
        /// <param name="xelement">élément décrivant les compétitions</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les compétition</returns>

        public static ICollection<Competition> LectureCompetitions(XElement xelement)
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
