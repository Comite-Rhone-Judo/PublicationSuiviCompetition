using FranceJudo.Core.Logging;
using FranceJudo.Core.Network.Tcp.Client;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;
using JudoClient.Communication;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JudoClient
{
    #region CLASSES DE SUPPORT

    /// <summary>
    /// Arguments d'evenement retournant toutes les informations du serveur decouvert.
    /// Remplace l'ancien delegue a 5 parametres pour une signature plus moderne.
    /// </summary>
    public class ServerFoundEventArgs : EventArgs
    {
        public IPEndPoint EndPoint { get; }
        public string Machine { get; }
        public string User { get; }
        public XElement Competition { get; }

        public ServerFoundEventArgs(IPEndPoint endPoint, string machine, string user, XElement competition)
        {
            EndPoint = endPoint;
            Machine = machine;
            User = user;
            Competition = competition;
        }
    }

    #endregion

    /// <summary>
    /// Implementation moderne, asynchrone et Thread-Safe du scanner reseau pour les serveurs Judo.
    /// </summary>
    public class ScannerServeurJudo
    {
        #region MEMBRES

        private readonly List<string> _ipsToScan = new List<string>();

        #endregion

        #region PROPRIETES

        public bool BypassPing { get; set; } = false;
        public int PingTimeoutMs { get; set; } = 1500;
        public int TcpTimeoutMs { get; set; } = 3000;

        // Throttling : Limite de tests simultanes pour ne pas saturer la table ARP / routeur
        public int MaxConcurrentScans { get; set; } = 30;

        #endregion

        #region EVENEMENTS

        public event EventHandler<ServerFoundEventArgs> OnServerFound;

        #endregion

        #region METHODES PUBLIQUES

        /// <summary>
        /// Configure le scanner pour balayer une plage reseau specifique
        /// </summary>
        public void SetNetworkRange(string ipAddress, string subnetMask)
        {
            _ipsToScan.Clear();
            LogTools.Logger?.Debug($"ScannerServeurJudo: Definition de la plage IP basee sur {ipAddress} avec le masque {subnetMask}");

            try
            {
                uint mask = ParseIp(subnetMask);
                uint ip = ParseIp(ipAddress);

                uint first = (ip & mask) + 1;
                uint last = (ip & mask) + ~mask;

                for (uint host = first; host < last; host++)
                {
                    string currentIp = ToIpString(host);
                    _ipsToScan.Add(currentIp);
                    LogTools.Logger?.Debug($"ScannerServeurJudo: Ajout de l'hote {currentIp} a la liste de scan");
                }

                LogTools.Logger?.Debug($"ScannerServeurJudo: Plage calculee - {_ipsToScan.Count} adresses IP a scanner au total");
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Debug($"ScannerServeurJudo: Erreur lors du calcul de la plage IP - {ex.Message}");
            }
        }

        /// <summary>
        /// Configure le scanner avec une liste d'IPs exactes
        /// </summary>
        public void SetExplicitIps(IEnumerable<string> ips)
        {
            _ipsToScan.Clear();
            LogTools.Logger?.Debug("ScannerServeurJudo: Definition explicite des adresses IP a scanner");

            foreach (var ip in ips)
            {
                _ipsToScan.Add(ip);
                LogTools.Logger?.Debug($"ScannerServeurJudo: Ajout de l'hote {ip} a la liste de scan");
            }

            LogTools.Logger?.Debug($"ScannerServeurJudo: Total de {_ipsToScan.Count} adresses IP definies explicitement");
        }

        /// <summary>
        /// Execute l'analyse du reseau de maniere asynchrone et non bloquante.
        /// </summary>
        public async Task<int> ScanAsync(IProgress<int> progress, CancellationToken token)
        {
            int portsPerIp = ConstantNetwork.PortServerMax - ConstantNetwork.PortServerMin + 1;
            int totalTests = _ipsToScan.Count * portsPerIp;
            int testsCompleted = 0;
            int serversFound = 0;

            LogTools.Logger?.Debug($"ScannerServeurJudo: ScanAsync - Demarrage du scan. Total adresses: {_ipsToScan.Count}, Ports par IP: {portsPerIp}, BypassPing: {BypassPing}");

            using var semaphore = new SemaphoreSlim(MaxConcurrentScans, MaxConcurrentScans);
            var tasks = new List<Task>();

            foreach (var ip in _ipsToScan)
            {
                if (token.IsCancellationRequested)
                {
                    LogTools.Logger?.Debug("ScannerServeurJudo: ScanAsync - Scan annule par l'utilisateur.");
                    break;
                }

                tasks.Add(Task.Run(async () =>
                {
                    // LE VERROU EST PLACÉ ICI : Il limite les Pings ET les requêtes TCP (Lissage parfait)
                    await semaphore.WaitAsync(token);
                    try
                    {
                        if (token.IsCancellationRequested) return;

                        if (!BypassPing)
                        {
                            LogTools.Logger?.Debug($"ScannerServeurJudo: ScanAsync - Debut du ping pour l'adresse IP {ip}");
                            bool isAlive = await PingAsync(ip, PingTimeoutMs, token);
                            if (!isAlive)
                            {
                                LogTools.Logger?.Debug($"ScannerServeurJudo: ScanAsync - Echec du ping pour {ip}, abandon.");
                                int c = Interlocked.Add(ref testsCompleted, portsPerIp);
                                progress?.Report((c * 100) / totalTests);
                                return;
                            }
                            LogTools.Logger?.Debug($"ScannerServeurJudo: ScanAsync - Ping reussi pour {ip}, passage au scan TCP");
                        }
                        else
                        {
                            LogTools.Logger?.Debug($"ScannerServeurJudo: ScanAsync - BypassPing actif pour {ip}.");
                        }

                        for (int port = ConstantNetwork.PortServerMin; port <= ConstantNetwork.PortServerMax; port++)
                        {
                            if (token.IsCancellationRequested) return;

                            bool success = await TestJudoServerAsync(ip, port, token);
                            if (success)
                            {
                                Interlocked.Increment(ref serversFound);
                            }

                            int c = Interlocked.Increment(ref testsCompleted);
                            progress?.Report((c * 100) / totalTests);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, token));
            }

            await Task.WhenAll(tasks);
            LogTools.Logger?.Debug($"ScannerServeurJudo: ScanAsync - Scan termine. Serveurs trouves: {serversFound}");
            return serversFound;
        }
        #endregion

        #region METHODES PRIVEES
            
        private async Task<bool> PingAsync(string ip, int timeout, CancellationToken token)
        {
            try
            {
                LogTools.Logger?.Debug($"ScannerServeurJudo: PingAsync - Envoi du ping vers l'adresse {ip} avec timeout de {timeout}ms");
                using var ping = new Ping();

                using var reg = token.Register(() =>
                {
                    try { ping.SendAsyncCancel(); } catch { }
                });

                var reply = await ping.SendPingAsync(ip, timeout);
                LogTools.Logger?.Debug($"ScannerServeurJudo: PingAsync - Resultat pour l'adresse {ip}: Statut = {reply.Status}, RoundtripTime = {reply.RoundtripTime}ms");
                return reply.Status == IPStatus.Success;
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Debug($"ScannerServeurJudo: PingAsync - Exception lors du ping vers l'adresse {ip} - {ex.Message}");
                return false;
            }
        }

        private async Task<bool> TestJudoServerAsync(string ip, int port, CancellationToken token)
        {
            LogTools.Logger?.Debug($"ScannerServeurJudo: Test TCP vers {ip}:{port} demarre...");

            using var ctsTimeout = CancellationTokenSource.CreateLinkedTokenSource(token);
            ctsTimeout.CancelAfter(TcpTimeoutMs);

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var reg = ctsTimeout.Token.Register(() =>
            {
                LogTools.Logger?.Debug($"ScannerServeurJudo: Timeout ou Annulation de la tache pour {ip}:{port}");
                tcs.TrySetResult(false);
            });

            ClientJudo clientJudo = null;

            void onConn(object sender)
            {
                try
                {
                    LogTools.Logger?.Debug($"ScannerServeurJudo: Connexion TCP etablie sur {ip}:{port}, envoi de DemandConnectionTest");
                    clientJudo.DemandConnectionTest();
                }
                catch (Exception ex)
                {
                    LogTools.Logger?.Debug($"ScannerServeurJudo: Erreur lors de DemandConnectionTest sur {ip}:{port} - {ex.Message}");
                }
            }

            void onEnd(object sender)
            {
                LogTools.Logger?.Debug($"ScannerServeurJudo: Deconnexion distante inattendue sur {ip}:{port}");
                tcs.TrySetResult(false);
            }

            void OnAccept(object sender, XElement xvaleur)
            {
                try
                {
                    string machine = xvaleur.Attribute(ConstantXML.Machine)?.Value ?? string.Empty;
                    string user = xvaleur.Attribute(ConstantXML.User)?.Value ?? string.Empty;
                    XElement xcomp = xvaleur.Element(ConstantXML.Competition);

                    LogTools.Logger?.Debug($"ScannerServeurJudo: Serveur valide trouve sur {ip}:{port} (Machine: {machine})");
                    OnServerFound?.Invoke(this, new ServerFoundEventArgs(new IPEndPoint(IPAddress.Parse(ip), port), machine, user, xcomp));
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    LogTools.Logger?.Debug($"ScannerServeurJudo: Erreur lors de la lecture du XML de reponse sur {ip}:{port} - {ex.Message}");
                    tcs.TrySetResult(false);
                }
            }

            try
            {
                clientJudo = new ClientJudo(ip, port);

                clientJudo.NetworkClient.OnConnection += onConn;
                clientJudo.NetworkClient.OnEndConnection += onEnd;
                clientJudo.TraitementConnexion.OnAcceptConnectionTest += OnAccept;

                if (clientJudo.IsConnected)
                {
                    onConn(clientJudo.NetworkClient);
                }

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Debug($"ScannerServeurJudo: Echec de creation/connexion du ClientJudo vers {ip}:{port} - {ex.Message}");
                return false;
            }
            finally
            {
                if (clientJudo != null)
                {
                    clientJudo.NetworkClient.OnConnection -= onConn;
                    clientJudo.NetworkClient.OnEndConnection -= onEnd;
                    clientJudo.TraitementConnexion.OnAcceptConnectionTest -= OnAccept;
                    clientJudo.Dispose();
                }
            }
        }

        private uint ParseIp(string ipAddress)
        {
            var splitted = ipAddress.Split('.');
            uint ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + uint.Parse(splitted[i]);
            }
            return ip;
        }

        private string ToIpString(uint value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = (value & bitmask) >> ((3 - i) * 8);
                bitmask >>= 8;
                parts[i] = masked.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return string.Join(".", parts);
        }

        #endregion
    }
}