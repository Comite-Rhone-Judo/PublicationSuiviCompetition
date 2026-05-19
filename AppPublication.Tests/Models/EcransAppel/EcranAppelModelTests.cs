#nullable enable
using AppPublication.Models.EcransAppel;
using System.Collections.Generic;
using System.Net;
using Xunit;
using static AppPublication.Models.EcransAppel.EcranAppelModel;

namespace AppPublication.Tests.Models.EcransAppel
{
    public class EcranAppelModelTests
    {
        [Fact]
        public void Constructeur_ValeursParDefaut_SontCorrectes()
        {
            // Act
            EcranAppelModel modele = new EcranAppelModel();

            // Assert
            Assert.Equal(0, modele.Id);
            Assert.Equal("Nouvel Écran", modele.Description);
            Assert.Equal("", modele.Hostname);
            Assert.Equal(IPAddress.None, modele.AdresseIP);
            Assert.NotNull(modele.TapisIds);
            Assert.Empty(modele.TapisIds);
            Assert.Equal(1, modele.Groupement);
            Assert.Equal(DispositionAffichage.Colonne, modele.Disposition);
            Assert.Equal(DispositionAffichage.Colonne, modele.DispositionCombat);
            Assert.False(modele.AjusteTailleTexte);
            Assert.Equal(8, modele.NbCombatsPage);
        }

        [Fact]
        public void Clone_CreeUneCopieProfondeDeLaListe()
        {
            //  
            List<int> tapisInitiaux = new List<int> { 1, 2, 3 };
            EcranAppelModel original = new EcranAppelModel(
                id: 42,
                description: "Ecran Tapis 1 à 3",
                hostname: "PC-ECRAN",
                adresseIP: IPAddress.Loopback,
                tapisIds: tapisInitiaux
            );

            // Act
            EcranAppelModel copie = original.Clone();
            copie.TapisIds.Add(4); // Modification de la copie
            copie.Id = 99;

            // Assert
            Assert.NotSame(original, copie);
            Assert.Equal(42, original.Id);
            Assert.Equal(99, copie.Id);

            // Vérification CRITIQUE : la liste d'origine ne doit pas contenir le "4"
            Assert.Equal(3, original.TapisIds.Count);
            Assert.Equal(4, copie.TapisIds.Count);
        }
    }
}