using NReco.PdfGenerator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Tools.Export
{
    public static class ExportPDF_NReco
    {
        public static byte[] ToPDF(string filename, bool landscape)
        {
            return ToPDF(new List<string>() { filename }, true, landscape);
            //PdfConverter pdfConverter = GetPdfConverter(landscape, true);
            //AddFooter(ref pdfConverter, filename, 0, 0);
            //AddHeader(ref pdfConverter, filename);

            //byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlFile(filename + ".html");

            //return pdfBytes;
        }

        public static byte[] ToPDF(List<string> pdf, bool margin, bool landscape)
        {
            if (pdf.Count == 0)
            {
                return new byte[0];
            }

            List<string> urls = new List<string>();
            foreach (string url in pdf)
            {
                urls.Add(url + ".html");
            }

            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();

            htmlToPdf.Margins.Left = margin ? 10 : 0;
            htmlToPdf.Margins.Right = margin ? 10 : 0;
            htmlToPdf.Margins.Top = margin ? 10 : 0;
            htmlToPdf.Margins.Bottom = margin ? 5 : 0;

            htmlToPdf.Orientation = landscape ? PageOrientation.Landscape : PageOrientation.Portrait;

            if (File.Exists(pdf.First() + "_footer.html"))
            {
                htmlToPdf.PageFooterHtml = pdf.First() + "_footer.html";
            }

            if (File.Exists(pdf.First() + "_header.html"))
            {
                htmlToPdf.PageHeaderHtml = pdf.First() + "_header.html";
            }

            //htmlToPdf.PageHeight =  landscape ? 1920 : 2715;
            //htmlToPdf.PageHeight =  landscape ? 2715 : 1920;


            using (var memoryStream = new MemoryStream())
            {
                htmlToPdf.GeneratePdfFromFiles(urls.ToArray(), null, memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
