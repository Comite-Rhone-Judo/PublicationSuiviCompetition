using AppPublication.ViewModels.Configuration;
using System.Windows;

namespace AppPublication.Views.Configuration
{
    public partial class NetworkScannerView : Window
    {
        public NetworkScannerView()
        {
            InitializeComponent();
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as NetworkScannerViewModel;
            if (vm != null && vm.SelectedDevice != null)
            {
                // Confirme la sélection
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un appareil dans la liste.", "Sélection requise", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            // Annule la sélection
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Sécurité : Si on ferme la fenêtre via la croix rouge (X) pendant un scan,
            // on s'assure que le CancellationToken coupe bien la tâche en arrière-plan.
            var vm = DataContext as NetworkScannerViewModel;
            vm?.AnnulerRecherche();
        }
    }
}