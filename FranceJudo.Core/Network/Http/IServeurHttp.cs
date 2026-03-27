using System.Net;

namespace FranceJudo.Core.Network.Http
{
    public interface IServeurHttp
    {
        /// <summary>
        /// Adresse IP d'ecoute
        /// </summary>
        IPAddress ListeningIpAddress { get; set; }

        int PortMin { get; set; }

        int PortMax { get; set; }   

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

        /// <summary>
        /// Ajoute un module au serveur HTTP
        /// </summary>
        /// <param name="module"></param>
        void AddModule(object module)   ;
    }
}