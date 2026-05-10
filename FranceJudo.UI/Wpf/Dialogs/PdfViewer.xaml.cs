using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;

namespace FranceJudo.UI.Wpf.Dialogs
{
    [SupportedOSPlatform("windows")]
    public partial class PdfViewer : HandyControl.Controls.Window
    {
        private readonly byte[] _document;
        private string _tempFilePath;

        // Flags pour gérer l'impression silencieuse (mode fantôme)
        private bool _isNavigated = false;
        private bool _pendingSilentPrint = false;

        public PdfViewer(byte[] document, string title = "", bool allowPrint = true, bool allowSave = true)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(title))
            {
                this.Title += " - " + title;
            }

            PDFButton.IsEnabled = allowPrint;
            SaveButton.IsEnabled = allowSave;

            _document = document;

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                await pdfWebView.EnsureCoreWebView2Async();

                // Abonnement à la fin du chargement pour déclencher l'impression silencieuse si demandée
                pdfWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

                string tempDir = Path.GetTempPath();
                string randomName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                string tempFileName = $"JudoPrint_{randomName}.pdf";

                _tempFilePath = Path.Combine(tempDir, tempFileName);
                File.WriteAllBytes(_tempFilePath, _document);

                pdfWebView.CoreWebView2.Navigate(_tempFilePath);
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show($"Impossible d'initialiser le composant PDF : {ex.Message}");
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _isNavigated = true;

            // Si un ordre d'impression silencieux était en attente (bloc catch de PrintPDF)
            if (_pendingSilentPrint)
            {
                await ExecuteSilentPrintAsync();
                this.Close(); // Auto-destruction de la fenêtre fantôme après impression
            }
        }

        /// <summary>
        /// Méthode publique appelée par le code externe pour imprimer (Mode silencieux / Fallback)
        /// </summary>
        public void Print()
        {
            if (_isNavigated)
            {
                // Si la fenêtre est déjà ouverte et le PDF chargé, on imprime directement
                _ = ExecuteSilentPrintAsync();
            }
            else
            {
                // Si appelé immédiatement après l'instanciation (new PdfViewer(bytes).Print();)
                _pendingSilentPrint = true;

                // Astuce WPF : WebView2 ne charge rien si la fenêtre n'est pas "montrée" à l'OS.
                // On la rend invisible pour forcer le processus sans perturber l'utilisateur.
                if (!this.IsVisible)
                {
                    this.Width = 0;
                    this.Height = 0;
                    this.ShowInTaskbar = false;
                    this.WindowStyle = WindowStyle.ToolWindow; // Évite l'animation d'ouverture
                    this.Show();
                }
            }
        }

        /// <summary>
        /// Imprime le PDF silencieusement sur l'imprimante par défaut
        /// </summary>
        private async Task ExecuteSilentPrintAsync()
        {
            try
            {
                // CreatePrintSettings crée par défaut une configuration pointant vers l'imprimante par défaut de Windows
                var printSettings = pdfWebView.CoreWebView2.Environment.CreatePrintSettings();
                await pdfWebView.CoreWebView2.PrintAsync(printSettings);
            }
            catch (Exception ex)
            {
                // En mode silencieux, on logue l'erreur ou on l'ignore pour ne pas bloquer l'appli
                Console.WriteLine($"Erreur d'impression silencieuse : {ex.Message}");
            }
        }

        /// <summary>
        /// Bouton Imprimer de la barre d'outils (Mode interactif)
        /// </summary>
        private void PDFButton_Click(object sender, RoutedEventArgs e)
        {
            // ShowPrintUI() est la méthode moderne de WebView2 (mieux que window.print())
            pdfWebView?.CoreWebView2.ShowPrintUI();
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".pdf",
                Filter = "Fichier PDF |*.pdf",
                FileName = "Document_Judo.pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllBytes(dlg.FileName, _document);
                }
                catch (Exception ex)
                {
                    HandyControl.Controls.MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}");
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            pdfWebView.Dispose();

            try
            {
                if (!string.IsNullOrEmpty(_tempFilePath) && File.Exists(_tempFilePath))
                {
                    File.Delete(_tempFilePath);
                }
            }
            catch { }

            base.OnClosed(e);
        }
    }
}