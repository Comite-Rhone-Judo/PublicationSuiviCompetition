using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Reflection;
using FranceJudo.Core.XML;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;
using System.Xml.Linq; // Indispensable pour l'usage de XDocument
using System.Xml.Xsl;

namespace FranceJudo.Core.Export
{
    public static class ExportHTML
    {
        // Dictionnaire pour mettre en cache les XSLT compilés
        private static readonly ConcurrentDictionary<string, Lazy<XslCompiledTransform>> _xsltCache = new ConcurrentDictionary<string, Lazy<XslCompiledTransform>>();

        /// <summary>
        /// Realise un export HTML
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fileSave"></param>
        /// <param name="argsList"></param>
        /// <param name="xslt_st"></param>
        /// <param name="fileExtension"></param>
        public static void ToHTML(XmlSource source, string fileSave, XsltArgumentList argsList, string xslt_st, AssemblyResourceDictionary resDict, string fileExtension = "html", bool useCache = true)
        {
            XslCompiledTransform xslt = null;

            if (useCache)
            {
                var lazyXslt = _xsltCache.GetOrAdd(xslt_st, key => new Lazy<XslCompiledTransform>(() => GetXsltFromResource(key, resDict)));
                xslt = lazyXslt.Value;
            }
            else
            {
                // Lit directement sans passer par le cache
                xslt = GetXsltFromResource(xslt_st, resDict);
            }

            string fileSaveWithExt = Path.ChangeExtension(fileSave, fileExtension);

            // Create the FileStream.
            try
            {
                FileSystemHelper.NeedAccessFile(fileSaveWithExt);
                using (FileStream fs = new FileStream(fileSaveWithExt, FileMode.Create))
                {
                    // Utilisation d'un XmlReader pour lire le XDocument à la volée sans allocation mémoire
                    using (XmlReader reader = source.CreateReader())
                    {
                        // Execute the transformation.
                        xslt.Transform(reader, argsList, fs);
                    }
                }
            }
            catch(TimeoutException tex)
            {
                // C'est frequent donc on ne va pas polluer en mode normal, on ne trace qu'en mode debug (au pire le fichier sera actualisa au coup suivant)
                LogTools.Logger.Debug(tex, $"Le fichier '{fileSaveWithExt}' est actuellement utilisé par un autre processus et n'a pas pu être accédé dans le délai imparti.");
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
            finally
            {
                FileSystemHelper.ReleaseFile(fileSaveWithExt);
            }
        }

        /// <summary>
        /// Lit le XSLT depuis les ressources de l'assembly
        /// </summary>
        /// <param name="xslt_st"></param>
        /// <returns></returns>
        private static XslCompiledTransform GetXsltFromResource(string xslt_st, AssemblyResourceDictionary resDict)
        {
            XslCompiledTransform xslt = null;
            XmlReaderSettings readerSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            // Charge le XSLT depuis les ressources de l'assembly pour la 1ere fois
            XsltSettings settings = new XsltSettings
            {
                EnableDocumentFunction = true,
                EnableScript = true
            };

            // On utilise le dictionnaire pour obtenir le flux de manière ciblée
            using (Stream resourceStream = resDict.GetStream(xslt_st))
            {
                if (resourceStream == null)
                    throw new FileNotFoundException($"Le fichier XSLT '{xslt_st}' est introuvable dans le dictionnaire.");

                using (XmlReader xsltReader = XmlReader.Create(resourceStream, readerSettings))
                {
                    xslt = new XslCompiledTransform();

                    // On passe notre dictionnaire au Resolver pour qu'il sache où chercher les <xsl:include> !
                    InAssemblyUrlResolver resolver = new InAssemblyUrlResolver(resDict);

                    xslt.Load(xsltReader, settings, resolver);
                }
            } // Le Stream resourceStream est proprement libéré ici !

            return xslt;
        }
    }
}