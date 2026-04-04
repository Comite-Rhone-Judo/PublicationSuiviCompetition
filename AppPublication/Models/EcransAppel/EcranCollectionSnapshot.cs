using System.Collections.Generic;
using System.Linq;

namespace AppPublication.Models.EcransAppel
{
    // Objet immuable servant de photographie
    public class EcranCollectionSnapshot
    {
        public IReadOnlyList<EcranAppelModel> Ecrans { get; }
        public EcranAppelModel Default { get; }

        public EcranCollectionSnapshot(IEnumerable<EcranAppelModel> ecrans, EcranAppelModel defaultEcran)
        {
            Ecrans = ecrans.ToArray();
            Default = defaultEcran;
        }
    }
}