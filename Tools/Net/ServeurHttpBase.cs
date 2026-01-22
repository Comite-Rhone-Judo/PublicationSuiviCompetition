using HttpServer.HttpModules;
using Tools.Enum;
using HttpListener = HttpServer.HttpListener;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using Tools.Logging;

namespace Tools.Net
{
    public class ServeurHttpBase : IServeurHttp
    {
        #region MEMBER
        protected IPAddress _ipAddress = null;
        protected int _port = NetworkTools.PortSiteMin;
        protected HttpServer.HttpServer _server = null;
        protected bool _isStart = false;
        protected string _localRoolPath = string.Empty;
        protected FileModule _defaultFileModule = null;     // Le module de gestion des fichiers statiques par defaut

        #endregion

        #region PROPERTY

        public IPAddress ListeningIpAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                _ipAddress = value;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
        }

        public bool IsStart
        {
            get
            {
                return _isStart;
            }
            private set
            {
                _isStart = value;
            }
        }

        public string LocalRootPath
        {
            get
            {
                return _localRoolPath;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value) && _localRoolPath != value)
                {
                    if(_isStart)
                    {
                        throw new InvalidOperationException("Impossible de changer le repertoire racine lorsque le serveur est demarre");
                    }

                    // On retire l'ancien module s'il n'est pas null
                    if (_defaultFileModule != null)
                    {
                        _server.Remove(_defaultFileModule);
                        _defaultFileModule = null;
                    }

                    // On change pour une valeur non nulle
                    _localRoolPath = value;

                    _defaultFileModule = new FileModule("/", _localRoolPath, true);
                    _defaultFileModule.AddDefaultMimeTypes();
                    _server.Add(_defaultFileModule);    // On l'ajoute toujours en dernier
                }
            }
        }
        #endregion

        #region CONSTRUCTOR
        public ServeurHttpBase()
        {
            try
            {
                // On cherche les adresses IP de la machine
                string strHostName = Dns.GetHostName();
                var ipAdresses = Dns.GetHostAddresses(strHostName).Where(o => o.AddressFamily == AddressFamily.InterNetwork).Reverse();

                // Par defaut on assigne la premiere adresse trouvee
                if (ipAdresses != null && ipAdresses.Count() > 0)
                {
                    ListeningIpAddress = ipAdresses.First();
                }

                // Initialise le serveur interne
                _server = new HttpServer.HttpServer();
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }
        #endregion

        #region METHODES PUBLIQUES
        public void Start()
        {
            try
            {
                if (!_isStart)
                {
                    // Verifie que l'on dispose bien du repertoire racine
                    if (String.IsNullOrEmpty(LocalRootPath))
                    {
                        throw new ArgumentOutOfRangeException(nameof(LocalRootPath));
                    }

                    _isStart = true;

                    // Cherche si un port d'ecoute est disponible (exception si Nok)
                    FindAvailablePort();

                    // Ecoute sur l'adresse specifiee, sur toutes sinon
                    IPAddress adr = (ListeningIpAddress != null) ? ListeningIpAddress : IPAddress.Any;

                    // Demarre le serveur d'ecoute (les modules doivent etre ajoutes avant)
                    _server.Start(adr, _port);
                }
            }
            catch (Exception ex)
            {
                _isStart = false;
                LogTools.Error(ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (_isStart)
                {
                    _isStart = false;
                    _server.Stop();
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        /// <summary>
        /// Ajoute un module au serveur HTTP. On s'assure que le module par defaut est toujours le dernier de la liste
        /// </summary>
        /// <param name="module"></param>
        public void AddModule(HttpModule module)
        {
            if(_server != null)
            {
                // On retire le module par defaut s'il existe
                if (_defaultFileModule != null)
                {
                    _server.Remove(_defaultFileModule);
                }

                // On ajoute le nouveau module
                _server.Add(module);

                // On remet le module par defaut en dernier
                if (_defaultFileModule != null)
                {
                    _server.Add(_defaultFileModule);
                }
            }
        }

        /// <summary>
        /// Pour l'interface
        /// </summary>
        /// <param name="module"></param>
        public void AddModule(object module)
        {
            if (!(module is HttpModule)) { throw new ArgumentException("Le module doit etre de type HttpModule", nameof(module)); }

            AddModule(module as HttpModule);
        }

        #endregion

        #region METHODES PRIVEES
        protected virtual void FindAvailablePort()
        {
            int port = NetworkTools.PortSiteMin;
            bool freePort = false;

            while (!freePort && port <= NetworkTools.PortSiteMax)
            {
                try
                {
                    //LogTools.Trace("GestionSite PORT " + port, LogTools.Level.DEBUG);
                    IPAddress adr = (ListeningIpAddress != null) ? ListeningIpAddress : IPAddress.Any;
                    // HttpListener listener = HttpListener.Create(System.Net.IPAddress.Any, port);
                    HttpListener listener = HttpListener.Create(adr, port);

                    //_server.RequestReceived += server_RequestReceived;
                    listener.Start(100);
                    listener.Stop();

                    freePort = true;
                    LogTools.Logger.Debug($"Port d'ecoute disponible: {port}");
                }
                catch /*(Exception ex)*/
                {
                    //LogTools.Log(ex);
                    freePort = false;
                    port++;
                }
            }

            if(!freePort)
            {
                LogTools.Logger.Error("Impossible de trouver un port disponible");
                throw new ArgumentOutOfRangeException("Impossible de trouver un port disponible");
            }

            _port = port;
        }
        #endregion
    }
}