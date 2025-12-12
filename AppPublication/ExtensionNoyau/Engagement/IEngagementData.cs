using KernelImpl.Noyau.Deroulement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.ExtensionNoyau.Engagement
{
    public  interface IEngagementData
    {
        IReadOnlyList<GroupeEngagements> GroupesEngages { get; }
    }
}
