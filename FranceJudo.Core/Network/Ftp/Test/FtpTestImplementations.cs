using FluentFTP;
using FluentFTP.Helpers;
using FranceJudo.Core.IO;
using System;
using System.Net;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;


namespace FranceJudo.Core.Network.Ftp.Test
{
    public class DnsResolutionTest : FtpTestStepBase
    {
        public DnsResolutionTest() { Name = "Résolution du nom de domaine (DNS)"; }

        public override bool Execute(MiniSite site, FtpClient client, CancellationToken token)
        {
            try
            {
                // Résolution synchrone
                var addresses = Dns.GetHostAddresses(site.SiteFTPDistant);
                if (addresses == null || addresses.Length == 0)
                    throw new Exception("Résolution DNS impossible.");

                SuccessMessage = $"'{site.SiteFTPDistant}' trouvée";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur DNS : {ex.Message}";
                return false;
            }
        }
    }

    public class ProfileCheckTest : FtpTestStepBase
    {
        public ProfileCheckTest() { Name = "Vérification du profil de configuration FTP"; }

        public override bool Execute(MiniSite site, FtpClient client, CancellationToken token)
        {
            try
            {
                // Appel direct à votre méthode synchrone d'origine !
                site.CheckConfigurationSiteDistant(client);
                SuccessMessage = $"profil de '{client.Host}' trouvée";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Profil invalide : {ex.Message}";
                return false;
            }
        }
    }

    public class ConnectionTest : FtpTestStepBase
    {
        public ConnectionTest() { Name = "Connexion au serveur FTP"; }

        public override bool Execute(MiniSite site, FtpClient client, CancellationToken token)
        {
            try
            {
                if (!client.IsConnected)
                {
                    client.Connect(); // Synchrone
                }
                SuccessMessage = $"Login '{client.Credentials.UserName}' connecté";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Échec de connexion : {ex.Message}";
                return false;
            }
        }
    }

    public class RemoteDirectoryTest : FtpTestStepBase
    {
        public RemoteDirectoryTest() { Name = "Vérification et lecture du répertoire distant"; }

        public override bool Execute(MiniSite site, FtpClient client, CancellationToken token)
        {
            // Ici, il faut faire attention, le répertoire final peut ne pas exister avant le 1er upload de la competition
            // donc on va tester l'existence du répertoire Parent dans lequel la compétition sera publiée

            // Récupère le répertoire parent
            string parentPath = site.RepertoireSiteFTPDistant.GetFtpDirectoryName();

            // Si on ne peut pas extraire le parent, c'est qu'on est sans doute deja a la racine, on garde la path d'orgine
            parentPath = (string.IsNullOrEmpty(parentPath)) ? site.RepertoireSiteFTPDistant : parentPath;

            try
            {
                bool dirExists = client.DirectoryExists(parentPath);
                if (!dirExists)
                {
                    throw new Exception($"Le répertoire '{parentPath}' n'existe pas.");
                }

                var listing = client.GetListing(parentPath) ?? throw new Exception("Le serveur refuse de lister le contenu (droits insuffisants).");
                SuccessMessage = $"Répertoire '{parentPath}' trouvé";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur répertoire : {ex.Message}";
                return false;
            }
        }
    }

    public class FileTransferTest : FtpTestStepBase
    {
        public FileTransferTest() { Name = "Transfert et suppression d'un fichier de test"; }

        public override bool Execute(MiniSite site, FtpClient client, CancellationToken token)
        {
            try
            {
                // Récupère le répertoire parent
                string parentPath = site.RepertoireSiteFTPDistant.GetFtpDirectoryName();

                // Si on ne peut pas extraire le parent, c'est qu'on est sans doute deja a la racine, on garde la path d'orgine
                parentPath = (string.IsNullOrEmpty(parentPath)) ? site.RepertoireSiteFTPDistant : parentPath;

                string testFileName = FileSystemHelper.PathJoin(parentPath, $"test_connexion_{Guid.NewGuid()}.txt", unixStyle: true);
                // string testFileName = $"{site.RepertoireSiteFTPDistant.TrimEnd('/')}/test_connexion_{Guid.NewGuid()}.txt";

                byte[] testData = System.Text.Encoding.UTF8.GetBytes("Test publication");

                var uploadStatus = client.UploadBytes(testData, testFileName, FtpRemoteExists.Overwrite, true);
                if (uploadStatus != FtpStatus.Success)
                    throw new Exception("Le serveur a refusé l'écriture du fichier.");

                client.DeleteFile(testFileName);

                SuccessMessage = $"Transfert vers '{parentPath}' réalisé";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Échec d'écriture/suppression : {ex.Message}";
                return false;
            }
        }
    }

    public class DisconnectTest : FtpTestStepBase
    {
        public DisconnectTest() { Name = "Fermeture propre de la connexion"; }

        public override bool Execute(MiniSite site, FtpClient client, CancellationToken token)
        {
            try
            {
                if (client != null && client.IsConnected)
                {
                    client.Disconnect();
                }

                SuccessMessage = $"'{client.Host}' déconnecté";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur à la déconnexion : {ex.Message}";
                return false;
            }
        }
    }
}