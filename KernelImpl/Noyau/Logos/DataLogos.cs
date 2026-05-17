
using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Media.Images;
using FranceJudo.Metier.IO;
using FranceJudo.Metier.Noyau.Logos;
using FranceJudo.Metier.XML;
using KernelImpl.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace KernelImpl.Noyau.Logos
{
    public class DataLogos : ILogosData
    {
        private readonly DeduplicatedCachedData<string, string> _fedeCache = new DeduplicatedCachedData<string, string>();
        private readonly DeduplicatedCachedData<string, string> _ligueCache = new DeduplicatedCachedData<string, string>();
        private readonly DeduplicatedCachedData<string, string> _logosCache = new DeduplicatedCachedData<string, string>();

        // Accesseurs O(1)
        public IReadOnlyList<string> Fede { get { return _fedeCache.Cache; } }
        public IReadOnlyList<string> Ligue { get { return _ligueCache.Cache; } }
        public IReadOnlyList<string> Sponsors { get { return _logosCache.Cache; } }


        /// <summary>
        /// lecture des ligues
        /// </summary>
        /// <param name="element">element XML contenant les ligues</param>
        /// <param name="DC"></param>
        public void LectureLogos(XElement element)
        {
            ICollection<string> allLogos = LectureLogosCommissaire(element);

            ICollection<string> logos = allLogos.Where(o => o.Contains(AppDirectoryManager.Logo3Dir)).ToList();
            ICollection<string> fede = allLogos.Where(o => o.Contains(AppDirectoryManager.Logo1Dir)).ToList();
            ICollection<string> ligues = allLogos.Where(o => o.Contains(AppDirectoryManager.Logo2Dir)).ToList();

            _logosCache.UpdateFullSnapshot(logos, o => o);
            _fedeCache.UpdateFullSnapshot(fede, o => o);
            _ligueCache.UpdateFullSnapshot(ligues, o => o);
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
                FileSystemHelper.DeleteDirectory(AppDirectoryManager.Logo1Dir);
                FileSystemHelper.CreateDirectory(AppDirectoryManager.Logo1Dir);

                FileSystemHelper.DeleteDirectory(AppDirectoryManager.Logo2Dir);
                FileSystemHelper.CreateDirectory(AppDirectoryManager.Logo2Dir);

                FileSystemHelper.DeleteDirectory(AppDirectoryManager.Logo3Dir);
                FileSystemHelper.CreateDirectory(AppDirectoryManager.Logo3Dir);

            }
            catch (Exception ex)
            {
                LogTools.Logger?.Error(ex);
            }
            finally
            {
                urls = urls.Concat(LectureElement(xelement, ConstantXML.LogoFede, AppDirectoryManager.Logo1Dir)).ToList();
                urls = urls.Concat(LectureElement(xelement, ConstantXML.LogoLigue, AppDirectoryManager.Logo2Dir)).ToList();
                urls = urls.Concat(LectureElement(xelement, ConstantXML.LogoSponsor, AppDirectoryManager.Logo3Dir)).ToList();
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
