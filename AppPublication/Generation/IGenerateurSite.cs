using Tools.Net;

namespace AppPublication.Generation
{
    public interface IGenerateurSite
    {
        /// <summary>
        /// Nettoyage initial avant la generation
        /// </summary>
        void CleanupInitial();

        /// <summary>
        /// Execute les taches au 1er demarrage de la generation
        /// </summary>
        void Demarrage();

        /// <summary>
        /// Prepare la session de generation. Retourne true si la generation peut commencer.
        /// </summary>
        /// <returns></returns>
        bool PrepareGeneration();


        /// <summary>
        /// Execute la generation. Retourne True si la generation s'est bien passee.
        /// </summary>
        bool ExecuteGeneration();

        /// <summary>
        ///  Termine un cycle de generation
        /// </summary>
        UploadStatus ExecuteSynchronisation();
    }
}
