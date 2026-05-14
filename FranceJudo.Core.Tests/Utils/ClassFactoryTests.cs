using System;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Utils;

namespace FranceJudo.Core.Tests.Utils
{
    public class ClassFactoryTests : IDisposable
    {
        // Le constructeur joue le rôle du [SetUp]
        public ClassFactoryTests()
        {
            // On s'assure que la liste statique est vierge avant chaque test
            ClassFactory.AssembliesExternes.Clear();
        }

        // Le Dispose joue le rôle du [TearDown]
        public void Dispose()
        {
            ClassFactory.AssembliesExternes.Clear();
        }

        #region Tests - Cas Nominaux

        [Fact]
        public void CreateInstance_NomCourt_InstancieCorrectement()
        {
            // Act
            // Le test runner appelle la méthode, la factory va donc chercher dans l'assembly appelant (le projet de test)
            var instance = ClassFactory.CreateInstance<ICombatTestService>("CombatTestServiceNominal");

            // Assert
            instance.Should().NotBeNull();
            instance.Should().BeOfType<CombatTestServiceNominal>();
        }

        [Fact]
        public void CreateInstance_NomComplet_InstancieCorrectement()
        {
            // Arrange
            string nomComplet = "FranceJudo.Core.Tests.Utils.CombatTestServiceNominal";

            // Act
            var instance = ClassFactory.CreateInstance<ICombatTestService>(nomComplet);

            // Assert
            instance.Should().NotBeNull();
            instance.Should().BeOfType<CombatTestServiceNominal>();
        }

        #endregion

        #region Tests - Cas d'Erreur (Exceptions)

        [Fact]
        public void CreateInstance_TypeIntrouvable_LeveTypeLoadException()
        {
            // Arrange
            string nomFantome = "ServiceJudoInexistant";

            // Act
            Action act = () => ClassFactory.CreateInstance<ICombatTestService>(nomFantome);

            // Assert (FluentAssertions permet de tester les exceptions très proprement)
            act.Should().Throw<TypeLoadException>()
               .WithMessage($"*est introuvable dans les assemblies analyses*");
        }

        [Fact]
        public void CreateInstance_TypeIncompatible_LeveInvalidCastException()
        {
            // Act
            // On demande l'interface ICombatTestService, mais on passe une classe qui ne l'implémente pas
            Action act = () => ClassFactory.CreateInstance<ICombatTestService>("ServiceIncompatible");

            // Assert
            act.Should().Throw<InvalidCastException>()
               .WithMessage($"*n'herite pas de*");
        }

        [Fact]
        public void CreateInstance_NomsAmbigus_LeveAmbiguousMatchException()
        {
            // Act
            // Deux classes s'appellent "ServiceAmbigu" dans deux namespaces différents
            Action act = () => ClassFactory.CreateInstance<ICombatTestService>("ServiceAmbigu");

            // Assert
            act.Should().Throw<System.Reflection.AmbiguousMatchException>()
               .WithMessage($"*Plusieurs classes portent le nom*");
        }

        #endregion
    }

    #region Bouchons de Test (Stubs)

    // Ces classes ne servent qu'à nourrir les tests ci-dessus.
    // Elles miment la structure de FranceJudo sans en avoir la complexité.

    public interface ICombatTestService { }

    public class CombatTestServiceNominal : ICombatTestService { }

    public class ServiceIncompatible { } // N'implémente pas l'interface !

    // Cas pour tester l'AmbiguousMatchException
    namespace EspaceA
    {
        public class ServiceAmbigu : ICombatTestService { }
    }

    namespace EspaceB
    {
        public class ServiceAmbigu : ICombatTestService { }
    }

    #endregion
}