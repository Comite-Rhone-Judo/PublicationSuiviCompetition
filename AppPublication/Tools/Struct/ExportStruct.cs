using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System.Collections.Generic;
using Tools.Enum;

namespace AppPublication.Tools.Struct
{
    public class ExportStruct
    {
        public ExportEnum Selected { get; set; }
        public bool WithLogo { get; set; }
        public bool Landscape { get; set; }
        public List<i_vue_epreuve_interface> Epreuves { get; set; }
        public List<Phase> Phases { get; set; }
        public int? Tapis { get; set; }
        public Competition Competition { get; set; }
        public int Type_pesee { get; set; }
        public string Login { get; set; }
        public string Mdp { get; set; }
        public string Structure { get; set; }
        public string Fond { get; set; }
        public bool Diplomepart { get; set; }


        public string ResultPDF { get; set; }
        public bool Print { get; set; }


        public ExportStruct()
        {
            Selected = ExportEnum.Pesee;
            WithLogo = false;
            Landscape = false;
            Epreuves = new List<i_vue_epreuve_interface>();
            Competition = null;
            Type_pesee = 2;
            Login = "";
            Mdp = "";
            Structure = "";
            Fond = "";
            Diplomepart = false;
            Tapis = null;
            Phases = new List<Phase>();

            ResultPDF = "";
            Print = false;
        }
    }
}
