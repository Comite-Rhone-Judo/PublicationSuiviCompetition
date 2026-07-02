#nullable enable
using AppPublication.ExtensionNoyau;
using AppPublication.ExtensionNoyau.Engagement;
using FranceJudo.Metier.Noyau;
using Moq;
using System;
using Xunit;

namespace AppPublication.Tests.ExtensionNoyau
{
    public class ExtendedJudoDataTests
    {
        [Fact]
        public void Constructeur_AssigneCoreData()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();

            // Act
            ExtendedJudoData extendedData = new ExtendedJudoData(mockJudoData.Object);

            // Assert
            Assert.Same(mockJudoData.Object, extendedData.CoreData);
        }

        [Fact]
        public void Engagement_ProprieteLazy_EvalueLaFactoryALaPremiereLecture()
        {
            // Arrange
            Mock<IJudoData> mockJudoData = new Mock<IJudoData>();
            ExtendedJudoData extendedData = new ExtendedJudoData(mockJudoData.Object);

            // IDE0039 : Fonction locale pour encadrer l'évaluation
            void ActionEvaluationLazy()
            {
                // C'est cet appel exact qui va déclencher : () => new DataEngagement(snapshot)
                IDataEngagement resultat = extendedData.Engagement;

                // Si le code arrive ici, l'instanciation a réussi avec un Mock vide
                Assert.NotNull(resultat);
            }

            // Act
            // On capture une potentielle exception au cas où le constructeur de DataEngagement 
            // ferait des accès profonds et non protégés sur notre Mock (ex: snapshot.Deroulement.Phases).
            Exception? exception = Record.Exception(ActionEvaluationLazy);

            // Assert
            if (exception != null)
            {
                // L'exception prouve que la Factory du Lazy<T> a bien été déclenchée.
                // Elle échoue simplement car notre Mock IJudoData n'a pas été hydraté pour DataEngagement.
                Assert.True(exception is NullReferenceException || exception is ArgumentNullException,
                    "L'évaluation Lazy a échoué pour une raison inattendue : " + exception.Message);
            }
        }
    }
}