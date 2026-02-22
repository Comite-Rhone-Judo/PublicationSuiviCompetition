using AppPublication.Controles;
using AppPublication.ViewModels.Configuration;
using System.Windows;

namespace AppPublication.Views.Configuration
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationPublication.xaml
    /// </summary>
    public partial class ConfigurationPublicationView : Window
    {
        public ConfigurationPublicationView(SitePublicationCoordinator dataCtx)
        {
            if (dataCtx != null)
            {
                this.DataContext = dataCtx;
            }

            InitializeComponent();
        }

        private void ButOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
