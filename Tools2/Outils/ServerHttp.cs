using HttpServer;
using HttpServer.HttpModules;
using HttpServer.Sessions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tools.Enum;
using HttpListener = HttpServer.HttpListener;

namespace Tools.Outils
{
    public class ServerHttp
    {
        ////private bool _isStart = false;
        //private static ulong _sent_data = 0;
        //private List<string> _prefixes = new List<string>();


        //WebServer _server = null;

        //public ServerHttp(string server_ipaddress = null)
        //{
        //    try
        //    {
        //        string strHostName = Dns.GetHostName();
        //        foreach (IPAddress ipaddress in Dns.GetHostAddresses(strHostName).Where(o => o.AddressFamily == AddressFamily.InterNetwork).Reverse())
        //        {
        //            _ipAddress = (_ipAddress != null ? _ipAddress : ipaddress);
        //            for (int port = NetworkTools.PortSiteMin; port <= NetworkTools.PortSiteMax; port++)
        //            {
        //                _prefixes.Add("http://" + ipaddress.ToString() + ":" + port + "/site/");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogTools.Log(ex);
        //    }
        //}

        //public string ReadFile(HttpListenerRequest request)
        //{
        //    string result = "";
        //    try
        //    {
        //        string s = request.RawUrl;
        //        string filename = ConstantFile.ExportSite_dir + s.Replace("site/", "");

        //        if (File.Exists(filename))
        //        {
        //            FileAndDirectTools.NeedAccessFile(filename);
        //            try
        //            {
        //                result = File.ReadAllText(filename);
        //                //_sent_data += (ulong)result.Length;
        //                //string mess = "HTTP_SERVER request " + filename + "\r\n";
        //                //mess += "HTTP_SERVER sent    " + OutilsTools.SizeSuffix((ulong)(result.Length)) + "  ---  " + OutilsTools.SizeSuffix((ulong)(_sent_data));
        //                //LogTools.Trace(mess, LogTools.Level.INFO);
        //            }
        //            catch (Exception ex) { throw ex; }
        //            finally
        //            {
        //                FileAndDirectTools.ReleaseFile(filename);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return result;
        //}

        //public void Start()
        //{
        //    try
        //    {
        //        if(_server == null)
        //        {
        //            _server = new WebServer(ReadFile, _prefixes.ToArray());
        //            _server.Run();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogTools.Trace(ex);
        //    }
        //}

        //public void Stop()
        //{
        //    try
        //    {
        //        _server.Stop();
        //        _server = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogTools.Trace(ex);
        //    }
        //}

        //private IPAddress _ipAddress = null;
        //private int _port = NetworkTools.PortSiteMin;
        //public IPAddress IpAddress
        //{
        //    get
        //    {
        //        return _ipAddress;
        //    }
        //    set
        //    {
        //        _ipAddress = value;
        //    }
        //}

        //public int Port
        //{
        //    get
        //    {
        //        return _port;
        //    }
        //}



        private IPAddress _ipAddress = null;
        private int _port = NetworkTools.PortSiteMin;
        private HttpServer.HttpServer _server = null;
        private bool _isStart = false;

        public static ulong _sent_data = 0;

        public IPAddress IpAddress
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

        public ServerHttp(string server_ipaddress = null)
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
                LogTools.Log(ex);
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
                    HttpListener listener = HttpListener.Create(System.Net.IPAddress.Any, port);

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
                    _isStart = true;
                    CreateNewInstance();
                    _server = new HttpServer.HttpServer();
                    _server.Add(new MyModule());

                    _server.Start(IPAddress.Any, _port);
                }
            }
            catch (Exception ex)
            {
                _isStart = false;
                LogTools.Trace(ex);
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
                LogTools.Trace(ex);
            }
        }

        //void server_RequestReceived(object sender, HttpServer.RequestEventArgs e)
        //{
        //    try
        //    {
        //        ThreadPool.QueueUserWorkItem((c) =>
        //        {
        //            var ctx = c as HttpServer.RequestEventArgs;

        //            string s = e.Request.UriPath.Substring(1);

        //            HttpServer.IHttpClientContext context = (HttpServer.IHttpClientContext)sender;
        //            HttpServer.IHttpRequest request = e.Request;
        //            HttpServer.IHttpResponse response = request.CreateResponse(context);

        //            string filename = ConstantFile.ExportSite_dir + s.Replace("site/", "");

        //            if (File.Exists(filename))
        //            {
        //                if (Path.GetExtension(filename) == ".css")
        //                {
        //                    response.ContentType = "text/css";
        //                }

        //                if (Path.GetExtension(filename) == ".js")
        //                {
        //                    response.ContentType = "application/javascript";
        //                }

        //                FileAndDirectTools.NeedAccessFile(filename);
        //                string result = "";
        //                try
        //                {
        //                    result = File.ReadAllText(filename);//reader.ReadToEnd();
        //                }
        //                catch
        //                { }
        //                finally
        //                {
        //                    FileAndDirectTools.ReleaseFile(filename);
        //                }

        //                try
        //                {
        //                    _sent_data += (ulong)result.Length;
        //                    string mess = "HTTP_SERVER request " + filename + "\r\n";
        //                    mess += "HTTP_SERVER sent    " + OutilsTools.SizeSuffix((ulong)(result.Length)) + "  ---  " + OutilsTools.SizeSuffix((ulong)(_sent_data));
        //                    LogTools.Trace(mess, LogTools.Level.INFO);

        //                    using (StreamWriter writer = new StreamWriter(response.Body))
        //                    {
        //                        writer.Write(result);
        //                        writer.Flush();
        //                        response.Send();
        //                    }
        //                }
        //                catch
        //                { }

        //            }
        //        }, e);                
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
    }


    class MyModule : HttpModule
    {
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
            string filename = ConstantFile.ExportSite_dir + s.Replace("site/", "");

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

                //try
                //{
                //    _sent_data += (ulong)result.Length;
                //    string mess = "HTTP_SERVER request " + filename + "\r\n";
                //    mess += "HTTP_SERVER sent    " + OutilsTools.SizeSuffix((ulong)(result.Length)) + "  ---  " + OutilsTools.SizeSuffix((ulong)(_sent_data));
                //    LogTools.Trace(mess, LogTools.Level.INFO);

                //    using (StreamWriter writer = new StreamWriter(response.Body))
                //    {
                //        writer.Write(result);
                //        writer.Flush();
                //        response.Send();
                //    }
                //}
                //catch
                //{
                //}

            }


                

            // return true to tell webserver that we've handled the url
            return true;
        }
    }
}
