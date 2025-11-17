
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Arbitrage
{
    public class DataArbitrage
    {
        private ConcurrentDictionary<int, Commissaire> _commissaires = new ConcurrentDictionary<int, Commissaire>();

        // private IList<Commissaire> _commissaires = new List<Commissaire>();
        public IList<Commissaire> Commissaires { get { return _commissaires.Values.ToList(); } }

        private ConcurrentDictionary<int, Arbitre> _arbitres = new ConcurrentDictionary<int, Arbitre>();
        // private IList<Arbitre> _arbitres = new List<Arbitre>();
        public IList<Arbitre> Arbitres { get { return _arbitres.Values.ToList(); } }

        private ConcurrentDictionary<int, Delegue> _delegues = new ConcurrentDictionary<int, Delegue>();
        // private IList<Delegue> _delegues = new List<Delegue>();
        public IList<Delegue> Delegues { get { return _delegues.Values.ToList(); } }

        /// <summary>
        /// lecture des commissaires
        /// </summary>
        /// <param name="element">element XML contenant les commissaires</param>
        /// <param name="DC"></param>
        public void lecture_commissaires(XElement element)
        {
            ICollection<Commissaire> commissaires = Commissaire.LectureCommissaire(element, null);
            CollectionHelper.UpdateConcurrentCollection(ref _commissaires, commissaires, o => o.id);
        }

        /// <summary>
        /// lecture des arbitres
        /// </summary>
        /// <param name="element">element XML contenant les arbitres</param>
        /// <param name="DC"></param>
        public void lecture_arbitres(XElement element)
        {
            ICollection<Arbitre> arbitres = Arbitre.LectureArbitre(element, null);
            CollectionHelper.UpdateConcurrentCollection(ref _arbitres, arbitres, o => o.id);
        }

        /// <summary>
        /// lecture des delegues
        /// </summary>
        /// <param name="element">element XML contenant les delegues</param>
        /// <param name="DC"></param>
        public void lecture_delegues(XElement element)
        {
            ICollection<Delegue> delegues = Delegue.LectureDelegue(element, null);
            CollectionHelper.UpdateConcurrentCollection(ref _delegues, delegues, o => o.id);
        }
    }
}
