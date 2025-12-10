using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Enum;

namespace KernelImpl.Noyau.Organisation
{
    public interface i_vue_epreuve_interface
    {
        int id { get; set; }
        string remoteID { get; set; }
        string nom { get; set; }
        DateTime debut { get; set; }
        DateTime fin { get; set; }
        string ordre { get; set; }
        string nom_compet { get; set; }
        CompetitionDisciplineEnum discipline_competition { get; set; }

        int competition { get; set; }
        int categorieAge { get; set; }
        string remoteId_cateage { get; set; }
        string nom_cateage { get; set; }
        string nom_catepoids { get; set; }
        string lib_sexe { get; set; }

        int ceintureMin { get; set; }
        int ceintureMax { get; set; }
        int anneeMin { get; set; }
        int anneeMax { get; set; }

        Nullable<int> phase1 { get; set; }
        Nullable<int> phase2 { get; set; }

        System.Xml.Linq.XElement ToXml(IJudoData DC);
    }
}
