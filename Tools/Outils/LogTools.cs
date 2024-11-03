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

        private static readonly string _email = "seguin@critt-informatique.fr";
        private static readonly string _pass = "seguin86";

        private static readonly string _host = "ex2.mail.ovh.net";
        private static readonly int _port = 587;
        private static readonly bool _useDefaultCredentials = true;
        private static readonly bool _enableSsl = false;


        private static readonly string _separatorHeader = "__________________________________________________________________________________________________________________________";
        private static readonly string _separatorTrace = " - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";

        private static readonly string _header1 = "      ";
        private static readonly string _header2 = Environment.MachineName + " " + Environment.UserName;
        public static string HeaderType = "";
        public static string HeaderCompetition = "";
        private static string _version = "[TAS] v." + OutilsTools.GetVersionApp().ToString();

        /// <summary>
        /// Define a static logger variable so that it references
        /// </summary>
        private static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public static Logger Logger { get { return _logger; } }

        static readonly object lockerLog = new object();

        // TODO reprendre les fonctions de Log, on peut utiliser directement les fonctions de NLog
        /// <summary>
        /// Enregistre dans le fichier de LOG l'exception
        /// </summary>
        /// <param name="ex">l'exception</param>

        public static void Trace(Exception ex, Level level = Level.ERROR)
        {
            List<string> messages = new List<string>();

            messages.Add(_separatorHeader);
            messages.Add(_version + "    " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            messages.Add(HeaderType + _header1 + " - " + level.ToString());
            if (!string.IsNullOrWhiteSpace(HeaderCompetition)) { messages.Add(HeaderCompetition); }
            messages.Add("");

            Exception ex2 = ex;
            while (ex2 != null)
            {
                if (!String.IsNullOrWhiteSpace(ex2.Message))
                {
                    foreach (string text in ex2.Message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        messages.Add(text);
                    }
                }

                if (!String.IsNullOrWhiteSpace(ex2.StackTrace))
                {
                    foreach (string text in ex2.StackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        messages.Add(text);
                    }
                }

                ex2 = ex2.InnerException;
                if (ex2 != null)
                {
                    messages.Add(_separatorTrace);
                }
            }
            Trace(messages, level);
        }

        public static void Trace(string message, Level level = Level.ERROR)
        {
            List<string> messages = new List<string>();

            messages.Add(_separatorHeader);
            messages.Add(HeaderType + _header1 + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " - " + level.ToString());
            messages.Add("");

            foreach (string text in message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                messages.Add(text);
            }
            Trace(messages, level);
        }

        private static void Trace(List<string> messages, Level level = Level.ERROR)
        {
            using (TimedLock.Lock(lockerLog))
            {
                switch (level)
                {
                    case Level.FATAL:
                        foreach (string message in messages)
                        {
                            Logger.Fatal(message);
                        }
                        break;
                    case Level.ERROR:
                        foreach (string message in messages)
                        {
                            Logger.Error(message);
                        }
                        break;
                    case Level.WARN:
                        if (OutilsTools.IsDebug)
                        {
                            foreach (string message in messages)
                            {
                                Logger.Warn(message);
                            }
                        }
                        break;
                    case Level.INFO:
                        if (OutilsTools.IsDebug)
                        {
                            foreach (string message in messages)
                            {
                                Logger.Info(message);
                            }
                        }
                        break;
                    case Level.DEBUG:
                        if (OutilsTools.IsDebug)
                        {
                            foreach (string message in messages)
                            {
                                Logger.Debug(message);
                            }
                        }
                        break;
                    default:
                        foreach (string message in messages)
                        {
                            Logger.Error(message);
                        }
                        break;
                }
            }
        }


        static ICollection<Type> types = new List<Type>
        {
            typeof(SocketException),
            typeof(JudoClientException),
            typeof(JudoServerException)
        };

        static ICollection<string> messages = new List<string>
        {
            "TcpClient",
            "Socket",
            "RenderTransform.ScaleX"
        };

        private static bool IsNonTraiteException(Exception ex)
        {
            Exception ex2 = ex;
            while (ex2 != null)
            {
                foreach (Type type in types)
                {
                    if (ex2.GetType() == type)
                    {
                        return true;
                    }
                }

                foreach (string message in messages)
                {
                    if (ex2.Message != null && ex2.Message.Contains(message))
                    {
                        return true;
                    }
                    if (ex2.StackTrace != null && ex2.StackTrace.Contains(message))
                    {
                        return true;
                    }
                }

                ex2 = ex2.InnerException;
            }
            return false;
        }

        private struct ContainsException
        {
            public Exception exept { get; set; }
            public DateTime date { get; set; }
        }

        private static IList<ContainsException> exceptions = new List<ContainsException>();

        private static bool IsContainsException(Exception ex)
        {
            string ex1 = ex.Message + " " + ex.StackTrace;
            foreach (ContainsException ex2 in exceptions)
            {
                if ((DateTime.Now - ex2.date).TotalMinutes > 15)
                {
                    break;
                }

                if (ex1 == (ex2.exept.Message + " " + ex2.exept.StackTrace))
                {
                    return true;
                }
            }

            exceptions.Add(new ContainsException { exept = ex, date = DateTime.Now });
            return false;
        }

        /// <summary>
        /// Enregistre dans le fichier de LOG l'exception et l'affiche dans une fenêtre
        /// </summary>
        /// <param name="ex">l'exception</param>

        public static void Log(Exception ex, LogTools.Level level = LogTools.Level.ERROR)
        {
            if (LogTools.IsContainsException(ex))
            {
                return;
            }

            if (!LogTools.IsNonTraiteException(ex))
            {
                LogTools.Trace(ex, level);

                string message = "";
                //message += "Erreur\n";
                message += ex.Message + "\n\n";
                message += "Une erreur est survenue. Un mail automatique va être transmis lors de votre prochaine connexion, à l\'administrateur de l'application.";

                LogTools.PrintAlert(message, "Erreur", "OK");
            }
            else
            {
                LogTools.Trace(ex, LogTools.Level.INFO);
            }
        }

        /// <summary>
        /// Affiche un message d'alert (trace l'alerte dans le fichier de LOG)
        /// </summary>
        /// <param name="message"></param>
        public static void Alert(string message, string header = "Attention", string button = "OK")
        {
            LogTools.Trace(message, Level.WARN);
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

        /// <summary>
        /// TODO ajouter la fonction pour faire un zip des traces + copie sur le bureau pour envoie
        /// </summary>
        public static void PackageLog()
        {

        }

        // TODO A supprimer, c'est inutile
        /// <summary>
        /// EnvoiLog : Envoie par MAIL les différents logs 
        /// </summary>
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
    }
}
