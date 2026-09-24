#nullable enable
using System;
using System.Configuration;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Configuration;

namespace FranceJudo.Core.Tests.Configuration
{
    [Collection("ConfigurationSequential")]
    public class ConfigComponentsTests
    {
        #region Définition de l'Architecture Bouchon (Stubs)

        public class StubElement : ConfigElementBase<StubSection>
        {
            [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
            public string Name
            {
                get => (string)this["name"];
                set => SetValueAndMarkDirty("name", value);
            }
        }

        public class StubCollection : ConfigCollectionBase<StubSection, StubElement>
        {
            protected override object GetElementKey(StubElement element) => element.Name;
        }

        [SectionName("stubTestSection")]
        public class StubSection : ConfigSectionBase<StubSection>
        {
            [ConfigurationProperty("title", DefaultValue = "Init")]
            public string Title
            {
                get => (string)this["title"];
                set => SetValueAndMarkDirty("title", value);
            }

            [ConfigurationProperty("items", IsDefaultCollection = false)]
            public StubCollection Items => (StubCollection)this["items"];
        }

        #endregion

        [Fact]
        public void Instance_ImplementeLeSingletonCorrectement()
        {
            // Act
            var instance1 = StubSection.Instance;
            var instance2 = StubSection.Instance;

            // Assert
            instance1.Should().NotBeNull();
            instance1.Should().BeSameAs(instance2, "La mécanique de Double-Check Locking doit garantir une instance unique.");
            instance1.SectionName.Should().Be("stubTestSection", "L'attribut [SectionName] doit être lu correctement.");
        }

        [Fact]
        public void SetValueAndMarkDirty_ModificationProprieteDirecte_DeclencheL_Evenement_SansToucherAuTick()
        {
            // Arrange
            var section = StubSection.Instance;
            section.ClearDirtyFlag(); // On remet à zéro l'état

            // On s'assure que la valeur va vraiment changer pour déclencher le Dirty
            string nouvelleValeur = "Titre_" + Guid.NewGuid().ToString();
            long tickAvant = section.LastModifiedTick;

            bool eventFired = false;
            void handler(InternalConfigSectionBase s) => eventFired = true;
            InternalConfigSectionBase.SectionBecameDirty += handler;

            try
            {
                // Act : Modification directe sur la racine
                section.Title = nouvelleValeur;

                // Assert
                eventFired.Should().BeTrue("L'événement statique doit remonter pour alerter le ConfigurationService.");
                section.IsDirty.Should().BeTrue("Le flag interne doit être passé à true.");
                section.LastModifiedTick.Should().Be(tickAvant, "Une propriété directe de la racine ne doit pas altérer le LastModifiedTick des enfants.");
            }
            finally
            {
                InternalConfigSectionBase.SectionBecameDirty -= handler;
            }
        }

        [Fact]
        public void NotifyChildModification_MiseAJourParUnEnfant_MetAJourLeTickEtRendDirty()
        {
            // Arrange
            var section = StubSection.Instance;
            section.ClearDirtyFlag();
            long tickAvant = section.LastModifiedTick;

            // Act : On simule la mécanique d'un ConfigElementBase qui modifie ses données
            // Cela va appeler this.LastModifiedTick = DateTime.Now.Ticks
            section.NotifyChildModification();

            // Assert
            section.LastModifiedTick.Should().BeGreaterThan(tickAvant, "Le tick de modification doit avoir été incrémenté par l'enfant.");

            // Et comme on a utilisé SetValueAndMarkDirty pour sauvegarder le Tick, la section DOIT être sale
            section.IsDirty.Should().BeTrue("La mise à jour du Tick technique doit avoir basculé la section entière en Dirty.");
        }

        [Fact]
        public void DeepCopyRecursive_CloneLesProprietesEtCollections()
        {
            // Arrange
            var source = StubSection.Instance;
            source.Title = "CopieMoi";
            source.Items.Clear();
            source.Items.Add(new StubElement { Name = "Enfant1" });

            // On crée une instance vierge via Activator (comme le fait le Fallback du Service)
            var target = (StubSection)Activator.CreateInstance(typeof(StubSection), true)!;

            // Act
            source.CopyValuesTo(target);

            // Assert
            target.Title.Should().Be("CopieMoi");
            target.Items.Count.Should().Be(1);
            target.Items["Enfant1"].Should().NotBeNull("La collection enfant doit avoir été clonée par réflexion.");
        }

        [Fact]
        public void InvalidateContext_DetruitLeSingletonEtForceLeRechargement()
        {
            // Arrange
            var instanceAvant = StubSection.Instance;

            // Act
            InternalConfigSectionBase.InvalidateContext();
            var instanceApres = StubSection.Instance;

            // Assert
            instanceAvant.Should().NotBeSameAs(instanceApres, "InvalidateContext doit avoir vidé le cache statique (_instance = null), forçant la recréation de l'objet.");
        }

        [Fact]
        public void ConfigCollectionBase_MethodesCRUD_ModifientLaCollectionEtNotifientLeParent()
        {
            // Arrange
            var section = StubSection.Instance;
            section.ClearDirtyFlag();
            var collection = section.Items;
            var element = new StubElement { Name = "TestAdd" };

            // Act 1 : Add
            collection.Add(element);

            // Assert 1
            section.IsDirty.Should().BeTrue("L'ajout doit propager la notification au parent.");

            // Act 2 : Remove
            section.ClearDirtyFlag();
            collection.Remove(element);

            // Assert 2
            section.IsDirty.Should().BeTrue("La suppression par objet doit notifier le parent.");

            // Act 3 : Remove par clé
            collection.Add(new StubElement { Name = "KeyRemove" });
            section.ClearDirtyFlag();
            collection.Remove("KeyRemove");
            section.IsDirty.Should().BeTrue("La suppression par clé doit notifier le parent.");

            // Act 4 : Clear
            collection.Add(new StubElement { Name = "ToClear" });
            section.ClearDirtyFlag();
            collection.Clear();
            section.IsDirty.Should().BeTrue("Le vidage complet doit notifier le parent.");
        }

        [Fact]
        public void ConfigCollectionBase_Indexeur_SetEtGet_ManipulentLesElementsProprement()
        {
            // Arrange
            var section = StubSection.Instance;
            var collection = section.Items;
            collection.Clear();
            collection.Add(new StubElement { Name = "Index0" });
            section.ClearDirtyFlag();

            // Act
            // Utilise le set de l'indexeur numérique
            collection[0] = new StubElement { Name = "NewIndex0" };

            // Utilise le get de l'indexeur par chaîne
            var fromStringKey = collection["NewIndex0"];

            // Assert
            collection[0].Name.Should().Be("NewIndex0");
            fromStringKey.Should().NotBeNull();
            section.IsDirty.Should().BeTrue("L'affectation via indexeur doit rendre la section Dirty.");
        }

        [Fact]
        public void InternalConfigSectionBase_GetConfigValue_ValeursNullesOuInvalides_RetourneDefaut()
        {
            // L'Architecte utilise la réflexion ici car GetConfigValue est protected.
            // On veut couvrir le cas "catch" (conversion impossible).
            var section = StubSection.Instance;
            var method = typeof(InternalConfigSectionBase).GetMethod("GetConfigValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var genericMethod = method!.MakeGenericMethod(typeof(int));

            // On triche : on cherche à lire "title" (qui contient du texte) en demandant un int.
            // Cela va faire exploser le Convert.ChangeType et déclencher le catch interne !
            var resultat = genericMethod.Invoke(section, new object[] { "title", 42 });

            resultat.Should().Be(42, "L'erreur de casting doit être attrapée et renvoyer la valeur par défaut.");
        }
    }
}