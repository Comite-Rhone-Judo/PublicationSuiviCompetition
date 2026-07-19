namespace AppPublication.Generation
{
    public class ResultatOperation
    {
        /// <summary>
        /// L'étape de génération concernée
        /// </summary>
        public EtapeGenerateurSiteEnum Etape { get; }

        // Indique si l'etape de synchronisation a ete executee (ex. sync arretee)
        public bool IsActive { get; }

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


        /// <summary>
        /// Nombre d'éléments total générés (-1 si non applicable)
        /// </summary>
        public long NbElementsTotal { get; private set; }

        public ResultatOperation(EtapeGenerateurSiteEnum etape, bool isSuccess, bool isComplete, long nbElements = -1, long nbElementsTotal = -1)
        {
            Etape = etape;
            IsSuccess = isSuccess;
            IsActive = true;
            IsComplete = isComplete;
            NbElements = nbElements;
            NbElementsTotal = nbElementsTotal;
            NbElements = nbElements;
        }

        public ResultatOperation(EtapeGenerateurSiteEnum etape, bool isActive)
        {
            Etape = etape;
            IsSuccess = true;
            IsActive = isActive;
            IsComplete = true;
            NbElements = -1;
            NbElementsTotal = -1;
        }
    }
}