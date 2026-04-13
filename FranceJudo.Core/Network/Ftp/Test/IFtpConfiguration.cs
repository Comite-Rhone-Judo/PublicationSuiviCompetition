using FluentFTP;

public interface IFtpConfiguration
{
    string Host { get; }
    string Username { get; }
    string Password { get; }
    string RemotePath { get; }
    bool UseActiveMode { get; }

    FtpProfile CurrentProfile { get; }

    // Délègue la responsabilité de l'auto-configuration à l'implémenteur
    bool ResolveProfile(FtpClient client);
}