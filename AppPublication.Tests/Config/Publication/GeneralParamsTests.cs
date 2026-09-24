using AppPublication.Config.Publication;
using System;
using System.Collections.Generic;
using Xunit;

namespace AppPublication.Tests.Config.Publication
{
    public class GeneralParamsTests
    {
        [Fact]
        public void Proprietes_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            GeneralParams parametres = new GeneralParams();

            // Assert
            Assert.Equal(string.Empty, parametres.NiveauPublicationFFJudo);
            Assert.Equal(string.Empty, parametres.EntitePublicationFFJudo);
            Assert.NotNull(parametres.RepertoireRacine); // Environnement dépendant, on vérifie juste qu'il n'est pas null
            Assert.Null(parametres.Logo);
            Assert.True(parametres.EasyConfig);
            Assert.Equal(string.Empty, parametres.URLDistant);
            Assert.False(parametres.IsolerCompetition);
            Assert.Equal(string.Empty, parametres.RepertoireRacineSiteFTPDistant);
            Assert.True(parametres.EffacerAuDemarrage);
        }

        [Fact]
        public void Setters_ModifientLesValeurs()
        {
            // Arrange
            GeneralParams parametres = new GeneralParams
            {
                // Act
                NiveauPublicationFFJudo = "National",
                EasyConfig = false,
                IsolerCompetition = true
            };

            // Assert
            Assert.Equal("National", parametres.NiveauPublicationFFJudo);
            Assert.False(parametres.EasyConfig);
            Assert.True(parametres.IsolerCompetition);
        }

        [Fact]
        public void GetNiveauPublicationFFJudo_RetourneCorrespondanceOuDefaut()
        {
            // Arrange
            GeneralParams parametres = new GeneralParams
            {
                NiveauPublicationFFJudo = "Comite"
            };
            List<string> candidats = new List<string> { "Ligue", "Comite", "Club" };
            static string selecteur(string s) { return s; }

            // Act
            string resultatMatch = parametres.GetNiveauPublicationFFJudo(candidats, selecteur);

            parametres.NiveauPublicationFFJudo = "Inconnu";
            string resultatDefaut = parametres.GetNiveauPublicationFFJudo(candidats, selecteur);

            // Assert
            Assert.Equal("Comite", resultatMatch);
            Assert.Equal("Ligue", resultatDefaut); // Retourne le premier si pas de match
        }

        [Fact]
        public void GetEntitePublicationFFJudo_PrioriseValeurInitiale()
        {
            // Arrange
            GeneralParams parametres = new GeneralParams
            {
                EntitePublicationFFJudo = "ClubA"
            };
            List<string> candidats = new List<string> { "ClubA", "ClubB", "ClubC" };
            static string selecteur(string s) { return s; }

            // Act
            // Test avec une valeur initiale fournie (doit ignorer la config interne)
            string resultatInitial = parametres.GetEntitePublicationFFJudo(candidats, selecteur, "ClubB");

            // Test sans valeur initiale (doit utiliser la config interne)
            string resultatConfig = parametres.GetEntitePublicationFFJudo(candidats, selecteur, null);

            // Assert
            Assert.Equal("ClubB", resultatInitial);
            Assert.Equal("ClubA", resultatConfig);
        }

        [Fact]
        public void GetLogo_RetourneNull_SiCandidatsEstNull()
        {
            // Arrange
            GeneralParams parametres = new GeneralParams { Logo = "logo.png" };
            static string selecteur(string s) { return s; }

            // Act
            string resultat = parametres.GetLogo<string>(parametres.Logo, null, selecteur);

            // Assert
            Assert.Null(resultat);
        }
    }
}