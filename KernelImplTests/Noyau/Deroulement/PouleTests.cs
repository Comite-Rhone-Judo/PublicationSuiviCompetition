#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using KernelImpl.Noyau.Deroulement;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class PouleTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            Poule original = new Poule
            {
                id = 12,
                numero = 4,
                phase = 1,
                etat = 0,
                nbparticipant = 4
            };

            XElement xml = original.ToXml(dc);

            Poule copie = new Poule();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.numero.Should().Be(original.numero);
            copie.phase.Should().Be(original.phase);
        }
    }
}