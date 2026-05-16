#nullable enable
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using KernelImpl.Internal;

namespace KernelImpl.Tests.Internal
{
    public class CacheExtensionsTests
    {
        // Classe implémentant l'interface requise par l'extension
        internal class SmartEntity : IEntityWithKey<string>
        {
            public string Code { get; set; } = string.Empty;
            public string Data { get; set; } = string.Empty;

            // Implémentation explicite de l'interface
            public string EntityKey => Code;
        }

        [Fact]
        public void UpdateFullSnapshot_ViaExtension_UtiliseLaCleDeLInterface()
        {
            // Arrange
            var cache = new DeduplicatedCachedData<string, SmartEntity>();
            var items = new[]
            {
                new SmartEntity { Code = "FR", Data = "France" },
                new SmartEntity { Code = "FR", Data = "France (Doublon)" }
            };

            // Act : On utilise la méthode d'extension SANS passer le Func<TValue, TKey>
            cache.UpdateFullSnapshot(items);

            // Assert
            cache.Cache.Should().HaveCount(1, "L'extension doit avoir correctement routé la clé 'Code' pour dédupliquer.");
            cache.Cache[0].Data.Should().Be("France (Doublon)");
        }

        [Fact]
        public void UpdateDifferentialSnapshot_ViaExtension_UtiliseLaCleDeLInterface()
        {
            // Arrange
            var cache = new DeduplicatedCachedData<string, SmartEntity>();
            cache.UpdateFullSnapshot(new[] { new SmartEntity { Code = "JP", Data = "Japon" } });

            var updates = new[] { new SmartEntity { Code = "JP", Data = "Japon (Mis à jour)" } };

            // Act : Utilisation de la méthode d'extension
            cache.UpdateDifferentialSnapshot(updates);

            // Assert
            cache.Cache.Should().HaveCount(1);
            cache.Cache[0].Data.Should().Be("Japon (Mis à jour)", "L'extension a correctement identifié la clé pour la fusion.");
        }
    }
}