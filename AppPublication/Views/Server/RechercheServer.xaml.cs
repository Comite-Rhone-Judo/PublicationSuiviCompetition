using JudoClient;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Telerik.Windows.Controls;
using Tools.Enum;
using Tools.Logging;
using Tools.Struct;
using Tools.Outils;

namespace AppPublication.Views.Server
{
    /// <summary>
    /// Logique d'interaction pour RechercheServer.xaml
    /// </summary>
    public partial class RechercheServer : RadWindow, IDisposable
    {
        private delegate void EmptyDelegate();

        RechercheServeurJudo recherche;
        BackgroundWorker recherche_Worker;

        public RechercheServer()
        {
            InitializeComponent();
            OutilsTools.ShowInTaskbar(this);

            recherche_Worker = new BackgroundWorker();
            recherche_Worker.DoWork += new DoWorkEventHandler(recherche_Worker_DoWork);
            recherche_Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(recherche_Worker_RunWorkerCompleted);
            recherche_Worker.WorkerReportsProgress = false;
            recherche_Worker.WorkerSupportsCancellation = true;

            recherche = new RechercheServeurJudo();
            recherche.onServerTrouve += recherche_onServerTrouve;
            recherche.onTermine += recherche_onTermine;
        }

        private void recherche_Worker_DoWork(object sender, DoWorkEventArgs args)
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

        private void recherche_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
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

        void recherche_onTermine(object sender, int pings, int connecte)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Busy.Visibility = System.Windows.Visibility.Collapsed;


                if (recherche.test_recherche > 1 && pings > 0 && connecte == 0)
                {
                    string message = "";
                    message = "La recherche de serveurs n\'a pas abouti. Les causes peuvent être les suivantes :\n";
                    message += "   - L\'application GESTION DES COMPETITION n\'est lancée sur aucune des machines.\n";
                    message += "   - Le pare-feu (windows ou de l'anti-virus comme AVG) bloque les ports " + NetworkTools.PortServerMin + " à " + NetworkTools.PortServerMax + ".\n";
                    message += "   - Le réseau WIFI, sur lequel sont les machines, est paramétré en réseau PUBLIC alors qu'il doit être en réseau PRIVE.\n";
                    string header = "Recherche Serveur";

                    LogTools.Alert(message, header);
                }
            }));
        }



        void recherche_onServerTrouve(object sender, System.Net.IPEndPoint serverEndPoint, string machine, string user, XElement xcompetition)
        {
            string compet = xcompetition.Element(ConstantXML.Competition_Titre).Value;
            string adressSite = xcompetition.Attribute(ConstantXML.AddressSite).Value;
            int portSite = int.Parse(xcompetition.Attribute(ConstantXML.PortSite).Value);

            SaveToLog(
                new ServerFind
                {
                    IEP = serverEndPoint,
                    machine = machine,
                    user = user,
                    competition = compet,
                    addressSite = adressSite,
                    portSite = portSite,
                });
        }


        void SaveToLog(object o)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LB1.Items.Add(o);
            }));
        }

        private void ButSeConnecterServer_Click_1(object sender, RoutedEventArgs e)
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

                Controles.DialogControleur.Instance.Connection.IpAdress = choice.addressSite.ToString();
                Controles.DialogControleur.Instance.Connection.Port = choice.portSite.ToString();
                Controles.DialogControleur.Instance.Connection.Client = new ClientJudo(IEP.Address.ToString(), IEP.Port);

                LB1.Items.Clear();

                recherche_Worker.CancelAsync(); // Demande l'annumation de la tache de recherche

                this.Close();
            }
        }

        private void UI_Closed(object sender, WindowClosedEventArgs e)
        {
            recherche_Worker.Dispose();
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}
