
using KernelImpl.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    /// <summary>
    /// Description des Phases
    /// </summary>
    public class Phase : IEntityWithKey<int>
    {
        int IEntityWithKey<int>.EntityKey => id;

        public int id { get; set; }
        public string libelle { get; set; }
        public int typePhase { get; set; }
        public int nbPoules { get; set; }
        public int niveauRepechage { get; set; }
        public bool bresilien { get; set; }
        public int precedent { get; set; }
        public int suivant { get; set; }
        public Nullable<int> epreuve { get; set; }
        public int niveauRepeches { get; set; }
        public int etat { get; set; }
        public int nbCombatsFinalistes { get; set; }
        public int nbCombatsTotal { get; set; }
        public int nbJudoka { get; set; }
        public int nbQualifieMin { get; set; }
        public int nbQualifieMax { get; set; }
        public int nbJudokaPoule { get; set; }
        public bool isEquipe { get; set; }
        public bool barrage3 { get; set; }
        public bool barrage5 { get; set; }
        public bool barrage7 { get; set; }
        public int ecartement { get; set; }
        public Nullable<DateTime> date { get; set; }
        public int niveauRepechage2 { get; set; }
        public int niveauRepeches2 { get; set; }


        public Organisation.i_vue_epreuve_interface GetVueEpreuve(JudoData DC)
        {
            Organisation.i_vue_epreuve_interface ep = null;
            if (DC.competition.IsEquipe())
            {
                ep = DC.Organisation.VueEpreuveEquipes.FirstOrDefault(o => o.id == this.epreuve);
            }
            else
            {
                ep = DC.Organisation.VueEpreuves.FirstOrDefault(o => o.id == this.epreuve);
            }
            return ep;
        }

        public XElement ToXml()
        {
            XElement xphase = new XElement(ConstantXML.Phase);

            xphase.SetAttributeValue(ConstantXML.Phase_ID, id);
            xphase.SetAttributeValue(ConstantXML.Phase_Epreuve, epreuve);
            xphase.SetAttributeValue(ConstantXML.Phase_Libelle, libelle);
            xphase.SetAttributeValue(ConstantXML.Phase_Etat, etat);
            xphase.SetAttributeValue(ConstantXML.Phase_TypePhase, typePhase);
            xphase.SetAttributeValue(ConstantXML.Phase_NiveauRepechage, niveauRepechage);
            xphase.SetAttributeValue(ConstantXML.Phase_Bresilien, bresilien);
            xphase.SetAttributeValue(ConstantXML.Phase_NiveauRepeches, niveauRepeches);
            xphase.SetAttributeValue(ConstantXML.Phase_NbPoules, nbPoules);
            xphase.SetAttributeValue(ConstantXML.Phase_NbCombatsTotal, nbCombatsTotal);
            xphase.SetAttributeValue(ConstantXML.Phase_NbCombatsFinalistes, nbCombatsFinalistes);
            xphase.SetAttributeValue(ConstantXML.Phase_NbQualifiesComplet, nbQualifieMax);
            xphase.SetAttributeValue(ConstantXML.Phase_NbQualifiesIncomplet, nbQualifieMin);
            xphase.SetAttributeValue(ConstantXML.Phase_NbJudokaPoule, nbJudokaPoule);
            xphase.SetAttributeValue(ConstantXML.Phase_NbJudoka, nbJudoka);
            xphase.SetAttributeValue(ConstantXML.Phase_Suivant, suivant);
            xphase.SetAttributeValue(ConstantXML.Phase_Precedent, precedent);
            xphase.SetAttributeValue(ConstantXML.Phase_IsEquipe, isEquipe);
            xphase.SetAttributeValue(ConstantXML.Phase_Barrage3, barrage3);
            xphase.SetAttributeValue(ConstantXML.Phase_Barrage5, barrage5);
            xphase.SetAttributeValue(ConstantXML.Phase_Barrage7, barrage7);
            xphase.SetAttributeValue(ConstantXML.Phase_Ecartement, ecartement);

            xphase.SetAttributeValue(ConstantXML.Phase_Date_Tirage, date.HasValue ? ((DateTime)date).ToString("ddMMyyyy") : "");
            xphase.SetAttributeValue(ConstantXML.Phase_Time_Tirage, date.HasValue ? ((DateTime)date).ToString("HHmmss") : "");

            return xphase;
        }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_ID));
            this.typePhase = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_TypePhase));
            this.nbPoules = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NbPoules));
            this.niveauRepechage = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NiveauRepechage));
            this.niveauRepeches = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NiveauRepeches));
            this.precedent = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_Precedent));
            this.suivant = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_Suivant));
            this.etat = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_Etat));
            this.nbCombatsFinalistes = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NbCombatsFinalistes));
            this.nbCombatsTotal = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NbCombatsTotal));
            this.nbJudoka = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NbJudoka));
            this.nbQualifieMin = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NbQualifiesIncomplet));
            this.nbQualifieMax = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NbQualifiesComplet));
            this.nbJudokaPoule = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_NbJudokaPoule));
            this.ecartement = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Phase_Ecartement));

            this.epreuve = XMLTools.LectureNullableInt(xinfo.Attribute(ConstantXML.Phase_Epreuve));

            this.libelle = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Phase_Libelle));

            this.bresilien = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Phase_Bresilien));
            this.isEquipe = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Phase_IsEquipe));
            this.barrage3 = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Phase_Barrage3));
            this.barrage5 = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Phase_Barrage5));
            this.barrage7 = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Phase_Barrage7));

            this.date =
                XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Phase_Date_Tirage), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Phase_Time_Tirage), "HHmmss");

        }

        /// <summary>
        /// Lecture des Phases
        /// </summary>
        /// <param name="xelement">élément décrivant les Phases</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Phases</returns>

        public static ICollection<Phase> LecturePhases(XElement xelement, OutilsTools.MontreInformation1 MI)
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
    }
}
