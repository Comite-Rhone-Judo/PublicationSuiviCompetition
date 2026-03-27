
using KernelImpl.Internal;
using FranceJudo.Core.XML;
using FranceJudo.Metier.Regles;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.XML;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;

namespace KernelImpl.Noyau.Deroulement
{
    public class Groupe_Combats : IGroupe_Combats, IEntityWithKey<int>
    {
        int IEntityWithKey<int>.EntityKey => id;

        public int id { get; set; }
        public int decoupage { get; set; }
        public int tapis { get; set; }
        public string libelle { get; set; }
        public int numero { get; set; }
        public Nullable<DateTime> horaire_debut { get; set; }
        public Nullable<DateTime> horaire_fin { get; set; }
        public bool verrouille { get; set; }


        public IEpreuve GetEpreuve(IJudoData DC)
        {
            IPhase_Decoupage decoup = DC.Deroulement.Decoupages.FirstOrDefault(o => o.id == this.decoupage);
            if (decoup == null)
            {
                return null;
            }

            IPhase phase = DC.Deroulement.Phases.FirstOrDefault(o => o.id == decoup.phase);
            if (phase == null)
            {
                return null;
            }

            return DC.Organisation.Epreuves.FirstOrDefault(o => o.id == phase.epreuve);
        }

        public IPhase GetPhase(IJudoData DC)
        {
            IPhase_Decoupage decoup = DC.Deroulement.Decoupages.FirstOrDefault(o => o.id == this.decoupage);
            if (decoup == null)
            {
                return null;
            }

            IPhase phase = DC.Deroulement.Phases.FirstOrDefault(o => o.id == decoup.phase);
            return phase;
        }

        public void LoadXml(XElement xinfo)
        {
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Groupe_ID));
            this.tapis = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Groupe_Tapis));
            this.libelle = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Groupe_Libelle));

            this.horaire_debut =
                XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Groupe_Horaire_Debut_Date), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Groupe_Horaire_Debut_Time), "HHmmss");

            this.horaire_fin =
                XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Groupe_Horaire_Fin_Date), "ddMMyyyy", DateTime.Now) +
                XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Groupe_Horaire_Fin_Time), "HHmmss");

            this.verrouille = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Groupe_Verrouille));
            this.decoupage = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Groupe_Decoupage));
        }

        public XElement ToXml(IJudoData DC = null)
        {
            XElement xgroupe = new XElement(ConstantXML.Groupe);

            xgroupe.SetAttributeValue(ConstantXML.Groupe_ID, id);
            xgroupe.SetAttributeValue(ConstantXML.Groupe_Tapis, tapis);
            xgroupe.SetAttributeValue(ConstantXML.Groupe_Libelle, libelle);

            xgroupe.SetAttributeValue(ConstantXML.Groupe_Horaire_Debut_Date, horaire_debut.HasValue ? ((DateTime)horaire_debut).ToString("ddMMyyyy") : null);
            xgroupe.SetAttributeValue(ConstantXML.Groupe_Horaire_Debut_Time, horaire_debut.HasValue ? ((DateTime)horaire_debut).ToString("HHmmss") : null);
            xgroupe.SetAttributeValue(ConstantXML.Groupe_Horaire_Fin_Date, horaire_fin.HasValue ? ((DateTime)horaire_fin).ToString("ddMMyyyy") : null);
            xgroupe.SetAttributeValue(ConstantXML.Groupe_Horaire_Fin_Time, horaire_fin.HasValue ? ((DateTime)horaire_fin).ToString("HHmmss") : null);

            xgroupe.SetAttributeValue(ConstantXML.Groupe_Verrouille, verrouille);
            xgroupe.SetAttributeValue(ConstantXML.Groupe_Decoupage, decoupage);

            return xgroupe;
        }


        /// <summary>
        /// Lecture des Groupe_Combats
        /// </summary>
        /// <param name="xelement">élément décrivant les Groupe_Combats</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Feuilles</returns>

        public static ICollection<Groupe_Combats> LectureGroupes(XElement xelement)
        {
            ICollection<Groupe_Combats> groupes = new List<Groupe_Combats>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Groupe))
            {
                Groupe_Combats groupe = new Groupe_Combats();
                groupe.LoadXml(xinfo);
                groupes.Add(groupe);
            }
            return groupes;
        }
    }
}
