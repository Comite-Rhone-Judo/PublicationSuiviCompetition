using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelImpl
{
    /// <summary>
    /// Contrat de service pour le gestionnaire de données Judo.
    /// Supporte l'injection de dépendance.
    /// </summary>
    public interface IJudoDataManager
    {
        /// <summary>
        /// Obtient une vue figée et thread-safe des données (Lecture).
        /// </summary>
        IJudoData Snapshot { get; }

        /// <summary>
        /// Vérifie et assure la cohérence des données, retourne True si les données sont cohérentes.
        /// </summary>
        bool EnsureDataConsistency();

        /// <summary>
        /// Exécute une modification des données de manière sécurisée (Écriture).
        /// </summary>
        void RunSafeDataUpdate(Action actionMiseAJour);

        /// <summary>
        /// Obtient l'accès direct a la source de donnees
        /// </summary>
        IJudoData Data { get; }
    }
}
