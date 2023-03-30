
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Logos
{
    public class DataLogos 
    {
        private IList<string> _fede = new List<string>();
        public IList<string> Fede { get { return _fede; } set { _fede = value; } }

        private IList<string> _ligue = new List<string>();
        public IList<string> Ligue { get { return _ligue; } set { _ligue = value; } }

        private IList<string> _logos = new List<string>();
        public IList<string> Sponsors { get { return _logos; } set { _logos = value; } }



        /// <summary>
        /// lecture des ligues
        /// </summary>
        /// <param name="element">element XML contenant les ligues</param>
        /// <param name="DC"></param>
        public void lecture_logos(XElement element)
        {
            ICollection<string> logos = XMLTools.LectureLogosCommissaire(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_logos as ICollection).SyncRoot))
            {
                _logos.Clear();
                foreach (string logo in logos)
                {
                    if (!_logos.Contains(logo) && logo.Contains(ConstantFile.Logo3_dir))
                    {
                        _logos.Add(logo);
                    }

                    if (!_fede.Contains(logo) && logo.Contains(ConstantFile.Logo1_dir))
                    {
                        _fede.Add(logo);
                    }

                    if (!_ligue.Contains(logo) && logo.Contains(ConstantFile.Logo2_dir))
                    {
                        _ligue.Add(logo);
                    }
                }
            }
        }

    }
}
