using Tools.Net;

namespace AppPublication.Generation
{
    #region CLASSE ANNEXE

    /// <summary>
    /// Definie les étapes de génération
    /// </summary>
    public enum EtapeGenerateurSiteEnum
    {
        None = -1,
        CleanupInitial = 0,
        Demarrage = 1,
        PrepareGeneration = 2,
        ExecuteGeneration = 3,
        ExecuteSynchronisation = 4
    }

    public class OperationProgress
    {
        /// <summary>
        /// L'étape de génération concernée
        /// </summary>
        public EtapeGenerateurSiteEnum Etape { get; }

        /// <summary>
        /// La progression en % de l'opération
        /// </summary>
        public float ProgressPercent { get; }

        public OperationProgress(EtapeGenerateurSiteEnum etape, float progressPercent)
        {
            Etape = etape;
            ProgressPercent = progressPercent;
        }
    }

    public class ResultatOperation
    {
        /// <summary>
        /// L'étape de génération concernée
        /// </summary>
        public EtapeGenerateurSiteEnum Etape { get;  }

        /// <summary>
        /// Etat final de la generation
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Generation complete ou non
        /// </summary>
        public bool IsComplete { get; }

        /// <summary>
        /// Nombre d'éléments générés (-1 si non applicable)
        /// </summary>
        public long NbElements { get; }

        public ResultatOperation(EtapeGenerateurSiteEnum etape, bool isSuccess, bool isComplete, long nbElements = -1)
        {
            Etape = etape;
            IsSuccess = isSuccess;
            IsComplete = isComplete;
            NbElements = nbElements;
        }
    }

    #endregion

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
        ResultatOperation ExecuteGeneration();

        /// <summary>
        ///  Termine un cycle de generation. Retourne un ResultatGeneration contenant les informations sur la fin de la tache
        /// </summary>
        ResultatOperation ExecuteSynchronisation();
    }
}
