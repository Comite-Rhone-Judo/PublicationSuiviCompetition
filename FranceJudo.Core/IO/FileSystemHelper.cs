using FranceJudo.Core.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace FranceJudo.Core.IO
{
    public static class FileSystemHelper
    {
        // Classe interne pour gérer le verrou et son compteur d'utilisation
        private class FileLockEntry
        {
            public readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
            public int ReferenceCount = 0;
        }

        // Remplacement du dictionnaire + Mutex buggé par un ConcurrentDictionary très performant
        // Gère nativement la concurrence sans avoir besoin d'un 'lock' global.
        private static readonly ConcurrentDictionary<string, FileLockEntry> _fileLocks = new ConcurrentDictionary<string, FileLockEntry>(StringComparer.OrdinalIgnoreCase);

        // Champ statique encapsulé proprement
        public static Encoding TheEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Bloque l'exécution jusqu'à ce que l'application ait l'exclusivité d'accès interne sur le chemin du fichier.
        /// </summary>
        public static void NeedAccessFile(string file)
        {
            FileLockEntry entry;

            // ÉTAPE 1 : Récupérer ou créer l'entrée en incrémentant proprement le compteur
            // On utilise un lock local ici pour garantir l'atomicité entre la création et l'incrément
            lock (_fileLocks)
            {
                entry = _fileLocks.GetOrAdd(file, _ => new FileLockEntry());
                Interlocked.Increment(ref entry.ReferenceCount);
            }

            // ÉTAPE 2 : Attente du sémaphore (Logique inchangée)
            if (!entry.Semaphore.Wait(TimeSpan.FromSeconds(5)))
            {
                // En cas d'échec, il ne faut pas oublier de décrémenter car on n'ira pas dans ReleaseFile
                DecrementReference(file, entry);
                LogTools.Logger.Debug("Impossible d'obtenir l'accès au verrou logique pour le fichier '{0}'", file);
                throw new TimeoutException($"Le fichier {file} est verrouillé par un autre thread interne.");
            }

            WaitForPhysicalFileRelease(file);
        }


        /// <summary>
        /// Libère l'accès au fichier pour les autres threads de l'application.
        /// </summary>
        public static void ReleaseFile(string file)
        {
            if (_fileLocks.TryGetValue(file, out FileLockEntry entry))
            {
                // Libère le sémaphore pour le prochain thread
                entry.Semaphore.Release();

                // Décrémente et nettoie si nécessaire
                DecrementReference(file, entry);
            }
        }

        private static void WaitForPhysicalFileRelease(string file)
        {
            int index = 0;
            while (File.Exists(file) && IsFileLocked(file))
            {
                if (++index > 20)
                {
                    LogTools.Logger.Debug("Le système d'exploitation bloque le fichier '{0}'", file);
                    throw new UnauthorizedAccessException($"L'accès au fichier {file} est refusé par l'OS.");
                }
                Thread.Sleep(100);
            }
        }

        private static void DecrementReference(string file, FileLockEntry entry)
        {
            lock (_fileLocks)
            {
                if (Interlocked.Decrement(ref entry.ReferenceCount) == 0)
                {
                    // Si plus personne n'utilise ce fichier, on le retire du dictionnaire
                    if (_fileLocks.TryRemove(file, out var removedEntry))
                    {
                        removedEntry.Semaphore.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Détermine l'encodage d'un fichier en toute sécurité.
        /// </summary>
        public static Encoding GetFileEncoding(string srcFile)
        {
            if (!File.Exists(srcFile)) return Encoding.Default;

            byte[] buffer = new byte[4];

            using (var file = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Read(buffer, 0, 4);
            }

            // UTF-8 avec BOM
            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf) return Encoding.UTF8;

            // UTF-16 Big Endian (BE) - Correction ici
            if (buffer[0] == 0xfe && buffer[1] == 0xff) return Encoding.BigEndianUnicode;

            // UTF-16 Little Endian (LE)
            if (buffer[0] == 0xff && buffer[1] == 0xfe) return Encoding.Unicode;

            // UTF-32
            if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff) return Encoding.UTF32;

            return Encoding.Default;
        }

        /// <summary>
        /// Vérifie si un fichier est verrouillé par un processus externe.
        /// </summary>
        public static bool IsFileLocked(string filename)
        {
            try
            {
                // Un using est obligatoire ici aussi pour éviter de verrouiller le fichier
                // si la vérification réussit !
                using (var stream = new FileInfo(filename).Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        /// <summary>
        /// Suppression d'un fichier (sans Race Condition)
        /// </summary>
        public static bool DeleteFile(string filename)
        {
            if (!File.Exists(filename)) return true;

            try
            {
                // On essaie directement. On ne vérifie pas "IsFileLocked" avant, 
                // c'est à l'OS de nous jeter si on n'a pas le droit.
                File.Delete(filename);
                return true;
            }
            catch (Exception ex)
            {
                LogTools.Logger.Warn(ex, $"Erreur lors de la suppression de {filename}");
                return false;
            }
        }

        /// <summary>
        /// Suppression d'un répertoire (Optimisé via le Framework)
        /// </summary>
        public static bool DeleteDirectory(string directoryname, bool onlyContent = false)
        {
            if (!Directory.Exists(directoryname)) return true;

            try
            {
                if (onlyContent)
                {
                    // Vider le contenu sans supprimer la racine
                    DirectoryInfo di = new DirectoryInfo(directoryname);
                    foreach (FileInfo file in di.GetFiles()) file.Delete();
                    foreach (DirectoryInfo dir in di.GetDirectories()) dir.Delete(true);
                }
                else
                {
                    // Délégation totale au système d'exploitation (beaucoup plus rapide)
                    Directory.Delete(directoryname, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, $"Erreur lors de la suppression du dossier {directoryname}");
                return false;
            }
        }

        public static void CreateDirectory(string directory) // Correction de la faute de frappe : CreateDirectorie
        {
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Fatal(ex, $"Impossible de créer le répertoire {directory}");
                }
            }
        }

        public static string PathJoin(string path1, string path2, bool endWithSeparator = false, bool unixStyle = false)
        {
            if (string.IsNullOrEmpty(path1)) return path2;
            if (string.IsNullOrEmpty(path2)) return path1;

            char dirSep = unixStyle ? Path.AltDirectorySeparatorChar : Path.DirectorySeparatorChar;
            string temp = (path1.TrimEnd(dirSep) + dirSep + path2.TrimStart(dirSep)).TrimEnd(dirSep);

            return endWithSeparator ? temp + Path.DirectorySeparatorChar : temp;
        }

        public static string GetMimeType(this FileInfo fileInfo)
        {
            string mimeType = "application/octet-stream";

            try
            {
                // Attention : Ce code est 100% couplé à Windows.
                using (RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(fileInfo.Extension.ToLower()))
                {
                    if (regKey != null)
                    {
                        object contentType = regKey.GetValue("Content Type");
                        if (contentType != null) mimeType = contentType.ToString();
                    }
                }
            }
            catch
            {
                // Fallback silencieux si on n'a pas les droits sur le registre ou si on n'est pas sur Windows
            }

            return mimeType;
        }

        private static readonly string[] _sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(this ulong value)
        {
            if (value == 0) { return "0.0 " + _sizeSuffixes[0]; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, _sizeSuffixes[mag]);
        }
    }
}