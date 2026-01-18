using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Enum;

namespace AppPublication.ExtensionNoyau.Engagement
{
    public  interface IEngagementData
    {
        /// <summary>
        /// Les groupes d'engages
        /// </summary>
        IReadOnlyList<GroupeEngagements> GroupesEngages { get; }


        /// <summary>
        /// Les types de groupes pour chaque competition
        /// </summary>
        IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes { get; }
    }
}
