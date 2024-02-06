using AppPublication.Controles;
using Telerik.Windows.Controls;

namespace AppPublication.IHM.Commissaire
{
    /// <summary>
    /// Logique d'interaction pour Statistiques.xaml
    /// </summary>
    public partial class Statistiques : RadWindow
    {
        public Statistiques(GestionStatistiques statDataContext)
        {
            if (statDataContext != null)
            {
                this.DataContext = statDataContext;
            }

            InitializeComponent();
        }
    }
}
