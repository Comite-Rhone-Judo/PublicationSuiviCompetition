#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using KernelImpl.Noyau.Organisation;

namespace KernelImpl.Tests.Noyau.Organisation
{
    public class Epreuve_EquipeTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            DateTime dateDebut = new DateTime(2026, 05, 17, 9, 0, 0);

            Epreuve_Equipe original = new Epreuve_Equipe
            {
                id = 50,
                libelle = "Equipe Seniors Mixte",
                debut = dateDebut,
                fin = dateDebut.AddHours(8),
                remoteID = "EQ_50",
                competition = 1,
                ceintureMin = 3,
                ceintureMax = 8,
                anneeMin = 1990,
                anneeMax = 2005,
                categorieAge = 3,
                epreuveRef = 0,
                type = EpreuveEquipeTypeEnum.Mixte
            };

            XElement xml = original.ToXml(dc);

            Epreuve_Equipe copie = new Epreuve_Equipe();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.libelle.Should().Be(original.libelle);
            copie.categorieAge.Should().Be(original.categorieAge);
            copie.type.Should().Be(original.type);
        }
    }
}