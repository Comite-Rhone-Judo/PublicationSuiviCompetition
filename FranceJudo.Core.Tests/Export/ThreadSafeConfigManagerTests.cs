using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Export;
using FranceJudo.Core.Utils; // En supposant que ICloneableObject y soit

namespace FranceJudo.Core.Tests.Export
{
    public class ThreadSafeConfigManagerTests
    {
        // 1. Bouchon implémentant ICloneableObject
        private class DummyConfig : ICloneableObject<DummyConfig>
        {
            public int Valeur { get; set; }

            public DummyConfig Clone()
            {
                return new DummyConfig { Valeur = this.Valeur };
            }
        }

        [Fact]
        public void Snapshot_RetourneUnClone_ModificationsExternesIsolees()
        {
            // Arrange
            var initial = new DummyConfig { Valeur = 10 };
            var manager = new ThreadSafeConfigManager<DummyConfig>(initial);

            // Act
            var snapshot1 = manager.Snapshot;
            snapshot1.Valeur = 99; // On tente de polluer l'état interne depuis l'extérieur

            var snapshot2 = manager.Snapshot;

            // Assert
            snapshot1.Should().NotBeSameAs(snapshot2, "Chaque appel à Snapshot doit renvoyer une nouvelle instance.");
            snapshot2.Valeur.Should().Be(10, "La modification du premier snapshot ne doit pas avoir affecté la configuration centrale.");
        }

        [Fact]
        public void Modifier_MiseAJourSecurisee_AltereLaConfigurationActive()
        {
            // Arrange
            var manager = new ThreadSafeConfigManager<DummyConfig>(new DummyConfig { Valeur = 10 });

            // Act
            manager.Modifier(config => config.Valeur = 42);

            // Assert
            manager.Snapshot.Valeur.Should().Be(42);
        }

        [Fact]
        public void SetConfiguration_RemplaceL_InstanceParUnClone()
        {
            // Arrange
            var manager = new ThreadSafeConfigManager<DummyConfig>(new DummyConfig { Valeur = 10 });
            var nouvelleConfig = new DummyConfig { Valeur = 100 };

            // Act
            manager.SetConfiguration(nouvelleConfig);

            // On modifie l'objet injecté pour vérifier que le manager a bien fait un Clone()
            nouvelleConfig.Valeur = 999;

            // Assert
            manager.Snapshot.Valeur.Should().Be(100, "Le manager doit avoir stocké un clone de la configuration initiale (100), et ignorer la modification ultérieure (999).");
        }
    }
}