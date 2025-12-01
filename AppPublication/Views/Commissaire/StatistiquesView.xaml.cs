using AppPublication.Controles;
using Telerik.Windows.Controls;

namespace AppPublication.Views.Commissaire
{
    /// <summary>
    /// Logique d'interaction pour Statistiques.xaml
    /// </summary>
    public partial class StatistiquesView : RadWindow
    {
        public StatistiquesView(GestionStatistiques statDataContext)
        {
            if (statDataContext != null)
            {
                this.DataContext = statDataContext;
            }

            InitializeComponent();
        }
    }
}
