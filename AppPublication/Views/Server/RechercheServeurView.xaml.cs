using AppPublication.ViewModels.Server;
using System;
using System.Windows.Input;

namespace AppPublication.Views.Server
{
    public partial class RechercheServeurView : HandyControl.Controls.Window
    {
        private readonly RechercheServeurViewModel _viewModel;

        public RechercheServeurView()
        {
            InitializeComponent();
            _viewModel = new RechercheServeurViewModel();

            // Permet au ViewModel de commander la fermeture de la fenêtre
            _viewModel.OnRequestClose = this.Close;

            this.DataContext = _viewModel;
        }

        private void UI_Closed(object sender, EventArgs e)
        {
            _viewModel?.CancelSearch();
        }

        // --- AJOUTEZ CE BLOC ---
        // Relais purement UI vers la commande du ViewModel (Totalement valide en MVVM)
        private void Serveur_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Vérifie que l'action est autorisée (aucun scan en cours et serveur sélectionné)
            if (_viewModel != null && _viewModel.CmdConnecterServeur.CanExecute(null))
            {
                _viewModel.CmdConnecterServeur.Execute(null);
            }
        }
        // -----------------------
    }
}