using AppPublication.Controles;
using System.Windows;

namespace AppPublication.Views.Commissaire
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationPublication.xaml
    /// </summary>
    public partial class ConfigurationPublication : Window
    {
        public ConfigurationPublication(GestionSite dataCtx)
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
