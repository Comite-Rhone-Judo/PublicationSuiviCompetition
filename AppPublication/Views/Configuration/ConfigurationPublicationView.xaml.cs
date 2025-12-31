using AppPublication.Controles;
using System.Windows;

namespace AppPublication.Views.Configuration
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationPublication.xaml
    /// </summary>
    public partial class ConfigurationPublicationView : Window
    {
        public ConfigurationPublicationView(GestionSite dataCtx)
        {
            if (dataCtx != null)
            {
                this.DataContext = dataCtx;
            }

            InitializeComponent();
        }

        // TODO Voir pour avoir une partie commune et des parties spécifiques selon le type de publication avec des onglets

        private void ButOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
