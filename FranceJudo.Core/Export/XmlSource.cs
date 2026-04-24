using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace FranceJudo.Core.Export
{
    /// <summary>
    /// Wrapper pour XDocument gérant l'économie de RAM via un Flush disque optionnel.
    /// </summary>
    public class XmlSource : IDisposable
    {
        private XDocument _document;
        private string _tempFilePath;
        private FileStream _tempStream;

        public XDocument Document => _document;

        public XmlSource(XDocument doc, bool flushToDisk)
        {
            _document = doc ?? throw new ArgumentNullException(nameof(doc));

            if (flushToDisk)
            {
                FlushToDisk();
            }
        }

        private void FlushToDisk()
        {
            _tempFilePath = Path.GetTempFileName();
            // FileOptions.DeleteOnClose : le fichier est supprimé par l'OS dès que le Stream est disposé
            _tempStream = new FileStream(_tempFilePath, FileMode.Create, FileAccess.ReadWrite,
                FileShare.None, 4096, FileOptions.DeleteOnClose);

            _document.Save(_tempStream);
            _tempStream.Flush();

            // LIBÉRATION MÉMOIRE : On détruit l'objet XDocument pour le GC
            _document = null;
        }

        public XmlReader CreateReader()
        {
            if (_document != null)
                return _document.CreateReader();

            _tempStream.Position = 0;
            return XmlReader.Create(_tempStream);
        }

        public void Dispose()
        {
            _tempStream?.Dispose();
            // Sécurité supplémentaire si DeleteOnClose n'a pas suffi
            if (_tempFilePath != null && File.Exists(_tempFilePath))
            {
                try { File.Delete(_tempFilePath); } catch { /* Ignore */ }
            }
        }
    }
}