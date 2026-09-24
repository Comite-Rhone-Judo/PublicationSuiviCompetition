using System.Threading.Tasks;

namespace AppPublication.Generation
{
    public interface IGenerateurSite
    {
        /// <summary>
        /// Nettoyage initial avant la generation
        /// </summary>
        ResultatOperation CleanupInitial();

        /// <summary>
        /// Execute les taches au 1er demarrage de la generation
        /// </summary>
        ResultatOperation Demarrage();

        /// <summary>
        /// Prepare la session de generation. Retourne true si la generation peut commencer.
        /// </summary>
        /// <returns></returns>
        ResultatOperation PrepareGeneration();


        /// <summary>
        /// Execute la generation. Retourne un ResultatGeneration contenant les informations sur la fin de la tache
        /// </summary>
        Task<ResultatOperation> ExecuteGeneration();

        /// <summary>
        ///  Termine un cycle de generation. Retourne un ResultatGeneration contenant les informations sur la fin de la tache
        /// </summary>
        Task<ResultatOperation> ExecuteSynchronisation();
    }
}
