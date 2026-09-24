using AppPublication.Controles;
using HandyControl.Controls;

namespace AppPublication.Views.Configuration
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationPublication.xaml
    /// </summary>
    public partial class ConfigurationPublicationSiteInterneView : Window
    {
        public ConfigurationPublicationSiteInterneView(SitePublicationCoordinator dataCtx)
        {
            if (dataCtx != null)
            {
                this.DataContext = dataCtx;
            }

            InitializeComponent();
        }

        private void ButOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (EcransAppelGrid != null)
            {
                // 1. On valide la cellule en cours (le 'true' force la sortie du mode édition)
                EcransAppelGrid.CommitEdit(System.Windows.Controls.DataGridEditingUnit.Cell, true);

                // 2. On valide la ligne entière pour fermer la transaction globale
                EcransAppelGrid.CommitEdit(System.Windows.Controls.DataGridEditingUnit.Row, true);
            }

            DialogResult = true;
        }
    }
}
