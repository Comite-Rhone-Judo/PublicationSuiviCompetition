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
            EcransAppelGrid?.CommitEdit();
            DialogResult = true;
        }
    }
}
