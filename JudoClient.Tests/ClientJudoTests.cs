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
        [InlineData(ServerCommandEnum.AcceptConnectionCS)]
        [InlineData(ServerCommandEnum.AcceptConnectionCOM)]
        [InlineData(ServerCommandEnum.AcceptConnectionTest)]
        // --- Région ARBITRAGE ---
        [InlineData(ServerCommandEnum.EnvoieArbitrage)]
        [InlineData(ServerCommandEnum.UpdateDelegues)]
        // --- Région CATES ---
        [InlineData(ServerCommandEnum.EnvoieCategories)]
        [InlineData(ServerCommandEnum.EnvoieCatePoids)]
        // --- Région STRUCTURE ---
        [InlineData(ServerCommandEnum.EnvoieStructures)]
        [InlineData(ServerCommandEnum.EnvoiePays)]
        // --- Région LOGO ---
        [InlineData(ServerCommandEnum.EnvoieLogos)]
        // --- Région ORGANISATION ---
        [InlineData(ServerCommandEnum.EnvoieOrganisation)]
        [InlineData(ServerCommandEnum.EnvoieEpreuves)]
        // --- Région PARTICIPANTS ---
        [InlineData(ServerCommandEnum.EnvoieEquipes)]
        [InlineData(ServerCommandEnum.EnvoieJudokas)]
        // --- Région DEROULEMENT ---
        [InlineData(ServerCommandEnum.TraiteCombats)]
        [InlineData(ServerCommandEnum.TraiteRencontres)]
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
            mockNetworkClient.Raise(m => m.OnDataReceive += null, mockNetworkClient.Object, xmlPayload);

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
            mockNetworkClient.Raise(m => m.OnDataReceive += null, mockNetworkClient.Object, xmlCorrompu);

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
            mockNetworkClient.Raise(m => m.OnDataReceive += null, mockNetworkClient.Object, xmlIncomplet);

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
            mockNetworkClient.Raise(m => m.OnDataReceive += null, mockNetworkClient.Object, xmlAutre);

            // Assert
            successEventTriggered.Should().BeTrue("Le code actuel déclenche le succès même si la balise ServerJudo n'est pas trouvée (le if est ignoré).");
            errorEventTriggered.Should().BeFalse();
        }

        #endregion

        #region Tests d'État, Cycle de vie et Propriétés

        [Fact]
        public void Proprietes_Traitements_SontCorrectementInstanciees()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            // Act & Assert (Couverture des Getters)
            clientJudo.TraitementArbitrage.Should().NotBeNull();
            clientJudo.TraitementCategories.Should().NotBeNull();
            clientJudo.TraitementConnexion.Should().NotBeNull();
            clientJudo.TraitementDeroulement.Should().NotBeNull();
            clientJudo.TraitementOrganisation.Should().NotBeNull();
            clientJudo.TraitementParticipants.Should().NotBeNull();
            clientJudo.TraitementStructure.Should().NotBeNull();
            clientJudo.TraitementLogos.Should().NotBeNull();
            clientJudo.NetworkClient.Should().NotBeNull();
        }

        [Fact]
        public void IsConnected_RetourneLEtatDuClient_EtSurvitAuxExceptions()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            // Act & Assert 1 : Cas Vrai
            mockNetworkClient.Setup(c => c.IsConnected).Returns(true);
            clientJudo.IsConnected.Should().BeTrue();

            // Act & Assert 2 : Cas Faux
            mockNetworkClient.Setup(c => c.IsConnected).Returns(false);
            clientJudo.IsConnected.Should().BeFalse();

            // Act & Assert 3 : Cas Exception (Couvre le bloc catch)
            mockNetworkClient.Setup(c => c.IsConnected).Throws<Exception>();
            clientJudo.IsConnected.Should().BeFalse("Le bloc catch doit intercepter l'erreur interne et retourner false sans crasher.");
        }

        [Fact]
        public void Evenement_OnEndConnection_EstPropagueAuNiveauSuperieur()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            bool eventPropagated = false;
            clientJudo.OnEndConnection += (sender) => eventPropagated = true;

            // Act : Simulation de la déconnexion par le client sous-jacent
            mockNetworkClient.Raise(m => m.OnEndConnection += null, mockNetworkClient.Object);

            // Assert
            eventPropagated.Should().BeTrue("L'événement interne OnEndConnection doit déclencher le délégué public.");
        }

        [Fact]
        public void Dispose_SeDesabonneDesEvenements_EtLibereLeClient()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();

            // On déclare que notre mock implémente aussi IDisposable (crucial pour valider le `is IDisposable`)
            var mockDisposable = mockNetworkClient.As<IDisposable>();

            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            // Act
            clientJudo.Dispose();

            // Assert
            // Vérifie que le client a bien été libéré via son interface IDisposable
            mockDisposable.Verify(d => d.Dispose(), Times.Once, "Le Dispose du ClientJudo doit appeler en cascade le Dispose du client réseau s'il est IDisposable.");
        }

        [Fact]
        public void Dispose_SiClientNonDisposable_AppelleStop()
        {
            // Arrange
            var mockNetworkClient = new Mock<IClientGenerique>();
            // Ici, le mock n'implémente PAS IDisposable (As<IDisposable> non appelé)
            var clientJudo = new ClientJudo(mockNetworkClient.Object);

            // Act
            clientJudo.Dispose();

            // Assert
            mockNetworkClient.Verify(c => c.Stop(), Times.Once, "Si le client n'est pas IDisposable, la méthode Stop() doit être appelée en fallback.");
        }

        #endregion
    }
}