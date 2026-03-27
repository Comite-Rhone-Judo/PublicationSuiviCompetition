
using KernelImpl.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Drawing;
using FranceJudo.Core.Logging;
using FranceJudo.Core.IO;
using FranceJudo.Core.Media.Images;
using FranceJudo.Metier.Noyau.Logos;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.XML;


namespace KernelImpl.Noyau.Logos
{
    public class DataLogos : ILogosData
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
            ICollection<string> allLogos = LectureLogosCommissaire(element);

            // TODO To be replaced by AppDirectoryManager
            ICollection<string> logos = allLogos.Where(o => o.Contains(AppDirectoryManager.Logo3_dir)).ToList();
            ICollection<string> fede = allLogos.Where(o => o.Contains(AppDirectoryManager.Logo1_dir)).ToList();
            ICollection<string> ligues = allLogos.Where(o => o.Contains(AppDirectoryManager.Logo2_dir)).ToList();

            _logosCache.UpdateFullSnapshot(logos, o => o);
            _fedeCache.UpdateFullSnapshot(logos, o => o);
            _ligueCache.UpdateFullSnapshot(logos, o => o);
        }

        #region METHODES PRIVEES
        /// <summary>
        /// Lecture des Ligues
        /// </summary>
        /// <param name="xelement">élément décrivant les Ligues</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Ligues</returns>

        public static ICollection<string> LectureLogosCommissaire(XElement xelement)
        {
            ICollection<string> urls = new List<string>();

            try
            {
                FileSystemHelper.DeleteDirectory(AppDirectoryManager.Logo1_dir);
                FileSystemHelper.CreateDirectorie(AppDirectoryManager.Logo1_dir);

                FileSystemHelper.DeleteDirectory(AppDirectoryManager.Logo2_dir);
                FileSystemHelper.CreateDirectorie(AppDirectoryManager.Logo2_dir);

                FileSystemHelper.DeleteDirectory(AppDirectoryManager.Logo3_dir);
                FileSystemHelper.CreateDirectorie(AppDirectoryManager.Logo3_dir);

            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
            finally
            {
                urls = urls.Concat(LectureElement(xelement, ConstantXML.LogoFede, AppDirectoryManager.Logo1_dir)).ToList();
                urls = urls.Concat(LectureElement(xelement, ConstantXML.LogoLigue, AppDirectoryManager.Logo2_dir)).ToList();
                urls = urls.Concat(LectureElement(xelement, ConstantXML.LogoSponsor, AppDirectoryManager.Logo3_dir)).ToList();
            }

            return urls;
        }

        private static ICollection<string> LectureElement(XElement xelement, string element, string directory)
        {
            ICollection<string> urls = new List<string>();
            foreach (XElement xinfo in xelement.Descendants(element))
            {
                string val = xinfo.Element(ConstantXML.Logo_Valeur) != null ? xinfo.Element(ConstantXML.Logo_Valeur).Value : "";
                string nom = xinfo.Element(ConstantXML.Logo_Nom) != null ? xinfo.Element(ConstantXML.Logo_Nom).Value : "";
                if (!String.IsNullOrWhiteSpace(val))
                {
                    using (Image img = ImageHelper.StringToImage(val))
                    {
                        int index = 0;
                        while (File.Exists(directory + nom))
                        {
                            string filename = Path.GetFileNameWithoutExtension(directory + nom);
                            string extension = Path.GetExtension(directory + nom);

                            nom = filename + "_" + ++index + extension;
                        }

                        img.Save(directory + nom);
                        urls.Add(directory + nom);
                    }
                }
            }
            return urls;
        }
        #endregion
    }
}
