using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Enum
{
    /// <summary>
    /// Enumération des états des judokas
    /// </summary>
    public enum EtatJudokaEnum
    {
        Aucun = 0,
        Inscrit = 1,
        Present = 2,
        Absent = 3, 
        AuPoids = 4,
        HorsPoids = 5,
        HorsCategorie = 6
    }
}
