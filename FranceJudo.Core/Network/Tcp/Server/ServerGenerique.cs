using FranceJudo.Core.Exceptions;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Tcp.Server
{
    public class ServerGenerique
    {
        public static ulong _sent_data = 0;
        public static ulong _receive_data = 0;

        public delegate void OnConnectionHandler(object sender, TcpClient client);
        public delegate void OnDataSentHandler(object sender, TcpClient client);
        public delegate void OnDataRecieveHandler(object sender, TcpClient client, string donnees);
        public delegate void OnEndConnectionHandler(object sender, TcpClient client);

        public event OnConnectionHandler OnConnection;
        public event OnDataRecieveHandler OnDataRecieve;
        public event OnDataSentHandler OnDataSent;
        public event OnEndConnectionHandler OnEndConnection;

        private TcpListener _tcpListener;
        private readonly Synchronized<List<TcpClient>> _clients = new Synchronized<List<TcpClient>>(new List<TcpClient>());
        private readonly Synchronized<List<SentData>> _sentData = new Synchronized<List<SentData>>(new List<SentData>());
        private readonly int _port;
        private readonly IPAddress _localaddr;
        private readonly string _endMsgTag;
        private CancellationTokenSource _serverCts;

        public string EndMsgTag => _endMsgTag;

        public ServerGenerique(IPAddress localaddr, int port, string endMsgTag)
        {
            _localaddr = localaddr;
            _port = port;
            _endMsgTag = endMsgTag;
        }

        public void Start()
        {
            _serverCts = new CancellationTokenSource();

            ClearAllClients();

            _tcpListener = new TcpListener(_localaddr, _port);
            _tcpListener.Start();

            // Boucle asynchrone non-bloquante pour accepter les clients
            _ = AcceptLoopAsync(_serverCts.Token);
        }

        public void Stop()
        {
            _serverCts?.Cancel();
            _tcpListener?.Stop();
            ClearAllClients();
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // En .NET Standard 2.0, AcceptTcpClientAsync ne prend pas de CancellationToken.
                    // L'arrêt est géré par la levée d'une exception lors du _tcpListener.Stop()
                    TcpClient client = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

                    OnConnection?.Invoke(this, client);

                    // On délègue le traitement initial au ThreadPool pour ne pas bloquer l'acceptation
                    _ = Task.Run(() => HandleReceive(client), token);
                }
            }
            catch (ObjectDisposedException) { /* Arrêt via Stop(), comportement normal */ }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted || ex.SocketErrorCode == SocketError.Interrupted) { }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void HandleReceive(TcpClient client)
        {
            LogEvent("Receive connect\r\n");

            _clients.SafeWriteAction(liste => liste.Add(client));

            ClientConnection objClientConnection = new ClientConnection(client, _endMsgTag);

            // 1. D'abord on s'abonne ! (Fix de la Race Condition)
            objClientConnection.OnMessageReceived += OnReceive;
            objClientConnection.OnRemoteHostClosed += OnRemoteHostClose;

            // 2. Ensuite on lance la boucle de lecture réseau
            objClientConnection.StartRead();
        }

        private void OnReceive(ClientConnection sender, string data)
        {
            try
            {
                OnDataRecieve?.Invoke(_tcpListener, sender.Client, data);

                _receive_data += (ulong)data.Length;

                LogEvent($"Receive {((ulong)data.Length).SizeSuffix()}  ---  {_receive_data.SizeSuffix()}");
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void OnRemoteHostClose(ClientConnection sender)
        {
            LogEvent("Remote Close\r\n");

            _clients.SafeWriteAction(liste => liste.Remove(sender.Client));
            OnEndConnection?.Invoke(_tcpListener, sender.Client);
        }

        public void Write(TcpClient tcpClient, string data)
        {
            _sentData.SafeWriteAction(liste =>
            {
                SentData sent = liste.FirstOrDefault(o => o?.Client == tcpClient);
                if (sent == null)
                    liste.Add(new SentData { Data = data, Client = tcpClient, Tentative = 1 });
                else
                    sent.Tentative += 1;
            });

            _ = WriteAsync(tcpClient, data);
        }

        public void Write(string data)
        {
            var snapshotClients = _clients.SafeReadAction(liste => liste.ToList());
            foreach (TcpClient client in snapshotClients)
            {
                Write(client, data);
            }
        }

        private async Task WriteAsync(TcpClient client, string data)
        {
            if (client == null || !client.Connected)
            {
                HandleWriteFailure(client);
                return;
            }

            try
            {
                string finalMessage = data + "\n<EOF>";
                byte[] bytes = FileSystemHelper.TheEncoding.GetBytes(finalMessage);

                var stream = client.GetStream();

                // Utilisation stricte de la surcharge .NET Standard 2.0 (sans Memory ni Span)
                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                _sentData.SafeWriteAction(liste => liste.RemoveAll(o => o?.Client == client));

                _sent_data += (ulong)data.Length;

                OnDataSent?.Invoke(this, client);
                LogEvent($"Sent {((ulong)data.Length).SizeSuffix()}  ---  {_sent_data.SizeSuffix()}");
            }
            catch (Exception ex)
            {
                LogError(ex);
                HandleWriteFailure(client);
            }
        }

        private void HandleWriteFailure(TcpClient client)
        {
            SentData sent = _sentData.SafeReadAction(liste => liste.FirstOrDefault(o => o?.Client == client));
            // Limitation des retry à 3 pour éviter une boucle infinie de relance asynchrone
            if (sent != null && sent.Tentative < 3)
            {
                Write(client, sent.Data);
            }
        }

        private void ClearAllClients()
        {
            _clients.SafeWriteAction(liste =>
            {
                foreach (TcpClient client in liste)
                {
                    if (client?.Connected == true) client.GetStream()?.Close();
                    client?.Close();
                }
                liste.Clear();
            });
        }

        private void LogEvent(string message) => LogTools.Logger?.Debug(message);
        private void LogError(Exception ex) => LogTools.Logger?.Error(new ServerException(ex.Message, ex));
    }

    internal class SentData
    {
        public string Data { get; set; }
        public TcpClient Client { get; set; }
        public int Tentative { get; set; }
    }
}