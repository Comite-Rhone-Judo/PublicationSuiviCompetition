using AppPublication.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            this.DialogResult = true;
            this.Close();
        }
    }
}
