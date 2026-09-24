#nullable enable
using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network.Http.Context;

namespace FranceJudo.Core.Tests.Network.Http.Context
{
    public class ContextProviderTests
    {
        // Classe bouchon pour simuler un contexte métier
        private class DummyService
        {
            public string Data { get; set; } = string.Empty;
        }

        [Fact]
        public void GetContext_TypeInconnu_RetourneNull()
        {
            // Arrange
            var provider = new ContextProvider();

            // Act
            var result = provider.GetContext<DummyService>();

            // Assert
            result.Should().BeNull("Si le contexte n'a jamais été enregistré, la méthode doit retourner null au lieu de planter.");
        }

        [Fact]
        public void Register_PuisGetContext_RetourneLInstanceExacte()
        {
            // Arrange
            var provider = new ContextProvider();
            var myService = new DummyService { Data = "Judo2026" };

            // Act
            provider.Register(myService);
            var result = provider.GetContext<DummyService>();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(myService, "Le provider doit retourner la référence exacte de l'objet enregistré.");
        }

        [Fact]
        public void Register_TypeDejaEnregistre_EcraseLAncienContexte()
        {
            // Arrange
            var provider = new ContextProvider();
            var service1 = new DummyService { Data = "Ancien" };
            var service2 = new DummyService { Data = "Nouveau" };

            // Act
            provider.Register(service1);
            provider.Register(service2); // On écrase

            var result = provider.GetContext<DummyService>();

            // Assert
            result!.Data.Should().Be("Nouveau", "Le dictionnaire doit écraser l'ancienne valeur si on enregistre à nouveau le même type.");
        }
    }
}