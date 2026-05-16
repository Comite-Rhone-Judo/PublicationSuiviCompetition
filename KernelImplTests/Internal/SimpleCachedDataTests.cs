#nullable enable
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using KernelImpl.Internal;

namespace KernelImpl.Tests.Internal
{
    public class SimpleCachedDataTests
    {
        // On utilise une classe simple avec un constructeur vide pour le test
        public class DummyConfig
        {
            public int Version { get; set; } = 1;
        }

        [Fact]
        public void Constructeur_InitialiseAvecUneInstanceParDefaut()
        {
            // Arrange & Act
            var cache = new SimpleCachedData<DummyConfig>();

            // Assert
            cache.Cache.Should().NotBeNull("Le constructeur par défaut doit instancier un nouvel objet via le new().");
            cache.Cache.Version.Should().Be(1);
        }

        [Fact]
        public void UpdateSnapshot_RemplaceAtomiquementLObjet()
        {
            // Arrange
            var cache = new SimpleCachedData<DummyConfig>();
            var newConfig = new DummyConfig { Version = 42 };

            // Act
            cache.UpdateSnapshot(newConfig);

            // Assert
            cache.Cache.Should().BeSameAs(newConfig, "La référence de l'objet dans le cache doit être strictement identique à celle fournie.");
            cache.Cache.Version.Should().Be(42);
        }

        [Fact]
        public void UpdateSnapshot_AvecValeurNull_EmpecheLesCrashsEnCreantUneNouvelleInstance()
        {
            // Arrange
            var cache = new SimpleCachedData<DummyConfig>();
            cache.UpdateSnapshot(new DummyConfig { Version = 99 });

            // Act : On tente d'injecter du poison (null)
            cache.UpdateSnapshot(null!);

            // Assert
            cache.Cache.Should().NotBeNull("Le cache ne doit jamais contenir null.");
            cache.Cache.Version.Should().Be(1, "Un nouvel objet vierge doit avoir été créé en remplacement du null.");
        }
    }
}