using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Tools.Enum;
using Tools.Outils;

namespace Tools.Export
{
    public static class ExportHTML
    {
        public static void ToHTMLSite(XmlDocument xml, ExportEnum export_type, string fileSave, XsltArgumentList argsList)
        {
            //argsList.AddParam("style", "", ExportTools.getStyleDirectory(site: true));

            argsList.AddParam("js", "", ExportTools.getEmbeddedJS());
            //argsList.AddParam("menu", "", ExportTools.getDirectory(true, null, null) + @"\menu.html");

            // Ces fichiers sont generes en parallele par le processus de generation, le fait de les appeler ici provoque des blocages
            // sur les fichiers et leur generation n'est pas compte dans la liste des fichiers generes
            // ExportTools.ExportStyleAndJS(false);
            // ExportTools.ExportImg(false);

            string xslt = ExportTools.GetXsltSite(export_type);
            ExportHTML.ToHTML(xml, fileSave, argsList, xslt);
        }

        /// <summary>
        /// Realise un export HTML
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="fileSave"></param>
        /// <param name="argsList"></param>
        /// <param name="xslt_st"></param>
        public static void ToHTML(XmlDocument xml, string fileSave, XsltArgumentList argsList, string xslt_st)
        {
            XsltSettings settings = new XsltSettings();
            settings.EnableDocumentFunction = true;
            settings.EnableScript = true;

            var resource = ResourcesTools.GetAssembyResource(xslt_st);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.DtdProcessing = DtdProcessing.Parse;

            XmlReader xsltReader = XmlReader.Create(resource, readerSettings);

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltReader, settings, null);

            // Create the FileStream.
            try
            {
                FileAndDirectTools.NeedAccessFile(fileSave + ".html");
                using (FileStream fs = new FileStream(fileSave + ".html", FileMode.Create))
                {
                    // Execute the transformation.
                    xslt.Transform(xml, argsList, fs);
                }
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                FileAndDirectTools.ReleaseFile(fileSave + ".html");
            }

            if (OutilsTools.IsDebug || fileSave.Contains("menu"))
            {
                try
                {
                    FileAndDirectTools.NeedAccessFile(fileSave + ".xml");
                    xml.Save(fileSave + ".xml");
                }
                catch (Exception ex)
                {
                    LogTools.Log(ex);
                }
                finally
                {
                    FileAndDirectTools.ReleaseFile(fileSave + ".xml");
                }
            }
        }
    }
}
