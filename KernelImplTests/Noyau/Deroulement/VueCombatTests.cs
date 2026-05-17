#nullable enable
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Moq;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Participants;
using KernelImpl.Internal;
using KernelImpl.Noyau.Deroulement;

namespace KernelImpl.Tests.Noyau.Deroulement
{
    public class VueCombatTests
    {
        [Fact]
        public void Constructeur_AvecParticipantsIndividuels_InitialiseLaVueEtResoutLesJudokas()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            Mock<IParticipantsData> mockParticipants = new Mock<IParticipantsData>();

            mockDc.Setup(dc => dc.Participants).Returns(mockParticipants.Object);

            Mock<IJudoka> mockJudoka1 = new Mock<IJudoka>();
            mockJudoka1.Setup(j => j.id).Returns(10);
            mockJudoka1.Setup(j => j.licence).Returns("LIC_FRA_10");
            mockJudoka1.Setup(j => j.nom).Returns("GARRIVIER");
            mockJudoka1.Setup(j => j.prenom).Returns("Mickael");
            mockJudoka1.Setup(j => j.club).Returns("Alliance Judo");

            Mock<IJudoka> mockJudoka2 = new Mock<IJudoka>();
            mockJudoka2.Setup(j => j.id).Returns(20);
            mockJudoka2.Setup(j => j.licence).Returns("LIC_FRA_20");
            mockJudoka2.Setup(j => j.nom).Returns("BERNARD");
            mockJudoka2.Setup(j => j.prenom).Returns("Lucas");
            mockJudoka2.Setup(j => j.club).Returns("JC Lyon");

            List<IJudoka> listeJudokas = new List<IJudoka> { mockJudoka1.Object, mockJudoka2.Object };
            mockParticipants.Setup(p => p.Judokas).Returns(listeJudokas);

            // On s'assure que la liste des équipes est vide pour forcer le passage dans le bloc individuel
            mockParticipants.Setup(p => p.Equipes).Returns(new List<IEquipe>());

            // Instanciation stricte avec uniquement les VRAIES propriétés de Combat
            Combat combat = new Combat
            {
                id = 42,
                participant1 = 10,
                participant2 = 20,
                vainqueur = 10,
                virtuel = false
            };

            VueCombat vue = new VueCombat(combat, mockDc.Object);

            vue.combat_id.Should().Be(42, "L'identifiant du combat doit être correctement mappé.");
            vue.judoka1_nom1.Should().Be("GARRIVIER", "La vue doit résoudre le nom du judoka 1 depuis le cache.");
            vue.judoka2_nom1.Should().Be("BERNARD", "La vue doit résoudre le nom du judoka 2 depuis le cache.");

            IEntityWithKey<int> entity = vue;
            entity.EntityKey.Should().Be(42);
        }

        [Fact]
        public void Constructeur_AvecParticipantsEquipes_InitialiseLaVueEtResoutLesEquipes()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            Mock<IParticipantsData> mockParticipants = new Mock<IParticipantsData>();

            mockDc.Setup(dc => dc.Participants).Returns(mockParticipants.Object);

            Mock<IEquipe> mockEquipe1 = new Mock<IEquipe>();
            mockEquipe1.Setup(e => e.id).Returns(300);
            mockEquipe1.Setup(e => e.libelle).Returns("Comite Rhone Judo");

            Mock<IEquipe> mockEquipe2 = new Mock<IEquipe>();
            mockEquipe2.Setup(e => e.id).Returns(400);
            mockEquipe2.Setup(e => e.libelle).Returns("Comite Loire Judo");

            List<IEquipe> listeEquipes = new List<IEquipe> { mockEquipe1.Object, mockEquipe2.Object };
            mockParticipants.Setup(p => p.Equipes).Returns(listeEquipes);

            // On s'assure que la liste des judokas individuels est vide pour forcer le passage dans le bloc "else"
            mockParticipants.Setup(p => p.Judokas).Returns(new List<IJudoka>());

            // Instanciation stricte de Combat
            Combat combat = new Combat
            {
                id = 84,
                participant1 = 300,
                participant2 = 400,
                vainqueur = 300
            };

            VueCombat vue = new VueCombat(combat, mockDc.Object);

            vue.combat_id.Should().Be(84);
            vue.judoka1_nom1.Should().Be("Comite Rhone Judo", "La vue doit utiliser le libellé de l'équipe pour le nom du participant 1.");
            vue.judoka2_nom1.Should().Be("Comite Loire Judo", "La vue doit utiliser le libellé de l'équipe pour le nom du participant 2.");

            IEntityWithKey<int> entity = vue;
            entity.EntityKey.Should().Be(84);
        }
    }
}