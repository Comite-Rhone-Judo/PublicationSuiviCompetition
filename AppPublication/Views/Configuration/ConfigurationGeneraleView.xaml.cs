using AppPublication.Controles;
using HandyControl.Controls;

namespace AppPublication.Views.Configuration
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationPublication.xaml
    /// </summary>
    public partial class ConfigurationGeneraleView : Window
    {
        public ConfigurationGeneraleView(SitePublicationCoordinator dataCtx)
        {
            if (dataCtx != null)
            {
                this.DataContext = dataCtx;
            }

            InitializeComponent();
        }

        private void ButOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
