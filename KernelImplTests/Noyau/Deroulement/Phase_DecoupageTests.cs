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
    public class Phase_DecoupageTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            Phase_Decoupage original = new Phase_Decoupage
            {
                id = 8,
                phase = 2,
                decoupage_finales = 1,
                decoupage_tableau = 2,
                decoupage_poule = 3
            };

            XElement xml = original.ToXml(dc);

            Phase_Decoupage copie = new Phase_Decoupage();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.phase.Should().Be(original.phase);
            copie.decoupage_finales.Should().Be(original.decoupage_finales);
            copie.decoupage_tableau.Should().Be(original.decoupage_tableau);
            copie.decoupage_poule.Should().Be(original.decoupage_poule);
        }
    }
}