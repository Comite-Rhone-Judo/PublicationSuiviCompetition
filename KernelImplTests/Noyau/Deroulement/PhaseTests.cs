#nullable enable
using System;
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
    public class PhaseTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            DateTime dateTest = new DateTime(2026, 05, 17, 9, 0, 0);

            Phase original = new Phase
            {
                id = 3,
                libelle = "Demi-Finale",
                typePhase = FranceJudo.Metier.Noyau.Deroulement.TypePhaseEnum.Tableau,
                nbPoules = 0,
                niveauRepechage = 1,
                bresilien = true,
                precedent = 2,
                suivant = 4,
                epreuve = 10,
                date = dateTest
            };

            XElement xml = original.ToXml(dc);

            Phase copie = new Phase();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.libelle.Should().Be(original.libelle);
            copie.typePhase.Should().Be(original.typePhase);
            copie.bresilien.Should().Be(original.bresilien);
            copie.epreuve.Should().Be(original.epreuve);
            copie.date.Should().Be(original.date);
        }
    }
}