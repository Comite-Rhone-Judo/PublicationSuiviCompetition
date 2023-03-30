using EvoPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using Tools.Enum;
using Tools.Outils;

namespace Tools.Export
{
    public class ExportPDF_EVO : ExportPDF
    {
        private static int width = 1920;
        private const string _licence = "xUtbSllZSlhbWEpeRFpKWVtEW1hEU1NTUw==";//"4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

        /// <summary>
        /// Create a PdfConverter object
        /// </summary>
        /// <returns></returns>
        private PdfConverter GetPdfConverter(bool landscape, bool marrgin)
        {
            ExportPDF_EVO.width = landscape ? 2715 : 1920;

            PdfConverter pdfConverter = new PdfConverter();

            // set the license key - required
            pdfConverter.LicenseKey = ExportPDF_EVO._licence;
            pdfConverter.NavigationTimeout = 60;

            pdfConverter.PdfDocumentInfo.AuthorName = "CRITT Informatique, Mickaël ROUX";
            pdfConverter.PdfDocumentInfo.Title = "Outils de gestion des compétitions";
            pdfConverter.PdfDocumentInfo.Subject = "";
            pdfConverter.PdfDocumentInfo.CreatedDate = DateTime.Now;

            pdfConverter.HtmlViewerWidth = ExportPDF_EVO.width;
            pdfConverter.HtmlViewerHeight = 0;

            //set the PDF page size - default value is A4
            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            // set the PDF compression level - default value is Normal
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            pdfConverter.PdfDocumentOptions.CompressCrossReference = true;
            // set the PDF page orientation (portrait or landscape) - default value is portrait
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = landscape ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;
            // show or hide header and footer - default value is false
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = false;

            //set the PDF document margins - default margins are 0
            pdfConverter.PdfDocumentOptions.LeftMargin = marrgin ? 15 : 0;
            pdfConverter.PdfDocumentOptions.RightMargin = marrgin ? 15 : 0;
            pdfConverter.PdfDocumentOptions.TopMargin = marrgin ? 10 : 0;
            pdfConverter.PdfDocumentOptions.BottomMargin = marrgin ? 5 : 0;

            // set if the HTTP links are enabled in the generated PDF - default value is true
            pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = false;

            // set if the HTML content is resized if necessary to fit the PDF page width - default is true
            pdfConverter.PdfDocumentOptions.FitWidth = true;

            // set if the PDF page should be automatically resized to the size of the HTML content when FitWidth is false
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = false;

            // embed the true type fonts in the generated PDF document - default value is false
            pdfConverter.PdfDocumentOptions.EmbedFonts = false;

            // compress the images in PDF with JPEG to reduce the PDF document size - default is true
            pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;
            pdfConverter.PdfDocumentOptions.JpegCompressionLevel = 10;

            // set if the JavaScript is enabled during conversion - default value is true
            pdfConverter.JavaScriptEnabled = false;
            pdfConverter.PdfSecurityOptions.CanCopyContent = true;

            // set if the converter should try to avoid breaking the images between PDF pages - default value is false
            //pdfConverter.AvoidImageBreak = false;

            return pdfConverter;
        }

        private void AddHeader(ref PdfConverter pdfConverter, string filename)
        {
            if (File.Exists(filename + "_header.html"))
            {
                try
                {
                    FileAndDirectTools.NeedAccessFile(filename + "_footer.html");

                    int width = ExportPDF_EVO.width;
                    HtmlToPdfElement headerHtml = new HtmlToPdfElement(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight, filename + "_header.html", width, 0);

                    headerHtml.FitHeight = true;
                    headerHtml.FitWidth = true;
                    pdfConverter.PdfHeaderOptions.AddElement(headerHtml);

                    pdfConverter.PdfDocumentOptions.ShowHeader = true;
                    pdfConverter.PdfHeaderOptions.HeaderHeight = 45;
                }
                catch (Exception ex)
                {
                    LogTools.Log(ex);
                }
                finally
                {
                    FileAndDirectTools.ReleaseFile(filename + "_footer.html");
                }
            }
            else
            {
                pdfConverter.PdfDocumentOptions.ShowHeader = false;
            }
        }

        private void AddFooter(ref PdfConverter pdfConverter, string filename)
        {
            if (File.Exists(filename + "_footer.html"))
            {
                //TextElement footerTextElement = new TextElement(0, 30, " &p; " /*" &p;/&P; "*/, new Font(new FontFamily("Times New Roman"), 8, GraphicsUnit.Point));
                //footerTextElement.TextAlign = HorizontalTextAlign.Right;
                //footerTextElement.VerticalTextAlign = VerticalTextAlign.Bottom;
                //pdfConverter.PdfFooterOptions.AddElement(footerTextElement);

                try
                {
                    FileAndDirectTools.NeedAccessFile(filename + "_footer.html");

                    int width = ExportPDF_EVO.width;
                    HtmlToPdfElement footerHtml = new HtmlToPdfElement(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight, filename + "_footer.html", width, 0);
                    footerHtml.FitHeight = true;
                    footerHtml.FitWidth = true;
                    pdfConverter.PdfFooterOptions.AddElement(footerHtml);

                    pdfConverter.PdfDocumentOptions.ShowFooter = true;
                    pdfConverter.PdfFooterOptions.FooterHeight = 40;
                }
                catch (Exception ex)
                {
                    LogTools.Log(ex);
                }
                finally
                {
                    FileAndDirectTools.ReleaseFile(filename + "_footer.html");
                }
            }
            else
            {
                pdfConverter.PdfDocumentOptions.ShowFooter = false;
            }
        }

        private void AddLogos(Document document)
        {
            PdfPage firstPage = document.Pages[0];

            string logo1 = "";

            foreach (string logo in Directory.GetFiles(ConstantFile.Logo1_dir))
            {
                logo1 = logo;
            }

            Template template = document.AddTemplate(new RectangleF(0, 0, firstPage.ClientRectangle.Width, firstPage.ClientRectangle.Height));

            if (logo1 != "")
            {
                string img = "<img border=\"0\" align=\"left\" height=\"100px\" src=\"file:///" + logo1 + "\"></img>";

                HtmlToPdfElement templateStampImageElement = new HtmlToPdfElement(35, 7, 300, 300, img, null);
                templateStampImageElement.FitHeight = true;
                template.AddElement(templateStampImageElement);
            }


            string logo2 = "";

            foreach (string logo in Directory.GetFiles(ConstantFile.Logo2_dir))
            {
                logo2 = logo;
            }

            if (logo2 != "")
            {
                string img2 = "<img border=\"0\" align=\"right\" height=\"100px\" src=\"file:///" + logo2 + "\"></img>";

                HtmlToPdfElement templateStampImageElement2 = new HtmlToPdfElement(firstPage.ClientRectangle.Width - 335, 7, 300, 300, img2, null);
                templateStampImageElement2.FitHeight = true;
                template.AddElement(templateStampImageElement2);
            }

            List<string> sponsors = new List<string>();
            foreach (string logo in Directory.GetFiles(ConstantFile.Logo3_dir))
            {
                sponsors.Add(logo);
            }

            for (int index = 0; index < sponsors.Count; index++)
            {
                string imgsponsor = "<img border=\"0\" align=\"left\" height=\"80px\" src=\"file:///" + sponsors.ElementAt(index) + "\"></img>";

                HtmlToPdfElement templatehtml = new HtmlToPdfElement(index * ((int)(firstPage.ClientRectangle.Width * 0.75 ) / sponsors.Count) + 150, firstPage.ClientRectangle.Height - 35, 300, 300, imgsponsor, null);
                templatehtml.FitHeight = true;
                template.AddElement(templatehtml);

            }
        }

        private Document AddFooterNumbering(string pdfToModify, string filename, bool withlogo)
        {
            Document document = new Document(pdfToModify);
            document.LicenseKey = ExportPDF_EVO._licence;
            if (!File.Exists(filename + "_footer.html"))
            {
                return document;
            }

            // set the license key

            PdfPage firstPage = document.Pages[0];

            float templateStampXLocation = firstPage.ClientRectangle.Width - 40;
            float templateStampYLocation = firstPage.ClientRectangle.Height - 60;

            Template template = document.AddTemplate(new RectangleF(templateStampXLocation, templateStampYLocation, 30, 40));



            TextElement footerTextElement = new TextElement(0, 30, " &p;/&P; ", new Font(new FontFamily("Georgia"), 6, GraphicsUnit.Point));
            footerTextElement.TextAlign = HorizontalTextAlign.Right;
            footerTextElement.VerticalTextAlign = VerticalTextAlign.Bottom;
            template.AddElement(footerTextElement);

            if (withlogo)
            {
                AddLogos(document);
            }


            return document;
        }

        /* public override string ToPDF(List<ExportPDFItem> pdf)
         {
             if (pdf.Count == 0)
             {
                 return "";
             }

             #region solution 3 SANS le /&P;

             //byte[] pdfBytes = null;

             // for (int index = 0; index < pdf.Count; index++)
             int index = 0;
             foreach(ExportPDFItem pdfItem in pdf)
             {
                 if (_regenere || !File.Exists(pdfItem.PdfName + ".pdf"))
                 {
                     // PdfConverter pdfConverter = GetPdfConverter(_landscape, _margin);
                     PdfConverter pdfConverter = GetPdfConverter(pdfItem.IsLandscape, _margin);
                     AddFooter(ref pdfConverter, pdfItem.PdfName);
                     AddHeader(ref pdfConverter, pdfItem.PdfName);

                     try
                     {
                         FileAndDirectTools.NeedAccessFile(pdfItem.PdfName + ".html");
                         FileAndDirectTools.NeedAccessFile(pdfItem.PdfName + ".pdf");
                         string test = pdfItem.PdfName;
                         pdfConverter.SavePdfFromHtmlFileToFile(pdfItem.PdfName + ".html", pdfItem.PdfName + ".pdf");
                     }
                     catch (Exception ex)
                     {
                         LogTools.Log(ex);
                     }
                     finally
                     {
                         FileAndDirectTools.ReleaseFile(pdfItem.PdfName + ".html");
                         FileAndDirectTools.ReleaseFile(pdfItem.PdfName + ".pdf");
                     }

                     if (!_background && MI1 != null)
                     {
                         MI1(pdf.Count + (index), pdf.Count * 2, "Génération PDF ...");
                     }

                 }
                 index++;
             }

             Document document1 = new Document(pdf.FirstOrDefault().PdfName + ".pdf");
             document1.LicenseKey = ExportPDF_EVO._licence;

             FileStream pdfStream2 = null;
             string filename1 = "";
             try
             {
                 ExportPDFItem pdfItem;
                 for (index = 1; index < pdf.Count; index++)
                 // foreach(ExportPDFItem pdfItem in pdf)
                 {
                     pdfItem = pdf.ElementAt(index);
                     try
                     {
                         FileAndDirectTools.NeedAccessFile(pdfItem.PdfName + ".pdf");
                         pdfStream2 = new FileStream(pdfItem.PdfName + ".pdf", FileMode.Open);
                         Document document2 = new Document(pdfStream2);
                         document1.AppendDocument(document2);
                     }
                     catch (Exception ex)
                     {
                         LogTools.Log(ex);
                     }
                     finally
                     {
                         FileAndDirectTools.ReleaseFile(pdfItem.PdfName + ".pdf");
                     }

                 }

                 document1.AutoCloseAppendedDocs = true;

                 try
                 {
                     index = 0;
                     string filename = pdf.FirstOrDefault().PdfName + "_tmp" + index + ".pdf";
                     while (File.Exists(filename) && FileAndDirectTools.IsFileLocked(filename))
                     {
                         index++;
                         filename = pdf.FirstOrDefault().PdfName + "_tmp" + index + ".pdf";
                     }

                     if (File.Exists(pdf.FirstOrDefault().PdfName + "_footer.html"))
                     {
                         document1.Save(filename);
                         document1.Close();
                         document1 = AddFooterNumbering(filename, pdf.FirstOrDefault().PdfName, _withlogo);
                     }

                     index = 1;
                     filename1 = pdf.FirstOrDefault().PdfName + "_tmp" + index + ".pdf";
                     while (File.Exists(filename1) && FileAndDirectTools.IsFileLocked(filename1))
                     {
                         index++;
                         filename1 = pdf.FirstOrDefault().PdfName + "_tmp" + index + ".pdf";
                     }

                     document1.Save(filename1);

                 }
                 catch (Exception ex)
                 {
                     LogTools.Log(ex);
                 }
                 finally
                 {
                     document1.Close();
                 }
             }
             catch (Exception ex)
             {
                 LogTools.Log(ex);
             }
             finally
             {
                 if (pdfStream2 != null)
                     pdfStream2.Close();
             }

             return filename1;

             #endregion
         }*/

        public override string ToPDF(List<string> pdf)
        {
            if (pdf.Count == 0)
            {
                return "";
            }

            #region solution 3 SANS le /&P;

            //byte[] pdfBytes = null;

            for (int index = 0; index < pdf.Count; index++)
            {
                if (_regenere || !File.Exists(pdf.ElementAt(index) + ".pdf"))
                {
                    PdfConverter pdfConverter = GetPdfConverter(_landscape, _margin);
                    AddFooter(ref pdfConverter, pdf.ElementAt(index));
                    AddHeader(ref pdfConverter, pdf.ElementAt(index));

                    try
                    {
                        FileAndDirectTools.NeedAccessFile(pdf.ElementAt(index) + ".html");
                        FileAndDirectTools.NeedAccessFile(pdf.ElementAt(index) + ".pdf");
                        string test = pdf.ElementAt(index).ToString();
                        pdfConverter.SavePdfFromHtmlFileToFile(pdf.ElementAt(index) + ".html", pdf.ElementAt(index) + ".pdf");
                    }
                    catch (Exception ex)
                    {
                        LogTools.Log(ex);
                    }
                    finally
                    {
                        FileAndDirectTools.ReleaseFile(pdf.ElementAt(index) + ".html");
                        FileAndDirectTools.ReleaseFile(pdf.ElementAt(index) + ".pdf");
                    }

                    if (!_background && MI1 != null)
                    {
                        MI1(pdf.Count + (index), pdf.Count * 2, "Génération PDF ...");
                    }

                }
            }

            Document document1 = new Document(pdf.FirstOrDefault() + ".pdf");
            document1.LicenseKey = ExportPDF_EVO._licence;

            FileStream pdfStream2 = null;
            string filename1 = "";
            try
            {
                for (int index = 1; index < pdf.Count; index++)
                {
                    try
                    {
                        FileAndDirectTools.NeedAccessFile(pdf.ElementAt(index) + ".pdf");
                        pdfStream2 = new FileStream(pdf.ElementAt(index) + ".pdf", FileMode.Open);
                        Document document2 = new Document(pdfStream2);
                        document1.AppendDocument(document2);
                    }
                    catch (Exception ex)
                    {
                        LogTools.Log(ex);
                    }
                    finally
                    {
                        FileAndDirectTools.ReleaseFile(pdf.ElementAt(index) + ".pdf");
                    }

                }

                document1.AutoCloseAppendedDocs = true;

                try
                {
                    int index = 0;
                    string filename = pdf.FirstOrDefault() + "_tmp" + index + ".pdf";
                    while (File.Exists(filename) && FileAndDirectTools.IsFileLocked(filename))
                    {
                        index++;
                        filename = pdf.FirstOrDefault() + "_tmp" + index + ".pdf";
                    }

                    if (File.Exists(pdf.FirstOrDefault() + "_footer.html"))
                    {
                        document1.Save(filename);
                        document1.Close();
                        document1 = AddFooterNumbering(filename, pdf.FirstOrDefault(), _withlogo);
                    }

                    index = 1;
                    filename1 = pdf.FirstOrDefault() + "_tmp" + index + ".pdf";
                    while (File.Exists(filename1) && FileAndDirectTools.IsFileLocked(filename1))
                    {
                        index++;
                        filename1 = pdf.FirstOrDefault() + "_tmp" + index + ".pdf";
                    }

                    document1.Save(filename1);

                }
                catch (Exception ex)
                {
                    LogTools.Log(ex);
                }
                finally
                {
                    document1.Close();
                }
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                if (pdfStream2 != null)
                    pdfStream2.Close();
            }

            return filename1;

            #endregion
        }
        public override string ToPDFClassement(XmlDocument xclassement, string filename)
        {
            string filename1 = filename + ".pdf";

            Document document1 = new Document();
            document1.LicenseKey = ExportPDF_EVO._licence;

            try
            {
                FileAndDirectTools.NeedAccessFile(filename1);
                //int index = 0;

                foreach (XmlNode element in xclassement.DocumentElement.SelectNodes(
                    String.Format("descendant::{0}/{1}[@{2}='{3}']", ConstantXML.Classement, ConstantXML.Participant, ConstantXML.Participant_ClassementFinal, 1)))
                {
                    PdfPage page = document1.AddPage(PdfPageSize.A4, new Margins(0), PdfPageOrientation.Landscape);
                    AddPageDiplome(page, getFond(1), element.FirstChild, xclassement);
                }

                foreach (XmlNode element in xclassement.DocumentElement.SelectNodes(
                    String.Format("descendant::{0}/{1}[@{2}='{3}']", ConstantXML.Classement, ConstantXML.Participant, ConstantXML.Participant_ClassementFinal, 2)))
                {
                    PdfPage page = document1.AddPage(PdfPageSize.A4, new Margins(0), PdfPageOrientation.Landscape);
                    AddPageDiplome(page, getFond(2), element.FirstChild, xclassement);
                }

                foreach (XmlNode element in xclassement.DocumentElement.SelectNodes(
                    String.Format("descendant::{0}/{1}[@{2}='{3}']", ConstantXML.Classement, ConstantXML.Participant, ConstantXML.Participant_ClassementFinal, 3)))
                {
                    PdfPage page = document1.AddPage(PdfPageSize.A4, new Margins(0), PdfPageOrientation.Landscape);
                    AddPageDiplome(page, getFond(3), element.FirstChild, xclassement);
                }

                foreach (XmlNode element in xclassement.DocumentElement.SelectNodes(
                    String.Format("descendant::{0}/{1}[@{2}='{3}']", ConstantXML.Classement, ConstantXML.Participant, ConstantXML.Participant_ClassementFinal, 4)))
                {
                    PdfPage page = document1.AddPage(PdfPageSize.A4, new Margins(0), PdfPageOrientation.Landscape);
                    AddPageDiplome(page, getFond(3), element.FirstChild, xclassement);
                }

                document1.Save(filename1);
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                document1.Close();
                FileAndDirectTools.ReleaseFile(filename1);
            }
            return filename1;
            //ExportXML.ExportClassementFinal(DC, epreuve)
        }

        public override string ToPDFParticipation(XmlDocument xclassement, string filename)
        {
            string filename1 = filename + ".pdf";
            string filename_tmp = filename + "_tmp.pdf";

            Document document1 = new Document();
            document1.LicenseKey = ExportPDF_EVO._licence;

            try
            {
                FileAndDirectTools.NeedAccessFile(filename_tmp);

                foreach (XmlNode element in xclassement.DocumentElement.SelectNodes(
                    String.Format("descendant::{0}/{1}[@etat=4]", ConstantXML.Epreuve_Inscrits, ConstantXML.Judoka)))
                {
                    PdfPage page = document1.AddPage(PdfPageSize.A4, new Margins(0), PdfPageOrientation.Landscape);

                    //AddPageDiplome(document1, 0, element, xclassement);
                }
                if (document1.Pages.Count > 0)
                {
                    Template template = document1.AddTemplate(new RectangleF(0, 0, document1.Pages[0].ClientRectangle.Width, document1.Pages[0].ClientRectangle.Height));
                    ImageElement image = new ImageElement(0, 0, document1.Pages[0].ClientRectangle.Width - 1, getFond(0));
                    template.AddElement(image);

                    document1.Save(filename_tmp);

                    FileAndDirectTools.ReleaseFile(filename_tmp);
                    document1.Close();
                }
                else
                {
                    document1.Close();
                    FileAndDirectTools.ReleaseFile(filename_tmp);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
                FileAndDirectTools.ReleaseFile(filename_tmp);
                document1.Close();
            }

            Document document2 = null;

            try
            {
                FileAndDirectTools.NeedAccessFile(filename_tmp);

                document2 = new Document(filename_tmp);
                document2.LicenseKey = ExportPDF_EVO._licence;
                
                FileAndDirectTools.NeedAccessFile(filename1);
                int index = 0;

                foreach (XmlNode element in xclassement.DocumentElement.SelectNodes(
                    String.Format("descendant::{0}/{1}[@etat=4]", ConstantXML.Epreuve_Inscrits, ConstantXML.Judoka)))
                {
                    try
                    {
                        AddPageDiplome(document2.Pages[index], null, element, xclassement);
                    }
                    catch(Exception ex)
                    {
                        LogTools.Trace(ex, LogTools.Level.ERROR);
                    }                    
                    index++;
                }
                document2.Save(filename1);
            }
            catch (Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                document2.Close();
                FileAndDirectTools.ReleaseFile(filename_tmp);
                FileAndDirectTools.ReleaseFile(filename1);
            }

            return filename;
            //ExportXML.ExportClassementFinal(DC, epreuve)
        }

        private string getFond(int classement)
        {
            string fond = _fond;
            if (classement == 1)
            {
                fond += "_or.jpg";
            }
            else if (classement == 2)
            {
                fond += "_argent.jpg";
            }
            else if (classement == 3)
            {
                fond += "_bronze.jpg";
            }
            else
            {
                fond += "_participation.jpg";
            }

            return fond;
        }

        private void AddPageDiplome(PdfPage page, string fond, XmlNode xelement, XmlDocument xclassement)
        {
            //string img = "<img border=\"0\" align=\"left\" height=\"1915px\" width=\"2715px\" src=\"file:///" + getFond(classement) + "\"></img>";
            //HtmlToPdfElement templateStampImageElement = new HtmlToPdfElement(0, 0, page.ClientRectangle.Width, page.ClientRectangle.Height, img, null);
            //templateStampImageElement.FitHeight = true;
            //templateStampImageElement.FitWidth = true;
            //page.AddElement(templateStampImageElement);

            if (!string.IsNullOrWhiteSpace(fond))
            {
                ImageElement image = new ImageElement(0, 0, page.ClientRectangle.Width - 1, fond);
                page.AddElement(image);
            }

            //float x = 0;
            float y = page.ClientRectangle.Height / 2;

            string nom = XMLTools.LectureString(xelement.Attributes[ConstantXML.Judoka_Nom]);
            string prenom = XMLTools.LectureString(xelement.Attributes[ConstantXML.Judoka_Prenom]);
            string competition = xclassement.SelectSingleNode("//competition[1]/titre").InnerText;
            string club_id = XMLTools.LectureString(xelement.Attributes[ConstantXML.Judoka_Club]);
            string club_nom = xclassement.SelectSingleNode(String.Format("//club[@ID='{0}']/nom", club_id)).InnerText;
            string ep_nom = xelement.SelectSingleNode("ancestor::epreuve[1]/@nom").InnerText;
            DateTime date = XMLTools.LectureDate(xclassement.SelectSingleNode("//" + ConstantXML.Competition).Attributes[ConstantXML.Competition_Date], "ddMMyyyy");

            TextElement NomTextElement = new TextElement(0, (page.ClientRectangle.Height / 2F) + 6, page.ClientRectangle.Width, -1, nom + " " + prenom, new Font(new FontFamily("Georgia"), 16, FontStyle.Bold, GraphicsUnit.Point), new PdfColor(Color.Black));
            NomTextElement.TextAlign = HorizontalTextAlign.Center;
            NomTextElement.VerticalTextAlign = VerticalTextAlign.Bottom;
            page.AddElement(NomTextElement);

            TextElement CompetTextElement = new TextElement(page.ClientRectangle.Width / 2F - 30, (page.ClientRectangle.Height / 2F) + 50, page.ClientRectangle.Width, -1, competition, new Font(new FontFamily("Georgia"), 13, FontStyle.Bold, GraphicsUnit.Point), new PdfColor(Color.Black));
            CompetTextElement.TextAlign = HorizontalTextAlign.Left;
            CompetTextElement.VerticalTextAlign = VerticalTextAlign.Bottom;
            page.AddElement(CompetTextElement);

            TextElement clubTextElement = new TextElement(page.ClientRectangle.Width / 2F - 30, (page.ClientRectangle.Height / 2F) + 72, page.ClientRectangle.Width, -1, club_nom, new Font(new FontFamily("Georgia"), 13, FontStyle.Bold, GraphicsUnit.Point), new PdfColor(Color.Black));
            clubTextElement.TextAlign = HorizontalTextAlign.Left;
            clubTextElement.VerticalTextAlign = VerticalTextAlign.Bottom;
            page.AddElement(clubTextElement);

            TextElement epTextElement = new TextElement(page.ClientRectangle.Width / 2F - 30, (page.ClientRectangle.Height / 2F) + 94, page.ClientRectangle.Width, -1, ep_nom, new Font(new FontFamily("Georgia"), 13, FontStyle.Bold, GraphicsUnit.Point), new PdfColor(Color.Black));
            epTextElement.TextAlign = HorizontalTextAlign.Left;
            epTextElement.VerticalTextAlign = VerticalTextAlign.Bottom;
            page.AddElement(epTextElement);

            TextElement dateTextElement = new TextElement(page.ClientRectangle.Width / 2F - 30, (page.ClientRectangle.Height / 2F) + 116, page.ClientRectangle.Width, -1, date.ToString("dd-MM-yyyy"), new Font(new FontFamily("Georgia"), 13, FontStyle.Bold, GraphicsUnit.Point), new PdfColor(Color.Black));
            dateTextElement.TextAlign = HorizontalTextAlign.Left;
            dateTextElement.VerticalTextAlign = VerticalTextAlign.Bottom;
            page.AddElement(dateTextElement);
        }
    }
}
