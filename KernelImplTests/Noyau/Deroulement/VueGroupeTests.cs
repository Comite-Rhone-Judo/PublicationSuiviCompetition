#nullable enable
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Deroulement;
using FranceJudo.Metier.Noyau.Organisation;
using KernelImpl.Internal;
using KernelImpl.Noyau.Deroulement;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class VueGroupeTests
    {
        [Fact]
        public void Constructeur_AvecGroupeCombats_ResoutLArborescencePhaseEpreuve()
        {
            // Arrange : Préparation de l'arbre IJudoData
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            Mock<IDeroulementData> mockDeroulement = new Mock<IDeroulementData>();
            Mock<IOrganisationData> mockOrganisation = new Mock<IOrganisationData>();

            mockDc.Setup(dc => dc.Deroulement).Returns(mockDeroulement.Object);
            mockDc.Setup(dc => dc.Organisation).Returns(mockOrganisation.Object);

            // 1. Simulation du Découpage
            Mock<IPhase_Decoupage> mockDecoupage = new Mock<IPhase_Decoupage>();
            mockDecoupage.Setup(d => d.id).Returns(10);
            mockDecoupage.Setup(d => d.phase).Returns(20);

            List<IPhase_Decoupage> listeDecoupages = new List<IPhase_Decoupage> { mockDecoupage.Object };
            mockDeroulement.Setup(d => d.Decoupages).Returns(listeDecoupages);

            // 2. Simulation de la Phase
            Mock<IPhase> mockPhase = new Mock<IPhase>();
            mockPhase.Setup(p => p.id).Returns(20);
            mockPhase.Setup(p => p.libelle).Returns("Phase Finale");
            mockPhase.Setup(p => p.etat).Returns(1);
            mockPhase.Setup(p => p.typePhase).Returns(2);
            mockPhase.Setup(p => p.epreuve).Returns(new int?(30)); // Propriété Nullable<int>

            List<IPhase> listePhases = new List<IPhase> { mockPhase.Object };
            mockDeroulement.Setup(d => d.Phases).Returns(listePhases);

            // 3. Simulation de l'Épreuve
            Mock<IEpreuve> mockEpreuve = new Mock<IEpreuve>();
            mockEpreuve.Setup(e => e.id).Returns(30);
            mockEpreuve.Setup(e => e.nom).Returns("Seniors Masculins");

            List<IEpreuve> listeEpreuves = new List<IEpreuve> { mockEpreuve.Object };
            mockOrganisation.Setup(o => o.Epreuves).Returns(listeEpreuves);

            DateTime dateDebut = new DateTime(2026, 05, 17, 9, 0, 0);

            // Création de l'entité source (IDE0017 respecté)
            Groupe_Combats groupe = new Groupe_Combats
            {
                id = 7,
                decoupage = 10,
                tapis = 1,
                libelle = "Groupe Matin",
                horaire_debut = dateDebut,
                verrouille = true
            };

            // Act : Appel du véritable constructeur métier
            VueGroupe vue = new VueGroupe(groupe, mockDc.Object);

            // Assert : Vérification de la cascade de résolution
            vue.groupe_id.Should().Be(7);
            vue.phase_id.Should().Be(20, "Le constructeur doit avoir résolu l'ID de la phase via le découpage.");
            vue.phase_libelle.Should().Be("Phase Finale");
            vue.epreuve_id.Should().Be(30, "Le constructeur doit avoir résolu l'ID de l'épreuve via la phase.");
            vue.epreuve_nom.Should().Be("Seniors Masculins");

            // Vérification de l'interface IEntityWithKey
            IEntityWithKey<int> entity = vue;
            entity.EntityKey.Should().Be(7);
        }

        [Fact]
        public void ToXml_GenereUnXElementValide_SansPlanter()
        {
            // Arrange
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            Mock<IDeroulementData> mockDeroulement = new Mock<IDeroulementData>();

            mockDc.Setup(dc => dc.Deroulement).Returns(mockDeroulement.Object);
            // On renvoie une liste vide pour éviter un plantage du constructeur s'il ne trouve pas de découpage
            mockDeroulement.Setup(d => d.Decoupages).Returns(new List<IPhase_Decoupage>());

            DateTime dateDebut = new DateTime(2026, 05, 17, 9, 0, 0);

            Groupe_Combats groupe = new Groupe_Combats
            {
                id = 7,
                tapis = 1,
                libelle = "Groupe Matin",
                horaire_debut = dateDebut, // Obligatoire car ToXml effectue un cast explicite (DateTime)this.groupe_debut
                verrouille = true
            };

            VueGroupe vue = new VueGroupe(groupe, mockDc.Object);

            // On assigne manuellement ces champs car ils seraient vides suite à la liste vide de découpages mockée ci-dessus
            vue.epreuve_nom = "Epreuve Test";
            vue.phase_libelle = "Phase Test";

            // Act
            XElement xml = vue.ToXml(mockDc.Object);

            // Assert
            xml.Should().NotBeNull();
            xml.HasAttributes.Should().BeTrue("L'élément XML généré doit contenir les attributs mappés depuis la vue.");
        }
    }
}