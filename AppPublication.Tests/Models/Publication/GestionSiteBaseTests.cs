#nullable enable
using AppPublication.Models.Publication;
using AppPublication.Models.Statistiques;
using AppPublication.Statistiques;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau;
using Moq;
using Xunit;

namespace AppPublication.Tests.Models.Publication
{
    // Bouchon (Stub) pour tester la logique de base
    public class TestableGestionSiteBase : GestionSiteBase
    {
        public bool HookIdCompetitionCalled { get; private set; } = false;
        public bool HookRepertoireRacineCalled { get; private set; } = false;

        public TestableGestionSiteBase(IJudoDataManager dataManager)
            : base(dataManager, new GestionStatistiques()) { }

        public override void InitFromConfigFile() { }
        public override void ForceRefreshUrls() { }
        protected override void OnInterfaceLocalPublicationChanged() { }
        protected override void OnSelectedLogoChanged(string logoName) { }
        protected override void OnSelectedLogoDarkChanged(string logoName) { }
        protected override void UpdateDelaiGenerationConfig(int newValue) { }
        protected override void OnUseLogoUniqueChanged(bool newValue) { }

        protected override void OnUseIntituleCommunChanged(bool newValue) { }

        protected override void OnIntituleCommunChanged(string newValue) { }

        protected override void OnIdCompetitionChanged(string newValue)
        {
            HookIdCompetitionCalled = true;
        }

        protected override void OnRepertoireRacineChanged(string newValue)
        {
            HookRepertoireRacineCalled = true;
        }
    }

    public class GestionSiteBaseTests
    {
        [Fact]
        public void IdCompetition_Setter_ChangeLaValidite_EtDeclencheLeHook()
        {
            // Arrange
            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            TestableGestionSiteBase gestionnaire = new TestableGestionSiteBase(mockDataManager.Object)
            {
                // Act
                IdCompetition = "COMPET-2026"
            };

            // Assert
            Assert.Equal("COMPET-2026", gestionnaire.IdCompetition);
            Assert.True(gestionnaire.IsIdCompetitionValide);
            Assert.True(gestionnaire.HookIdCompetitionCalled);
        }

        [Fact]
        public void IdCompetition_AvecUnknown_RendLaCompetitionInvalide()
        {
            // Arrange
            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            TestableGestionSiteBase gestionnaire = new TestableGestionSiteBase(mockDataManager.Object)
            {
                IdCompetition = "VALIDE" // Set initial
            };

            // Act
            gestionnaire.IdCompetition = GestionSiteBase.kUnknownIdCompetition;

            // Assert
            Assert.False(gestionnaire.IsIdCompetitionValide);
        }

        [Fact]
        public void RepertoireRacine_Setter_DeclencheLeHook()
        {
            // Arrange
            Mock<IJudoDataManager> mockDataManager = new Mock<IJudoDataManager>();
            TestableGestionSiteBase gestionnaire = new TestableGestionSiteBase(mockDataManager.Object)
            {
                // Act
                RepertoireRacine = "C:\\Judo\\Site"
            };

            // Assert
            Assert.Equal("C:\\Judo\\Site", gestionnaire.RepertoireRacine);
            Assert.True(gestionnaire.HookRepertoireRacineCalled);
        }
    }
}