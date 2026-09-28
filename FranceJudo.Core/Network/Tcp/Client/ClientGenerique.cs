using FranceJudo.Core.Exceptions;
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Tcp.Client
{
    public class ClientGenerique : IClientGenerique, IDisposable
    {
        #region CONSTANTES
        private const int READ_BUFFER_SIZE = 10240;
        private const string EOF_MARKER = "\n<EOF>";
        #endregion

        #region EVENT HANDLERS
        public event OnConnectionHandler OnConnection;
        public event OnDataReceiveHandler OnDataReceive
            ;
        public event OnDataSentHandler OnDataSent;
        public event OnEndConnectionHandler OnEndConnection;
        #endregion

        #region MEMBRES
        private readonly StringBuilder _buffer = new StringBuilder();
        private TcpClient _objClient;
        private CancellationTokenSource _cts;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        private readonly string _endMsgTag;
        private readonly int _port;
        private readonly string _ip;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Obtient l'adresse IP du client
        /// </summary>
        public string IP => _ip;
        /// <summary>
        /// Obtient le port du client
        /// </summary>
        public int Port => _port;
        /// <summary>
        /// Obtient le tag de message de fin
        /// </summary>
        public string EndMsgFlag => _endMsgTag;
        /// <summary>
        /// Obtient le point de terminaison du client
        /// </summary>
        public System.Net.IPEndPoint EndPoint => (System.Net.IPEndPoint)_objClient?.Client?.RemoteEndPoint;

        /// <summary>
        /// Indique si le client est connecté au serveur
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    var client = _objClient;
                    return client?.Client != null && client.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion

        #region CONSTRUCTORS

        public ClientGenerique(string hostNameOrAddress, int port, string endMsgTag)
        {
            _ip = hostNameOrAddress;
            _port = port;
            _endMsgTag = endMsgTag;
        }
        #endregion

        #region METHODES PUBLIQUES

        /// <summary>
        /// Libère les ressources utilisées par le client
        /// </summary>
        public void Dispose()
        {
            Stop();
            _writeLock?.Dispose();
            _cts?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Connecte le client au serveur
        /// </summary>
        public void Connect()
        {
            Stop();

            _cts = new CancellationTokenSource();
            _buffer.Clear();

            _ = Task.Run(() => ConnectInternalAsync(_cts.Token));
        }

        /// <summary>
        /// Arrête la connexion du client
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
            CloseClient();
        }

        /// <summary>
        /// Écrit des données sur le client
        /// </summary>
        /// <param name="data">Les données à écrire</param>
        public void Write(string data)
        {
            if (!IsConnected || string.IsNullOrEmpty(data)) return;

            var token = _cts?.Token ?? CancellationToken.None;
            _ = Task.Run(() => WriteAsync(data, token));
        }

        #endregion

        #region METHODES PRIVEES

        /// <summary>
        /// Conne le client au serveur de manière asynchrone
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ConnectInternalAsync(CancellationToken token)
        {
            _objClient = new TcpClient(AddressFamily.InterNetwork)
            {
                NoDelay = true,
                LingerState = new LingerOption(true, 20)
            };

            try
            {
                await Task.Run(() =>
                {
                    var socket = _objClient.Client;
                    var result = socket.BeginConnect(_ip, _port, null, null);

                    bool success = result.AsyncWaitHandle.WaitOne(1000, true);

                    if (!success || !socket.Connected)
                    {
                        try { socket.Close(); } catch { }
                        throw new TimeoutException($"Connection to {_ip}:{_port} timed out.");
                    }

                    socket.EndConnect(result);
                }, token);

                LogDebug($"Create Client to {_ip}:{_port} \t{DateTime.Now}\t{_objClient.GetHashCode()}");

                if (_objClient.Connected)
                {
                    OnConnection?.Invoke(this);
                    _ = ReadLoopAsync(token);
                }
            }
            catch (OperationCanceledException)
            {
                CloseClient();
            }
            catch (Exception ex)
            {
                CloseClient();
                LogError(ex);
            }
        }

        /// <summary>
        /// Lit les données reçues du client de manière asynchrone
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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

                    _ = Task.Run(() => LogDebug($"Receive\t\t{DateTime.Now}\t{_objClient?.GetHashCode()}\t{strReceiveData}"));

                    lock (_buffer)
                    {
                        _buffer.Append(strReceiveData);
                        string currentContent = _buffer.ToString();

                        if (currentContent.Contains(EOF_MARKER))
                        {
                            ProcessReceivedData(currentContent);
                        }
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
                OnEndConnection?.Invoke(this);
                CloseClient();
                LogDebug($"Connect Closed\t{DateTime.Now}");
            }
        }
        
        /// <summary>
        /// Traite les données reçues du client
        /// </summary>
        /// <param name="content">Le contenu à traiter</param>
        private void ProcessReceivedData(string content)
        {
            int index;
            bool dataProcessed = false;

            while ((index = content.IndexOf(EOF_MARKER)) >= 0)
            {
                string message = content.Substring(0, index);
                content = content.Substring(index + EOF_MARKER.Length);
                dataProcessed = true;

                if (message.EndsWith(_endMsgTag))
                {
                    Task.Run(() => OnDataReceive?.Invoke(this, message));
                }
            }

            if (dataProcessed)
            {
                _buffer.Clear();
                _buffer.Append(content);
            }
        }


        /// <summary>
        /// Écrit des données sur le client de manière asynchrone
        /// </summary>
        /// <param name="data">Les données à écrire</param>
        /// <param name="token">Le jeton d'annulation</param>
        /// <returns></returns>
        private async Task WriteAsync(string data, CancellationToken token)
        {
            await _writeLock.WaitAsync(token);
            try
            {
                string finalMessage = data + EOF_MARKER;
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
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        /// Ferme la connexion avec le client
        /// </summary>
        private void CloseClient()
        {
            var client = Interlocked.Exchange(ref _objClient, null);
            if (client != null)
            {
                try { client.GetStream()?.Close(); } catch { }
                try { client.Close(); } catch { }
                try { client.Dispose(); } catch { }
            }
        }

        private void LogDebug(string message) => LogTools.Logger?.Debug(message);
        private void LogError(Exception ex) => LogTools.Logger?.Error(new TcpClientException(ex.Message, ex));
        #endregion
    }
}