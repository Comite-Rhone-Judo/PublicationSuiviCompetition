using AppPublication.Controles;
using AppPublication.Views.Server;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.UI.Wpf.Behaviors;
using HandyControl.Controls;
using System;
using System.Collections.ObjectModel;



namespace AppPublication.Views.Main
{
    /// <summary>
    /// Logique d'interaction pour IndividuelleWindow.xaml
    /// </summary>
    public partial class MainView : Window //, ICommissaireWindow
    {

        readonly private ObservableCollection<i_vue_epreuve_interface> _source1 = new ObservableCollection<i_vue_epreuve_interface>();
        readonly private ObservableCollection<ICompetition> _source2 = new ObservableCollection<ICompetition>();

        public MainView()
        {
            InitializeComponent();

            NetworkConnecte.DataContext = DialogControleur.Instance;
            NetworkNonConnecte.DataContext = DialogControleur.Instance;
        }

        private void MainWin_Closed_1(object sender, EventArgs e)
        {
            DialogControleur.Instance.Connection.Client?.NetworkClient.Stop();

            App.Current.Shutdown();
        }

        private void MainWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 1. Instanciation de votre fenêtre personnalisée avec le titre et le message
            var confirmDialog = new FranceJudo.UI.Wpf.Dialogs.ConfirmWindow(
                "Fermeture de l'application",
                "Voulez-vous vraiment fermer l'application ?"
            );

            // 2. On lie la boîte de dialogue à la fenêtre principale. 
            // Cela empêche l'utilisateur de cliquer derrière et garantit le centrage.
            confirmDialog.Owner = this;

            // 3. Affichage modal (bloque le code ici tant que l'utilisateur n'a pas répondu)
            bool? result = confirmDialog.ShowDialog();

            // 4. Vérification du résultat. 
            // Votre ConfirmWindow met DialogResult à true uniquement sur le bouton OK.
            if (result != true)
            {
                e.Cancel = true; // On annule la fermeture
            }
        }

        private void BoutonFindServer_Click_1(object sender, EventArgs e)
        {
            (new RechercheServer()).ShowDialog();
        }

        private void QRCodeLocalCopy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowHelper.CopyVisualToClipboard(QRCodeLocalImage);
        }

        private void QRCodeEcransAppelCopy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowHelper.CopyVisualToClipboard(QRCodeEcransAppelImage);
        }

        private void QRCodeDistantCopy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowHelper.CopyVisualToClipboard(QRCodeDistantImage);
        }

        private void Window_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // On doit configurer les mots de passe par defaut ici car le composant Password ne supporte pas le Binding sur cette propriete
            if (e.NewValue != null && e.NewValue.GetType() == typeof(DialogControleur))
            {
                DialogControleur dc = (DialogControleur)e.NewValue;
                AdvancedPwd.Password = dc.SiteCoordinator.GestionnaireSitePublique.SiteDistant.PasswordSiteFTPDistant;
                EasyConfigPwd.Password = dc.SiteCoordinator.GestionnaireSitePublique.SiteFranceJudo.PasswordSiteFTPDistant;
            }
        }
    }
}

