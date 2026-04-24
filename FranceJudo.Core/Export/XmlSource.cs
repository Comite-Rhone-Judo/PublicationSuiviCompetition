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
        private readonly string _tempFilePath;
        private bool _isFlushed = false;

        public XDocument Document => _document;

        public XmlSource(XDocument doc, bool flushToDisk)
        {
            _document = doc ?? throw new ArgumentNullException(nameof(doc));

            if (flushToDisk)
            {
                // On prépare le chemin temporaire uniquement si on flush
                _tempFilePath = Path.GetTempFileName();
                FlushToDisk();
            }
        }

        private void FlushToDisk()
        {
            LogTools.Logger.Debug($"Flush to disk: {_tempFilePath} document {_document.Root?.Attribute("type")?.Value}");

            // 1. On écrit le fichier et ON LE FERME complètement.
            // Cela libère le verrou exclusif et empêche les collisions de pointeurs entre threads.
            using (var fs = new FileStream(_tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                _document.Save(fs);
            }

            // 2. LIBÉRATION MÉMOIRE : On détruit l'objet XDocument pour le GC (Gen 0)
            _document = null;
            _isFlushed = true;
        }

        public XmlReader CreateReader()
        {
            // Les paramètres magiques pour l'optimisation mémoire
            var settings = new XmlReaderSettings
            {
                NameTable = new NameTable(),
                IgnoreWhitespace = true,
                CloseInput = true // CRUCIAL : Demande au XmlReader de fermer le FileStream sous-jacent à la fin
            };

            // Cas A : Le document est resté en RAM (Petit fichier)
            if (!_isFlushed)
            {
                return XmlReader.Create(_document.CreateReader(), settings);
            }

            // Cas B : Le document a été flushé sur le disque (Gros fichier partagé)
            // On ouvre un NOUVEAU flux indépendant pour ce thread. 
            // FileShare.Read autorise vos autres threads à l'ouvrir en même temps.
            var fs = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return XmlReader.Create(fs, settings);
        }

        public void Dispose()
        {
            // Comme nous avons retiré FileOptions.DeleteOnClose (pour gérer les flux multiples),
            // c'est à nous de supprimer physiquement le fichier quand le batcher a terminé.
            if (_isFlushed && _tempFilePath != null && File.Exists(_tempFilePath))
            {
                try
                {
                    File.Delete(_tempFilePath);
                }
                catch
                {
                    // Sécurité : ignore l'erreur si l'OS n'a pas encore totalement relâché le fichier
                }
            }
        }
    }
}