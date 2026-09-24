using System;
using System.ComponentModel;
using Xunit;
using FluentAssertions;
using FranceJudo.Core.Foundation;

namespace FranceJudo.Core.Tests.Foundation
{
    public class NotificationBaseTests
    {
        // Classe bouchon pour tester la classe abstraite
        private class TestModel : NotificationBase
        {
            private string _nom = string.Empty;
            public string Nom
            {
                get => _nom;
                set
                {
                    if (_nom != value)
                    {
                        _nom = value;
                        // Le CallerMemberName va automatiquement injecter "Nom"
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        [Fact]
        public void NotifyPropertyChanged_Declenche_EvenementLocalEtGlobal()
        {
            // Arrange
            var model = new TestModel();
            bool localEventFired = false;
            bool globalEventFired = false;
            string? propertyNameReceived = string.Empty;

            // Abonnement local
            model.PropertyChanged += (sender, args) =>
            {
                localEventFired = true;
                propertyNameReceived = args.PropertyName;
            };

            // Abonnement statique global
            void globalHandler() => globalEventFired = true;
            NotificationBase.OnPropertyModifiedGlobally += globalHandler;

            try
            {
                // Act : La modification de la propriété appelle NotifyPropertyChanged
                model.Nom = "Teddy Riner";

                // Assert
                localEventFired.Should().BeTrue("L'événement local INotifyPropertyChanged doit être déclenché.");
                propertyNameReceived.Should().Be("Nom", "Le CallerMemberName doit avoir capturé le nom de la propriété.");
                globalEventFired.Should().BeTrue("Le délégué statique global doit être notifié.");
            }
            finally
            {
                // Nettoyage impératif du délégué statique
                NotificationBase.OnPropertyModifiedGlobally -= globalHandler;
            }
        }
    }
}