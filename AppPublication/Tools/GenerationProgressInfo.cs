using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Tools
{
    public class GenerationProgressInfo
    {
        public static GenerationProgressInfo InitInstance(int id, int nbGeneration)
        {
            return new GenerationProgressInfo(id, -1, nbGeneration);
        }
        public static GenerationProgressInfo ProgressInstance(int id, int progress)
        {
            return new GenerationProgressInfo(id, progress, -1);
        }

        private GenerationProgressInfo(int id, int progress = -1, int nbSubTask = -1)
        {
            Id = id;
            Progress = progress;
            NbGeneration = nbSubTask;
        }

        public bool IsInit
        {
            get
            {
                return NbGeneration > 0 && Progress <= -1 ;
            }
        }

        public bool IsProgress
        {
            get
            {
                return NbGeneration <= -1 && Progress > 0;
            }
        }

        public int Id { get; set; }
        public int Progress { get; set; }

        public int NbGeneration {  get; set; }
    }
}
