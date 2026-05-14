#nullable enable
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FranceJudo.Core.Logging;

namespace FranceJudo.Core.Network.Udp
{
    public class ServerUDP : IDisposable
    {
        private readonly int _listenPort;
        private UdpClient? _listener;
        private CancellationTokenSource? _cts;
        private Task? _listenTask;

        public event EventHandler<string>? OnDataReceive;

        public ServerUDP(int port)
        {
            _listenPort = port;
        }

        public void Start()
        {
            if (_listener != null) return;

            _cts = new CancellationTokenSource();
            _listener = new UdpClient(_listenPort);

            // On lance la tâche. Note : on ne passe plus le token à Task.Run 
            // pour éviter que le framework ne marque la tâche comme "Canceled" 
            // avant même qu'elle ne démarre dans certains cas de race condition.
            _listenTask = Task.Run(() => StartListenerAsync(_cts.Token));
            LogTools.Logger?.Info($"[UDP SERVER] Démarré sur le port {_listenPort}");
        }

        public void Stop()
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                _cts.Cancel();

                // Fermer le socket débloque immédiatement ReceiveAsync() en .NET Standard 2.0
                _listener?.Close();

                if (_listenTask != null)
                {
                    // CORRECTION : On entoure le Wait d'un try/catch car Wait propage 
                    // l'annulation sous forme d'AggregateException.
                    try
                    {
                        // On laisse 500ms à la boucle pour se terminer proprement
                        _listenTask.Wait(500);
                    }
                    catch (AggregateException ex)
                    {
                        // On utilise Handle pour ignorer proprement les exceptions attendues lors d'un arrêt
                        ex.Handle(e => e is OperationCanceledException
                                    || e is ObjectDisposedException
                                    || e is SocketException);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex, "[UDP SERVER] Erreur lors de l'arrêt");
            }
            finally
            {
                _listener?.Dispose();
                _listener = null;
                _cts?.Dispose();
                _cts = null;
                _listenTask = null;
            }
        }

        private async Task StartListenerAsync(CancellationToken token)
        {
            if (_listener == null) return;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // ReceiveAsync() sans paramètre pour .NET Standard 2.0
                    UdpReceiveResult result = await _listener.ReceiveAsync();

                    string message = Encoding.UTF8.GetString(result.Buffer);
                    OnDataReceive?.Invoke(this, message);
                }
            }
            catch (Exception) when (token.IsCancellationRequested)
            {
                // Si on a annulé, toute exception (ObjectDisposed, SocketException) est un arrêt normal.
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex, "[UDP SERVER] Erreur critique dans la boucle de réception");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}