using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Environment;

namespace FranceJudo.Core.Tests.Environment
{
    public class AppInformationTests
    {
        [Fact]
        public void Instance_AccesConcurrentMassif_GarantitUneSeuleInstanceMemory()
        {
            var instances = new ConcurrentBag<AppInformation>();

            Parallel.For(0, 100, i =>
            {
                instances.Add(AppInformation.Instance);
            });

            AppInformation premiereInstance = instances.ToArray()[0];
            instances.Should().AllSatisfy(inst => inst.Should().BeSameAs(premiereInstance));
        }

        [Fact]
        public void Proprietes_SontHydrateesAvecLesDonneesDeLEnvironnement()
        {
            // Act
            var info = AppInformation.Instance;

            // Assert
            // On vérifie que le Singleton a bien fait appel au helper AppEnvironment
            info.AppVersion.Should().Be(AppEnvironment.GetVersionInformation());
            info.AppCompany.Should().Be(AppEnvironment.GetCompanyInformation());
            info.AppCopyright.Should().Be(AppEnvironment.GetCopyrightInformation());
            info.AppTrademark.Should().Be(AppEnvironment.GetTrademarkInformation());
        }

        [Fact]
        public void ModificationPropriete_EmetL_Evenement_PropertyChanged_AvecLeBonNom()
        {
            var info = AppInformation.Instance;
            string versionOriginale = info.AppVersion;
            bool versionModifiee = false;

            info.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AppInformation.AppVersion))
                {
                    versionModifiee = true;
                }
            };

            PropertyInfo? propInfo = typeof(AppInformation).GetProperty(nameof(AppInformation.AppVersion));

            try
            {
                propInfo?.SetValue(info, "VersionPiratee");

                versionModifiee.Should().BeTrue();
                info.AppVersion.Should().Be("VersionPiratee");
            }
            finally
            {
                // Nettoyage impératif pour ne pas casser le Singleton en mémoire
                propInfo?.SetValue(info, versionOriginale);
            }
        }
    }
}