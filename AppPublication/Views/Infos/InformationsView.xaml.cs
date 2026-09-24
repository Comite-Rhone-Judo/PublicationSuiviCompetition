using AppPublication.Controles;
using HandyControl.Controls;


namespace AppPublication.Views.Infos
{
    /// <summary>
    /// Logique d'interaction pour Statistiques.xaml
    /// </summary>
    public partial class InformationsView : Window
    {
        public InformationsView()
        {
            this.DataContext = DialogControleur.Instance;

            InitializeComponent();
        }
    }
}
