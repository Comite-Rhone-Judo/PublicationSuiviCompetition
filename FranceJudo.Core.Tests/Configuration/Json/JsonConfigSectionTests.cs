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

        [Fact]
        public void SetupCollectionSync_CollectionNull_FaitUnRetourAnticipeSansPlantage()
        {
            // Arrange
            var section = new DummySection();

            // Act : On utilise la réflexion car SetupCollectionSync est protected
            // et la collection de notre DummySection est instanciée en dur.
            var methodInfo = typeof(JsonConfigSection).GetMethod("SetupCollectionSync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var genericMethod = methodInfo!.MakeGenericMethod(typeof(DummyItem));

            Action act = () => genericMethod.Invoke(section, new object[] { null!, (Action)(() => { }) });

            // Assert
            act.Should().NotThrow("La méthode doit gérer les collections nulles gracieusement via son return anticipé.");
        }

        [Fact]
        public void SetupCollectionSync_ElementsDejaPresents_SontAbonnesInitialement()
        {
            // Arrange
            var section = new DummySection();
            var itemInitial = new DummyItem();

            // On ajoute l'élément AVANT de lier la synchronisation (simule un chargement depuis le disque)
            section.Items.Add(itemInitial);

            int notificationCount = 0;

            // Act
            section.InitializeSync(() => notificationCount++);

            // Assert
            itemInitial.OnChanged.Should().NotBeNull("La boucle foreach finale doit abonner les éléments préexistants.");

            // Preuve que l'abonnement fonctionne
            itemInitial.OnChanged.Invoke();
            notificationCount.Should().Be(1, "La modification d'un élément préexistant doit notifier le parent.");
        }

        [Fact]
        public void SetupCollectionSync_SuppressionElement_DeclencheNotificationSansChercherDeNouveauxElements()
        {
            // Arrange
            var section = new DummySection();
            var itemASupprimer = new DummyItem();
            section.Items.Add(itemASupprimer);

            int notificationCount = 0;
            section.InitializeSync(() => notificationCount++);

            // Act : Une suppression génère un événement où e.NewItems est null
            section.Items.Remove(itemASupprimer);

            // Assert
            notificationCount.Should().Be(1, "La modification de structure (suppression) doit appeler notifyAction.");
            // Si la méthode ne gérait pas le (e.NewItems != null), elle lèverait une NullReferenceException ici.
        }
    }
}