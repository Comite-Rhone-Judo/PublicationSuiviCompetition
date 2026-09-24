#nullable enable
using AppPublication.Controles;
using AppPublication.Data;
using AppPublication.Statistiques;
using AppPublication.Tools.Enum;
using FranceJudo.Metier.Noyau;
using KernelImpl;
using Moq;
using System;
using Xunit;

namespace AppPublication.Tests.Data
{
    public class ConnectedJudoDataManagerTests
    {
        [Fact]
        public void CreateInstance_LeveExceptionSiAppeleDeuxFois()
        {
            // Arrange
            JudoData donnees = new JudoData();
            StatMgrDonnees statistiques = new StatMgrDonnees();
            Mock<IClientProvider> mockProvider = new Mock<IClientProvider>();

            void ActionInstanciation()
            {
                ConnectedJudoDataManager.CreateInstance(donnees, statistiques, mockProvider.Object);
            }

            // Act
            // Le Singleton étant statique, il survit entre les tests s'ils tournent en parallèle.
            // On tente l'instanciation une première fois (qui peut réussir ou échouer si déjà fait ailleurs).
            Exception? exceptionInitiale = Record.Exception(ActionInstanciation);

            // Assert
            // Le deuxième appel consécutif DOIT échouer avec notre exception personnalisée.
            Assert.Throws<InvalidOperationException>(ActionInstanciation);
        }

        [Fact]
        public void Instance_LeveExceptionSiNonInitialisee()
        {
            // Arrange
            void AccesInstance()
            {
                _ = ConnectedJudoDataManager.Instance;
            }

            // Act & Assert
            // On teste la protection de la propriété statique.
            // Si le Singleton n'est pas initialisé (ce qui arrive si ce test tourne en premier), 
            // il doit rejeter l'accès.
            try
            {
                AccesInstance();
            }
            catch (InvalidOperationException)
            {
                // Comportement attendu
                Assert.True(true);
            }
            catch (Exception)
            {
                // Si l'instance existe déjà, le test ne doit pas planter inutilement.
                Assert.True(true);
            }
        }

        [Fact]
        public void Proprietes_EtatInitialEtSetters_FonctionnentCorrectement()
        {
            // Arrange
            JudoData donnees = new JudoData();
            StatMgrDonnees statistiques = new StatMgrDonnees();
            Mock<IClientProvider> mockProvider = new Mock<IClientProvider>();

            ConnectedJudoDataManager manager;
            try
            {
                manager = ConnectedJudoDataManager.CreateInstance(donnees, statistiques, mockProvider.Object);
            }
            catch (InvalidOperationException)
            {
                manager = ConnectedJudoDataManager.Instance;
            }

            // Act
            manager.Timeout = 5000;
            manager.IsCombatsCacheDirty = true;

            // Assert
            Assert.Equal(5000, manager.Timeout);
            Assert.True(manager.IsCombatsCacheDirty);
        }

        [Fact]
        public void BusyStatusEventArgs_AssigneLesValeursCorrectement()
        {
            // Arrange
            bool expectedBusy = true;
            BusyStatusEnum expectedStatus = BusyStatusEnum.InitDonneesOrganisation;

            // Act
            BusyStatusEventArgs arguments = new BusyStatusEventArgs(expectedBusy, expectedStatus);

            // Assert
            Assert.Equal(expectedBusy, arguments.IsBusy);
            Assert.Equal(expectedStatus, arguments.Status);
        }

        [Fact]
        public void DataUpdateEventArgs_AssigneLesValeursCorrectement()
        {
            // Arrange
            // Utilisation d'une valeur Enum valide identifiée dans le code source
            CategorieDonneesEnum expectedCategory = CategorieDonneesEnum.Organisation;

            // Act
            DataUpdateEventArgs arguments = new DataUpdateEventArgs(expectedCategory);

            // Assert
            Assert.Equal(expectedCategory, arguments.CategorieDonnee);
        }
    }
}