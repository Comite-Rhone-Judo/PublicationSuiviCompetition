using Tools.Enum;
using Tools.Logging;
using Tools.Files;
using Tools.Core;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Xsl;

namespace Tools.Export
{
    public static class ExportHTML
    {
        // Dictionnaire pour mettre en cache les XSLT compilés
        private static Dictionary<string, XslCompiledTransform> _xsltCache = new Dictionary<string, XslCompiledTransform>();
        private static object _xsltCacheLock = new object();

        public static void ToHTMLSite(XmlDocument xml, ExportEnum export_type, string fileSave, XsltArgumentList argsList, string fileExtension = "html", bool useCache = false)
        {
            //argsList.AddParam("style", "", ExportTools.getStyleDirectory(site: true));

            // argsList.AddParam("js", "", ExportTools.getEmbeddedJS());
            //argsList.AddParam("menu", "", ExportTools.getDirectory(true, null, null) + @"\menu.html");

            // Ces fichiers sont generes en parallele par le processus de generation, le fait de les appeler ici provoque des blocages
            // sur les fichiers et leur generation n'est pas compte dans la liste des fichiers generes
            // ExportTools.ExportStyleAndJS(false);
            // ExportTools.ExportImg(false);

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
        public static void ToHTML(XmlDocument xml, string fileSave, XsltArgumentList argsList, string xslt_st, string fileExtension = "html", bool useCache = true)
        {
            XslCompiledTransform xslt = null;

            if (useCache)
            {

                lock (_xsltCacheLock)
                {
                    bool isXsltCached = _xsltCache.TryGetValue(xslt_st, out xslt);
                    // Check if the XSLT is already compiled and cached
                    if (!isXsltCached)
                    {
                        // Lecture et mise en cache du XSLT
                        xslt = GetXsltFromResource(xslt_st);
                        _xsltCache[xslt_st] = xslt;
                    }
                }
            }
            else
            {
                // Lit directement sans passer par le cache
                xslt = GetXsltFromResource(xslt_st);
            }

            string fileSaveWithExt = fileSave + "." + fileExtension;

            // Create the FileStream.
            try
            {
                FileAndDirectTools.NeedAccessFile(fileSaveWithExt);
                using (FileStream fs = new FileStream(fileSaveWithExt, FileMode.Create))
                {
                    // Execute the transformation.
                    xslt.Transform(xml, argsList, fs);
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
            XmlReader xsltReader = XmlReader.Create(resource, readerSettings);

            xslt = new XslCompiledTransform();
            InAssemblyUrlResolver resolver = new InAssemblyUrlResolver();
            xslt.Load(xsltReader, settings, resolver);

            return xslt;
        }
    }
}

