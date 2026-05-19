#nullable enable
using AppPublication.Models.Statistiques;
using AppPublication.Statistiques;
using System.Reflection;
using Xunit;

namespace AppPublication.Tests.Models.Statistiques
{
    public class GestionStatistiquesTests
    {
        [Fact]
        public void Constructeur_InitialiseToutesLesProprietes_SansException()
        {
            // Act
            GestionStatistiques statistiques = new GestionStatistiques();

            // Assert
            // On vérifie que les 4 modules statistiques sont bien alloués en mémoire
            Assert.NotNull(statistiques.GenerationSite);
            Assert.NotNull(statistiques.GenerationSiteInterne);
            Assert.NotNull(statistiques.Synchronisation);
            Assert.NotNull(statistiques.Donnees);
        }

        [Fact]
        public void SettersPrives_DeclenchentINotifyPropertyChanged()
        {
            // Arrange
            GestionStatistiques statistiques = new GestionStatistiques();
            bool notificationRecue = false;

            // On s'abonne à l'événement de NotificationBase
            statistiques.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(GestionStatistiques.Donnees))
                {
                    notificationRecue = true;
                }
            };

            // Act
            // Le setter étant "private", le seul moyen de tester l'intégration de NotificationBase 
            // de l'extérieur pour garantir les 100% de couverture de la branche "set" est d'utiliser la Réflexion.
            PropertyInfo? proprieteDonnees = typeof(GestionStatistiques).GetProperty(nameof(GestionStatistiques.Donnees));

            StatMgrDonnees nouveauGestionnaire = new StatMgrDonnees();

            // On force l'appel au setter privé (ce qui déclenchera la ligne : { _donnees = value; NotifyPropertyChanged(); })
            proprieteDonnees?.SetValue(statistiques, nouveauGestionnaire, null);

            // Assert (xUnit2013 : Expected, Actual)
            // On s'assure que le mécanisme d'UI Binding de WPF recevra bien le signal
            Assert.True(notificationRecue);
            Assert.Same(nouveauGestionnaire, statistiques.Donnees);
        }
    }
}