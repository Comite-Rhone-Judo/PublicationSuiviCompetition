using FluentFTP;


namespace FranceJudo.Core.Network.Ftp
{
    // 1. On crée l'interface de la fabrique
    public interface IFtpClientFactory
    {
        // On retourne l'interface native de FluentFTP
        IFtpClient CreateClient(string host, string user, string pass);
    }

    // L'implémentation pour la production
    public class DefaultFtpClientFactory : IFtpClientFactory
    {
        public IFtpClient CreateClient(string host, string user, string pass)
        {
            return new FtpClient(host, user, pass);
        }
    }
}
