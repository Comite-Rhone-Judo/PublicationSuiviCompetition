#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using KernelImpl.Noyau.Deroulement;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class DataDeroulementTests
    {
        [Fact]
        public void ChargeRencontres_AvecIsFullTrue_EcraseLeCache()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;
            DataDeroulement data = new DataDeroulement();

            Rencontre r1 = new Rencontre { id = 1, temps = 120 };
            XElement root = new XElement("Root", r1.ToXml(dc));

            // Act : Chargement complet
            data.ChargeRencontres(root, true);

            // Assert
            data.Rencontres.Should().HaveCount(1);
            data.Rencontres[0].id.Should().Be(1);
        }

        [Fact]
        public void ChargeRencontres_AvecIsFullFalse_MetAJourLeCacheEnDifferentiel()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;
            DataDeroulement data = new DataDeroulement();

            // 1. Initialisation avec la rencontre ID 1
            Rencontre rInitiale = new Rencontre { id = 1, temps = 120 };
            XElement rootInit = new XElement("Root", rInitiale.ToXml(dc));
            data.ChargeRencontres(rootInit, true);

            // 2. Mise à jour différentielle (On change le temps de l'ID 1, et on ajoute l'ID 2)
            Rencontre rModifiee = new Rencontre { id = 1, temps = 300 };
            Rencontre rNouvelle = new Rencontre { id = 2, temps = 60 };
            XElement rootDiff = new XElement("Root", rModifiee.ToXml(dc), rNouvelle.ToXml(dc));

            // Act : Chargement différentiel
            data.ChargeRencontres(rootDiff, false);

            // Assert : Le cache doit fusionner les données (Upsert)
            data.Rencontres.Should().HaveCount(2);

            // On vérifie que la rencontre 1 a bien été mise à jour (Last Wins)
            Rencontre? result1 = data.Rencontres[0] as Rencontre;
            result1!.temps.Should().Be(300);
        }

        [Fact]
        public void ChargeFeuilles_GereCorrectementLeDifferentiel()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;
            DataDeroulement data = new DataDeroulement();

            Feuille f1 = new Feuille { id = 10 };
            XElement root = new XElement("Root", f1.ToXml(dc));

            // Act : Chargement complet
            data.ChargeFeuilles(root, true);

            // Assert
            data.Feuilles.Should().HaveCount(1);
            data.Feuilles[0].id.Should().Be(10);
        }
    }
}