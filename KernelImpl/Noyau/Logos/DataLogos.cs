
using KernelImpl.Internal;
using KernelImpl.Noyau.Deroulement;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Logos
{
    public class DataLogos
    {
        private readonly DeduplicatedCachedData<string, string> _fedeCache = new DeduplicatedCachedData<string, string>();
        private readonly DeduplicatedCachedData<string, string> _ligueCache = new DeduplicatedCachedData<string, string>();
        private readonly DeduplicatedCachedData<string, string> _logosCache = new DeduplicatedCachedData<string, string>();

        // Accesseurs O(1)
        public IReadOnlyList<string> Fede { get { return _fedeCache.Cache; } }
        public IReadOnlyList<string> Ligue { get { return _fedeCache.Cache; } }
        public IReadOnlyList<string> Sponsors { get { return _fedeCache.Cache; } }


        /// <summary>
        /// lecture des ligues
        /// </summary>
        /// <param name="element">element XML contenant les ligues</param>
        /// <param name="DC"></param>
        public void lecture_logos(XElement element)
        {
            ICollection<string> allLogos = XMLTools.LectureLogosCommissaire(element, null);

            ICollection<string> logos = allLogos.Where(o => o.Contains(ConstantFile.Logo3_dir)).ToList();
            ICollection<string> fede = allLogos.Where(o => o.Contains(ConstantFile.Logo1_dir)).ToList();
            ICollection<string> ligues = allLogos.Where(o => o.Contains(ConstantFile.Logo2_dir)).ToList();

            _logosCache.UpdateFullSnapshot(logos, o => o);
            _fedeCache.UpdateFullSnapshot(logos, o => o);
            _ligueCache.UpdateFullSnapshot(logos, o => o);
        }
    }
}
