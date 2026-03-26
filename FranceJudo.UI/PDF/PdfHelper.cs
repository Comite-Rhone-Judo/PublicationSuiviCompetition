using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Windows;

namespace FranceJudo.UI.PDF
{
    public static class PdfHelper
    {
        public static void OpenPDF(string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                try
                {
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = file;
                    info.Verb = "Open";

                    Process process = Process.Start(info);
                }
                catch
                {
                    if (Path.GetExtension(file) == ".pdf")
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(file);
                        PdfViewer viewer = new PdfViewer(bytes);
                        viewer.Show();
                    }
                }

            }
        }

        public static void PrintPDF(string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                try
                {
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = file;
                    info.Verb = "Print";

                    info.CreateNoWindow = true;

                    Process process = Process.Start(info);
                }
                catch
                {
                    if (Path.GetExtension(file) == ".pdf")
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(file);
                        PdfViewer viewer = new PdfViewer(bytes);
                        viewer.Print();
                    }
                }
            }
        }
    }
}
