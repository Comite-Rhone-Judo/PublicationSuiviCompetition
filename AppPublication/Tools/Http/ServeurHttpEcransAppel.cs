using HttpServer.HttpModules;
using Tools.Net;

namespace AppPublication.Tools.Http
{
    public class ServeurHttpEcransAppel : ServeurHttpBase
    {
        public ServeurHttpEcransAppel() : base(){}

        protected override void InitModules()
        {
            // Module de redirection des ecrans d'appel (en 1er)
            EcransAppelRedirectModule redirectModule = new EcransAppelRedirectModule("/redirect");
            _server.Add(redirectModule);

            // Module de gestion des fichiers statiques
            FileModule module = new FileModule("/", LocalRootPath, false);
            module.AddDefaultMimeTypes();
            _server.Add(module);

        }
    }
}
