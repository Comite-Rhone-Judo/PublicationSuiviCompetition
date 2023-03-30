using HiQPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Tools.Export
{
    public static class ExportPDF_HiQPdf
    {
        private static int width = 1920;


        private static PdfPage AddPage(PdfDocument document, bool landscape, bool margin)
        {
            PdfPage page1 = document.AddPage(PdfPageSize.A4, new PdfDocumentMargins(10));

            page1.Margins.Left = margin ? 10 : 0;
            page1.Margins.Right = margin ? 10 : 0;
            page1.Margins.Top = margin ? 10 : 0;
            page1.Margins.Bottom = margin ? 5 : 0;

            page1.Orientation = landscape ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;

            return page1;
        }

        /// <summary>
        /// Create a PdfConverter object
        /// </summary>
        /// <returns></returns>
        private static PdfDocument GetPdfConverter(bool landscape, bool margin)
        {
            // create an empty PDF document
            PdfDocument document = new PdfDocument();

            // set a demo serial number
            document.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            document.Properties.Author = "CRITT Informatique, Mickaël ROUX";
            document.Properties.Title = "Outils de gestion des compétitions";
            document.Properties.Subject = "";
            document.Properties.CreationDate = DateTime.Now;



            //pdfConverter.HtmlViewerWidth = ExportPDF_HiQPdf.width;
            //pdfConverter.HtmlViewerHeight = 0;

            ////set the PDF page size - default value is A4
            //pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            //// set the PDF compression level - default value is Normal
            //pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            //pdfConverter.PdfDocumentOptions.CompressCrossReference = true;
            //// set the PDF page orientation (portrait or landscape) - default value is portrait
            //pdfConverter.PdfDocumentOptions.PdfPageOrientation = landscape ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;
            //// show or hide header and footer - default value is false
            //pdfConverter.PdfDocumentOptions.ShowHeader = false;
            //pdfConverter.PdfDocumentOptions.ShowFooter = false;



            //// set if the HTTP links are enabled in the generated PDF - default value is true
            //pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = false;

            //// set if the HTML content is resized if necessary to fit the PDF page width - default is true
            //pdfConverter.PdfDocumentOptions.FitWidth = true;

            //// set if the PDF page should be automatically resized to the size of the HTML content when FitWidth is false
            //pdfConverter.PdfDocumentOptions.AutoSizePdfPage = false;

            //// embed the true type fonts in the generated PDF document - default value is false
            //pdfConverter.PdfDocumentOptions.EmbedFonts = false;

            //// compress the images in PDF with JPEG to reduce the PDF document size - default is true
            //pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;
            //pdfConverter.PdfDocumentOptions.JpegCompressionLevel = 50;

            //// set if the JavaScript is enabled during conversion - default value is true
            //pdfConverter.JavaScriptEnabled = false;
            //pdfConverter.PdfSecurityOptions.CanCopyContent = true;

            //// set if the converter should try to avoid breaking the images between PDF pages - default value is false
            ////pdfConverter.AvoidImageBreak = false;

            return document;
        }

        private static void AddHeader(PdfDocument page, string filename)
        {
            if (File.Exists(filename + "_header.html"))
            {
                page.CreateHeaderCanvas(45);

                // layout HTML in header
                PdfHtml headerHtml = new PdfHtml(0, 0, ExportPDF_HiQPdf.width, 45, filename + "_header.html");
                headerHtml.BrowserWidth = ExportPDF_HiQPdf.width;
                headerHtml.FitDestHeight = true;
                headerHtml.FontEmbedding = true;
                page.Header.Layout(headerHtml);
            }
        }

        private static void AddFooter(PdfDocument page, string filename)
        {
            if (File.Exists(filename + "_footer.html"))
            {
                page.CreateFooterCanvas(40);

                PdfHtml headerHtml = new PdfHtml(0, 0, ExportPDF_HiQPdf.width, 40, filename + "_footer.html");
                headerHtml.BrowserWidth = ExportPDF_HiQPdf.width;
                headerHtml.FitDestHeight = true;
                headerHtml.FontEmbedding = true;
                page.Footer.Layout(headerHtml);

                Font pageNumberingFont = new Font(new FontFamily("Times New Roman"), 8, GraphicsUnit.Point);
                float footerHeight = page.Footer.Height;
                float footerWidth = page.Footer.Width;

                PdfText pageNumberingText = new PdfText(5, footerHeight - 12, "Page {CrtPage} of {PageCount}", pageNumberingFont);
                pageNumberingText.HorizontalAlign = PdfTextHAlign.Right;
                pageNumberingText.EmbedSystemFont = true;
                pageNumberingText.ForeColor = Color.Black;
                page.Footer.Layout(pageNumberingText);
            }
            else
            {
                //pdfConverter.PdfDocumentOptions.ShowFooter = false;
            }
        }

        public static byte[] ToPDF(string filename, bool landscape)
        {
            ExportPDF_HiQPdf.width = landscape ? 2715 : 1920;

            PdfDocument pdfConverter = GetPdfConverter(landscape, true);

            PdfPage page1 = AddPage(pdfConverter, landscape, true);
            AddFooter(pdfConverter, filename);
            AddHeader(pdfConverter, filename);


            PdfHtml html1 = new PdfHtml(filename + ".html");
            html1.BrowserWidth = ExportPDF_HiQPdf.width;
            html1.WaitBeforeConvert = 1;
            PdfLayoutInfo html1LayoutInfo = page1.Layout(html1);

            return pdfConverter.WriteToMemory();
        }

        public static byte[] ToPDF(List<string> pdfs, bool margin, bool landscape)
        {
            if (pdfs.Count == 0)
            {
                return new byte[0];
            }

            //string result = "";

            //foreach (string pdf in pdfs)
            //{
            //    using (StreamReader sr = new StreamReader(pdf + ".html"))
            //    {
            //        string tmp = sr.ReadToEnd();

            //        if (result == "")
            //        {
            //            result = tmp;
            //        }
            //        else
            //        {
            //            int index1 = tmp.IndexOf("<body>");
            //            int index2 = tmp.IndexOf("</body>");

            //            string tmp2 = tmp.Substring(index1, index2 - index1).Replace("<body>", "").Replace("</body>", "");

            //            int index = result.IndexOf("</body>");
            //            result = result.Insert(index, tmp2);
            //        }
            //    }
            //}

            //string filename = pdfs.First() + "_tmp.html";
            //using (FileStream f = new FileStream(filename, FileMode.Create))
            //{
            //    using (StreamWriter s = new StreamWriter(f))
            //    {
            //        s.Write(result);
            //    }
            //}


            //ExportPDF_HiQPdf.width = landscape ? 2715 : 1920;

            //HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
            //htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            //htmlToPdfConverter.BrowserWidth = ExportPDF_HiQPdf.width;
            //htmlToPdfConverter.HtmlLoadedTimeout = 120;

            //// set PDF page size and orientation
            //htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
            //htmlToPdfConverter.Document.PageOrientation = landscape ?PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;

            //htmlToPdfConverter.Document.PdfStandard = PdfStandard.PdfA;
            //htmlToPdfConverter.Document.Margins = margin ? new PdfMargins(5) : new PdfMargins(0);            
            //htmlToPdfConverter.Document.FontEmbedding = true;
            //htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Auto;

            //// set header and footer
            //SetHeader(htmlToPdfConverter.Document, pdfs.First());
            //SetFooter(htmlToPdfConverter.Document, pdfs.First());

            //return htmlToPdfConverter.ConvertUrlToMemory(filename);



            PdfDocument pdfConverter = GetPdfConverter(landscape, margin);
            PdfPage page1 = AddPage(pdfConverter, landscape, margin);

            AddFooter(pdfConverter, pdfs.First());
            AddHeader(pdfConverter, pdfs.First());

            


            PdfHtml html1 = new PdfHtml(pdfs.First() + ".html");
            html1.BrowserWidth = ExportPDF_HiQPdf.width;
            html1.WaitBeforeConvert = 0;
            PdfLayoutInfo html1LayoutInfo = page1.Layout(html1);

            for (int i = 1; i < pdfs.Count; i++)
            {
                PdfPage page2 = AddPage(pdfConverter, landscape, margin);
                //AddFooter(page2, pdfs.ElementAt(i));
                //AddHeader(page2, pdfs.ElementAt(i));
                PointF location2 = PointF.Empty;

                PdfHtml html2 = new PdfHtml(location2.X, location2.Y, pdfs.ElementAt(i) + ".html");
                html2.BrowserWidth = ExportPDF_HiQPdf.width;
                html2.WaitBeforeConvert = 0;
                page2.Layout(html2);
            }

            return pdfConverter.WriteToMemory();
        }

        private static void SetHeader(PdfDocumentControl htmlToPdfDocument, string filename)
        {
            if (File.Exists(filename + "_header.html"))
            {
                htmlToPdfDocument.Header.Enabled = true;

                if (!htmlToPdfDocument.Header.Enabled)
                    return;

                // set header height
                htmlToPdfDocument.Header.Height = 45;

                float pdfPageWidth = htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                                            htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;


                // layout HTML in header
                PdfHtml headerHtml = new PdfHtml(0, 0, ExportPDF_HiQPdf.width, 45, filename + "_header.html");
                headerHtml.BrowserWidth = ExportPDF_HiQPdf.width;
                headerHtml.FitDestHeight = true;
                headerHtml.FontEmbedding = true;
                htmlToPdfDocument.Header.Layout(headerHtml);               
            }
            else
            {
                htmlToPdfDocument.Header.Enabled = false;
            }
        }

        private static void SetFooter(PdfDocumentControl htmlToPdfDocument, string filename)
        {
            if (File.Exists(filename + "_footer.html"))
            {
                htmlToPdfDocument.Footer.Enabled = true;
                htmlToPdfDocument.Footer.Height = 40;
                
                float pdfPageWidth = htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                                            htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;

                float footerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left - htmlToPdfDocument.Margins.Right;
                float footerHeight = htmlToPdfDocument.Footer.Height;


                PdfHtml footerHtml = new PdfHtml(0, 0, ExportPDF_HiQPdf.width, 40, filename + "_footer.html");
                footerHtml.BrowserWidth = ExportPDF_HiQPdf.width;
                footerHtml.FitDestHeight = true;
                footerHtml.FontEmbedding = true;
                htmlToPdfDocument.Footer.Layout(footerHtml);

                // add page numbering
                Font pageNumberingFont = new Font(new FontFamily("Times New Roman"), 8, GraphicsUnit.Point);
                //pageNumberingFont.Mea
                PdfText pageNumberingText = new PdfText(5, footerHeight - 12, "Page {CrtPage} of {PageCount}", pageNumberingFont);
                pageNumberingText.HorizontalAlign = PdfTextHAlign.Center;
                pageNumberingText.EmbedSystemFont = true;
                pageNumberingText.ForeColor = Color.DarkGreen;
                htmlToPdfDocument.Footer.Layout(pageNumberingText);              
            }
            else
            {
                htmlToPdfDocument.Footer.Enabled = false;
            }


            
        }
    }
}
