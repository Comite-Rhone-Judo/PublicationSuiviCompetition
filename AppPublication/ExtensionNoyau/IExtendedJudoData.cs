using KernelImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppPublication.ExtensionNoyau.Engagement;

namespace AppPublication.ExtensionNoyau
{
    public interface IExtendedJudoData
    {
        DataEngagement Deroulement { get; set; }
        void SyncAll();
    }
}
