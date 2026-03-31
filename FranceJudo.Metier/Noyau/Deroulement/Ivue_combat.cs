using FranceJudo.Metier.Noyau.Organisation;
using System;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface Ivue_combat
    {
        public int combat_id { get; set; }

        public int combat_numero { get; set; }
        public string combat_reference { get; set; }
        public int combat_score1 { get; set; }
        public int combat_score2 { get; set; }
        public int combat_penalite1 { get; set; }
        public int combat_penalite2 { get; set; }
        public DateTime combat_programmation { get; set; }
        public DateTime combat_debut { get; set; }
        public DateTime combat_fin { get; set; }
        public int combat_etat { get; set; }
        public Nullable<int> combat_vaiqueur { get; set; }
        public int combat_tapis { get; set; }
        public int combat_groupe { get; set; }
        public string combat_details { get; set; }
        public int combat_niveau { get; set; }
        public int combat_temps { get; set; }
        public int combat_tempsRecup { get; set; }

        public string combat_discipline { get; set; }
        public int judoka1_id { get; set; }
        public string judoka1_club { get; set; }
        public string judoka1_licence { get; set; }
        public string judoka1_nom { get; set; }
        public string judoka1_prenom { get; set; }
        public int judoka2_id { get; set; }
        public string judoka2_club { get; set; }
        public string judoka2_licence { get; set; }
        public string judoka2_nom { get; set; }
        public string judoka2_prenom { get; set; }
        public int phase_id { get; set; }
        public string phase_libelle { get; set; }
        public int phase_type { get; set; }
        public int phase_etat { get; set; }
        public int cateAge_id { get; set; }
        public string cateAge_nom { get; set; }
        public int catePoids_id { get; set; }
        public string catePoids_nom { get; set; }
        public int competition_id { get; set; }
        public string competition_nom { get; set; }
        public Nullable<DateTime> competition_date { get; set; }
        public string competition_lieu { get; set; }
        public int epreuve_id { get; set; }
        public string epreuve_nom { get; set; }
        public int epreuve_poidsMin { get; set; }
        public int epreuve_poidsMax { get; set; }
        public int epreuve_ceintureMin { get; set; }
        public int epreuve_ceintureMax { get; set; }
        public int epreuve_anneeMin { get; set; }
        public int epreuve_anneeMax { get; set; }
        public Nullable<DateTime> epreuve_debut { get; set; }
        public Nullable<DateTime> epreuve_fin { get; set; }
        public string phase_name { get; set; }
        public string epreuve_sexe { get; set; }
        public EpreuveSexe epreuve_sexeEnum { get; set; }

        public int judoka1_id1 { get; set; }
        public string judoka1_licence1 { get; set; }
        public string judoka1_nom1 { get; set; }
        public string judoka1_prenom1 { get; set; }
        public string judoka1_club1 { get; set; }
        public int judoka2_id1 { get; set; }
        public string judoka2_licence1 { get; set; }
        public string judoka2_nom1 { get; set; }
        public string judoka2_prenom1 { get; set; }
        public string judoka2_club1 { get; set; }
    }
}
