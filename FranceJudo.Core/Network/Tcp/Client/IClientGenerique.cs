namespace FranceJudo.Core.Network.Tcp.Client
{
    // 1. Les délégués sont maintenant publics et accessibles à tous
    public delegate void OnConnectionHandler(object sender);
    public delegate void OnDataRecieveHandler(object sender, string donnees);
    public delegate void OnDataSentHandler(object sender);
    public delegate void OnEndConnectionHandler(object sender);

    // 2. L'interface qui décrit le contrat
    public interface IClientGenerique
    {
        string IP { get; }
        int Port { get; }
        string EndMsgFlag { get; }
        System.Net.IPEndPoint EndPoint { get; }
        bool IsConnected { get; }

        event OnConnectionHandler OnConnection;
        event OnDataRecieveHandler OnDataRecieve;
        event OnDataSentHandler OnDataSent;
        event OnEndConnectionHandler OnEndConnection;

        void Connect();
        void Stop();
        void Write(string data);
    }
}