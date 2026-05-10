using AppPublication.Models.Statistiques;
using HandyControl.Controls;

namespace AppPublication.Views.Infos
{
    /// <summary>
    /// Logique d'interaction pour Statistiques.xaml
    /// </summary>
    public partial class StatistiquesView : Window
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
