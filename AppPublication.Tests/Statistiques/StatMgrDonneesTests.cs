#nullable enable
using AppPublication.Statistiques;
using System.Collections.Generic;
using Xunit;

namespace AppPublication.Tests.Statistiques
{
    public class StatMgrDonneesTests
    {
        [Fact]
        public void Constructeur_InitialiseTousLesCompteurs()
        {
            // Act
            StatMgrDonnees manager = new StatMgrDonnees();
            Dictionary<StatMgrDonnees.CompteurDonneesEnum, StatistiqueItem> compteurs = manager.CompteursDonnees;

            // Assert
            Assert.NotNull(compteurs);
            Assert.True(compteurs.ContainsKey(StatMgrDonnees.CompteurDonneesEnum.NbDemandeSnapshot));
            Assert.True(compteurs.ContainsKey(StatMgrDonnees.CompteurDonneesEnum.NbErreurReceptionDonnees));
        }

        [Fact]
        public void MethodesDenregistrement_IncremententLesBonsCompteurs()
        {
            // Arrange
            StatMgrDonnees manager = new StatMgrDonnees();

            // Act
            manager.EnregistrerDemandeSnapshot();
            manager.EnregistrerConnexion();
            manager.EnregistrerDelaiEchange(155.5);

            // Assert
            // Les compteurs simples : Valeur = nombre d'occurrences
            Assert.Equal(1f, manager.CompteursDonnees[StatMgrDonnees.CompteurDonneesEnum.NbDemandeSnapshot].Valeur);
            Assert.Equal(1f, manager.CompteursDonnees[StatMgrDonnees.CompteurDonneesEnum.NbConnexion].Valeur);

            // Le compteur Moyenneur (DelaiEchange)
            // 'Valeur' contient le nombre d'éléments (ici, 1 seul appel)
            Assert.Equal(1f, manager.CompteursDonnees[StatMgrDonnees.CompteurDonneesEnum.DelaiEchange].Valeur);

            // 'Moy' contient la véritable valeur calculée (ici, 155.5 / 1 = 155.5)
            Assert.Equal(155.5f, manager.CompteursDonnees[StatMgrDonnees.CompteurDonneesEnum.DelaiEchange].Moy);
        }
    }
}