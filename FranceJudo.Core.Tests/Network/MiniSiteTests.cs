#nullable enable
using System;
using System.Net;
using System.Linq;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Network;

namespace FranceJudo.Core.Tests.Network
{
    public class MiniSiteTests
    {
        #region Bouchons (Stubs) pour l'Architecture

        // Bouchon minimaliste du serveur HTTP pour vérifier l'orchestration
        public class StubServeurHttp : FranceJudo.Core.Network.Http.IServeurHttp
        {
            public IPAddress? ListeningIpAddress { get; set; }
            public int PortMin { get; set; }
            public int PortMax { get; set; }
            public int Port { get; } = 8080;
            public string LocalRootPath { get; set; } = string.Empty;
            public bool IsStart { get; private set; }

            public void Start() => IsStart = true;
            public void Stop() => IsStart = false;
            public void AddModule(object module) { }
        }

        // Bouchon de la classe abstraite MiniSite
        public class StubMiniSite : MiniSite
        {
            public StubMiniSite(bool local, FranceJudo.Core.Network.Http.IServeurHttp? instance = null)
                : base(local, instance)
            {
            }
        }

        #endregion

        [Fact]
        public void Constructeur_LocalSansServeur_LeveArgumentNullException()
        {
            // Act
            Action act = () => new StubMiniSite(local: true, instance: null);

            // Assert
            act.Should().Throw<ArgumentNullException>("En mode local, l'instance du serveur HTTP est obligatoire.");
        }

        [Fact]
        public void SelectInterfaceOrDefault_AdresseInconnue_PrendLaPremiereDispo_OuIgnore()
        {
            // Arrange
            var serveur = new StubServeurHttp();
            var miniSite = new StubMiniSite(true, serveur);

            // Précaution : si la machine de CI/CD n'a aucune interface IPv4 active (ex: uniquement du loopback), le test pourrait échouer
            if (miniSite.InterfacesLocal == null || !miniSite.InterfacesLocal.Any()) return;

            IPAddress fallbackAttendu = miniSite.InterfacesLocal.First();

            // Act : On passe une IP farfelue
            miniSite.SelectInterfaceOrDefault("255.255.255.255");

            // Assert
            miniSite.InterfaceLocalPublication.Should().Be(fallbackAttendu, "Si l'IP est introuvable, le système doit faire un fallback sur la première interface de la machine.");
        }

        [Fact]
        public void InterfaceLocalPublication_AffectationInvalide_LeveException()
        {
            // Arrange
            var serveur = new StubServeurHttp();
            var miniSite = new StubMiniSite(true, serveur);

            // Act
            Action act = () => miniSite.InterfaceLocalPublication = IPAddress.Parse("255.255.255.255"); // IP qui n'est pas sur la machine locale

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>("L'interface assignée DOIT exister dans la liste InterfacesLocal.");
        }

        [Fact]
        public void StartSite_ModeLocal_DemarreLeServeurEtPasseEnEtatListening()
        {
            // Arrange
            var serveur = new StubServeurHttp();
            var miniSite = new StubMiniSite(local: true, serveur);

            // On s'assure qu'on a une IP valide pour le test (sur CI/CD)
            if (miniSite.InterfacesLocal.Any())
            {
                miniSite.InterfaceLocalPublication = miniSite.InterfacesLocal.First();
            }

            // Act
            miniSite.StartSite();

            // Assert
            serveur.IsStart.Should().BeTrue("StartSite doit appeler Start() sur l'instance IServeurHttp interne.");
            miniSite.Status.State.Should().Be(StateMiniSiteEnum.Listening, "Le statut doit basculer sur Listening en mode local.");
            miniSite.IsActif.Should().BeTrue();
        }

        [Fact]
        public void StopSite_ModeLocal_ArreteLeServeurEtPasseEnEtatStopped()
        {
            // Arrange
            var serveur = new StubServeurHttp();
            var miniSite = new StubMiniSite(true, serveur);
            miniSite.StartSite(); // On le démarre d'abord

            // Act
            miniSite.StopSite();

            // Assert
            serveur.IsStart.Should().BeFalse("StopSite doit appeler Stop() sur le serveur local.");
            miniSite.Status.State.Should().Be(StateMiniSiteEnum.Stopped, "L'état final doit être Stopped.");
            miniSite.IsActif.Should().BeFalse();
        }

        [Fact]
        public void SetterDistant_RendentLaConfigurationValide_EtDeclenchentIsChanged()
        {
            // Arrange
            var miniSite = new StubMiniSite(local: false); // Pas besoin de serveur HTTP en distant
            miniSite.IsChanged = false; // Reset du flag

            // Act
            miniSite.SiteFTPDistant = "ftp.france-judo.com";
            miniSite.LoginSiteFTPDistant = "admin";
            miniSite.PasswordSiteFTPDistant = "secret";

            // Assert
            miniSite.IsChanged.Should().BeTrue("Les modificateurs distants doivent marquer l'objet comme modifié.");
            miniSite.IsFTPConfigPropertiesValid.Should().BeTrue("Le Site et le Login étant remplis, la pré-validation doit passer.");
        }
    }
}