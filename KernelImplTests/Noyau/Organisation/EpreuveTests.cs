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
    public class EpreuveTests
    {
        [Fact]
        public void Sexe_Set_SynchroniseSexeEnum()
        {
            Epreuve epreuve = new Epreuve
            {
                sexe = 1 // 1 = Féminin par convention dans EpreuveSexeEnum
            };

            epreuve.sexeEnum.Should().NotBeNull();
            ((int)epreuve.sexeEnum).Should().Be(1);
        }

        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            DateTime dateDebut = new DateTime(2026, 05, 17, 9, 0, 0);
            DateTime dateFin = new DateTime(2026, 05, 17, 18, 0, 0);

            Epreuve original = new Epreuve
            {
                id = 10,
                nom = "Seniors -73kg",
                debut = dateDebut,
                fin = dateFin,
                remoteID = "EPR_10",
                competition = 1,
                categoriePoids = 5,
                poidsMin = 66,
                poidsMax = 73,
                ceintureMin = 1,
                ceintureMax = 10,
                anneeMin = 1990,
                anneeMax = 2005,
                sexe = 0,
                epreuve_equipe = 0
            };

            XElement xml = original.ToXml(dc);

            Epreuve copie = new Epreuve();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.nom.Should().Be(original.nom);
            copie.poidsMin.Should().Be(original.poidsMin);
            copie.poidsMax.Should().Be(original.poidsMax);
            copie.sexe.Should().Be(original.sexe);
        }

        [Fact]
        public void ToStringAvecCompetitions_ConcateneNomEtCompetition()
        {
            Epreuve epreuve = new Epreuve { nom = "Seniors M", competition = 1 };
            Competition compet = new Competition { id = 1, nom = "Champ. France" };
            List<Competition> listeCompetitions = new List<Competition> { compet };

            string resultat = epreuve.ToString(listeCompetitions);

            resultat.Should().Be("Seniors M (Champ. France)");
        }
    }
}