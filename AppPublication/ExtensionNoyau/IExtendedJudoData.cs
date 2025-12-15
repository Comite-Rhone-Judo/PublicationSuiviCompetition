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
        /// <summary>
        /// Retourne la section de donnees d'engagement
        /// </summary>
        DataEngagement Engagement { get; set; }
        
        /// <summary>
        /// Synchronise les donnees etendue avec les donnees principales
        /// </summary>
        /// <param name="snapshot"></param>
        void SyncAll(IJudoData snapshot);
    }
}
