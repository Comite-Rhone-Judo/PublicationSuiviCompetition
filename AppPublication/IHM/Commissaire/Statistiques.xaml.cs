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
using Telerik.Windows.Controls;
using AppPublication.Controles;

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
