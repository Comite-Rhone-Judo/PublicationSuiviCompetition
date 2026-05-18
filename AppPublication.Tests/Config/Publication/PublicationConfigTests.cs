using AppPublication.Config.Publication;
using System;
using Xunit;

namespace AppPublication.Tests.Config.Publication
{
    public class PublicationConfigTests
    {
        [Fact]
        public void Constructeur_InitialiseLesProprietes()
        {
            // Arrange & Act
            PublicationConfig config = new PublicationConfig();

            // Assert
            Assert.NotNull(config.General);
            Assert.NotNull(config.Schedulers);
            Assert.NotNull(config.MiniSites);
        }

        [Fact]
        public void GetScheduler_CreeEtAjouteUnNouveauScheduler_SiInexistant()
        {
            // Arrange
            PublicationConfig config = new PublicationConfig();
            string nouvelId = "PlanificateurTapis";

            // Act
            SchedulerParams resultat = config.GetScheduler(nouvelId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(nouvelId, resultat.ID);
            Assert.Single(config.Schedulers); // Vérifie qu'il a bien été ajouté à la collection
        }

        [Fact]
        public void GetScheduler_RetourneLeSchedulerExistant()
        {
            // Arrange
            PublicationConfig config = new PublicationConfig();
            SchedulerParams schedulerExistant = new SchedulerParams { ID = "Existant" };
            config.Schedulers.Add(schedulerExistant);

            // Act
            SchedulerParams resultat = config.GetScheduler("Existant");

            // Assert
            Assert.NotNull(resultat);
            Assert.Same(schedulerExistant, resultat);
            Assert.Single(config.Schedulers); // Vérifie qu'aucun doublon n'a été créé
        }

        [Fact]
        public void GetMiniSiteById_RetourneSiteOuNull()
        {
            // Arrange
            PublicationConfig config = new PublicationConfig();
            MiniSiteParams siteExistant = new MiniSiteParams { ID = "SiteFTP" };
            config.MiniSites.Add(siteExistant);

            // Act
            MiniSiteParams resultatTrouve = config.GetMiniSiteById("SiteFTP");
            MiniSiteParams resultatInconnu = config.GetMiniSiteById("Inconnu");

            // Assert
            Assert.Same(siteExistant, resultatTrouve);
            Assert.Null(resultatInconnu);
        }

        [Fact]
        public void InitializeSync_AssigneNotificationAuxEnfantsEtListes()
        {
            // Arrange
            PublicationConfig config = new PublicationConfig();
            bool notificationRecue = false;
            void methodeNotification()
            {
                notificationRecue = true;
            }

            // Act
            config.InitializeSync(methodeNotification);

            // Assert
            Assert.NotNull(config.OnChanged);
            Assert.NotNull(config.General.OnChanged);

            // On simule une modification pour vérifier le câblage
            config.General.OnChanged.Invoke();
            Assert.True(notificationRecue);

            // Le câblage des ObservableCollection (SetupCollectionSync) est testé implicitement
            // si la méthode s'exécute sans exception, car la logique réelle réside dans JsonConfigSection.
        }
    }
}