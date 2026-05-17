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
    public class FeuilleTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            Feuille original = new Feuille
            {
                id = 5,
                repechage = true,
                source1 = 1,
                source2 = 2,
                reference = "REF_A",
                ref1 = "R1",
                ref2 = "R2",
                numero = 10,
                ordre = 2,
                pere = 0,
                classement1 = 1,
                classement2 = 2,
                niveau = 3,
                combat = 42,
                phase = 1,
                typeSource = false
            };

            XElement xml = original.ToXml(dc);

            Feuille copie = new Feuille();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.repechage.Should().Be(original.repechage);
            copie.source1.Should().Be(original.source1);
            copie.combat.Should().Be(original.combat);
            copie.reference.Should().Be(original.reference);
        }

        [Fact]
        public void LectureFeuilles_ParseUneListeDepuisUnXElement()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            Feuille feuille1 = new Feuille { id = 99 };
            XElement root = new XElement("Root", feuille1.ToXml(dc));

            ICollection<Feuille> liste = Feuille.LectureFeuilles(root);

            liste.Should().HaveCount(1);
            liste.First().id.Should().Be(99);
        }
    }
}