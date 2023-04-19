using AppPublication.Controles;
using AppPublication.Export;
using AppPublication.IHM.Server;
using AppPublication.Tools;
using AppPublication.Tools.LectureFile;
using AppPublication.Tools.Struct;
using KernelImpl.Noyau.Organisation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Tools.Enum;
using Tools.Export;
using Tools.Outils;
using Tools.Struct;
using Tools.Windows;

namespace AppPublication.IHM.Commissaire
{
    /// <summary>
    /// Logique d'interaction pour IndividuelleWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window //, ICommissaireWindow
    {

        private ObservableCollection<i_vue_epreuve_interface> _source1 = new ObservableCollection<i_vue_epreuve_interface>();
        private ObservableCollection<Competition> _source2 = new ObservableCollection<Competition>();

        public ExportWindow()
        {
            InitializeComponent();

            NetworkConnecte.DataContext = DialogControleur.Instance;
            NetworkNonConnecte.DataContext = DialogControleur.Instance;
        }

        private void RadMenuItem_Click_1(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {

            //QRCode1.Height = 500;
            string directory = ExportTools.getDirectory(true, null, null);
            using (FileStream fs = new FileStream(directory + "qrcode.png", FileMode.Create))
            {
                // Telerik.Windows.Media.Imaging.ExportExtensions.ExportToImage(QRCode1, fs, new PngBitmapEncoder());
            }

            /*byte[] document = ExportTools.ExportQrCode(QRCode1.Text);
                //DialogControleur.DC.GestionSite.ServerHTTP.IpAddress.ToString(), DialogControleur.DC.GestionSite.ServerHTTP.Port);

            if (document != null)
            {
                PdfViewer viewer = new PdfViewer(document);
                viewer.Show();
            }*/
        }

        private void ButSite_Click_1(object sender, RoutedEventArgs e)
        {
            DialogControleur DC = DialogControleur.Instance;
            try
            {
                string url = "";
                if (DialogControleur.Instance.GestionSite.MiniSiteLocal.IsLocal)
                {
                    url = ExportTools.GetURLSiteLocal(
                         DialogControleur.Instance.GestionSite.MiniSiteLocal.ServerHTTP.ListeningIpAddress.ToString(),
                         DialogControleur.Instance.GestionSite.MiniSiteLocal.ServerHTTP.Port,
                         DialogControleur.Instance.ServerData.competition.remoteId);
                }
                else if (!DialogControleur.Instance.GestionSite.MiniSiteLocal.IsLocal && DialogControleur.Instance.GestionSite.MiniSiteLocal.SiteFTPDistant == NetworkTools.FTP_EJUDO_SUIVI_URL)
                {
                    url = ExportTools.GetURLSiteFTP(DialogControleur.Instance.ServerData.competition.remoteId);
                }
                
                System.Diagnostics.Process.Start(url);
            }
            catch { }
        }

        private void MainWin_Closed_1(object sender, EventArgs e)
        {
            if (DialogControleur.Instance.Connection.Client != null)
            {
                DialogControleur.Instance.Connection.Client.Client.Stop();
            }
            App.Current.Shutdown();
        }

        private void MainWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogParameters param = new DialogParameters();
            param.OkButtonContent = "Oui";
            param.CancelButtonContent = "Non";
            param.Content = "Voulez-vous vraiment fermer l'application ?";
            param.Header = "Fermeture de l'application";

            ConfirmWindow win = new ConfirmWindow(param);
            win.ShowDialog();

            if (win.DialogResult.HasValue && !(bool)win.DialogResult)
            {
                e.Cancel = true;
            }
        }


        private void BoutonFindServer_Click_1(object sender, EventArgs e)
        {
            (new RechercheServer()).ShowDialog();
        }
        #region PRIVATE

        #endregion
    }
}

