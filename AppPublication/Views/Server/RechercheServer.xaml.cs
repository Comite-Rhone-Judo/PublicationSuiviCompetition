using FranceJudo.Core.Logging;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;
using JudoClient;
using HandyControl.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;


namespace AppPublication.Views.Server
{
    /// <summary>
    /// Logique d'interaction pour RechercheServer.xaml
    /// </summary>
    public partial class RechercheServer : Window, IDisposable
    {
        private delegate void EmptyDelegate();

        readonly RechercheServeurJudo recherche;
        readonly BackgroundWorker recherche_Worker;

        public RechercheServer()
        {
            InitializeComponent();

            recherche_Worker = new BackgroundWorker();
            recherche_Worker.DoWork += new DoWorkEventHandler(RechercheWorkerDoWork);
            recherche_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RechercheWorkerRunWorkerCompleted);
            recherche_Worker.WorkerReportsProgress = false;
            recherche_Worker.WorkerSupportsCancellation = true;

            recherche = new RechercheServeurJudo();
            recherche.OnServerTrouve += RechercheOnServerTrouve;
            recherche.OnTermine += RechercheOnTermine;
        }

        private void RechercheWorkerDoWork(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string text = ((args.Argument as object[]).ElementAt(0) as string);

            if (worker.CancellationPending)
            {
                args.Cancel = true;
                return;
            }
            recherche.DemarreRechecherche(text, worker);
        }

        private void RechercheWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (args.Cancelled)
            {
                recherche_Worker.RunWorkerAsync(new object[] { TextIpAdress.Text });
            }
            else
            {
                recherche_Worker.Dispose();
            }
        }

        void RechercheOnTermine(object sender, int pings, int connecte)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Busy.Visibility = System.Windows.Visibility.Collapsed;


                if (recherche.test_recherche > 1 && pings > 0 && connecte == 0)
                {
                    string message = "";
                    message = "La recherche de serveurs n\'a pas abouti. Les causes peuvent être les suivantes :\n";
                    message += "   - L\'application GESTION DES COMPETITION n\'est lancée sur aucune des machines.\n";
                    message += "   - Le pare-feu (windows ou de l'anti-virus comme AVG) bloque les ports " + ConstantNetwork.PortServerMin + " à " + ConstantNetwork.PortServerMax + ".\n";
                    message += "   - Le réseau WIFI, sur lequel sont les machines, est paramétré en réseau PUBLIC alors qu'il doit être en réseau PRIVE.\n";

                    LogTools.Alert(message);
                    LogTools.Logger?.Error(message);


                }
            }));
        }



        void RechercheOnServerTrouve(object sender, System.Net.IPEndPoint serverEndPoint, string machine, string user, XElement xcompetition)
        {
            string compet = xcompetition.Element(ConstantXML.Competition_Titre).Value;
            string adressSite = xcompetition.Attribute(ConstantXML.AddressSite).Value;
            int portSite = int.Parse(xcompetition.Attribute(ConstantXML.PortSite).Value);

            SaveToLog(
                new ServerFind
                {
                    IEP = serverEndPoint,
                    Machine = machine,
                    User = user,
                    Competition = compet,
                    AddresseSite = adressSite,
                    PortSite = portSite,
                });
        }


        void SaveToLog(object o)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LB1.Items.Add(o);
            }));
        }

        private void ButSeConnecterServer_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Busy.Visibility = System.Windows.Visibility.Visible;

            if (recherche_Worker.IsBusy)
            {
                recherche_Worker.CancelAsync();
            }
            else
            {
                recherche_Worker.RunWorkerAsync(new object[] { TextIpAdress.Text });
            }

            //int nbserver = recherche.DemareRechecherche(TextIpAdress.Text);
        }


        public static void DoEvents()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new EmptyDelegate(delegate { }));
        }

        private void LB1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LB1.SelectedValue != null)
            {
                ServerFind choice = (ServerFind)LB1.SelectedValue;

                System.Net.IPEndPoint IEP = choice.IEP;

                Controles.DialogControleur.Instance.Connection.IpAdress = choice.AddresseSite.ToString();
                Controles.DialogControleur.Instance.Connection.Port = choice.PortSite.ToString();
                Controles.DialogControleur.Instance.Connection.Client = new ClientJudo(IEP.Address.ToString(), IEP.Port);

                LB1.Items.Clear();

                recherche_Worker.CancelAsync(); // Demande l'annumation de la tache de recherche

                this.Close();
            }
        }

        private void UI_Closed(object sender, System.EventArgs e)
        {
            // Appelle la méthode Dispose correctement
            Dispose();
        }

        // --- DEBUT DU PATTERN IDISPOSABLE (Résout CA1816) ---

        private bool _disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // 1. Libérer les ressources managées (objets implémentant IDisposable)
                    if (recherche_Worker != null)
                    {
                        // On se désabonne des événements pour éviter les fuites de mémoire
                        recherche_Worker.DoWork -= RechercheWorkerDoWork;
                        recherche_Worker.RunWorkerCompleted -= RechercheWorkerRunWorkerCompleted;

                        // On libère le worker
                        recherche_Worker.Dispose();
                    }

                    // 2. Nettoyer les autres événements
                    if (recherche != null)
                    {
                        recherche.OnServerTrouve -= RechercheOnServerTrouve;
                        recherche.OnTermine -= RechercheOnTermine;
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'.
            Dispose(disposing: true);

            // C'est CETTE ligne qui supprime l'avertissement CA1816 :
            // Elle dit au Garbage Collector qu'il n'a pas besoin d'appeler le destructeur (finaliseur) 
            // de cette classe car nous avons déjà fait le ménage manuellement.
            GC.SuppressFinalize(this);
        }
    }
}