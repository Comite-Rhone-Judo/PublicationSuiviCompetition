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
    public class Groupe_CombatsTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait_AvecDates()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            DateTime dateTest = new DateTime(2026, 05, 17, 10, 45, 30);

            Groupe_Combats original = new Groupe_Combats
            {
                id = 7,
                decoupage = 1,
                tapis = 3,
                libelle = "Groupe Matin",
                numero = 1,
                horaire_debut = dateTest,
                horaire_fin = dateTest.AddHours(2),
                verrouille = true
            };

            XElement xml = original.ToXml(dc);

            Groupe_Combats copie = new Groupe_Combats();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.decoupage.Should().Be(original.decoupage);
            copie.tapis.Should().Be(original.tapis);
            copie.libelle.Should().Be(original.libelle);
            copie.numero.Should().Be(original.numero);
            copie.verrouille.Should().Be(original.verrouille);

            copie.horaire_debut.Should().Be(original.horaire_debut);
            copie.horaire_fin.Should().Be(original.horaire_fin);
        }
    }
}