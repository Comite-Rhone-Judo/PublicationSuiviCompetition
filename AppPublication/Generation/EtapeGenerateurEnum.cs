namespace AppPublication.Generation
{
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
}