namespace AppPublication.Generation
{
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
}