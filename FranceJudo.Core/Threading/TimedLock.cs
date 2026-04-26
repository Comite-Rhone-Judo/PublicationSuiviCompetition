using System;
using System.Threading;

namespace FranceJudo.Core.Threading
{
    /// <summary>
    /// Remplace le mot-clé 'lock' pour éviter les deadlocks (gels) infinis.
    /// S'utilise avec un bloc 'using' classique.
    /// ATTENTION : Ne jamais utiliser autour d'un code contenant 'await'.
    /// </summary>
    public readonly ref struct TimedLock
    {
        private readonly object _target;

        // Constructeur privé pour forcer l'utilisation de la méthode de fabrique 'Lock'
        private TimedLock(object target)
        {
            _target = target;
        }

        /// <summary>
        /// Tente d'obtenir un verrou exclusif sur l'objet spécifié pendant le délai de 10 secondes par defaut
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TimedLock Lock(object obj, int TimeoutSec = 10)
        {
            return Lock(obj, TimeSpan.FromSeconds(TimeoutSec <= 0 ? 10 : TimeoutSec));
        }

        /// <summary>
        /// Tente d'obtenir un verrou exclusif sur l'objet spécifié pendant le délai imparti.
        /// </summary>
        /// <param name="obj">L'objet servant de jeton de verrouillage (jamais this, jamais un type valeur)</param>
        /// <param name="timeout">Le temps d'attente maximum</param>
        /// <returns>Une structure jetable qui libérera le verrou à la fin du bloc using</returns>
        /// <exception cref="ArgumentNullException">Si l'objet de verrouillage est nul</exception>
        /// <exception cref="TimeoutException">Si le verrou n'est pas obtenu dans le temps imparti</exception>
        public static TimedLock Lock(object obj, TimeSpan timeout)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "L'objet de verrouillage ne peut pas être null.");

            bool lockTaken = false;
            try
            {
                // Surcharge atomique garantie par le framework .NET (Evite les fuites de verrou)
                Monitor.TryEnter(obj, timeout, ref lockTaken);

                if (!lockTaken)
                {
                    // Au lieu de geler l'application, on fait exploser une erreur traçable
                    throw new TimeoutException($"Impossible d'obtenir le verrou après {timeout.TotalSeconds}s. Deadlock potentiel évité.");
                }

                return new TimedLock(obj);
            }
            catch
            {
                // Sécurité absolue : si une erreur système survient juste après avoir pris le verrou, on le relâche
                if (lockTaken)
                    Monitor.Exit(obj);

                throw;
            }
        }

        /// <summary>
        /// Libère le verrou. Appelée automatiquement à la fermeture de l'accolade du 'using'.
        /// </summary>
        public void Dispose()
        {
            if (_target != null)
            {
                Monitor.Exit(_target);
            }
        }
    }
}