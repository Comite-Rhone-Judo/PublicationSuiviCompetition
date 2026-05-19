#nullable enable
using AppPublication.Statistiques;
using Xunit;

namespace AppPublication.Tests.Statistiques
{
    public class StatistiqueItemsTests
    {
        [Fact]
        public void Compteur_EnregistrerValeur_IncrementeDeUn_EtIgnoreLeParametre()
        {
            // Arrange
            StatistiqueItemCompteur compteur = new StatistiqueItemCompteur("TestCpt", "Compteur Test");

            // Act & Assert (xUnit2013 : Expected, Actual)
            Assert.Equal(0, compteur.Valeur); // Initialisé à 0 par le constructeur

            compteur.EnregistrerValeur(); // L'appel vide ajoute +1
            Assert.Equal(1, compteur.Valeur);

            compteur.EnregistrerValeur(999f); // Un appel paramétré ajoute quand même +1 (paramètre ignoré)
            Assert.Equal(2, compteur.Valeur);
            Assert.Equal("TestCpt", compteur.Nom);
            Assert.Equal("Compteur Test", compteur.Libelle);
        }

        [Fact]
        public void Moyenneur_EnregistrerValeur_CalculeMinMaxEtMoyenne()
        {
            // Arrange
            StatistiqueItemMoyenneur moyenneur = new StatistiqueItemMoyenneur("TestMoy", "Moyenneur Test");

            // Act & Assert
            moyenneur.EnregistrerValeur(10f);

            // Premier passage : Min, Max et Moy sont égaux à la valeur
            Assert.Equal(10f, moyenneur.Min);
            Assert.Equal(10f, moyenneur.Max);
            Assert.Equal(10f, moyenneur.Moy);
            Assert.Equal(1f, moyenneur.Valeur); // N éléments = 1

            // Deuxième passage : la moyenne doit s'ajuster
            moyenneur.EnregistrerValeur(20f);
            Assert.Equal(10f, moyenneur.Min);
            Assert.Equal(20f, moyenneur.Max);
            Assert.Equal(15f, moyenneur.Moy); // (10 + 20) / 2
            Assert.Equal(2f, moyenneur.Valeur); // N éléments = 2
        }

        [Fact]
        public void Moyenneur_EnregistrerValeurNulle_EstIgnore()
        {
            // Arrange
            StatistiqueItemMoyenneur moyenneur = new StatistiqueItemMoyenneur("TestMoy", "Moyenneur Test");

            // Act
            moyenneur.EnregistrerValeur(null);

            // Assert
            Assert.Null(moyenneur.Valeur); // Reste à l'état initial
            Assert.Null(moyenneur.Moy);
        }
    }
}