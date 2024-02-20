using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Tools.Enum;

namespace Tools.Outils
{
    public static class FileAndDirectTools
    {
        private static IDictionary<string, Mutex> Files_mutex = new Dictionary<string, Mutex>();

        public static void NeedAccessFile(string file)
        {
            int index = 0;
            while (File.Exists(file) && FileAndDirectTools.IsFileLocked(file))
            {
                if (++index > 20)
                {
                    throw new UnauthorizedAccessException();
                }
                Thread.Sleep(100);
            }
            using (TimedLock.Lock((FileAndDirectTools.Files_mutex as ICollection).SyncRoot))
            {
                if (!FileAndDirectTools.Files_mutex.ContainsKey(file))
                {
                    FileAndDirectTools.Files_mutex.Add(file, new Mutex(false, Guid.NewGuid().ToString()));
                }
            }
            bool res = FileAndDirectTools.Files_mutex[file].WaitOne();
        }

        public static void ReleaseFile(string file)
        {
            if (FileAndDirectTools.Files_mutex.ContainsKey(file))
            {
                FileAndDirectTools.Files_mutex[file].ReleaseMutex();
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
                if (stream != null)
                {
                    stream.Close();
                }
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

                if (!FileAndDirectTools.IsFileLocked(filename))
                {
                    File.Delete(filename);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogTools.Trace(ex, LogTools.Level.WARN);
            }

            return false;
        }

        /// <summary>
        /// Suppression d'un répertoire (avec vérification de suppression des fichier contenu)
        /// </summary>
        /// <param name="directoryname"></param>
        /// <returns></returns>
        public static bool DeleteDirectory(string directoryname)
        {
            if (Directory.Exists(directoryname))
            {
                try
                {
                    foreach (string directory in Directory.GetDirectories(directoryname))
                    {
                        FileAndDirectTools.DeleteDirectory(directory);
                    }

                    foreach (string file in Directory.GetFiles(directoryname))
                    {
                        FileAndDirectTools.DeleteFile(file);
                    }
                    Directory.Delete(directoryname);
                    return true;
                }
                catch (Exception ex)
                {
                    LogTools.Trace(ex, LogTools.Level.WARN);
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
                    LogTools.Log(ex, LogTools.Level.FATAL);
                }
            }
            //LogTools.Trace("REPERTOIRE CREE " + directory, LogTools.Level.DEBUG);
        }

        /// <summary>
        /// Création des répertoires nécessaires au fonctionnement de l'application (sauf l'export du site !!)
        /// </summary>
        /// 
        public static void InitDataDirectories()
        {

            string directory = ConstantFile.Export_dir;

            FileAndDirectTools.CreateDirectorie(ConstantFile.BD_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.Data_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.Export_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.ExportStyle_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.ExportStyleSite_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.ExportStyleIcon_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.ExportStyleDiplome_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.ExportJudoTV); 
            FileAndDirectTools.CreateDirectorie(ConstantFile.DirectorySave);
            FileAndDirectTools.CreateDirectorie(ConstantFile.SaveCSDirectory);
            FileAndDirectTools.CreateDirectorie(ConstantFile.SavePeseeDirectory);
            FileAndDirectTools.CreateDirectorie(ConstantFile.SaveCOMDirectory);
            FileAndDirectTools.CreateDirectorie(ConstantFile.Params_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.Logo1_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.Logo2_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.Logo3_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.Logo_tmp_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.MediaSon_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.MediaVideo_dir);
            FileAndDirectTools.CreateDirectorie(ConstantFile.MediaFlags_dir);

            FileAndDirectTools.CreateDirectorie(directory + "site");
            //if (!Directory.Exists(directory + "site"))
            //{
            //    Directory.CreateDirectory(directory + "site");
            //}

            if (OutilsTools.GetAppDirectory() == OutilsTools.GetDataDirectory())
            {
                //return;
            }

            string[] files = ResourcesTools.GetAssembyResourceName();
            foreach (string s1 in files)
            {
                if ((!s1.Contains(ConstantResource.Export) && !s1.Contains(ConstantResource.Media)) || s1.Contains(ConstantResource.Export_site_js) || s1.Contains(ConstantResource.Export_xslt))
                {
                    continue;
                }

                string dir_copy = ConstantFile.ExportStyle_dir;
                string fileName = s1.Replace(ConstantResource.Export_style_res, "");

                if (s1.Contains(ConstantResource.Export_site_style))
                {
                    dir_copy = ConstantFile.ExportStyleSite_dir;
                    fileName = s1.Replace(ConstantResource.Export_site_style, "");
                }

                if (s1.Contains(ConstantResource.Export_Icon))
                {
                    dir_copy = ConstantFile.ExportStyleIcon_dir;
                    fileName = s1.Replace(ConstantResource.Export_Icon, "");
                }

                if (s1.Contains(ConstantResource.Export_Diplome))
                {
                    dir_copy = ConstantFile.ExportStyleDiplome_dir;
                    fileName = s1.Replace(ConstantResource.Export_Diplome, "");
                }

                if (s1.Contains(ConstantResource.Media_Son))
                {
                    dir_copy = ConstantFile.MediaSon_dir;
                    fileName = s1.Replace(ConstantResource.Media_Son, "");
                }

                if (s1.Contains(ConstantResource.Media_Video))
                {
                    dir_copy = ConstantFile.MediaVideo_dir;
                    fileName = s1.Replace(ConstantResource.Media_Video, "");
                }

                if (s1.Contains(ConstantResource.Media_Flags))
                {
                    dir_copy = ConstantFile.MediaFlags_dir;
                    fileName = s1.Replace(ConstantResource.Media_Flags, "");
                }

                var resource = ResourcesTools.GetAssembyResource(s1);
                try
                {
                    FileAndDirectTools.NeedAccessFile(dir_copy + fileName);
                    using (FileStream fs = new FileStream(dir_copy + fileName, FileMode.Create))
                    {
                        byte[] bytes = new byte[resource.Length];
                        resource.Read(bytes, 0, (int)resource.Length);
                        fs.Write(bytes, 0, bytes.Length);
                        resource.Close();
                    }
                }
                catch { }
                finally
                {
                    FileAndDirectTools.ReleaseFile(dir_copy + fileName);
                }
            }
        }
    }
}
