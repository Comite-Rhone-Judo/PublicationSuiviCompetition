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
    public class ServeurHttpBasique : ServeurHttpBase
    {
        public ServeurHttpBasique(string server_ipaddress = null) : base(server_ipaddress){}

        protected override void InitModules()
        {
            FileModule module = new FileModule("/", LocalRootPath, false);
            module.AddDefaultMimeTypes();
            _server.Add(module);

        }
    }
}
