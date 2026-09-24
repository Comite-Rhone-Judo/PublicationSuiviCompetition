#nullable enable
using AppPublication.Statistiques;
using System;
using Xunit;

namespace AppPublication.Tests.Statistiques
{
    public class TaskExecutionInformationTests
    {
        [Fact]
        public void Constructeur_ValeursParDefaut_SontCorrectes()
        {
            // Act
            TaskExecutionInformation info = new TaskExecutionInformation();

            // Assert
            Assert.False(info.IsSuccess);
            Assert.Equal(0, info.DelaiExecutionMs);
            Assert.Equal(DateTime.MinValue, info.DateProchaineGeneration);

            // On s'assure que la date de démarrage est bien "Maintenant" (à une seconde près pour éviter les faux positifs)
            TimeSpan differenceDeTemps = DateTime.Now - info.DateDemarrage;
            Assert.True(differenceDeTemps.TotalSeconds < 1);
        }

        [Fact]
        public void Setters_DeclenchentINotifyPropertyChanged()
        {
            // Arrange
            TaskExecutionInformation info = new TaskExecutionInformation();
            bool notificationRecue = false;
            info.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(info.IsSuccess)) notificationRecue = true;
            };

            // Act
            info.IsSuccess = true;

            // Assert
            Assert.True(notificationRecue);
            Assert.True(info.IsSuccess);
        }
    }
}