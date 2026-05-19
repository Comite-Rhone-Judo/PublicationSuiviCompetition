#nullable enable
using System.Threading;
using Xunit;
using AppPublication.Tools;

namespace AppPublication.Tests.Tools
{
    public class EchangeMarkupTests
    {
        [Fact]
        public void ReponseRecue_SansDemandeEmise_RetourneNull()
        {
            // Arrange
            EchangeMarkup echange = new EchangeMarkup();

            // Act
            double? latence = echange.ReponseRecue();

            // Assert
            // La logique métier stipule que si aucune demande n'a été émise, on ignore la réponse (null)
            Assert.Null(latence);
        }

        [Fact]
        public void DemandeEmise_PuisReponseRecue_RetourneLatenceValide()
        {
            // Arrange
            EchangeMarkup echange = new EchangeMarkup();

            // Act
            echange.DemandeEmise();

            // On simule une latence réseau de 15 millisecondes
            Thread.Sleep(15);

            double? latence = echange.ReponseRecue();

            // Assert
            Assert.NotNull(latence);
            // On vérifie que le Stopwatch a bien mesuré un temps positif
            Assert.True(latence > 0);
        }

        [Fact]
        public void ReponseRecue_AppelDouble_RetourneNullAuSecondAppel()
        {
            // Arrange
            EchangeMarkup echange = new EchangeMarkup();
            echange.DemandeEmise();

            // Act
            double? latence1 = echange.ReponseRecue();
            double? latence2 = echange.ReponseRecue(); // Simule un doublon réseau

            // Assert
            // Le premier appel doit réussir
            Assert.NotNull(latence1);

            // Le second appel doit être bloqué par la protection `_reponseRecue = true`
            Assert.Null(latence2);
        }
    }
}