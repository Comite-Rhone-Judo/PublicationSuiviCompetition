#nullable enable
using System;
using System.Threading;

namespace FranceJudo.UI.Tests
{
    /// <summary>
    /// Classe de base pour les tests unitaires UI.
    /// Fournit le contexte de thread STA nécessaire aux composants WPF.
    /// </summary>
    public abstract class WpfTestBase
    {
        /// <summary>
        /// Exécute une action de test dans un thread configuré en STA (Single-Threaded Apartment).
        /// </summary>
        /// <param name="action">La logique de test à exécuter.</param>
        protected void RunInSTA(Action action)
        {
            Exception? threadEx = null;

            Thread thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    threadEx = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join(); // Attente de la fin de l'exécution du test

            if (threadEx != null)
            {
                throw new Exception("Erreur d'exécution dans le thread STA de l'UI.", threadEx);
            }
        }
    }
}