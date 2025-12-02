
using AppPublication.ViewModels;
using System.Windows;

namespace AppPublication.Views.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigurationEcransAppel.xaml
    /// </summary>
    public partial class ConfigurationEcransAppelView : Window
    {
        public ConfigurationEcransAppelView(ConfigurationEcransViewModel dataCtx)
        {
            InitializeComponent();

            // Injection du ViewModel lors de la création de la vue
            this.DataContext = dataCtx;
        }

        private void ButClose_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ConfigurationEcransViewModel;
            vm?.OnClose(); // Arrête les recherches en cours
            this.DialogResult = true;
            this.Close();
        }
    }
}
