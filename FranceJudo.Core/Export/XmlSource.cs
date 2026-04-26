using FranceJudo.Core.Logging;
using NLog.Targets;
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

        public XDocument Document => _document;

        public XmlSource(XDocument doc)
        {
            _document = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        public XmlReader CreateReader()
        {
            // Les paramètres magiques pour l'optimisation mémoire
            var settings = new XmlReaderSettings
            {
                NameTable = new NameTable(),
                IgnoreWhitespace = true
            };

            // Cas A : Le document est resté en RAM (Petit fichier)
            return XmlReader.Create(_document.CreateReader(), settings);
        }

        public void Dispose()
        {
            _document = null;
        }
    }
}