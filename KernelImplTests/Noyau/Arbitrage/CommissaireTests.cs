#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using KernelImpl.Noyau.Arbitrage;

namespace KernelImpl.Tests.Noyau.Arbitrage
{
    public class CommissaireTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            // Arrange
            var original = new Commissaire
            {
                id = 99,
                nom = "MARTIN",
                prenom = "Sophie",
                licence = "LIC98765",
                club = "Paris Judo",
                comite = "75",
                ligue = "IDF",
                naissance = new DateTime(1992, 5, 12),
                sexe = false,
                modification = false,
                estResponsable = true,
                present = false,
                remoteID = "REMOTE_99"
            };

            // Act
            XElement xml = original.ToXml();
            var copie = new Commissaire();
            copie.LoadXml(xml);

            // Assert
            copie.id.Should().Be(original.id);
            copie.nom.Should().Be(original.nom);
            copie.prenom.Should().Be(original.prenom); // Pas de formatage spécial ici dans le code
            copie.licence.Should().Be(original.licence);
            copie.club.Should().Be(original.club);
            copie.comite.Should().Be(original.comite);
            copie.ligue.Should().Be(original.ligue);
            copie.naissance.Date.Should().Be(original.naissance.Date);
            copie.sexe.Should().Be(original.sexe);
            copie.estResponsable.Should().Be(original.estResponsable);
            copie.present.Should().Be(original.present);
        }

        [Fact]
        public void LectureCommissaire_ParseUneListeDepuisUnXElement()
        {
            var c1 = new Commissaire { id = 10 }.ToXml();
            var root = new XElement("Root", c1);

            var liste = Commissaire.LectureCommissaire(root);

            liste.Should().HaveCount(1);
            liste.First().id.Should().Be(10);
        }
    }
}