using System;
using System.Collections.ObjectModel;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Configuration.Json;

namespace FranceJudo.Core.Tests.Configuration.Json
{
    public class JsonConfigSectionTests
    {
        private class DummyItem : JsonConfigElement { }

        private class DummySection : JsonConfigSection
        {
            public ObservableCollection<DummyItem> Items { get; } = new ObservableCollection<DummyItem>();

            public DummySection()
            {
                // On simule ce que fait ta classe fille
            }

            public void InitializeSync(Action notifyAction)
            {
                SetupCollectionSync(Items, notifyAction);
            }
        }

        [Fact]
        public void SetupCollectionSync_AjoutElement_DeclencheNotificationEtAbonneLEnfant()
        {
            // Arrange
            var section = new DummySection();
            int notificationCount = 0;
            section.InitializeSync(() => notificationCount++);

            var nouvelItem = new DummyItem();

            // Act 1 : Ajout dans la collection
            section.Items.Add(nouvelItem);

            // Assert 1
            notificationCount.Should().Be(1, "L'ajout d'un élément dans la collection doit déclencher la sauvegarde.");
            nouvelItem.OnChanged.Should().NotBeNull("Le nouvel élément doit avoir été automatiquement abonné au système de notification.");

            // Act 2 : Modification interne de l'élément
            nouvelItem.OnChanged.Invoke();

            // Assert 2
            notificationCount.Should().Be(2, "La modification d'un élément enfant doit remonter jusqu'à la racine.");
        }
    }
}