using FranceJudo.Core.Utils;
using KernelImpl.Noyau.Arbitrage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace KernelImpl.Tests.Noyau.Arbitrage
{
    public class ArbitreTests
    {
        [Fact]
        public void Sexe_Set_SynchroniseSexeEnum()
        {
            // Arrange
            Arbitre arbitre = new Arbitre
            {
                nom = "Test",
                prenom = "Test",
                sexe = true // true = Masculin par convention (selon EpreuveSexe)
            };

            // Act & Assert
            Assert.True((bool)arbitre.sexeEnum);
        }

        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            // Arrange : Création d'un arbitre avec toutes les propriétés renseignées
            Arbitre original = new Arbitre
            {
                id = 42,
                nom = "DUPONT",
                prenom = "Jean",
                licence = "LIC123456",
                club = "Judo Club Lyon",
                comite = "69",
                ligue = "AURA",
                naissance = new DateTime(1985, 10, 25), // On évite les heures/minutes car ToXml utilise "ddMMyyyy"
                sexe = true,
                modification = true,
                estResponsable = false,
                present = true,
                niveau = 3,
                remoteID = "REMOTE_42"
            };

            // Act 1 : Sérialisation
            XElement xml = original.ToXml(null);

            // Act 2 : Désérialisation dans une nouvelle instance
            Arbitre copie = new Arbitre();
            copie.LoadXml(xml);

            // Assert : On vérifie que toutes les données ont survécu au transit XML
            Assert.Equal(original.id, copie.id);
            Assert.Equal(original.nom, copie.nom);
            Assert.Equal(original.prenom.FormatPrenom(), copie.prenom); // Le ToXml applique un formatage sur le prénom
            Assert.Equal(original.licence, copie.licence);
            Assert.Equal(original.club, copie.club);
            Assert.Equal(original.comite, copie.comite);
            Assert.Equal(original.ligue, copie.ligue);
            Assert.Equal(original.naissance.Date, copie.naissance.Date);
            Assert.Equal(original.sexe, copie.sexe);
            Assert.Equal(original.modification, copie.modification);
            Assert.Equal(original.estResponsable, copie.estResponsable);
            Assert.Equal(original.present, copie.present);
            Assert.Equal(original.niveau, copie.niveau);
            Assert.Equal(original.remoteID, copie.remoteID);
        }

        [Fact]
        public void LectureArbitre_ParseUneListeDepuisUnXElement()
        {
            // Arrange
            Arbitre a1 = new Arbitre { id = 1, nom = "Un", prenom = "A" };
            Arbitre a2 = new Arbitre { id = 2, nom = "Deux", prenom = "B" };

            XElement xmlA1 = a1.ToXml(null);
            XElement xmlA2 = a2.ToXml(null);
            XElement root = new XElement("Root", xmlA1, xmlA2);

            // Act
            ICollection<Arbitre> liste = Arbitre.LectureArbitre(root);

            // Assert
            Assert.Equal(2, liste.Count);
            Assert.Equal(1, liste.First().id);
            Assert.Equal(2, liste.Last().id);
        }
    }
}