using AppPublication.Controles;
using System.Windows;
using System.Windows.Forms;

namespace AppPublication.IHM.Commissaire
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationEcranAppel.xaml
    /// </summary>
    public partial class ConfigurationEcranAppel : Window
    {
        /// <summary>
        /// Constructeur principal.
        /// </summary>
        /// <param name="dataCtx">Le ViewModel qui gère la logique de cet écran.</param>
        public ConfigurationEcranAppel(ConfigurationEcranAppelViewModel dataCtx)
        {
            InitializeComponent();

            // Définit le DataContext de la fenêtre
            this.DataContext = dataCtx;
        }

        private void ButOk_Click(object sender, RoutedEventArgs e)
        {
            // Ferme la fenêtre et renvoie un résultat positif
            DialogResult = true;
        }
    }
}