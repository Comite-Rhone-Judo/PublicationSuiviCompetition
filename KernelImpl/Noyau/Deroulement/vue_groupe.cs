
using System;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    public partial class vue_groupe : IIdEntity<int>
    {
        public int groupe_id { get; set; }

        public int id { get { return groupe_id; } }
        public int groupe_tapis { get; set; }
        public string groupe_libelle { get; set; }
        public Nullable<System.DateTime> groupe_debut { get; set; }
        public Nullable<System.DateTime> groupe_fin { get; set; }
        public bool groupe_verrouille { get; set; }
        public int nb_combats_restant { get; set; }
        public int phase_etat { get; set; }
        public string phase_libelle { get; set; }
        public int phase_id { get; set; }
        public int phase_type { get; set; }
        public Nullable<int> epreuve_id { get; set; }
        public string epreuve_nom { get; set; }
        public int epreuve_poidsMin { get; set; }
        public int epreuve_poidsMax { get; set; }
        public string epreuve_libsexe { get; set; }


        public vue_groupe(Groupe_Combats groupe, JudoData DC)
        {
            groupe_id = groupe.id;
            groupe_tapis = groupe.tapis;
            groupe_libelle = groupe.libelle;
            groupe_debut = groupe.horaire_debut;
            groupe_fin = groupe.horaire_fin;
            groupe_verrouille = groupe.verrouille;
            nb_combats_restant = DC.Deroulement.Combats.Count(o => o.groupe == groupe.id && o.vainqueur == null);

            Phase phase = groupe.GetPhase(DC);
            if (phase != null)
            {
                phase_etat = phase.etat;
                phase_libelle = phase.libelle;
                phase_type = phase.typePhase;
                phase_id = phase.id;

                Organisation.Epreuve epreuve = groupe.GetEpreuve(DC);
                if (epreuve != null)
                {
                    epreuve_id = epreuve.id;
                    epreuve_nom = epreuve.nom;
                    epreuve_poidsMin = epreuve.poidsMin;
                    epreuve_poidsMax = epreuve.poidsMax;
                    epreuve_libsexe = epreuve.sexeEnum.ToString();
                }
            }
        }


        public void LoadXml(XElement xinfo)
        {
            this.groupe_id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_Id));
            this.groupe_tapis = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_Tapis));

            this.groupe_libelle = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Vue_Groupe_Libelle));


            DateTime debut = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Vue_Groupe_DebutDate), "dd/MM/yyyy", DateTime.Now) +
                               XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Vue_Groupe_DebutTime), "HH:mm");

            DateTime fin = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Vue_Groupe_FinDate), "dd/MM/yyyy", DateTime.Now) +
                             XMLTools.LectureTime(xinfo.Attribute(ConstantXML.Vue_Groupe_FinTime), "HH:mm");

            this.groupe_debut = debut;
            this.groupe_fin = fin;

            this.groupe_verrouille = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Vue_Groupe_Verrouille));

            this.nb_combats_restant = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_Restant));
            this.phase_etat = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_PhaseEtat));
            this.phase_id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_PhaseId));
            this.phase_type = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_PhaseType));
            this.epreuve_id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_EpreuveId));
            this.epreuve_poidsMin = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_EpreuvePoidsMin));
            this.epreuve_poidsMax = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Vue_Groupe_EpreuvePoidsMax));

            this.phase_libelle = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Vue_Groupe_PhaseLibelle));
            this.epreuve_nom = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Vue_Groupe_EpreuveNom));
            this.epreuve_libsexe = XMLTools.LectureString(xinfo.Attribute(ConstantXML.Vue_Groupe_EpreuveLibsexe));
        }

        public XElement ToXml()
        {
            XElement xgroupe = new XElement(ConstantXML.Vue_Groupe);

            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_Id, this.groupe_id.ToString());

            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_Tapis, this.groupe_tapis.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_Libelle, this.groupe_libelle);
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_DebutDate, ((DateTime)this.groupe_debut).ToString("dd/MM/yyyy"));
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_DebutTime, ((DateTime)this.groupe_debut).ToString("HH:mm"));
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_Verrouille, this.groupe_verrouille.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_Restant, this.nb_combats_restant.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_PhaseEtat, this.phase_etat.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_PhaseLibelle, this.phase_libelle);
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_PhaseId, this.phase_id.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_PhaseType, this.phase_type.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_EpreuveId, this.epreuve_id.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_EpreuveNom, this.epreuve_nom);
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_EpreuvePoidsMin, this.epreuve_poidsMin.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_EpreuvePoidsMax, this.epreuve_poidsMax.ToString());
            xgroupe.SetAttributeValue(ConstantXML.Vue_Groupe_EpreuveLibsexe, this.epreuve_libsexe);
            return xgroupe;
        }
    }
}
