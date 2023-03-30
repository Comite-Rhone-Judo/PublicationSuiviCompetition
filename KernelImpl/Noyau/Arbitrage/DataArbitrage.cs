
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Arbitrage
{
    public class DataArbitrage
    {
        private IList<Commissaire> _commissaires = new List<Commissaire>();
        public IList<Commissaire> Commissaires { get { return _commissaires; } }

        private IList<Arbitre> _arbitres = new List<Arbitre>();
        public IList<Arbitre> Arbitres { get { return _arbitres; } }

        private IList<Delegue> _delegues = new List<Delegue>();
        public IList<Delegue> Delegues { get { return _delegues; } }

        /// <summary>
        /// lecture des commissaires
        /// </summary>
        /// <param name="element">element XML contenant les commissaires</param>
        /// <param name="DC"></param>
        public void lecture_commissaires(XElement element)
        {
            ICollection<Commissaire> commissaires = Commissaire.LectureCommissaire(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_commissaires as ICollection).SyncRoot))
            {
                foreach (Commissaire commissaire in commissaires)
                {
                    Commissaire p = _commissaires.FirstOrDefault(o => o.id == commissaire.id);
                    if (p != null)
                    {
                        _commissaires.Remove(p);
                    }
                    _commissaires.Add(commissaire);
                }               
            }
        }

        /// <summary>
        /// lecture des arbitres
        /// </summary>
        /// <param name="element">element XML contenant les arbitres</param>
        /// <param name="DC"></param>
        public void lecture_arbitres(XElement element)
        {
            ICollection<Arbitre> arbitres = Arbitre.LectureArbitre(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_arbitres as ICollection).SyncRoot))
            {
                foreach (Arbitre arbitre in arbitres)
                {
                    Arbitre p = _arbitres.FirstOrDefault(o => o.id == arbitre.id);
                    if (p != null)
                    {
                        _arbitres.Remove(p);
                    }
                    _arbitres.Add(arbitre);
                }              
            }
        }

        /// <summary>
        /// lecture des delegues
        /// </summary>
        /// <param name="element">element XML contenant les delegues</param>
        /// <param name="DC"></param>
        public void lecture_delegues(XElement element)
        {
            ICollection<Delegue> delegues = Delegue.LectureDelegue(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_delegues as ICollection).SyncRoot))
            {
                foreach (Delegue delegue in delegues)
                {
                    Delegue p = _delegues.FirstOrDefault(o => o.id == delegue.id);
                    if (p != null)
                    {
                        _delegues.Remove(p);
                    }
                    _delegues.Add(delegue);
                }              
            }
        }
    }
}
