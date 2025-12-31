
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using Tools.Logging;
using KernelImpl.Internal;

namespace KernelImpl.Noyau.Arbitrage
{
    public class DataArbitrage : IArbitrageData
    {
        // 1. Le stockage privé est le "DataCache"
        private readonly DeduplicatedCachedData<int, Commissaire> _commissairesCache = new DeduplicatedCachedData<int, Commissaire>();
        private readonly DeduplicatedCachedData<int, Arbitre> _arbitresCache = new DeduplicatedCachedData<int, Arbitre>();
        private readonly DeduplicatedCachedData<int, Delegue> _deleguesCache = new DeduplicatedCachedData<int, Delegue>();

        // 2. L'interface publique est JUSTE la liste O(1)
        public IReadOnlyList<Commissaire> Commissaires { get { return _commissairesCache.Cache; } }
        public IReadOnlyList<Arbitre> Arbitres { get { return _arbitresCache.Cache; } }
        public IReadOnlyList<Delegue> Delegues { get { return _deleguesCache.Cache; } }

        /// <summary>
        /// lecture des commissaires
        /// </summary>
        /// <param name="element">element XML contenant les commissaires</param>
        /// <param name="DC"></param>
        public void lecture_commissaires(XElement element)
        {
            ICollection<Commissaire> commissaires = Commissaire.LectureCommissaire(element, null);
            _commissairesCache.UpdateFullSnapshot(commissaires);
        }

        /// <summary>
        /// lecture des arbitres
        /// </summary>
        /// <param name="element">element XML contenant les arbitres</param>
        /// <param name="DC"></param>
        public void lecture_arbitres(XElement element)
        {
            ICollection<Arbitre> arbitres = Arbitre.LectureArbitre(element, null);
            _arbitresCache.UpdateFullSnapshot(arbitres); 
        }

        /// <summary>
        /// lecture des delegues
        /// </summary>
        /// <param name="element">element XML contenant les delegues</param>
        /// <param name="DC"></param>
        public void lecture_delegues(XElement element)
        {
            ICollection<Delegue> delegues = Delegue.LectureDelegue(element, null);
            _deleguesCache.UpdateFullSnapshot(delegues);
        }
    }
}
