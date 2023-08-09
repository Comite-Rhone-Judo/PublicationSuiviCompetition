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
        public static void ToHTMLTableau(XmlDocument xml, ExportEnum export_type, string fileSave, XsltArgumentList argsList, int niveauMax)
        {
            argsList.AddParam("style", "", ExportTools.getStyleDirectory(site: false));
            string xslt = ExportTools.GetXsltTabeau(export_type, niveauMax);

            ExportHTML.ToHTML(xml, fileSave, argsList, xslt);
        }

        public static void ToHTMLSite(XmlDocument xml, ExportEnum export_type, string fileSave, XsltArgumentList argsList)
        {
            //argsList.AddParam("style", "", ExportTools.getStyleDirectory(site: true));
            
            // TODO retirer le JS embarque
            
            argsList.AddParam("js", "", ExportTools.getJS());
            //argsList.AddParam("menu", "", ExportTools.getDirectory(true, null, null) + @"\menu.html");

            // Ces fichiers sont generes en parallele par le processus de generation, le fait de les appeler ici provoque des blocages
            // sur les fichiers et leur generation n'est pas compte dans la liste des fichiers generes
            // ExportTools.ExportStyleAndJS(false);
            // ExportTools.ExportImg(false);

            string xslt = ExportTools.GetXsltSite(export_type);
            ExportHTML.ToHTML(xml, fileSave, argsList, xslt);
        }

        public static void ToHTMLClassique(XmlDocument xml, ExportEnum export_type, bool site, string fileSave, XsltArgumentList argsList)
        {
            argsList.AddParam("style", "", ExportTools.getStyleDirectory(site: false));
            string xslt = ExportTools.GetXsltClassique(export_type);

            ExportHTML.ToHTML(xml, fileSave, argsList, xslt);
        }

        public static void ToHTML(XmlDocument xml, string fileSave, XsltArgumentList argsList, string xslt_st)
        {


            XsltSettings settings = new XsltSettings();
            settings.EnableDocumentFunction = true;
            settings.EnableScript = true;

            var resource = ResourcesTools.GetAssembyResource(xslt_st);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.DtdProcessing = DtdProcessing.Parse;

            XmlReader xsltReader = XmlReader.Create(resource, readerSettings);

            // TODO Supprimer le True en mode Release
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

        // OBSOLETE - PLUS UTILISE
        /*
        public static void ToHTML_Header(XmlDocument xml, ExportEnum export_type, string fileSave)
        {
            string directoryStyle = ExportTools.getStyleDirectory(false);

            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("style", "", directoryStyle);

            XsltSettings settings = new XsltSettings();
            settings.EnableDocumentFunction = true;
            settings.EnableScript = true;

            var resource = ResourcesTools.GetAssembyResource(ExportTools.GetXsltHeader(export_type));
            XmlReader xsltReader = XmlReader.Create(resource);

            XslCompiledTransform xslt_header = new XslCompiledTransform();
            xslt_header.Load(xsltReader, settings, null);

            //XslCompiledTransform xslt_header = new XslCompiledTransform();
            //xslt_header.Load(GetXsltFile(export_type, null, false,true,false) + "_header.xslt");// Outils.GetDataDirectory() + "data/template/" + xsltFile + ".xslt");

            // Create the FileStream.
            try
            {
                FileAndDirectTools.NeedAccessFile(fileSave + "_header.html");
                using (FileStream fs2 = new FileStream(fileSave + "_header.html", FileMode.Create))
                {
                    // Execute the transformation.
                    xslt_header.Transform(xml, argsList, fs2);
                }
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                FileAndDirectTools.ReleaseFile(fileSave + "_header.html");
            }

        }

        public static void ToHTML_Footer(XmlDocument xml, ExportEnum export_type, string fileSave)
        {
            string directoryStyle = ExportTools.getStyleDirectory(false);

            XsltArgumentList argsList = new XsltArgumentList();
            argsList.AddParam("style", "", directoryStyle);

            var resource = ResourcesTools.GetAssembyResource(ExportTools.GetXsltFooter(export_type));
            XmlReader xsltReader = XmlReader.Create(resource);

            XslCompiledTransform xslt_footer = new XslCompiledTransform();
            xslt_footer.Load(xsltReader);

            // Create the FileStream.
            try
            {
                FileAndDirectTools.NeedAccessFile(fileSave + "_footer.html");
                using (FileStream fs2 = new FileStream(fileSave + "_footer.html", FileMode.Create))
                {
                    // Execute the transformation.
                    xslt_footer.Transform(xml, argsList, fs2);
                }
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                FileAndDirectTools.ReleaseFile(fileSave + "_footer.html");
            }
        }

        public static void ToHTML_Menu(XmlDocument doc)
        {
            string file_menu = ExportTools.getDirectory(true, null, null) + @"\menu.html";

            // Load the style sheet.
            var resource_menu = ResourcesTools.GetAssembyResource(ExportTools.GetXsltClassique(ExportEnum.Site_Menu));
            XmlReader xsltReader_menu = XmlReader.Create(resource_menu);

            XslCompiledTransform xslt_menu = new XslCompiledTransform();
            xslt_menu.Load(xsltReader_menu);

            // Create the FileStream.
            try
            {
                FileAndDirectTools.NeedAccessFile(file_menu);
                using (FileStream fs = new FileStream(file_menu, FileMode.Create))
                {
                    if (OutilsTools.IsDebug)
                    {
                        doc.Save(ExportTools.getDirectory(true, null, null) + @"\menu.xml");
                    }

                    XsltArgumentList argsList_menu = new XsltArgumentList();

                    //string site_url = "http://";
                    //site_url += Controles.DialogControleur.currentControleur.Site.IpAddress.ToString();
                    //site_url += ":" + Controles.DialogControleur.currentControleur.Site.Port.ToString();
                    //site_url += "/site/";

                    //if (Controles.DialogControleur.currentControleur.competition.nom.Length > 30)
                    //{
                    //    site_url += ExportJudo.TraiteChaine(Controles.DialogControleur.currentControleur.competition.nom.Substring(0, 30));
                    //}
                    //else
                    //{
                    //    site_url += ExportJudo.TraiteChaine(Controles.DialogControleur.currentControleur.competition.nom);
                    //}  

                    ////site_url += ExportJudo.TraiteChaine(Controles.DialogControleur.currentControleur.competition.nom.Substring(0, 30));

                    //argsList_menu.AddParam("site_url", "", site_url);

                    // Execute the transformation.
                    xslt_menu.Transform(doc, argsList_menu, fs);
                }
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                FileAndDirectTools.ReleaseFile(file_menu);
            }
        }
        */
    }
}
