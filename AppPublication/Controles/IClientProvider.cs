using AppPublication.Tools.Enum;
using JudoClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Controles
{
    // Event args to pass client reference
    public class ClientReadyEventArgs : EventArgs
    {
        public ClientJudo Client { get; }
        public ClientReadyEventArgs(ClientJudo client)
        {
            Client = client;
        }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public DateTime DisconnectionTime { get; }
        public ClientDisconnectedEventArgs()
        {
            DisconnectionTime = DateTime.Now;
        }
    }

    /// <summary>
    /// EventArgs pour transporter l'état de connexion (pour l'UI)
    /// </summary>
    public class ConnectionStatusEventArgs : EventArgs
    {
        public bool IsBusy { get; }
        public BusyStatusEnum Status { get; }

        public ConnectionStatusEventArgs(bool isBusy, BusyStatusEnum status)
        {
            IsBusy = isBusy;
            Status = status;
        }
    }

    public interface IClientProvider
    {
        ClientJudo Client { get; }

        /// <summary>
        /// Déclenché quand le client est prêt et configuré
        /// </summary>
        event EventHandler<ClientReadyEventArgs> ClientReady;
        
        /// <summary>
        /// Déclenché quand le client se déconnecte
        /// </summary>
        event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        /// <summary>
        /// Déclenché pour notifier les changements d'état de connexion (pour l'UI)
        /// </summary>
        event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;
        void DisposeClient();
    }
}
