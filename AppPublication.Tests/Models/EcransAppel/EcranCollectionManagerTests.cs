#nullable enable
using AppPublication.Models.EcransAppel;
using Xunit;

namespace AppPublication.Tests.Models.EcransAppel
{
    public class EcranCollectionManagerTests
    {
        [Fact]
        public void Constructeur_InitialiseLesCompteursAZero()
        {
            // Act
            EcranCollectionManager manager = new EcranCollectionManager();

            // Assert (xUnit2013 : Expected, Actual)
            Assert.Empty(manager.Ecrans);
            Assert.Equal(0, manager.LastId);
            Assert.Equal(1, manager.NextId);
        }

        [Fact]
        public void Add_SansParametre_CreeEcranEtIncrementeId()
        {
            // Arrange
            EcranCollectionManager manager = new EcranCollectionManager();

            // Act
            EcranAppelModel nouvelEcran = manager.Add();

            // Assert
            Assert.Single(manager.Ecrans);
            Assert.Equal(1, nouvelEcran.Id);
            Assert.Equal(1, manager.LastId);
            Assert.Equal(2, manager.NextId);
        }

        [Fact]
        public void Add_AvecParametre_GereLaCollisionDId()
        {
            // Arrange
            EcranCollectionManager manager = new EcranCollectionManager();
            EcranAppelModel ecran = new EcranAppelModel { Id = 1 };
            manager.Add(ecran); // ID 1 est pris

            EcranAppelModel collision = new EcranAppelModel { Id = 1 };

            // Act
            manager.Add(collision); // Le manager doit changer l'ID de collision

            // Assert
            Assert.Equal(2, manager.Ecrans.Count);
            Assert.Equal(2, collision.Id); // ID mis à jour
            Assert.Equal(2, manager.LastId);
        }

        [Fact]
        public void Remove_ParId_RecalculeLeHighWatermark()
        {
            // Arrange
            EcranCollectionManager manager = new EcranCollectionManager();

            // Utilisation de la méthode Add() sans paramètres pour garantir la gestion des IDs (1 et 2)
            manager.Add();
            manager.Add();

            // Act
            manager.Remove(2);

            // Assert
            // Assert.Single vérifie qu'il reste bien exactement 1 élément
            Assert.Single(manager.Ecrans);

            // Vérification de l'état du manager
            Assert.Equal(1, manager.LastId); // Le High Watermark redescend à 1
            Assert.Equal(2, manager.NextId); // Le prochain ID reste 2
        }

        [Fact]
        public void NbTapis_Setter_MetAJourLeDefault()
        {
            // Arrange
            EcranCollectionManager manager = new EcranCollectionManager
            {
                // Act
                NbTapis = 4
            };

            // Assert
            Assert.Equal(4, manager.Default.TapisIds.Count);
            Assert.Equal(4, manager.Default.TapisIds[3]); // Le 4ème tapis existe
        }

        [Fact]
        public void Snapshot_RetourneLeMemeInstance_PuisNouvelleApresInvalidation()
        {
            // Arrange
            EcranCollectionManager manager = new EcranCollectionManager();

            // Act
            EcranCollectionSnapshot snap1 = manager.Snapshot;
            EcranCollectionSnapshot snap2 = manager.Snapshot;

            // Assert
            Assert.Same(snap1, snap2); // Même instance de cache

            // Invalidation
            manager.InvalidateSnapshot();
            EcranCollectionSnapshot snap3 = manager.Snapshot;

            Assert.NotSame(snap1, snap3); // Instance différente après invalidation
        }
    }
}