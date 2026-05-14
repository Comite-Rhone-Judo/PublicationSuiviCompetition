using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Configuration.Json;

namespace FranceJudo.Core.Tests.Configuration.Json
{
    public class JsonConfigElementTests
    {
        private class DummyElement : JsonConfigElement
        {
            private string? _nom;
            public string? Nom
            {
                get => _nom;
                set => SetValue(ref _nom, value); // Méthode protégée à tester
            }
        }

        [Fact]
        public void SetValue_ValeurDifferente_MetAJourLeChampEtDeclencheOnChanged()
        {
            // Arrange
            var element = new DummyElement { Nom = "Initial" };
            bool eventFired = false;
            element.OnChanged = () => eventFired = true;

            // Act
            element.Nom = "Nouveau";

            // Assert
            eventFired.Should().BeTrue("L'événement OnChanged doit être déclenché.");
            element.Nom.Should().Be("Nouveau");
        }

        [Fact]
        public void SetValue_ValeurIdentique_NeDeclenchePasOnChanged()
        {
            // Arrange
            var element = new DummyElement { Nom = "Initial" };
            bool eventFired = false;
            element.OnChanged = () => eventFired = true;

            // Act
            element.Nom = "Initial"; // Même valeur

            // Assert
            eventFired.Should().BeFalse("OnChanged ne doit pas se déclencher si la valeur est identique (optimisation).");
        }
    }
}