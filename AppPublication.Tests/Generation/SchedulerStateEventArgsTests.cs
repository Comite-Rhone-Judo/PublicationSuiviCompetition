#nullable enable
using AppPublication.Generation;
using AppPublication.Statistiques;
using FranceJudo.Core.Diagnostic;
using Xunit;

namespace AppPublication.Tests.Generation
{
    public class SchedulerStateEventArgsTests
    {
        [Fact]
        public void Constructeur_AssigneLesValeurs_Correctement()
        {
            // Arrange
            StateGenerationEnum etatAttendu = StateGenerationEnum.Idle;
            long delaiAttendu = 45;
            TaskExecutionInformation statistiquesAttendues = new TaskExecutionInformation();

            // Act
            SchedulerStateEventArgs arguments = new SchedulerStateEventArgs(etatAttendu, statistiquesAttendues, delaiAttendu);

            // Assert
            Assert.Equal(etatAttendu, arguments.State);
            Assert.Same(statistiquesAttendues, arguments.InfosExecution);
            Assert.Equal(delaiAttendu, arguments.DelaiNextSec);
        }

        [Fact]
        public void Constructeur_ValeursParDefaut_GereLesParametresOptionnels()
        {
            // Act
            // InfosExecution et delaiNextSec sont optionnels dans votre code
            SchedulerStateEventArgs arguments = new SchedulerStateEventArgs(StateGenerationEnum.Generating);

            // Assert
            Assert.Equal(StateGenerationEnum.Generating, arguments.State);
            Assert.Null(arguments.InfosExecution);
            Assert.Equal(long.MinValue, arguments.DelaiNextSec);
        }
    }
}