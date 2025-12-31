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
    public class ServeurHttp
    {
        private IPAddress _ipAddress = null;
        private int _port = NetworkTools.PortSiteMin;
        private HttpServer.HttpServer _server = null;
        private bool _isStart = false;
        private string _localRoolPath = string.Empty;

        public static ulong _sent_data = 0;

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

        public ServeurHttp(string server_ipaddress = null)
        {
            try
            {
                string strHostName = Dns.GetHostName();
                foreach (IPAddress ipaddress in Dns.GetHostAddresses(strHostName).Where(o => o.AddressFamily == AddressFamily.InterNetwork).Reverse())
                {
                    _ipAddress = ipaddress;
                    break;
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
        }

        private void CreateNewInstance()
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
                    // _server.Add(new BasicFileImgModule(LocalRootPath));
                    _server.Add(new FileModule("/", LocalRootPath, true));

                    // Ecoute sur l'adresse specifiee, sur toutes sinon
                    IPAddress adr = (ListeningIpAddress != null) ? ListeningIpAddress : IPAddress.Any;
                    _server.Start(adr, _port);
                    // _server.Start(IPAddress.Any, _port);
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
    }

    /*

    class BasicFileImgModule : HttpModule
    {
        private string _rootPath = string.Empty;

        public BasicFileImgModule(string rootPath)
        {
            _rootPath = rootPath;
        }

        /// <summary>
        /// Method that process the URL
        /// </summary>
        /// <param name="request">Information sent by the browser about the request</param>
        /// <param name="response">Information that is being sent back to the client.</param>
        /// <param name="session">Session used to </param>
        /// <returns>true if this module handled the request.</returns>
        public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            //if (session["times"] == null)
            //    session["times"] = 1;
            //else
            //    session["times"] = ((int)session["times"]) + 1;

            string s = request.UriPath.Substring(1);

            // Verifie si on a un espace dans l'URI: les + sont substitues par des ' ' dans les URLs
            s = s.Replace(" ", "+");
            // string filename = ConstantFile.ExportSite_dir + s.Replace("site/", "");
            string filename = Path.Combine(_rootPath, s);

            if (File.Exists(filename))
            {
                bool isText = true;
                if (Path.GetExtension(filename) == ".css")
                {
                    response.ContentType = "text/css";
                }

                if (Path.GetExtension(filename) == ".js")
                {
                    response.ContentType = "application/javascript";
                }

                if (Path.GetExtension(filename) == ".png")
                {
                    isText = false;
                    response.ContentType = "image/png";
                }


                FileAndDirectTools.NeedAccessFile(filename);
                string result = "";
                try
                {
                    if (isText)
                    {
                        result = File.ReadAllText(filename);//reader.ReadToEnd();
                    }
                    else
                    {
                        using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            response.ContentLength = stream.Length;
                            response.SendHeaders();
                            byte[] buffer = new byte[8192];
                            int bytesRead = stream.Read(buffer, 0, 8192);
                            while (bytesRead > 0)
                            {
                                response.SendBody(buffer, 0, bytesRead);
                                bytesRead = stream.Read(buffer, 0, 8192);
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    FileAndDirectTools.ReleaseFile(filename);

                    if (isText)
                    {
                        StreamWriter writer = new StreamWriter(response.Body);
                        writer.Write(result);
                        writer.Flush();
                    }

                }
            }




            // return true to tell webserver that we've handled the url
            return true;
        }
    }

    */
}
