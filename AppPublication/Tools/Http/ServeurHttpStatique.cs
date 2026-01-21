using HttpServer.HttpModules;
using Tools.Net;

namespace AppPublication.Tools.Http
{
    public class ServeurHttpStatique : ServeurHttpBase
    {
        public ServeurHttpStatique() : base(){}

        protected override void InitModules()
        {
            FileModule module = new FileModule("/", LocalRootPath, false);
            module.AddDefaultMimeTypes();
            _server.Add(module);

        }
    }
}
