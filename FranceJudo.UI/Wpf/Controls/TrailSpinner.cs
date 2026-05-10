using System.Windows;
using System.Windows.Controls;

namespace FranceJudo.UI.Wpf.Controls
{
    public class TrailSpinner : Control
    {
        static TrailSpinner()
        {
            // Indique à WPF de chercher le style par défaut dans Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrailSpinner),
                new FrameworkPropertyMetadata(typeof(TrailSpinner)));
        }

        // Vous pouvez ajouter des propriétés spécifiques si besoin (ex: épaisseur du trait)
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(TrailSpinner), new PropertyMetadata(2.0));

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }
    }
}