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
using System.Text.RegularExpressions;

namespace KernelImpl.Tests.Noyau.Organisation
{
    public class CompetitionTests
    {
        [Fact]
        public void ToXml_Et_LoadXml_FormentUnAllerRetourParfait()
        {
            Mock<IJudoData> mockDc = new Mock<IJudoData>();
            IJudoData dc = mockDc.Object;

            DateTime dateCompet = new DateTime(2026, 05, 17);

            Competition original = new Competition
            {
                id = 1,
                nom = "Championnat de France",
                date = dateCompet,
                lieu = "Paris",
                siteInternet = "www.judo.fr",
                remoteId = "COMP_1",
                codeAcces = "1234",
                type = CompetitionTypeEnum.Individuel,
                type2 = CompetitionType2Enum.Officielle, // Officielle
                discipline = CompetitionDisciplineEnum.Judo.ToString2(),
                nbTapis = 8,
                tempsCombat = 240,
                niveau = (int)EchelonEnum.National,
                couleur1 = "#FFFFFF",
                couleur2 = "#000000",
                version = "1.0",
                afficheCSA = (int)TypeCSAEnum.Aucun,
                afficheKinzas = true,
                afficheAutoTempsRecuperation = false,
                afficheAnimationVainqueur = true,
                reglementEquipe = ReglementEquipeEnum.FFJDA
            };

            XElement xml = original.ToXml(dc);

            Competition copie = new Competition();
            copie.LoadXml(xml);

            copie.id.Should().Be(original.id);
            copie.nom.Should().Be(original.nom);
            copie.lieu.Should().Be(original.lieu);
            copie.type.Should().Be(original.type);
            copie.nbTapis.Should().Be(original.nbTapis);
            copie.tempsCombat.Should().Be(original.tempsCombat);
            copie.afficheKinzas.Should().Be(original.afficheKinzas);
        }

        [Fact]
        public void MethodesDeVerificationDeType_RetournentLesBonsBooleens()
        {
            Competition competIndiv = new Competition { type = CompetitionTypeEnum.Individuel, type2 = CompetitionType2Enum.Officielle };
            Competition competEquipe = new Competition { type = CompetitionTypeEnum.Equipe, type2 = CompetitionType2Enum.ProLeague };
            Competition competShiai = new Competition { type = CompetitionTypeEnum.Shiai };

            competIndiv.IsIndividuelle().Should().BeTrue();
            competIndiv.IsOfficielle().Should().BeTrue();

            competEquipe.IsEquipe().Should().BeTrue();
            competEquipe.IsProLeague().Should().BeTrue();

            competShiai.IsShiai().Should().BeTrue();
        }
    }
}