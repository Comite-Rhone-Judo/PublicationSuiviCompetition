#nullable enable
using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using KernelImpl.Internal;

namespace KernelImpl.Tests.Internal
{
    public class DeduplicatedDataCacheTests
    {
        // Objet de test
        public class DummyItem
        {
            public int Id { get; set; }
            public string Value { get; set; } = string.Empty;
        }

        [Fact]
        public void Constructeur_InitialiseAvecUneListeVide()
        {
            var cache = new DeduplicatedCachedData<int, DummyItem>();
            cache.Cache.Should().NotBeNull();
            cache.Cache.Should().BeEmpty();
        }

        [Fact]
        public void UpdateFullSnapshot_ListeVideOuNull_VideLeCache()
        {
            // Arrange
            var cache = new DeduplicatedCachedData<int, DummyItem>();
            cache.UpdateFullSnapshot(new[] { new DummyItem { Id = 1, Value = "A" } }, x => x.Id);

            // Act
            cache.UpdateFullSnapshot(null!, x => x.Id);

            // Assert
            cache.Cache.Should().BeEmpty("Un snapshot null doit réinitialiser le cache avec une liste vide.");
        }

        [Fact]
        public void UpdateFullSnapshot_AvecDoublons_DedupliqueEnGardantLeDernier()
        {
            // Arrange
            var cache = new DeduplicatedCachedData<int, DummyItem>();
            var items = new List<DummyItem>
            {
                new DummyItem { Id = 1, Value = "Ancien" },
                new DummyItem { Id = 2, Value = "Unique" },
                new DummyItem { Id = 1, Value = "Nouveau" } // Doublon sur l'Id 1
            };

            // Act
            cache.UpdateFullSnapshot(items, x => x.Id);

            // Assert
            var result = cache.Cache;
            result.Should().HaveCount(2, "Les éléments ayant la même clé doivent être fusionnés.");

            // Le dictionnaire écrase l'ancienne valeur avec la nouvelle (Last wins)
            result.Should().ContainSingle(x => x.Id == 1 && x.Value == "Nouveau");
            result.Should().ContainSingle(x => x.Id == 2);
        }

        [Fact]
        public void UpdateDifferentialSnapshot_MetAJourEtAjouteLesElements()
        {
            // Arrange
            var cache = new DeduplicatedCachedData<int, DummyItem>();
            cache.UpdateFullSnapshot(new[]
            {
                new DummyItem { Id = 1, Value = "A_Initial" },
                new DummyItem { Id = 2, Value = "B_Initial" }
            }, x => x.Id);

            // Act : On modifie l'Id 1 et on ajoute un Id 3
            var changes = new List<DummyItem>
            {
                new DummyItem { Id = 1, Value = "A_Modifié" },
                new DummyItem { Id = 3, Value = "C_Nouveau" }
            };
            cache.UpdateDifferentialSnapshot(changes, x => x.Id);

            // Assert
            var result = cache.Cache;
            result.Should().HaveCount(3);
            result.Should().ContainSingle(x => x.Id == 1 && x.Value == "A_Modifié", "L'élément existant doit avoir été mis à jour.");
            result.Should().ContainSingle(x => x.Id == 2 && x.Value == "B_Initial", "L'élément non touché doit rester intact.");
            result.Should().ContainSingle(x => x.Id == 3 && x.Value == "C_Nouveau", "Le nouvel élément doit avoir été inséré.");
        }
    }
}