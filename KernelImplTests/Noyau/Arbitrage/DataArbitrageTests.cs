#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Arbitrage;
using FranceJudo.Metier.Noyau.Arbitrage;

namespace KernelImpl.Tests.Noyau.Arbitrage
{
    public class DataArbitrageTests
    {
        [Fact]
        public void ChargeArbitres_MetAJourLeCache()
        {
            // Arrange
            var data = new DataArbitrage();
            var arbitreXml = new Arbitre { id = 1, nom = "Arbitre1" }.ToXml();
            var root = new XElement("Root", arbitreXml);

            // Act
            data.ChargeArbitres(root);

            // Assert
            data.Arbitres.Should().HaveCount(1);
            data.Arbitres[0].id.Should().Be(1);
        }

        [Fact]
        public void ChargeCommissaires_MetAJourLeCache()
        {
            // Arrange
            var data = new DataArbitrage();
            var root = new XElement("Root", new Commissaire { id = 2 }.ToXml());

            // Act
            data.ChargeCommissaires(root);

            // Assert
            data.Commissaires.Should().HaveCount(1);
            data.Commissaires[0].id.Should().Be(2);
        }

        [Fact]
        public void ChargeDelegues_MetAJourLeCache()
        {
            // Arrange
            var data = new DataArbitrage();
            var root = new XElement("Root", new Delegue { id = 3, nom = "Del" }.ToXml());

            // Act
            data.ChargeDelegues(root);

            // Assert
            data.Delegues.Should().HaveCount(1);
            data.Delegues[0].id.Should().Be(3);
        }

        [Fact]
        public void InterfaceExplicite_PointeVersLesCachesConcrets()
        {
            // Arrange
            var data = new DataArbitrage();
            IArbitrageData iData = data;

            // Assert : Les propriétés de l'interface doivent référencer les mêmes listes IReadOnlyList
            iData.Arbitres.Should().BeSameAs(data.Arbitres);
            iData.Commissaires.Should().BeSameAs(data.Commissaires);
            iData.Delegues.Should().BeSameAs(data.Delegues);
        }
    }
}