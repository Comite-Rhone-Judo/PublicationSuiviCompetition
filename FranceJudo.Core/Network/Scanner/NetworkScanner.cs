using FranceJudo.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FranceJudo.Core.Network.Scanner
{
    public static class NetworkScanner
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        private static readonly SemaphoreSlim _throttler = new SemaphoreSlim(256, 256);

        /// <summary>
        /// Lance un scan complet du reseau sur l'interface specifiee avec possibilite d'annulation.
        /// </summary>
        public static async Task ScanNetworkAsync(NetworkInterface netInterface, IProgress<NetworkDevice> progress, CancellationToken cancellationToken)
        {
            LogTools.Logger?.Debug("Demarrage du scan reseau...");

            var ipInfo = netInterface.GetIPProperties().UnicastAddresses
                .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);

            if (ipInfo == null)
            {
                LogTools.Logger?.Error("L'interface selectionnee n'a pas d'adresse IPv4 valide.");
                throw new InvalidOperationException("L'interface selectionnee n'a pas d'adresse IPv4.");
            }

            LogTools.Logger?.Debug($"Interface analysee. Adresse IP locale : {ipInfo.Address}, Masque : {ipInfo.IPv4Mask}");

            // 1. Decouverte UPnP
            LogTools.Logger?.Debug("Recherche des equipements UPnP (Multicast) en cours...");
            HashSet<string> upnpDevices = await DiscoverUpnpDevicesAsync(cancellationToken);
            LogTools.Logger?.Debug($"{upnpDevices.Count} equipement(s) UPnP trouve(s) sur le reseau.");

            if (cancellationToken.IsCancellationRequested)
            {
                LogTools.Logger?.Debug("Scan annule par l'utilisateur apres l'etape UPnP.");
                return;
            }

            // 2. Recuperation des IP a scanner
            IEnumerable<string> ipsToScan = NetworkCalculator.GetUsableIps(ipInfo);
            List<Task> scanTasks = new List<Task>();

            LogTools.Logger?.Debug("Lancement du scan Ping et Ports sur la plage d'adresses IP...");

            // 3. Lancement des taches
            foreach (string ip in ipsToScan)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    LogTools.Logger?.Debug("Arret de la creation des taches de scan (Annulation demandee).");
                    break;
                }

                scanTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await _throttler.WaitAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }

                    try
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await AnalyzeSingleIpAsync(ip, upnpDevices, progress, cancellationToken);
                        }
                    }
                    finally
                    {
                        _throttler.Release();
                    }
                }, cancellationToken));
            }

            try
            {
                await Task.WhenAll(scanTasks);
                if (!cancellationToken.IsCancellationRequested)
                {
                    LogTools.Logger?.Debug("Scan reseau termine avec succes.");
                }
            }
            catch (OperationCanceledException)
            {
                LogTools.Logger?.Debug("Taches de scan interrompues (Annulation globale).");
            }
        }

        /// <summary>
        /// Analyse Async d'une adresse IP pour determiner son type, etc.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="upnpDevices"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task AnalyzeSingleIpAsync(string ip, HashSet<string> upnpDevices, IProgress<NetworkDevice> progress, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested) return;

                bool isOnline = false;
                DeviceType type = DeviceType.GenericNetworkDevice;

                // 1. COURT-CIRCUIT UPNP : Si déjà détecté, pas besoin de ping ni de scan de ports !
                if (upnpDevices.Contains(ip))
                {
                    isOnline = true;
                    type = DeviceType.SmartTvOrStreaming;
                    LogTools.Logger?.Debug($"Appareil identifie via UPnP (sans Ping) pour l'IP : {ip}");
                }
                else
                {
                    // 2. SINON : On tente un Ping classique
                    using Ping ping = new Ping();
                    PingReply reply = await ping.SendPingAsync(ip, 800);

                    if (cancellationToken.IsCancellationRequested) return;

                    if (reply.Status == IPStatus.Success)
                    {
                        isOnline = true;
                        LogTools.Logger?.Debug($"Reponse au ping recue pour l'IP : {ip}");

                        // On détermine le type en scannant les ports
                        type = await DetermineDeviceCategoryAsync(ip, cancellationToken);
                    }
                }

                // 3. Si l'appareil est en ligne (via UPnP OU Ping), on finalise
                if (isOnline)
                {
                    string mac = GetMacAddress(ip);

                    string hostname = "N/A";
                    if (type == DeviceType.WindowsPc || type == DeviceType.Mac || type == DeviceType.LinuxOrServer)
                    {
                        hostname = await ResolveHostnameAsync(ip, cancellationToken);
                    }

                    var device = new NetworkDevice
                    {
                        IpAddress = ip,
                        Hostname = hostname,
                        MacAddress = mac,
                        Category = type
                    };

                    LogTools.Logger?.Debug($"Nouvel appareil detecte - IP: {ip} | Hostname: {hostname} | Type: {type}");

                    progress?.Report(device);
                }
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Debug($"Aucune reponse ou erreur lors de l'analyse de l'IP {ip} : {ex.Message}");
            }
        }

        // --- NOUVELLE MÉTHODE ---
        /// <summary>
        /// Efectue une résolution DNS Async avec Timeout
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        private static async Task<string> ResolveHostnameAsync(string ip, CancellationToken cancellationToken, int timeoutMs = 1500)
        {
            if (cancellationToken.IsCancellationRequested) return "Annulé";

            try
            {
                // La résolution DNS peut bloquer, on l'isole donc dans un Task.Run avec un Timeout
                var resolveTask = Task.Run(() => Dns.GetHostEntry(ip).HostName, cancellationToken);

                if (await Task.WhenAny(resolveTask, Task.Delay(timeoutMs, cancellationToken)) == resolveTask)
                {
                    return await resolveTask;
                }

                LogTools.Logger?.Debug($"Timeout DNS inverse pour l'IP {ip}");
                return "Inconnu";
            }
            catch (Exception)
            {
                // Échec normal si l'appareil n'a pas de nom enregistré dans le routeur/DNS local
                return "Inconnu";
            }
        }

        /// <summary>
        /// Execute une recherche UPNP Async pour trouver les devices multimedia
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        private static async Task<HashSet<string>> DiscoverUpnpDevicesAsync(CancellationToken cancellationToken, int timeoutMs = 2000)
        {
            var discoveredIps = new HashSet<string>();
            string request = "M-SEARCH * HTTP/1.1\r\n" +
                             "HOST: 239.255.255.250:1900\r\n" +
                             "MAN: \"ssdp:discover\"\r\n" +
                             "MX: 1\r\n" +
                             "ST: ssdp:all\r\n\r\n";

            byte[] reqBytes = Encoding.UTF8.GetBytes(request);

            using var udpClient = new UdpClient();
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
                await udpClient.SendAsync(reqBytes, reqBytes.Length, endpoint);

                var receiveTask = Task.Run(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var result = await udpClient.ReceiveAsync();
                        string ip = result.RemoteEndPoint.Address.ToString();

                        if (discoveredIps.Add(ip))
                        {
                            LogTools.Logger?.Debug($"Reponse UPnP (SSDP) recue depuis l'adresse : {ip}");
                        }
                    }
                }, cancellationToken);

                await Task.WhenAny(receiveTask, Task.Delay(timeoutMs, cancellationToken));
            }
            catch (TaskCanceledException)
            {
                LogTools.Logger?.Debug("Decouverte UPnP interrompue par l'utilisateur.");
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex, "Erreur inattendue lors de l'execution de la decouverte UPnP multicast.");
            }

            return discoveredIps;
        }

        /// <summary>
        /// Determine la classificatio  d'un device
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<DeviceType> DetermineDeviceCategoryAsync(string ip, CancellationToken cancellationToken)
        {
            if (await IsPortOpenAsync(ip, 445, cancellationToken) || await IsPortOpenAsync(ip, 135, cancellationToken)) return DeviceType.WindowsPc;
            if (await IsPortOpenAsync(ip, 548, cancellationToken) || await IsPortOpenAsync(ip, 5900, cancellationToken)) return DeviceType.Mac;
            if (await IsPortOpenAsync(ip, 22, cancellationToken)) return DeviceType.LinuxOrServer;
            if (await IsPortOpenAsync(ip, 8008, cancellationToken) || await IsPortOpenAsync(ip, 3000, cancellationToken) || await IsPortOpenAsync(ip, 8001, cancellationToken)) return DeviceType.SmartTvOrStreaming;

            return DeviceType.GenericNetworkDevice;
        }

        /// <summary>
        /// Verifie si un port est ouvert
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        private static async Task<bool> IsPortOpenAsync(string ip, int port, CancellationToken cancellationToken, int timeoutMs = 400)
        {
            if (cancellationToken.IsCancellationRequested) return false;

            try
            {
                using TcpClient client = new TcpClient();
                Task connectTask = client.ConnectAsync(ip, port);

                if (await Task.WhenAny(connectTask, Task.Delay(timeoutMs, cancellationToken)) == connectTask)
                {
                    return client.Connected;
                }
                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// REcherche l'adresse MAC d'une adresse IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private static string GetMacAddress(string ipAddress)
        {
            try
            {
                IPAddress dst = IPAddress.Parse(ipAddress);
                byte[] macAddr = new byte[6];
                uint macAddrLen = (uint)macAddr.Length;

                if (SendARP(BitConverter.ToInt32(dst.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) != 0)
                {
                    LogTools.Logger?.Debug($"Resolution ARP echouee pour l'IP {ipAddress} (appareil hors LAN ou protege par pare-feu)");
                    return "Inconnue";
                }

                string[] str = new string[(int)macAddrLen];
                for (int i = 0; i < macAddrLen; i++)
                    str[i] = macAddr[i].ToString("X2");

                return string.Join(":", str);
            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex, $"Erreur critique lors de la recuperation MAC pour l'IP {ipAddress}");
                return "Erreur MAC";
            }
        }
    }
}