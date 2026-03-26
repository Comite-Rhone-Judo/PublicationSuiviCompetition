using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq; // Indispensable pour l'usage de XDocument
using System.Xml.Xsl;

namespace FranceJudo.Core.Export
{
    public static class     
    {
        // Dictionnaire pour mettre en cache les XSLT compilés
        private static readonly ConcurrentDictionary<string, Lazy<XslCompiledTransform>> _xsltCache = new ConcurrentDictionary<string, Lazy<XslCompiledTransform>>();

        public static void ToHTMLSite(XDocument xml, ExportEnum export_type, string fileSave, XsltArgumentList argsList, string fileExtension = "html", bool useCache = true)
        {
            string xslt = ExportTools.GetXsltSite(export_type);
            ExportHTML.ToHTML(xml, fileSave, argsList, xslt, fileExtension, useCache);
        }

        /// <summary>
        /// Realise un export HTML
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="fileSave"></param>
        /// <param name="argsList"></param>
        /// <param name="xslt_st"></param>
        /// <param name="fileExtension"></param>
        public static void ToHTML(XDocument xml, string fileSave, XsltArgumentList argsList, string xslt_st, string fileExtension = "html", bool useCache = true)
        {
            XslCompiledTransform xslt = null;

            if (useCache)
            {
                var lazyXslt = _xsltCache.GetOrAdd(xslt_st, key => new Lazy<XslCompiledTransform>(() => GetXsltFromResource(key)));
                xslt = lazyXslt.Value;
            }
            else
            {
                // Lit directement sans passer par le cache
                xslt = GetXsltFromResource(xslt_st);
            }

            string fileSaveWithExt = Path.ChangeExtension(fileSave, fileExtension);

            // Create the FileStream.
            try
            {
                FileAndDirectTools.NeedAccessFile(fileSaveWithExt);
                using (FileStream fs = new FileStream(fileSaveWithExt, FileMode.Create))
                {
                    // Utilisation d'un XmlReader pour lire le XDocument à la volée sans allocation mémoire
                    using (XmlReader reader = xml.CreateReader())
                    {
                        // Execute the transformation.
                        xslt.Transform(reader, argsList, fs);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
            finally
            {
                FileAndDirectTools.ReleaseFile(fileSaveWithExt);
            }
        }

        /// <summary>
        /// Lit le XSLT depuis les ressources de l'assembly
        /// </summary>
        /// <param name="xslt_st"></param>
        /// <returns></returns>
        private static XslCompiledTransform GetXsltFromResource(string xslt_st)
        {
            XslCompiledTransform xslt = null;
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.DtdProcessing = DtdProcessing.Parse;

            // Charge le XSLT depuis les ressources de l'assembly pour la 1ere fois
            XsltSettings settings = new XsltSettings();
            settings.EnableDocumentFunction = true;
            settings.EnableScript = true;
            var resource = ResourcesTools.GetAssembyResource(xslt_st);

            // L'ajout du bloc using garantit la bonne libération du flux de la ressource
            using (XmlReader xsltReader = XmlReader.Create(resource, readerSettings))
            {
                xslt = new XslCompiledTransform();
                InAssemblyUrlResolver resolver = new InAssemblyUrlResolver();
                xslt.Load(xsltReader, settings, resolver);
            }

            return xslt;
        }
    }
}