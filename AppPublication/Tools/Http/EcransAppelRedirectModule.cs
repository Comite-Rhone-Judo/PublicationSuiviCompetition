using System;
using HttpServer;
using HttpServer.Exceptions;
using HttpServer.HttpModules;
using HttpServer.Sessions;
using Tools.Logging;

namespace AppPublication.Tools.Http
{
    // TODO on doit avoir une connexion entre le module et le parametrage des ecrans d'appel
    public class EcransAppelRedirectModule : HttpModule
    {
        // Le path de reference pour la redirection
        private string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public EcransAppelRedirectModule(string path)
        {
            if (string.IsNullOrEmpty(path)) { throw new ArgumentNullException(nameof(path)); }

            Path = path;
        }

        public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            // Vérifie si l'URL commence par le chemin défini pour ce module
            if (!request.Uri.AbsolutePath.StartsWith(this.Path, StringComparison.InvariantCultureIgnoreCase))
            {
                return false; // Ce module ne gère pas cette requête, on passe au suivant
            }

            try
            {
                // 1. Récupérer l'identité du client (IP)
                // Note: request.RemoteEndPoint peut nécessiter un cast selon ton implémentation de IHttpRequest
                string clientIp = request.RemoteEndPoint.Address.ToString();

                // 2. Déterminer la cible en fonction de l'IP
                // Tu peux ici appeler ta logique métier ou ta configuration existante
                // string targetFile = DetermineTargetForClient(clientIp);
                string targetFile = string.Empty; // TODO Remplace par ta logique

                if (string.IsNullOrEmpty(targetFile))
                {
                    return false; // Pas de redirection définie pour ce client
                }

                // 3. Effectuer la redirection
                // Assure-toi que l'URL cible est relative à la racine du serveur web ou absolue
                response.Redirect(targetFile);

                return true; // La requête a été traitée
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de la redirection EcransAppel");
                // Log l'erreur ici via ton ILogWriter si disponible
                throw new InternalServerException();
            }
        }
    }
}
