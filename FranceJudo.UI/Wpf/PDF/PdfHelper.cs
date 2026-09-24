using FranceJudo.UI.Wpf.Dialogs;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;

namespace FranceJudo.UI.Wpf.PDF
{
    [SupportedOSPlatform("windows")]
    public static class PdfHelper
    {
        public static void OpenPDF(string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                try
                {
                    ProcessStartInfo info = new ProcessStartInfo
                    {
                        FileName = file,
                        Verb = "Open"
                    };

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
                    ProcessStartInfo info = new ProcessStartInfo
                    {
                        FileName = file,
                        Verb = "Print",
                        CreateNoWindow = true
                    };

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
