using System;
using System.Net;
using System.Threading;
using FluentFTP;

namespace FranceJudo.Core.Network.Ftp
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
            try
            {
                bool dirExists = client.DirectoryExists(site.RepertoireSiteFTPDistant);
                if (!dirExists)
                    throw new Exception($"Le répertoire '{site.RepertoireSiteFTPDistant}' n'existe pas.");

                var listing = client.GetListing(site.RepertoireSiteFTPDistant);
                if (listing == null)
                    throw new Exception("Le serveur refuse de lister le contenu (droits insuffisants).");

                SuccessMessage = $"Répertoire '/{site.RepertoireSiteFTPDistant}' trouvé";
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
                string testFileName = $"{site.RepertoireSiteFTPDistant.TrimEnd('/')}/test_connexion_{Guid.NewGuid()}.txt";
                byte[] testData = System.Text.Encoding.UTF8.GetBytes("Test publication");

                var uploadStatus = client.UploadBytes(testData, testFileName, FtpRemoteExists.Overwrite, true);
                if (uploadStatus != FtpStatus.Success)
                    throw new Exception("Le serveur a refusé l'écriture du fichier.");

                client.DeleteFile(testFileName);

                SuccessMessage = $"Transfert vers '/{site.RepertoireSiteFTPDistant}' réalisé";
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