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
    public abstract class ServeurHttpBase : IServeurHttp
    {
        #region MEMBER
        protected IPAddress _ipAddress = null;
        protected int _port = NetworkTools.PortSiteMin;
        protected HttpServer.HttpServer _server = null;
        protected bool _isStart = false;
        protected string _localRoolPath = string.Empty;

        // public static ulong _sent_data = 0;

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
                    _localRoolPath = value;
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
                    CreateNewInstance();
                    _server = new HttpServer.HttpServer();

                    InitModules();

                    // Ecoute sur l'adresse specifiee, sur toutes sinon
                    IPAddress adr = (ListeningIpAddress != null) ? ListeningIpAddress : IPAddress.Any;
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
        #endregion

        #region METHODES PRIVEES
        protected virtual void CreateNewInstance()
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
                    //LogTools.Trace("GestionSite PORT " + port + " OK", LogTools.Level.DEBUG);
                }
                catch /*(Exception ex)*/
                {
                    //LogTools.Log(ex);
                    freePort = false;
                    port++;
                }
            }

            //LogTools.Trace("GestionSite OK 1", LogTools.Level.DEBUG);

            _port = port;
        }


        /// <summary>
        /// Methodes abstraites initialisatn les modules du serveur HTTP
        /// </summary>
        protected abstract void InitModules();

        #endregion
    }
}