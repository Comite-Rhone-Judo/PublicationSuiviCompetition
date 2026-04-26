using FranceJudo.Core.IO;
using FranceJudo.Core.Logging;
using FranceJudo.Core.Reflection;
using FranceJudo.Core.XML;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;
using System.Xml.Linq; // Indispensable pour l'usage de XDocument
using System.Xml.XPath;
using System.Xml.Xsl;

namespace FranceJudo.Core.Export
{
    public static class ExportHTML
    {
        // Dictionnaire pour mettre en cache les XSLT compilés
        private static readonly ConcurrentDictionary<string, Lazy<XslCompiledTransform>> _xsltCache = new ConcurrentDictionary<string, Lazy<XslCompiledTransform>>();

        /// <summary>
        /// Réalise un export HTML à partir d'un XPathDocument.
        /// </summary>
        /// <param name="source">Le document source XPathDocument.</param>
        /// <param name="fileSave">Le chemin du fichier de sortie.</param>
        /// <param name="argsList">Les arguments XSLT.</param>
        /// <param name="xslt_st">Le nom de la ressource XSLT.</param>
        /// <param name="resDict">Le dictionnaire des ressources de l'assembly.</param>
        /// <param name="fileExtension">L'extension du fichier de sortie.</param>
        /// <param name="useCache">Indique si le cache doit être utilisé.</param>
        public static void ToHTML(XPathDocument source, string fileSave, XsltArgumentList argsList, string xslt_st, AssemblyResourceDictionary resDict, string fileExtension = "html", bool useCache = true)
        {
            ExecuteTransform(fileSave, xslt_st, resDict, fileExtension, useCache, (xslt, fs) =>
            {
                // CreateNavigator() est la voie rapide native pour l'XSLT
                xslt.Transform(source.CreateNavigator(), argsList, fs);
            });
        }

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
            ExecuteTransform(fileSave, xslt_st, resDict, fileExtension, useCache, (xslt, fs) =>
            {
                // Lecture optimisée à la volée
                using (XmlReader reader = source.CreateReader())
                {
                    xslt.Transform(reader, argsList, fs);
                }
            });
        }

        /// <summary>
        /// Méthode centrale commune : Gère le cache XSLT, les verrous système et les logs.
        /// </summary>
        private static void ExecuteTransform(string fileSave, string xslt_st, AssemblyResourceDictionary resDict, string fileExtension, bool useCache, Action<XslCompiledTransform, FileStream> transformAction)
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

            try
            {
                FileSystemHelper.NeedAccessFile(fileSaveWithExt);
                using (FileStream fs = new FileStream(fileSaveWithExt, FileMode.Create))
                {
                    // Exécute l'action spécifique (XmlSource ou XPathDocument)
                    transformAction(xslt, fs);
                }
            }
            catch (TimeoutException tex)
            {
                // C'est fréquent donc on ne va pas polluer en mode normal, on ne trace qu'en mode debug
                LogTools.Logger.Debug(tex, $"Le fichier '{fileSaveWithExt}' est actuellement utilise par un autre processus et n'a pas pu etre accede dans le delai imparti.");
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