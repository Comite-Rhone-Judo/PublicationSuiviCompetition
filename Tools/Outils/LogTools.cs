using NLog;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using Telerik.Windows.Zip;
using Tools.CustomException;
using Tools.Enum;
using Tools.Windows;
using NLog.Fluent;

namespace Tools.Outils
{
    public static class LogTools
    {
        public enum Level : int
        {
            FATAL = 0,
            ERROR = 1,
            WARN = 2,
            INFO = 3,
            DEBUG = 4
        }

        /// <summary>
        /// Define a static logger variable so that it references
        /// </summary>
        private static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        // private static Logger Logger { get { return _logger; } }

        #region PROXY vers le Logger

        public static void Error(string msg) { _logger.Error(msg); }

        public static void Error(Exception ex) { _logger.Error(ex); }

        public static void Warning(string msg) { _logger.Warn(msg); }

        public static void Warning(Exception ex) { _logger.Warn(ex); }

        public static void Info(string msg) { _logger.Info(msg); }

        public static void Info(Exception ex) { _logger.Info(ex); }

        public static void Fatal(string msg) { _logger.Fatal(msg); }

        public static void Fatal(Exception ex) { _logger.Fatal(ex); }

        public static void Debug(string msg) { _logger.Debug(msg); }

        public static void Debug(Exception ex) { _logger.Debug(ex); }

        #endregion

        public static void LogStartup() 
        {
            _logger.Info("-----------------------------------------------------------------------------------------------------");
            _logger.Info("App Publication is starting - Version " + OutilsTools.GetVersionApp().ToString());
        }


        /// <summary>
        /// Affiche un message d'alert (trace l'alerte dans le fichier de LOG)
        /// </summary>
        /// <param name="message"></param>
        public static void Alert(string message, string header = "Attention", string button = "OK")
        {
            LogTools.Warning(message);
            PrintAlert(message, header, button);
        }

        private static void PrintAlert(string message, string header, string button)
        {
            Application.Current.ExecOnUiThread(new Action(() =>
            {
                AlertWindow win = new AlertWindow(header, message);
                win.Show();
            }));
        }


        // TODO A remplacer par la creation du package
        /// <summary>
        /// Récupère le fichier de LOG
        /// </summary>
        /*
        public static void EnregistreLog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Zip File | *.zip";
            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                // Recupere le nom du fichier zip de destination
                string zipFileName = dialog.FileName;
                using (Stream stream = File.Open(zipFileName, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: false, entryNameEncoding: null))
                    {
                        foreach (string file in Directory.GetFiles(ConstantFile.Log))
                        {
                            if (file.Contains("__copy"))
                            {
                                continue;
                            }
                            string file2 = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + "__copy1" + Path.GetExtension(file);
                            File.Copy(file, file2, true);

                            using (FileStream fs1 = File.OpenRead(file2))
                            {
                                using (ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(file)))
                                {
                                    using (Stream entryStream = entry.Open())
                                    {
                                        fs1.CopyTo(entryStream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        */

        /// <summary>
        /// TODO ajouter la fonction pour faire un zip des traces + copie sur le bureau pour copie
        /// </summary>
        public static void PackageLog()
        {

        }

        // TODO A supprimer, c'est inutile
        /// <summary>
        /// EnvoiLog : Envoie par MAIL les différents logs 
        /// </summary>
        // Plus d'envoit de mail en direct
        /*
        public static void EnvoiLog()
        {
            try
            {
                bool envoi = OutilsTools.IsDebug;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_email);
                mail.To.Add(new MailAddress(_email));
                mail.IsBodyHtml = true;

                string body = "";
                string pattern = @"([0-9]{4}\-[0-9]{2}\-[0-9]{2})";
                Regex regex = new Regex(pattern);

                foreach (string file in Directory.GetFiles(ConstantFile.Log))
                {
                    if (file.Contains("__copy"))
                    {
                        continue;
                    }

                    string file2 = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + "__copy1" + Path.GetExtension(file);
                    File.Copy(file, file2, true);

                    string file_copy = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + "__copy2" + Path.GetExtension(file);
                    if (!File.Exists(file_copy))
                    {
                        using (FileStream st = File.Create(file_copy))
                        {

                        }
                    }


                    Encoding enc = FileAndDirectTools.GetFileEncoding(file2);

                    using (StreamReader reader = new StreamReader(file2, enc, true))
                    {
                        using (StreamReader reader_copy = new StreamReader(file_copy, enc, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                string line_copy = reader_copy.ReadLine();
                                if (line != null && line_copy != null && line_copy == line)
                                {
                                    continue;
                                }

                                if (OutilsTools.IsDebug || line.Contains("ERROR") || line.Contains("FATAL"))
                                {
                                    envoi = true;
                                }
                                body += line + "<br/>";
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(body) && envoi)
                {
                    mail.Body = body;
                    mail.Subject = "[TAS] v." + OutilsTools.GetVersionApp().ToString() + " " + Environment.MachineName + " " + Environment.UserName;
                    Thread thread = new Thread(new ThreadStart(() => Envoie(mail)));
                    thread.Start();
                }
            }
            catch
            {
            }
        }
        */

        // On ne travaille plus par envoie de mail directement depuis l'application
        /*
        public static void Envoie(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = _host;
            client.Port = _port;
            client.UseDefaultCredentials = _useDefaultCredentials;
            client.EnableSsl = _enableSsl;
            client.Credentials = new NetworkCredential(_email, _pass);
            try
            {
                client.Send(mail);
                foreach (string file in Directory.GetFiles(ConstantFile.Log))
                {
                    if (file.Contains("__copy"))
                    {
                        continue;
                    }

                    string file2 = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + "__copy2" + Path.GetExtension(file);
                    File.Copy(file, file2, true);
                }
            }
            catch
            {
            }
        }
        */
    }
}
