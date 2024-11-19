using AppPublication.Controles;
using Telerik.Windows.Controls;

namespace AppPublication.IHM.Commissaire
{
    /// <summary>
    /// Logique d'interaction pour Statistiques.xaml
    /// </summary>
    public partial class InformationsView : RadWindow
    {
        public InformationsView()
        {
            this.DataContext = DialogControleur.Instance;

            InitializeComponent();
        }
    }
}
