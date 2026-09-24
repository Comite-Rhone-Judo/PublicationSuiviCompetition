#nullable enable
using AppPublication.Models.EcransAppel;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppPublication.Tests.Models.EcransAppel
{
    public class EcranCollectionSnapshotTests
    {
        [Fact]
        public void Constructeur_IsoleLaListeOriginale()
        {
            // Arrange
            EcranAppelModel ecranDefaut = new EcranAppelModel();
            List<EcranAppelModel> listeOriginale = new List<EcranAppelModel>
            {
                new EcranAppelModel { Id = 1 },
                new EcranAppelModel { Id = 2 }
            };

            // Act
            EcranCollectionSnapshot snapshot = new EcranCollectionSnapshot(listeOriginale, ecranDefaut);

            // On modifie la source après avoir pris le snapshot
            listeOriginale.Add(new EcranAppelModel { Id = 3 });

            // Assert
            Assert.Same(ecranDefaut, snapshot.Default);
            Assert.Equal(2, snapshot.Ecrans.Count); // Le snapshot doit rester figé à 2 éléments
            Assert.Equal(1, snapshot.Ecrans[0].Id);
        }
    }
}