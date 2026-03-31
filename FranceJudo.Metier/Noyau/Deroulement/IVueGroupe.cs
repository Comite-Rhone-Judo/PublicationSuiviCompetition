using FranceJudo.Metier.XML;
using System;

namespace FranceJudo.Metier.Noyau.Deroulement
{
    public interface IVueGroupe : IXMLSerializable
    {
        public int groupe_id { get; set; }

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
    }
}
