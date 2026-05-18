using AppPublication.Controles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace AppPublication.Tests.Controles
{
    public class GestionConnectionTests
    {
        [Fact]
        public void Constructeur_ValeursParDefaut_SontCorrectes()
        {
            // Arrange & Act
            GestionConnection connection = new GestionConnection();

            // Assert
            Assert.False(connection.IsConnected);
            Assert.False(connection.HasErreurTransmission);
            Assert.Null(connection.Client);
            Assert.Null(connection.IpAdress);
            Assert.Null(connection.Port);
        }

        [Fact]
        public void HasErreurTransmission_Set_DeclencheNotifyPropertyChanged()
        {
            // Arrange
            GestionConnection connection = new GestionConnection();
            List<string> proprietesModifiees = new List<string>();

            connection.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != null) proprietesModifiees.Add(e.PropertyName);
            };

            // Act
            connection.HasErreurTransmission = true;

            // Assert
            Assert.True(connection.HasErreurTransmission);
            Assert.Contains("HasErreurTransmission", proprietesModifiees);
        }

        [Fact]
        public void DisposeClient_AvecClientNull_NePlantePas()
        {
            // Arrange
            GestionConnection connection = new GestionConnection();

            // Act
            Exception? exception = Record.Exception(delegate ()
            {
                connection.DisposeClient();
            });

            // Assert
            Assert.Null(exception);
            Assert.False(connection.IsConnected);
            Assert.False(connection.HasErreurTransmission);
        }

        [Fact]
        public void TesteConnection_AvecClientNull_NePlantePas()
        {
            // Arrange
            GestionConnection connection = new GestionConnection();

            // Act
            Exception? exception = Record.Exception(delegate ()
            {
                connection.TesteConnection();
            });

            // Assert
            Assert.Null(exception);
        }
    }
}