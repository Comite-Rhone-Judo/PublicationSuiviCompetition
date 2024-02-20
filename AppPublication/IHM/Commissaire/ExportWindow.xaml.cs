using AppPublication.Controles;
using AppPublication.IHM.Server;
using KernelImpl.Noyau.Organisation;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Tools.Enum;
using Tools.Export;
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

        private void QRCodeLocalCopy_Click(object sender, RoutedEventArgs e)
        {
            string tmpFile = Path.GetTempFileName();
            using (FileStream fs = new FileStream(tmpFile, FileMode.Create))
            {
                Telerik.Windows.Media.Imaging.ExportExtensions.ExportToImage(QRCodeLocal, fs, new PngBitmapEncoder());
                fs.Close();
            }
            BitmapImage img = new BitmapImage(new Uri(tmpFile));
            Clipboard.SetImage(img);
        }

        private void QRCodeDistantCopy_Click(object sender, RoutedEventArgs e)
        {
            string tmpFile = Path.GetTempFileName();
            using (FileStream fs = new FileStream(tmpFile, FileMode.Create))
            {
                Telerik.Windows.Media.Imaging.ExportExtensions.ExportToImage(QRCodeLocal, fs, new PngBitmapEncoder());
                fs.Close();
            }
            BitmapImage img = new BitmapImage(new Uri(tmpFile));
            Clipboard.SetImage(img);
        }
    }
}

