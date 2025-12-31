using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Outils
{
    public enum BatchProgressType
    {
        Initialization, // Définit le nombre total d'étapes (le poids de la tâche)
        Progress,       // Signale une avancée (étape actuelle ou incrément)
    }

    public class BatchProgressInfo
    {
        public BatchProgressType Type { get; set; }

        private BatchProgressInfo(BatchProgressType type, int val)
        {
            Type = type;
            Value = val;
        }

        /// <summary>
        /// Si Type == Initialization : c'est le TotalSteps.
        /// Si Type == Progress : c'est le CurrentStep.
        /// </summary>
        public int Value { get; set; }


        // Helpers pour simplifier l'écriture dans les tâches
        public static BatchProgressInfo Init(int total)
            => new BatchProgressInfo ( BatchProgressType.Initialization, total );

        public static BatchProgressInfo Step(int current)
            => new BatchProgressInfo (BatchProgressType.Progress, current);
    }
}