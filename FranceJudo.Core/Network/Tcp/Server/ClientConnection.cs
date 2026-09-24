using FranceJudo.Core.Exceptions;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Tcp.Server
{
    public class ClientConnection
    {
        public delegate void MessageReceive(ClientConnection sender, string Data);
        public delegate void RemoteHostClose(ClientConnection sender);

        public event MessageReceive OnMessageReceived;
        public event RemoteHostClose OnRemoteHostClosed;

        private const int READ_BUFFER_SIZE = 10240;
        private readonly TcpClient _Client;
        private string _chaine = "";
        private readonly string _endMsgTag;
        private readonly CancellationTokenSource _cts;

        public TcpClient Client => _Client;
        public string EndMsgFlag => _endMsgTag;

        public ClientConnection(TcpClient client, string endMsgTag)
        {
            _Client = client;
            _endMsgTag = endMsgTag;
            _cts = new CancellationTokenSource();
        }

        public void StartRead()
        {
            _ = ReadLoopAsync(_cts.Token);
        }

        public void Stop()
        {
            _cts.Cancel();
            CloseConnection();
        }

        private async Task ReadLoopAsync(CancellationToken token)
        {
            byte[] readBuffer = new byte[READ_BUFFER_SIZE];

            try
            {
                if (!_Client.Connected)
                {
                    CloseConnection();
                    return;
                }

                var stream = _Client.GetStream();

                while (!token.IsCancellationRequested)
                {
                    // CORRECTION ICI : Retour à la surcharge compatible .NET Standard 2.0 (4 arguments)
                    int bytesRead = await stream.ReadAsync(readBuffer, 0, READ_BUFFER_SIZE, token).ConfigureAwait(false);

                    if (bytesRead == 0) break;

                    string strReceiveData = FileSystemHelper.TheEncoding.GetString(readBuffer, 0, bytesRead);
                    _chaine += strReceiveData;

                    if (_chaine.Contains("\n<EOF>"))
                    {
                        ProcessData();
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                LogError(ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void ProcessData()
        {
            if (OnMessageReceived == null) return;

            string tmp = "";
            foreach (string data in _chaine.Split(new[] { "\n<EOF>" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (data.EndsWith(_endMsgTag))
                {
                    _ = Task.Run(() => OnMessageReceived.Invoke(this, data));
                }
                else
                {
                    tmp = data;
                }
            }
            _chaine = tmp;
        }

        private void CloseConnection()
        {
            OnRemoteHostClosed?.Invoke(this);
            _Client?.Close();
            LogEvent("Client Close\r\n");
        }

        private void LogEvent(string message) => LogTools.Logger?.Debug(message);
        private void LogError(Exception ex) => LogTools.Logger?.Error(new ServerException(ex.Message, ex));
    }
}