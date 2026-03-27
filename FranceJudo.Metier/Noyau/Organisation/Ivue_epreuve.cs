using System;
using System.Linq;
using System.Xml.Linq;

namespace FranceJudo.Metier.Noyau.Organisation
{
    public interface Ivue_epreuve : i_vue_epreuve_interface
    {
        public int categoriePoids { get; set; }
        public int poidsMin { get; set; }
        public int poidsMax { get; set; }
        public int sexe { get; set; }
        public EpreuveSexe sexeEnum { get; set; }
        public string remoteId_catepoids { get; set; }
        public Nullable<int> id_epreuve_equipe { get; set; }
        public string lib_epreuve_equipe { get; set; }
        public EpreuveEquipeTypeEnum type_epreuve_equipe { get; set; }
        public int epreuveRef_epreuve_equipe { get; set; }
    }
}
