using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Generation
{
    public class GenerationProgressInfo
    {
        /// <summary>
        /// Retrourne une instance d'un objet de type GenerationProgressInfo pour l'initialisation d'une tache de génération
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nbGeneration"></param>
        /// <returns></returns>
        public static GenerationProgressInfo InitInstance(int id, int nbGeneration)
        {
            return new GenerationProgressInfo(id, -1, nbGeneration);
        }
        /// <summary>
        /// Retrourne une instance d'un objet de type GenerationProgressInfo pour une tache de génération en cours de progression
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static GenerationProgressInfo ProgressInstance(int id, int progress)
        {
            return new GenerationProgressInfo(id, progress, -1);
        }

        /// <summary>
        /// Constructeur privé pour l'initialisation d'une instance de type GenerationProgressInfo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <param name="nbSubTask"></param>
        private GenerationProgressInfo(int id, int progress = -1, int nbSubTask = -1)
        {
            Id = id;
            Progress = progress;
            NbGeneration = nbSubTask;
        }

        /// <summary>
        /// Retourne Tue si l'objet représente une tache de génération initialisée
        /// </summary>
        public bool IsInit
        {
            get
            {
                return NbGeneration > 0 && Progress <= -1 ;
            }
        }

        /// <summary>
        /// Retourne true si l'objet représente une tache de génération en cours de progression
        /// </summary>
        public bool IsProgress
        {
            get
            {
                return NbGeneration <= -1 && Progress > 0;
            }
        }

        /// <summary>
        /// Identifiant de la tache de génération
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Progression de la tache de génération (en nb de generation realisees)
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Nombre de génération à réaliser
        /// </summary>
        public int NbGeneration {  get; set; }
    }
}
