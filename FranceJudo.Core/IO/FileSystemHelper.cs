using FranceJudo.Core.Logging;
using FranceJudo.Core.Threading;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace FranceJudo.Core.IO
{
    public static class FileSystemHelper
    {
        private static readonly IDictionary<string, Mutex> Files_mutex = new Dictionary<string, Mutex>();

        public static void NeedAccessFile(string file)
        {
            int index = 0;
            while (File.Exists(file) && FileSystemHelper.IsFileLocked(file))
            {
                if (++index > 20)
                {
                    LogTools.Logger.Debug("Impossible d'obtenir l'access au fichier '{0}'", file);
                    throw new UnauthorizedAccessException();
                }
                Thread.Sleep(100);
            }
            using (TimedLock.Lock((FileSystemHelper.Files_mutex as ICollection).SyncRoot))
            {
                if (!FileSystemHelper.Files_mutex.ContainsKey(file))
                {
                    FileSystemHelper.Files_mutex.Add(file, new Mutex(false, Guid.NewGuid().ToString()));
                }
            }
            bool res = FileSystemHelper.Files_mutex[file].WaitOne();
        }

        public static void ReleaseFile(string file)
        {
            if (FileSystemHelper.Files_mutex.ContainsKey(file))
            {
                FileSystemHelper.Files_mutex[file].ReleaseMutex();
            }
        }


        public static Encoding TheEncoding = Encoding.UTF8;

        /// <summary>
        /// Détermine l'encodage d'un fichier
        /// </summary>
        /// <param name="srcFile">le file</param>
        /// <returns>Encoding</returns>

        public static Encoding GetFileEncoding(string srcFile)
        {
            // *** Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            FileStream file = new FileStream(srcFile, FileMode.Open);
            file.Read(buffer, 0, 5);
            file.Close();

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;
            else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                // 1201 unicodeFFFE Unicode (Big-Endian)
                enc = Encoding.GetEncoding(1201);
            else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                // 1200 utf-16 Unicode
                enc = Encoding.GetEncoding(1200);


            return enc;
        }

        /// <summary>
        /// Check a file is in use
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileLocked(string filename)
        {
            FileInfo file = new FileInfo(filename);

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }

        /// <summary>
        /// Suppression de fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool DeleteFile(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    return true;
                }

                if (!FileSystemHelper.IsFileLocked(filename))
                {
                    File.Delete(filename);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogTools.Warning(ex);
            }

            return false;
        }

        /// <summary>
        /// Suppression d'un répertoire (avec vérification de suppression des fichier contenu)
        /// </summary>
        /// <param name="directoryname">Repertoire cible</param>
        /// <param name="onlyContent">Si True, efface le contenu mais pas le repertoire designe. Si False, efface aussi le repertoire designe</param>
        /// <returns></returns>
        public static bool DeleteDirectory(string directoryname, bool onlyContent = false)
        {
            if (Directory.Exists(directoryname))
            {
                try
                {
                    foreach (string directory in Directory.GetDirectories(directoryname))
                    {
                        FileSystemHelper.DeleteDirectory(directory);
                    }

                    foreach (string file in Directory.GetFiles(directoryname))
                    {
                        FileSystemHelper.DeleteFile(file);
                    }

                    // Si pas uniquement le contenu, efface le repertoire designe
                    if (!onlyContent)
                    {
                        Directory.Delete(directoryname);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    LogTools.Error(ex);
                }

                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Création d'un répertoire (s'il n'existe pas)
        /// </summary>
        /// <param name="directory"></param>
        public static void CreateDirectorie(string directory)
        {

            //LogTools.Trace("CREATION DU REPERTOIRE " + directory, LogTools.Level.DEBUG);
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    LogTools.Fatal(ex);
                }
            }
            //LogTools.Trace("REPERTOIRE CREE " + directory, LogTools.Level.DEBUG);
        }

        /// <summary>
        /// Combine 2 paths en un seul en prenant compte les caracteres de separation
        /// </summary>
        /// <param name="path1">Path de debut</param>
        /// <param name="path2">Path de fin</param>
        /// <returns></returns>
        public static string PathJoin(string path1, string path2, bool endWithSeparator = false, bool unixStyle = false)
        {
            if (string.IsNullOrEmpty(path1))
            {
                return path2;
            }

            if (string.IsNullOrEmpty(path2))
            {
                return path1;
            }

            char dirSep = unixStyle ? Path.AltDirectorySeparatorChar : Path.DirectorySeparatorChar;

            string temp = (path1.TrimEnd(dirSep) + dirSep + path2.TrimStart(dirSep)).TrimEnd(dirSep);

            return endWithSeparator ? temp + Path.DirectorySeparatorChar : temp;
        }

        /// <summary>
        /// Détermine le type MIME d'un fichier
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        public static string GetMimeType(this FileInfo fileInfo)
        {
            string mimeType = "application/octet-stream";

            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(fileInfo.Extension.ToLower());

            if (regKey != null)
            {
                object contentType = regKey.GetValue("Content Type");

                if (contentType != null)
                    mimeType = contentType.ToString();
            }

            return mimeType;
        }

        private static readonly string[] _sizeSuffixes =
           { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        /// <summary>
        /// Retourne un suffix standard de taille de fichier (KB, etc.)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SizeSuffix(this ulong value)
        {
            if (value == 0) { return "0.0 " + _sizeSuffixes[0]; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, _sizeSuffixes[mag]);
        }
    }
}
