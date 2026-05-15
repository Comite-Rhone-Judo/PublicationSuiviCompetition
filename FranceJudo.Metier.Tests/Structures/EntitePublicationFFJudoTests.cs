#nullable enable
using System.ComponentModel;
using Xunit;
using FluentAssertions;
using FranceJudo.Metier.Structures;

namespace FranceJudo.Core.Tests.Metier.Structures
{
    public class EntitePublicationFFJudoTests
    {
        [Fact]
        public void Constructeur_InitialiseCorrectementLesProprietes()
        {
            // Arrange & Act
            var entite = new EntitePublicationFFJudo("Nom1", "Lib1", 2, "Log1", "Ftp1", "Http1");

            // Assert
            entite.Nom.Should().Be("Nom1");
            entite.Libelle.Should().Be("Lib1");
            entite.Echelon.Should().Be(2);
            entite.Login.Should().Be("Log1");
            entite.RacineFtp.Should().Be("Ftp1");
            entite.RacineHttp.Should().Be("Http1");
        }

        [Fact]
        public void Setters_DeclenchentL_Evenement_PropertyChanged()
        {
            // Arrange
            var entite = new EntitePublicationFFJudo("", "", 0, "", "", "");

            // On utilise la fonction 'Monitor()' de FluentAssertions pour écouter les événements INotifyPropertyChanged
            using var monitor = entite.Monitor();

            // Act
            entite.Nom = "NouveauNom";
            entite.Libelle = "NouveauLibelle";
            entite.Echelon = 5;
            entite.Login = "NouveauLogin";
            entite.RacineFtp = "NouveauFtp";
            entite.RacineHttp = "NouveauHttp";

            // Assert
            monitor.Should().RaisePropertyChangeFor(x => x.Nom);
            monitor.Should().RaisePropertyChangeFor(x => x.Libelle);
            monitor.Should().RaisePropertyChangeFor(x => x.Echelon);
            monitor.Should().RaisePropertyChangeFor(x => x.Login);
            monitor.Should().RaisePropertyChangeFor(x => x.RacineFtp);
            monitor.Should().RaisePropertyChangeFor(x => x.RacineHttp);
        }
    }
}