using AppPublication.Models.EcransAppel;
using HttpServer;
using HttpServer.Exceptions;
using HttpServer.HttpModules;
using HttpServer.Sessions;
using NLog;
using System;
using System.ComponentModel.Design;
using Tools.Export;
using Tools.Logging;
using Tools.Net;

namespace AppPublication.Publication
{
    public class EcransAppelRedirectModule : HttpModule, IContextAware
    {
        private const string kDefaultPath = "/live/ecransAppel/go";

        // La configuration des ecrans d'appel
        private EcranCollectionManager _manager = null;
        private IContextProvider _provider = null;

        /// <summary>
        /// Injection du contexte de l'application
        /// </summary>
        /// <param name="container"></param>
        public void SetContext(IContextProvider container)
        {
            // On enregistre le provider, on ne va pas chercher la configuration tout de suite car elle n'est peut être pas encore initialisée
            _provider = container;
        }

        // Le path de reference pour la redirection
        private string _path = string.Empty;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EcransAppelRedirectModule() { }

        /// <summary>
        /// Initialise le module
        /// </summary>
        public override void Init()
        {
            if (_provider == null)
            {
                LogTools.Logger.Error("EcransAppelRedirectModule: Le fournisseur de contexte n'a pas ete initialise.");
                throw new InternalServerException();
            }

            // Recupere la configuration de l'export de la structure interne pour connaitre le path
            ExportSiteInterneUrls structInterne = _provider.GetContext<ExportSiteInterneUrls>();
            if (structInterne == null)
            {
                LogTools.Logger.Error("EcransAppelRedirectModule: Le contexte n'a pas ete initialise. ExportSiteInterneUrls manquant");
                throw new InternalServerException();
            }
            Path = structInterne.UrlPathEcransAppel;

            // Récupère la configuration des écrans d'appel
            if (_manager == null)
            {
                _manager = _provider.GetContext<EcranCollectionManager>();
            }
            if (_manager == null)
            {
                LogTools.Logger.Error("EcransAppelRedirectModule: Le contexte n'a pas ete initialise. ExportSiteInterneUrls manquant");
                throw new InternalServerException();
            }
        }

        public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            // Vérifie que le chemin est défini
            if (string.IsNullOrEmpty(this.Path))
            {
                LogTools.Logger.Error("EcransAppelRedirectModule: Le Path n'est pas defini.");
                throw new InternalServerException("EcransAppelRedirectModule: Le Path n'est pas defini.");
            }

            // Et que le contexte existe
            if (_manager == null)
            {
                LogTools.Logger.Error("EcransAppelRedirectModule: Le contexte n'est pas defini.");
                throw new InternalServerException("EcransAppelRedirectModule: Le context n'est pas defini.");
            }

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
