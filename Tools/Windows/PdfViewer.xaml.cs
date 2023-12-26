using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Fixed;
using Telerik.Windows.Documents.Fixed.FormatProviders;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Print;
using Tools.Outils;

namespace Tools.Windows
{
    /// <summary>
    /// Logique d'interaction pour PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : RadWindow
    {
        private byte[] _document = null;

        public PdfViewer(byte[] document)
        {
            InitializeComponent();

            OutilsTools.ShowInTaskbar(this);

            _document = document;

            MemoryStream stream = new MemoryStream();
            stream.Write(_document, 0, _document.Length);

            // RadFixedDocument document = new PdfFormatProvider(stream, FormatProviderSettings.ReadOnDemand).Import();
            this.pdfViewer.DocumentSource = new PdfDocumentSource(stream);
            //this.pdfViewer.Document = document;
        }

        #region EventHandlers

        private void tbCurrentPage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
            }
        }

        private void tbFind_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.pdfViewer.CommandDescriptors.FindCommandDescriptor.Command.Execute(this.tbFind.Text);
                this.btnPrev.Visibility = System.Windows.Visibility.Visible;
                this.btnNext.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Sauvegarde du fichier XML
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".pdf";

            dlg.Filter = "Fichier PDF |*.pdf";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;

                // Export the FileStream.
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(_document, 0, _document.Length);
                }
            }
        }

        private void PDFButton_Click(object sender, RoutedEventArgs e)
        {
            Print();
        }

        public void Print()
        {
            if(!this.pdfViewer.IsLoaded)
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(_document, 0, _document.Length);

                PdfFormatProvider prvdPdfPrint = new PdfFormatProvider(stream, FormatProviderSettings.ReadOnDemand);
                pdfViewer.Document = prvdPdfPrint.Import();
            }

            PrintSettings settings = new PrintSettings()
            {
                DocumentName = "Export DOCUMENT JUDO",
                UseDefaultPrinter = true
            };

            this.pdfViewer.Print(settings);
        }
    }
}
