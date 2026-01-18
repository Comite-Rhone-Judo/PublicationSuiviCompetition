using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tools.Enum;
using Tools.Logging;

namespace Tools.Net
{
    public interface ServeurHttp
    {
        /// <summary>
        /// Adresse IP d'ecoute
        /// </summary>
        IPAddress ListeningIpAddress { get; set; }

        /// <summary>
        /// Port d'ecoute automatiqiement assigne
        /// </summary>
        int Port { get; }

        /// <summary>
        /// True si le serveur est demarre
        /// </summary>
        bool IsStart { get; }

        /// <summary>
        /// La racine du site
        /// </summary>
        string LocalRootPath { get; set; }
     
        /// <summary>
        /// Demarre le serveur
        /// </summary>
        void Start();
        
        /// <summary>
        /// Arrete le serveur
        /// </summary>
        void Stop();
    }
}