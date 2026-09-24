#nullable enable
using FluentAssertions;
using FluentFTP;
using FluentFTP.Model.Functions;
using FluentFTP.Rules;
using FranceJudo.Core.Network;
using FranceJudo.Core.Network.Ftp;
using FranceJudo.Core.Network.Http;
using Moq;
using System;
using System.Linq;
using System.Net;
using Xunit;

namespace FranceJudo.Core.Tests.Network
{
    public class MiniSiteTests
    {

        [Fact]
        public void ProprietesDistant_Complement_GetSet_AffectationCorrecte()
        {
            // Arrange - Utilisation du Stub pour la classe abstraite
            StubMiniSite miniSite = new StubMiniSite(local: false)
            {
                // Act - Uniquement les propriétés réellement définies dans MiniSite.cs
                SiteFTPDistant = "ftp.france-judo.com",
                LoginSiteFTPDistant = "admin",
                PasswordSiteFTPDistant = "password123",
                RepertoireSiteFTPDistant = "/public_html/resultats",
                ModeActifFTPDistant = true,
                MaxRetryFTP = 3
            };

            // Assert
            miniSite.SiteFTPDistant.Should().Be("ftp.france-judo.com");
            miniSite.LoginSiteFTPDistant.Should().Be("admin");
            miniSite.PasswordSiteFTPDistant.Should().Be("password123");
            miniSite.RepertoireSiteFTPDistant.Should().Be("/public_html/resultats");
            miniSite.ModeActifFTPDistant.Should().BeTrue();
            miniSite.MaxRetryFTP.Should().Be(3);
        }

        [Fact]
        public void CheckConfigurationSiteDistant_ClientDeconnecte_GereLeComportementProprement()
        {
            // Arrange
            var miniSite = new StubMiniSite(local: false);

            // FtpClient de FluentFTP est bien IDisposable
            using var ftpClient = new FluentFTP.FtpClient("127.0.0.1", "user", "pass");

            // Act
            // Cette méthode utilise en interne 'IFtpConfiguration' implémentée par MiniSite
            Action act = () => miniSite.CheckConfigurationSiteDistant(ftpClient);

            // Assert
            // On vérifie que la logique de validation (Host, Port, etc.) ne lève pas d'exception de référence nulle
            // même si le client n'est pas connecté.
            act.Should().NotThrow<NullReferenceException>("La validation de configuration doit être robuste face à un client non connecté.");
        }

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
            public StubMiniSite(bool local, IServeurHttp? instance = null, IFtpClientFactory? factory = null)
                : base(local, instance, factory) { }
        }

        #endregion

        [Fact]
        public void UploadSite_SiteInactif_AnnuleUploadEtNeTouchePasAuReseau()
        {
            // Arrange
            var mockFactory = new Mock<IFtpClientFactory>();
            // On instancie le site distant, par défaut il n'est pas actif
            var site = new StubMiniSite(false, null, mockFactory.Object);

            // ATTENTION : On NE FAIT PAS site.StartSite() !

            var files = new List<FileInfo> { new FileInfo("dummy.txt") };

            // Act
            var result = site.UploadSite(@"C:\Temp", files);

            // Assert
            result.IsSuccess.Should().BeFalse("Un site qui n'a pas été démarré ne doit pas autoriser l'upload.");

            // La preuve absolue : on vérifie que la fabrique n'a jamais été sollicitée pour créer un client FTP
            mockFactory.Verify(f => f.CreateClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never, "Le réseau ne doit pas être contacté si le site est inactif.");
        }

        [Fact]
        public void UploadSite_SiteActifAvecMockFtp_SimuleUploadReussi()
        {
            // Arrange
            var mockFtpClient = new Mock<IFtpClient>
            {
                // LA MAGIE EST ICI : 
                // Moq va générer des objets bidons au lieu de 'null' pour TOUS les appels 
                // (ex: c.Config, c.AutoDetect(), etc.), ce qui évite toutes les NullReferenceException !
                DefaultValue = DefaultValue.Mock
            };

            // On crée une liste contenant un profil bidon pour satisfaire ta condition de connexion
            var fauxProfils = new List<FtpProfile>
            {
                new FtpProfile()
            };

            // On demande à Moq de retourner cette liste quand AutoDetect est appelé.
            // Note : Adapte le type de It.IsAny<...>() selon le type exact de la variable 'cfg' 
            // (ex: It.IsAny<bool>(), It.IsAny<FtpConfig>(), etc.)
            mockFtpClient.Setup(c => c.AutoDetect(It.IsAny<FtpAutoDetectConfig>())) // Remplace FtpConfig par le type de 'cfg' si besoin
                         .Returns(fauxProfils);

            // On crée un faux résultat d'upload pour tromper la vérification
            var fauxResultatsUpload = new List<FtpResult>
            {
                new FtpResult
                {
                    IsSuccess = true,
                    Name = "test.txt",
                    LocalPath = "C:/tmp",
                    RemotePath = "/out/test.txt"
                }
            };

            // On Setup spécifiquement UploadDirectory pour retourner notre fausse liste
            // Note : Si ça ne compile pas à cause du 6ème paramètre (rules), 
            // vérifie avec l'IntelliSense s'il attend un IList<FtpRule> ou IEnumerable<FtpRule>.
            mockFtpClient.Setup(c => c.UploadDirectory(
                    It.IsAny<string>(),             // localFolder
                    It.IsAny<string>(),             // remoteFolder
                    It.IsAny<FtpFolderSyncMode>(),  // mode
                    It.IsAny<FtpRemoteExists>(),    // existsMode
                    It.IsAny<FtpVerify>(),          // verifyOptions
                    It.IsAny<List<FtpRule>>(),     // rules (souvent IList ou IEnumerable)
                    It.IsAny<Action<FtpProgress>>() // progress
                )).Returns(fauxResultatsUpload);

            mockFtpClient.Setup(c => c.IsConnected).Returns(true);

            // On maintient uniquement les Setup des actions qui ont un VRAI impact sur notre test
            mockFtpClient.Setup(c => c.UploadFile(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FtpRemoteExists>(),
                    It.IsAny<bool>(), It.IsAny<FtpVerify>(), It.IsAny<Action<FtpProgress>>()))
                 .Returns(FtpStatus.Success);

            mockFtpClient.Setup(c => c.MoveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FtpRemoteExists>()))
                         .Returns(true);

            var mockFactory = new Mock<IFtpClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                       .Returns(mockFtpClient.Object);

            var site = new StubMiniSite(false, null, mockFactory.Object)
            {
                SiteFTPDistant = "fake.ftp.com",
                LoginSiteFTPDistant = "user",
                PasswordSiteFTPDistant = "pass",
                RepertoireSiteFTPDistant = "/out",
                SynchroniseDifferences = true
            };

            site.StartSite();

            var tempDir = Path.GetTempPath();
            var fakeFile = new FileInfo(Path.Combine(tempDir, "test.txt"));
            File.WriteAllText(fakeFile.FullName, "dummy content");
            var filesToUpload = new List<FileInfo> { fakeFile };

            try
            {
                // Act
                var result = site.UploadSite(tempDir, filesToUpload);

                // Assert
                result.IsSuccess.Should().BeTrue("L'upload mocké aurait dû réussir maintenant que le site est démarré.");
                result.nbUpload.Should().Be(1);
            }
            finally
            {
                if (File.Exists(fakeFile.FullName)) File.Delete(fakeFile.FullName);
            }
        }

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
            if (miniSite.InterfacesLocal == null || miniSite.InterfacesLocal.Count == 0) return;

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
            if (miniSite.InterfacesLocal.Count > 0)
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
            StubMiniSite miniSite = new StubMiniSite(local: false)
            {
                IsChanged = false, // Reset du flag

                // Act
                SiteFTPDistant = "ftp.france-judo.com",
                LoginSiteFTPDistant = "admin",
                PasswordSiteFTPDistant = "secret"
            }; // Pas besoin de serveur HTTP en distant

            // Assert
            miniSite.IsChanged.Should().BeTrue("Les modificateurs distants doivent marquer l'objet comme modifié.");
            miniSite.IsFTPConfigPropertiesValid.Should().BeTrue("Le Site et le Login étant remplis, la pré-validation doit passer.");
        }
    }
}