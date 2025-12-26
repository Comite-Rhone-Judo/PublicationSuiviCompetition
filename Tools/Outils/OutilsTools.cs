using Microsoft.Win32;
using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Tools.Windows;
using Tools.Enum;
using System.Reflection;

namespace Tools.Outils
{
    public static class OutilsTools
    {
        public delegate void MontreInformation1(int index, int maximum, string info1);
        public delegate void MontreInformation2(bool IsBusy, string info1);

        private static bool _debug = (Environment.MachineName == "DESKTOP-U229EAL");

        public static bool IsDebug = _debug /*|| !ApplicationDeployment.IsNetworkDeployed*/;

        public static Regex RegexLicence = new Regex("[F|M][0-3][0-9][0-1][0-9][1|2][0-9]{3}[A-Z|*|-]{5}[0-9]{2}");

        //private static int NB_TASK = Math.Max(6, Environment.ProcessorCount * 3);// 48;
        //private static LimitedConcurrencyLevel scheduler = new LimitedConcurrencyLevel(NB_TASK);
        //public static TaskFactory Factory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, scheduler);
        public static TaskFactory Factory = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.PreferFairness);

        public static int Grade1D_ID = 14;
        public static int Grade7D_ID = 20;
        public static int GradeNO_ID = 26;

        /// <summary>
        /// Affiche une RadWindow dans la TaskBar de Windows
        /// </summary>
        /// <param name="control"></param>

        public static void ShowInTaskbar(RadWindow control)
        {
            control.Loaded += new RoutedEventHandler(TaskbarRadWindow_Loaded);

        }

        static void TaskbarRadWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = ((RadWindow)sender).ParentOfType<System.Windows.Window>();
            if (window != null)
            {
                window.ShowInTaskbar = true;
                window.Title = ((RadWindow)sender).Header.ToString();
                //window.StateChanged += new EventHandler(window_StateChanged);
            }
        }

        static void window_StateChanged(object sender, EventArgs e)
        {
            var window = ((RadWindow)sender).ParentOfType<System.Windows.Window>();
            ((RadWindow)sender).WindowState = window.WindowState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetAddressClient(TcpClient client)
        {
            try
            {
                // return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                // Ajoute le remote port pour pouvoir distinguer deux clients lancés sur le meme poste
                string ipAddr = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                string port = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
                return string.Format("{0}_{1}", ipAddr, port);
            }
            catch
            {
                return client.Client.RemoteEndPoint.ToString();
            }
        }

        /// <summary>
        /// Traite une chaine prénom
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>

        public static string FormatPrenom(string chaine)
        {
            string result = "";
            bool maj = true;
            foreach (char ch in chaine.ToList())
            {
                result += (maj ? Char.ToUpper(ch) : Char.ToLower(ch));
                if (ch == '-' || ch == ' ')
                {
                    maj = true;
                }
                else
                {
                    maj = false;
                }
            }

            return result.Trim();
        }

        public static string SubString(string valeur, int start, int longeur)
        {
            if (string.IsNullOrWhiteSpace(valeur) || valeur.Length <= start)
            {
                return "";
            }

            if (valeur.Length - start >= longeur)
            {
                return valeur.Substring(start, longeur);
            }

            return valeur.Substring(start, valeur.Length - start);

        }

        /// <summary>
        /// Traite une chaine pour quelle soit compatible avec une URL
        /// Il ne s'agit pas d'un traitement d'encodage mais de remplacer les symboles dans les categories, etc.
        /// '+' => 'p'
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string TraiteChaineURL(string url)
        {
            return url.Replace("+", "p");
        }

        /// <summary>
        /// Traite une chaine pour la rendre compatible PATH
        /// </summary>
        /// <param name="chaine"></param>
        /// <returns></returns>

        public static string TraiteChaine(string chaine)
        {
            string result = chaine.Replace(" ", "_");

            char[] invalidPathChars = Path.GetInvalidFileNameChars();
            foreach (char invalid in invalidPathChars)
            {
                result = result.Replace(invalid, '_');
            }

            return result.ReplaceDiacritics();
        }

        private static string ReplaceDiacritics(this string source)
        {
            string sourceInFormD = source.Normalize(NormalizationForm.FormD);

            var output = new StringBuilder();
            foreach (char c in sourceInFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    output.Append(c);
            }

            return (output.ToString().Normalize(NormalizationForm.FormC));
        }

        /// <summary>
        /// Traite le texte reçu par la douchette
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ScanneTraiteLicence(string text)
        {
            string chaine1 = text;

            chaine1 = chaine1.Replace('à', '0');
            chaine1 = chaine1.Replace('&', '1');
            chaine1 = chaine1.Replace('é', '2');
            chaine1 = chaine1.Replace('\"', '3');
            chaine1 = chaine1.Replace('\'', '4');
            chaine1 = chaine1.Replace('(', '5');
            chaine1 = chaine1.Replace('-', '6');
            chaine1 = chaine1.Replace('è', '7');
            chaine1 = chaine1.Replace('_', '8');
            chaine1 = chaine1.Replace('ç', '9');


            if (OutilsTools.RegexLicence.IsMatch(chaine1) && !OutilsTools.RegexLicence.IsMatch(text))
            {
                return chaine1;
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// Regarde si première utilisation ou changement de version
        /// </summary>
        /// <returns></returns>

        public static bool ChangeLog()
        {
            //return true;
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                return false;
            }


            if (!ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Racine par defaut pour des donnees France Judo
        /// </summary>
        /// <param name="racine"></param>
        /// <returns></returns>
        public static string GetExportDir(string racine)
        {
            return Path.Combine(racine, "FRANCE-JUDO");
        }

        /// <summary>
        /// Répertoire des DATA
        /// </summary>
        /// <returns>PATH</returns>

        public static string GetDataDirectory()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.DataDirectory + "/";
            }
            else
            {
                return AppDomain.CurrentDomain.BaseDirectory.Replace(@"\", "/");
            }
        }

        /// <summary>
        /// Répertoire de l'APP
        /// </summary>
        /// <returns>PATH</returns>

        public static string GetAppDirectory()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory);
            return uri.LocalPath;// AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Retourne la version de l'APP
        /// </summary>
        /// <returns>Version</returns>

        public static String GetVersionInformation()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                String version = assembly.GetName().Version.ToString();

                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyVersionBeta)) as AssemblyVersionBeta;
                if (myAttr.Value > 0)
                {
                    version += String.Format("-beta{0:00}", myAttr.Value);
                }

                return version; // assembly.GetName().Version;
            }
        }

        public static string GetCompanyInformation()
        {
            string output = string.Empty;

            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;

                output = myAttr.Company;
            }
            catch { }

            return output;
        }

        public static string GetCopyrightInformation()
        {
            string output = string.Empty;

            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;

                output = myAttr.Copyright;
            }
            catch { }

            return output;
        }

        public static string GetTrademarkInformation()
        {
            string output = string.Empty;

            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var myAttr = Attribute.GetCustomAttribute(assembly, typeof(AssemblyTrademarkAttribute)) as AssemblyTrademarkAttribute;

                output = myAttr.Trademark;
            }
            catch { }

            return output;
        }


        /// <summary>
        /// Execute un action dans le thread principale
        /// </summary>
        /// <param name="app"></param>
        /// <param name="action"></param>
        /// <param name="priority"></param>

        public static void ExecOnUiThread(this Application app, Action action, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (app == null)
            {
                return;
            }
            var dispatcher = app.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(priority, action);
        }

        private static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(ulong value)
        {
            //if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 " + SizeSuffixes[0]; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        /// <summary>
        /// Traite un fichier IMAGE pour le dimensionner
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="maxW"></param>
        /// <param name="maxH"></param>
        /// <param name="tag"></param>
        /// <returns></returns>

        public static MemoryStream CreerImage(FileStream stream, int maxW, int maxH, string tag)
        {

            MemoryStream storeStream = new MemoryStream();
            using (Bitmap bmp = new Bitmap(stream))
            {
                double actualW = bmp.Width * 1.0;
                double actualH = bmp.Height * 1.0;

                if (actualH <= maxH && actualW <= maxW)
                {
                    return null;
                    //maxH = (int)actualH;
                    //maxW = (int)actualW;
                }

                int newW = 0;
                int newH = 0;
                if (maxW != 0 && maxH != 0 && actualW != 0 && actualH != 0)
                {

                    double rapportW = maxW / actualW;
                    double rapportH = maxH / actualH;
                    double rapport = rapportW;
                    if (rapportW > rapportH)
                    {
                        rapport = rapportH;
                    }

                    newW = (int)(actualW * rapport);
                    newH = (int)(actualH * rapport);
                }
                else
                {
                    newW = (int)actualW;
                    newH = (int)actualH;
                }

                using (Image thumbnail = new Bitmap(newW, newH))
                {
                    using (Graphics graphic = Graphics.FromImage(thumbnail))
                    {
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphic.SmoothingMode = SmoothingMode.HighQuality;
                        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphic.CompositingQuality = CompositingQuality.HighQuality;

                        graphic.DrawImage(bmp, 0, 0, newW, newH);

                        if (tag != "")
                        {

                            Font font = new Font("Times New Roman", 48);
                            SizeF sizef = graphic.MeasureString(tag, font, Int32.MaxValue);
                            int currfontsize = 48;

                            while ((sizef.Height > (newH / 4) || sizef.Width > (newW / 2)) && currfontsize >= 12)
                            {
                                switch (currfontsize)
                                {
                                    case 48:
                                        currfontsize = 36;
                                        break;
                                    case 36:
                                        currfontsize = 24;
                                        break;
                                    case 24:
                                        currfontsize = 20;
                                        break;
                                    default:
                                        currfontsize = currfontsize - 2;
                                        break;
                                }
                                font = new Font("Times New Roman", currfontsize);
                                sizef = graphic.MeasureString(tag, font, Int32.MaxValue);

                            }

                            SolidBrush blueBrush = new SolidBrush(System.Drawing.Color.Black);
                            RectangleF rect = new RectangleF(0, newH - sizef.Height, sizef.Width, sizef.Height);


                            graphic.FillRectangle(blueBrush, rect);
                            blueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

                            graphic.DrawString(tag, font, blueBrush, rect);
                        }

                        ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 100L);

                        //thumbnail.Save(Response.OutputStream, info[1], encoderParameters);
                        thumbnail.Save(storeStream, Info[1], encoderParameters);
                        return storeStream;
                    }
                }
            }
        }

        /// <summary>
        /// Détermine le type MIME d'un fichier
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        public static string GetMimeType(FileInfo fileInfo)
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

        /// <summary>
        /// Serialise a image
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ImageToString(string path)
        {
            if (path == null)

                throw new ArgumentNullException("path");

            using (Image im = Image.FromFile(path))
            {
                using (MemoryStream ms = new MemoryStream())
                {

                    im.Save(ms, im.RawFormat);

                    byte[] array = ms.ToArray();

                    return Convert.ToBase64String(array);
                }
            }
        }

        /// <summary>
        /// Deserialize an image
        /// </summary>
        /// <param name="imageString"></param>
        /// <returns></returns>
        public static Image StringToImage(string imageString)
        {

            if (imageString == null)

                throw new ArgumentNullException("imageString");

            byte[] array = Convert.FromBase64String(imageString);

            Image image = Image.FromStream(new MemoryStream(array));

            return image;

        }


        public static void OpenPDF(string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                try
                {
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = file;
                    info.Verb = "Open";

                    Process process = Process.Start(info);
                }
                catch
                {
                    if (Path.GetExtension(file) == ".pdf")
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(file);
                        PdfViewer viewer = new PdfViewer(bytes);
                        viewer.Show();
                    }
                }

            }
        }

        public static void PrintPDF(string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                try
                {
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = file;
                    info.Verb = "Print";

                    info.CreateNoWindow = true;

                    Process process = Process.Start(info);
                }
                catch
                {
                    if (Path.GetExtension(file) == ".pdf")
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(file);
                        PdfViewer viewer = new PdfViewer(bytes);
                        viewer.Print();
                    }
                }
            }
        }
        
        public static string ScoreJsonToString(ScoresJujitsu scores)
        {
            ScoresJujitsu scoresJson = new ScoresJujitsu();

            scoresJson.avantages1 = scores.avantages1;
            scoresJson.avantages2 = scores.avantages2;
            scoresJson.points2_1 = scores.points2_1;
            scoresJson.points3_1 = scores.points3_1;
            scoresJson.points4_1 = scores.points4_1;
            scoresJson.points2_2 = scores.points2_2;
            scoresJson.points3_2 = scores.points3_2;
            scoresJson.points4_2 = scores.points4_2;
            scoresJson.ippon1 = scores.ippon1;
            scoresJson.ippon2 = scores.ippon2;
            scoresJson.penalites1 = scores.penalites1;
            scoresJson.penalites2 = scores.penalites2;


            scoresJson.ippon1_1_1 = scores.ippon1_1_1;
            scoresJson.ippon1_1_2 = scores.ippon1_1_2;
            scoresJson.ippon1_2_1 = scores.ippon1_2_1;
            scoresJson.ippon1_2_2 = scores.ippon1_2_2;
            scoresJson.ippon1_3_1 = scores.ippon1_3_1;
            scoresJson.ippon1_3_2 = scores.ippon1_3_2;
            scoresJson.ippon2_1_1 = scores.ippon2_1_1;
            scoresJson.ippon2_1_2 = scores.ippon2_1_2;
            scoresJson.ippon2_2_1 = scores.ippon2_2_1;
            scoresJson.ippon2_2_2 = scores.ippon2_2_2;
            scoresJson.ippon2_3_1 = scores.ippon2_3_1;
            scoresJson.ippon2_3_2 = scores.ippon2_3_2;
            scoresJson.waza1 = scores.waza1;
            scoresJson.waza2 = scores.waza2;
            scoresJson.shido1 = scores.shido1;
            scoresJson.shido2 = scores.shido2;
            scoresJson.chui1 = scores.chui1;
            scoresJson.chui2 = scores.chui2;
            scoresJson.tempsMedical1 = scores.tempsMedical1;
            scoresJson.tempsMedical2 = scores.tempsMedical2;
            return Newtonsoft.Json.JsonConvert.SerializeObject(scoresJson);
        }
		
		/*
         * 20 points max par combat
         * waza ari = 1 point
         * ippon ou 2 waza ari = 10 points
         * hansoku make = 20 points
         * ex: 
         * 1 ippon + 1 waza ari = 11 points
         * 1 ippon + 2 waza ari = 20 points
         * 3 waza ari = 11 points
         * 2 shidos à 1 = match nul
         */
        public static System.Collections.Generic.List<int> getScoresProLeague(int waza1, int ippon1, int yuko1, int shido1, EtatCombattantEnum etatCombattant1, 
            int waza2, int ippon2, int yuko2, int shido2, EtatCombattantEnum etatCombattant2)
        {
            System.Collections.Generic.List<int> res = new System.Collections.Generic.List<int>();
            //int res = 0;

            int score1 = getScoreProLeague(waza1, ippon1, yuko1);
            int score2 = getScoreProLeague(waza2, ippon2, yuko2);

            if (etatCombattant1 == EtatCombattantEnum.HansokuMakeX || etatCombattant1 == EtatCombattantEnum.HansokuMakeH || etatCombattant1 == EtatCombattantEnum.Forfait
                || etatCombattant1 == EtatCombattantEnum.Abandon || etatCombattant1 == EtatCombattantEnum.Medical)
            {
                score2 += 20;
            }
            if (etatCombattant2 == EtatCombattantEnum.HansokuMakeX || etatCombattant2 == EtatCombattantEnum.HansokuMakeH || etatCombattant2 == EtatCombattantEnum.Forfait
                || etatCombattant2 == EtatCombattantEnum.Abandon || etatCombattant2 == EtatCombattantEnum.Medical)
            {
                score1 += 20;
            }

            if (shido1 >= 3)
            {
                score2 += 20;
            }
            if (shido2 >= 3)
            {
                score1 += 20;
            }

            if (score1 > 20) score1 = 20;
            if (score2 > 20) score2 = 20;

            res.Add(score1);
            res.Add(score2);

            return res;
        }
        private static int getScoreProLeague(int waza, int ippon, int yuko)
        {
            int res = 0;
            int scoreWaza = 0;
            if (waza == 1)
            {
                scoreWaza = 1;
            }
            else if (waza == 2)
            {
                scoreWaza = 10;
            }
            else if (waza == 3)
            {
                scoreWaza = 11;
            }
            if (waza > 3)
            {
                scoreWaza = 20;
            }

            res = scoreWaza + (ippon * 10);
            //res = scoreWaza + (ippon * 10) + (etatCombattant == EtatCombattantEnum.HansokuMakeX ? 20 : 0);

            //if (res > 20) res = 20;

            return res;
        }

    }
}
