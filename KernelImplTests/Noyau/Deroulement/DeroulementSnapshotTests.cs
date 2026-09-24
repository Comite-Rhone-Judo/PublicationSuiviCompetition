#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using KernelImpl.Noyau.Deroulement;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class DeroulementSnapshotTests
    {
        [Fact]
        public void Constructeur_AvecSourceNulle_NePlantePas()
        {
            DeroulementSnapshot snapshot = new DeroulementSnapshot(null!);

            snapshot.Combats.Should().BeNull();
            snapshot.Rencontres.Should().BeNull();
            snapshot.Feuilles.Should().BeNull();
            snapshot.Groupes.Should().BeNull();
            snapshot.Phases.Should().BeNull();
        }

        [Fact]
        public void Constructeur_AvecSourceValide_CopieLesReferencesDesListes()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;
            DataDeroulement source = new DataDeroulement();

            source.ChargeRencontres(new XElement("Root", new Rencontre { id = 1 }.ToXml(dc)), true);
            source.ChargeFeuilles(new XElement("Root", new Feuille { id = 1 }.ToXml(dc)), true);

            DeroulementSnapshot snapshot = new DeroulementSnapshot(source);

            snapshot.Rencontres.Should().BeSameAs(source.Rencontres);
            snapshot.Feuilles.Should().BeSameAs(source.Feuilles);
            // Les autres listes non chargées seront pointées vers les instances vides du cache
            snapshot.Combats.Should().BeSameAs(source.Combats);
            snapshot.Phases.Should().BeSameAs(source.Phases);
        }
    }
}