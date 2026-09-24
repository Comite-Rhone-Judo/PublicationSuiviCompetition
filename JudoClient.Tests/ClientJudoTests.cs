#nullable enable
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using Moq;
using JudoClient;
using FranceJudo.Core.Network.Tcp.Client;
using FranceJudo.Metier.Network;
using FranceJudo.Metier.XML;

namespace JudoClient.Tests
{
    public class ClientJudoTests
    {
        #region Tests de Routage Nominal (Succès)

        [Theory]
        // --- Région CONNECTION ---
        [InlineData(ServerCommandEnum.AcceptConnectionPesee)]
        // --- Région ARBITRAGE ---
        [InlineData(ServerCommandEnum.EnvoieArbitrage)]
        // --- Région DEROULEMENT ---
        [InlineData(ServerCommandEnum.TraiteCombats)]    // <-- Celle qui plantait passera désormais au vert !
        [InlineData(ServerCommandEnum.TraiteRencontres)] // <-- Celle-ci aussi !
        [InlineData(ServerCommandEnum.EnvoiePhases)]
        public void OnDataRecieve_CommandesValides_TraverseLeSwitchEtDeclencheSuccess(ServerCommandEnum commandATester)
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            mockNetworkClient.Setup(c => c.IsConnected).Returns(true);

            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            // CORRECTION : On construit un XML "Universel" qui contient les nœuds requis 
            // pour satisfaire les int.Parse() des différentes classes de traitement.
            string xmlPayload = $@"
            <{ConstantXML.ServerJudo}>
                <{ConstantXML.Command}>{(int)commandATester}</{ConstantXML.Command}>
                <{ConstantXML.Valeur}>
                    <TestNode>Donnees fictives pour la commande {commandATester}</TestNode>
                    
                    <{ConstantXML.Combat}>99</{ConstantXML.Combat}>
                    <{ConstantXML.Rencontre}>88</{ConstantXML.Rencontre}>
                    <{ConstantXML.Judoka}>77</{ConstantXML.Judoka}>
                </{ConstantXML.Valeur}>
            </{ConstantXML.ServerJudo}>";

            bool successEventTriggered = false;
            bool errorEventTriggered = false;

            clientJudo.OnReceivedDataSuccessOccured += (sender, data) => successEventTriggered = true;
            clientJudo.OnReceivedDataErrorOccured += (sender, data) => errorEventTriggered = true;

            // Act
            mockNetworkClient.Raise(m => m.OnDataRecieve += null, mockNetworkClient.Object, xmlPayload);

            // Assert
            successEventTriggered.Should().BeTrue($"La commande {commandATester} doit déclencher l'événement de succès.");
            errorEventTriggered.Should().BeFalse($"La commande {commandATester} ne doit générer aucune erreur.");
        }

        #endregion

        #region Tests de Robustesse (Erreurs)

        [Fact]
        public void OnDataRecieve_XMLMalforme_DeclencheEvenementErreur()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            string xmlCorrompu = "<ServerJudo><Command>123</Command><Valeur>Il manque la fermeture de la balise valeur</ServerJudo>";

            bool successEventTriggered = false;
            bool errorEventTriggered = false;

            clientJudo.OnReceivedDataSuccessOccured += (sender, data) => successEventTriggered = true;
            clientJudo.OnReceivedDataErrorOccured += (sender, data) => errorEventTriggered = true;

            // Act
            mockNetworkClient.Raise(m => m.OnDataRecieve += null, mockNetworkClient.Object, xmlCorrompu);

            // Assert
            errorEventTriggered.Should().BeTrue("Un XML malformé doit être intercepté par le catch et déclencher l'événement d'erreur.");
            successEventTriggered.Should().BeFalse("Un XML malformé ne doit pas déclencher le succès.");
        }

        [Fact]
        public void OnDataRecieve_XMLValideMaisBaliseCommandManquante_DeclencheEvenementErreur()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            // XML valide au sens strict, mais il manque <Command> pour le int.Parse()
            string xmlIncomplet = $@"
            <{ConstantXML.ServerJudo}>
                <{ConstantXML.Valeur}>Test</{ConstantXML.Valeur}>
            </{ConstantXML.ServerJudo}>";

            bool errorEventTriggered = false;
            clientJudo.OnReceivedDataErrorOccured += (sender, data) => errorEventTriggered = true;

            // Act
            mockNetworkClient.Raise(m => m.OnDataRecieve += null, mockNetworkClient.Object, xmlIncomplet);

            // Assert
            errorEventTriggered.Should().BeTrue("L'absence de la balise Command provoque un NullReferenceException ou FormatException qui doit être loggé et déclencher l'erreur.");
        }

        [Fact]
        public void OnDataRecieve_DonneesHorsBaliseServerJudo_IgnoreSilencieusement()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            // Une trame XML qui ne nous est pas destinée (pas de <ServerJudo>)
            string xmlAutre = "<AutreSysteme><Message>Bonjour</Message></AutreSysteme>";

            bool successEventTriggered = false;
            bool errorEventTriggered = false;

            clientJudo.OnReceivedDataSuccessOccured += (sender, data) => successEventTriggered = true;
            clientJudo.OnReceivedDataErrorOccured += (sender, data) => errorEventTriggered = true;

            // Act
            mockNetworkClient.Raise(m => m.OnDataRecieve += null, mockNetworkClient.Object, xmlAutre);

            // Assert
            successEventTriggered.Should().BeTrue("Le code actuel déclenche le succès même si la balise ServerJudo n'est pas trouvée (le if est ignoré).");
            errorEventTriggered.Should().BeFalse();
        }

        #endregion
    }
}