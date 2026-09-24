#nullable enable
using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Noyau;
using KernelImpl;
using KernelImpl.Noyau.Arbitrage;
using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Logos;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using KernelImpl.Noyau.Structures;

namespace KernelImpl.Tests
{
    public class JudoDataTests
    {
        [Fact]
        public void Constructeur_InitialiseTousLesDataManagers()
        {
            // Arrange & Act
            var judoData = new JudoData();

            // Assert
            judoData.Arbitrage.Should().NotBeNull("Le gestionnaire d'arbitrage doit être instancié.");
            judoData.Categories.Should().NotBeNull("Le gestionnaire des catégories doit être instancié.");
            judoData.Deroulement.Should().NotBeNull("Le gestionnaire de déroulement doit être instancié.");
            judoData.Logos.Should().NotBeNull("Le gestionnaire des logos doit être instancié.");
            judoData.Organisation.Should().NotBeNull("Le gestionnaire d'organisation doit être instancié.");
            judoData.Participants.Should().NotBeNull("Le gestionnaire des participants doit être instancié.");
            judoData.Structures.Should().NotBeNull("Le gestionnaire des structures doit être instancié.");
        }

        [Fact]
        public void ProprietesSetters_ModifientCorrectementLesValeurs()
        {
            // Arrange
            var judoData = new JudoData();
            var newArbitrage = new DataArbitrage();
            var newCategories = new DataCategories();

            // Act
            judoData.Arbitrage = newArbitrage;
            judoData.Categories = newCategories;

            // Assert
            judoData.Arbitrage.Should().BeSameAs(newArbitrage);
            judoData.Categories.Should().BeSameAs(newCategories);
        }

        [Fact]
        public void IJudoData_ProprietesExplicites_PointentVersLesInstancesConcretes()
        {
            // Arrange
            var judoData = new JudoData();

            // Act : On cast explicitement vers l'interface comme le ferait la couche UI ou Métier
            IJudoData iJudoData = judoData;

            // Assert : On vérifie que le pontage vers l'interface fonctionne
            iJudoData.Arbitrage.Should().BeSameAs(judoData.Arbitrage);
            iJudoData.Categories.Should().BeSameAs(judoData.Categories);
            iJudoData.Deroulement.Should().BeSameAs(judoData.Deroulement);
            iJudoData.Logos.Should().BeSameAs(judoData.Logos);
            iJudoData.Organisation.Should().BeSameAs(judoData.Organisation);
            iJudoData.Participants.Should().BeSameAs(judoData.Participants);
            iJudoData.Structures.Should().BeSameAs(judoData.Structures);
        }

        [Fact]
        public void Implements_IJudoDataManager_Correctement()
        {
            // Arrange
            var judoData = new JudoData();

            // Act & Assert
            judoData.Data.Should().BeSameAs(judoData, "La propriété Data doit retourner l'instance courante.");
            judoData.EnsureDataConsistency().Should().BeTrue("Par défaut, la cohérence des données est considérée comme valide.");
        }

        [Fact]
        public void RunSafeDataUpdate_ExecuteLActionSousVerrou()
        {
            // Arrange
            var judoData = new JudoData();
            bool actionExecuted = false;

            // Act
            judoData.RunSafeDataUpdate(() =>
            {
                // Cette action est exécutée dans le contexte du EnterWriteLock
                actionExecuted = true;
            });

            // Assert
            actionExecuted.Should().BeTrue("L'action passée à RunSafeDataUpdate doit être exécutée de manière synchrone.");
        }

        [Fact]
        public void Snapshot_RetourneUneNouvelleInstanceImmuable_DeTypeJudoDataSnapshot()
        {
            // Arrange
            var judoData = new JudoData();

            // Act : L'appel à la propriété déclenche la création du JudoDataSnapshot sous verrou de lecture
            var snapshot = judoData.Snapshot;

            // Assert
            snapshot.Should().NotBeNull();
            snapshot.Should().BeOfType<JudoDataSnapshot>("La propriété Snapshot doit retourner l'implémentation concrète JudoDataSnapshot.");

            // Vérification que le snapshot est bien une copie et non l'instance principale
            snapshot.Should().NotBeSameAs(judoData, "Le snapshot doit être une nouvelle instance en mémoire.");

            // On valide que le constructeur de JudoDataSnapshot a bien ponté les sous-snapshots
            snapshot.Arbitrage.Should().NotBeNull();
            snapshot.Deroulement.Should().NotBeNull();
        }
    }
}