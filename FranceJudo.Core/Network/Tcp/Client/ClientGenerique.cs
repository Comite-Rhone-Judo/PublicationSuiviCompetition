using FranceJudo.Core.Exceptions;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Tcp.Client
{
    public class ClientGenerique
    {
        public delegate void OnConnectionHandler(object sender);
        public delegate void OnDataRecieveHandler(object sender, string donnees);
        public delegate void OnDataSentHandler(object sender);
        public delegate void OnEndConnectionHandler(object sender);

        public event OnConnectionHandler OnConnection;
        public event OnDataRecieveHandler OnDataRecieve;
        public event OnDataSentHandler OnDataSent;
        public event OnEndConnectionHandler OnEndConnection;

        private const int READ_BUFFER_SIZE = 10240;
        private string _chaine = string.Empty;
        private TcpClient _objClient;
        private CancellationTokenSource _cts;
        private readonly string _endMsgTag;
        private readonly int _port;
        private readonly string _ip;

        public string IP => _ip;
        public int Port => _port;
        public string EndMsgFlag => _endMsgTag;
        public System.Net.IPEndPoint EndPoint => (System.Net.IPEndPoint)_objClient?.Client?.RemoteEndPoint;

        public bool IsConnected
        {
            get
            {
                try
                {
                    return _objClient?.Client != null && _objClient.Connected;
                }
                catch
                {
                    Log("ClientGenerique IsConnected - Exception sur la verification de la connection");
                    return false;
                }
            }
        }

        public ClientGenerique(string hostNameOrAddress, int port, string endMsgTag)
        {
            _ip = hostNameOrAddress;
            _port = port;
            _endMsgTag = endMsgTag;
        }

        public void Connect()
        {
            _cts = new CancellationTokenSource();
            _ = ConnectAsync(_cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
            CloseClient();
        }

        public void Write(string data)
        {
            if (IsConnected)
            {
                _ = WriteAsync(data, _cts.Token);
            }
        }

        private async Task ConnectAsync(CancellationToken token)
        {
            _objClient = new TcpClient
            {
                NoDelay = true,
                LingerState = new LingerOption(true, 20)
            };

            try
            {
                var connectTask = _objClient.ConnectAsync(_ip, _port);
                var timeoutTask = Task.Delay(1000, token); // Timeout de 1 sec

                if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
                {
                    CloseClient();
                    throw new TimeoutException($"Connection to {_ip}:{_port} timed out.");
                }

                await connectTask; // Surface les exceptions potentielles

                Log($"Create Client\t{DateTime.Now}\t{_objClient.GetHashCode()}");

                if (_objClient.Connected)
                {
                    OnConnection?.Invoke(this);
                    _ = ReadLoopAsync(token);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private async Task ReadLoopAsync(CancellationToken token)
        {
            try
            {
                var stream = _objClient.GetStream();
                byte[] buffer = new byte[READ_BUFFER_SIZE];

                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (bytesRead == 0) break;

                    string strReceiveData = FileSystemHelper.TheEncoding.GetString(buffer, 0, bytesRead);
                    _chaine += strReceiveData;

                    if (_chaine.Contains("\n<EOF>"))
                    {
                        ProcessReceivedData();
                    }

                    _ = Task.Run(() => Log($"Receive\t\t{DateTime.Now}\t{_objClient?.GetHashCode()}\t{strReceiveData}"), token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                LogError(ex);
            }
            finally
            {
                OnEndConnection?.Invoke(this);
                CloseClient();
                Log($"Connect Closed\t{DateTime.Now}\t{_objClient?.GetHashCode()}");
            }
        }

        private void ProcessReceivedData()
        {
            if (OnDataRecieve == null) return;

            string tmp = string.Empty;
            foreach (string data in _chaine.Split(new[] { "\n<EOF>" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (data.EndsWith(_endMsgTag))
                {
                    OnDataRecieve(this, data);
                }
                else
                {
                    tmp = data;
                }
            }
            _chaine = tmp;
        }

        private async Task WriteAsync(string data, CancellationToken token)
        {
            try
            {
                string finalMessage = data + "\n<EOF>";
                byte[] bytes = FileSystemHelper.TheEncoding.GetBytes(finalMessage);

                var stream = _objClient.GetStream();
                await stream.WriteAsync(bytes, 0, bytes.Length, token);
                await stream.FlushAsync(token);

                OnDataSent?.Invoke(this);
            }
            catch (Exception ex)
            {
                CloseClient();
                LogError(ex);
            }
        }

        private void CloseClient()
        {
            if (_objClient == null) return;
            try
            {
                if (_objClient.Connected) _objClient.GetStream()?.Close();
            }
            catch (Exception ex) { LogError(ex); }
            finally { _objClient.Close(); }
        }

        private void Log(string message) => LogTools.Logger?.Debug(message);
        private void LogError(Exception ex) => LogTools.Logger?.Error(new TcpClientException(ex.Message, ex));
    }
}