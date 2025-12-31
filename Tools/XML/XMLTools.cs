using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Files;
using Tools.Logging;

namespace Tools.XML
{
    /// <summary>
    /// Outils de lecture des fichiers XML
    /// </summary>

    public static class XMLTools
    {
        /// <summary>
        /// Lecture d'une date
        /// </summary>
        /// <param name="ladate">attribut date</param>
        /// <param name="format">format de la date</param>
        /// <param name="default_value">date par défaut si mauvaise lecture</param>
        /// <returns>la date</returns>

        public static DateTime LectureDate(XAttribute ladate, string format, DateTime default_value)
        {
            try
            {
                if (ladate == null || string.IsNullOrWhiteSpace(ladate.Value))
                {
                    return default_value;
                }
                return DateTime.ParseExact(ladate.Value, format, null);
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", ladate.Value);
                return default_value;
            }
        }

        /// <summary>
        /// Lecture d'une date
        /// </summary>
        /// <param name="ladate">attribut date</param>
        /// <param name="format">format de la date</param>
        /// <returns>la date</returns>

        public static DateTime LectureDate(XmlAttribute ladate, string format)
        {
            try
            {
                if (ladate == null)
                {
                    return DateTime.Now;
                }
                return DateTime.ParseExact(ladate.Value, format, null);
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", ladate.Value);
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Lecture d'une date
        /// </summary>
        /// <param name="ladate">date</param>
        /// <param name="format">format de la date</param>
        /// <returns>la date</returns>

        public static DateTime LectureDate(string ladate, string format)
        {
            ////--
            /*
            try
            {
                return DateTime.ParseExact(ladate, format, null);
            }
            catch
            {
                return DateTime.Now;
            }*/
            ////--
            if (ladate != "")
            {
                try
                {
                    return DateTime.ParseExact(ladate, format, null);
                }
                catch
                {
                    return DateTime.Now;
                }
            }
            else
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Lecture d'un time
        /// </summary>
        /// <param name="ladate">date</param>
        /// <param name="format">format de la date</param>
        /// <returns>time</returns>

        public static TimeSpan LectureTime(XAttribute ladate, string format)
        {
            try
            {
                if (ladate == null || string.IsNullOrWhiteSpace(ladate.Value))
                {
                    return DateTime.Now.TimeOfDay;
                }
                return DateTime.ParseExact(ladate.Value, format, null).TimeOfDay;
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", ladate.Value);
                return DateTime.Now.TimeOfDay;
            }
        }

        /// <summary>
        /// Lecture d'un Int32
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static int LectureInt(XAttribute xattribute, int default_val = 0)
        {
            int result = default_val;
            try
            {
                if (xattribute != null && !String.IsNullOrWhiteSpace(xattribute.Value))
                {
                    int.TryParse(xattribute.Value, out result);
                }
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", xattribute.Value);
            }
            return result;
        }

        /// <summary>
        /// Lecture d'un Int32
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static double LectureDouble(XAttribute xattribute, double default_val = 0)
        {
            double result = default_val;
            try
            {
                if (xattribute != null && !String.IsNullOrWhiteSpace(xattribute.Value))
                {
                    double.TryParse(xattribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
                }
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", xattribute.Value);
            }
            return result;
        }

        /// <summary>
        /// Lecture d'un Int32
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static int LectureInt(XmlAttribute xattribute, int default_val = 0)
        {
            int result = default_val;
            try
            {
                if (xattribute != null && !String.IsNullOrWhiteSpace(xattribute.Value))
                {
                    int.TryParse(xattribute.Value, out result);
                }
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", xattribute.Value);
            }
            return result;
        }

        /// <summary>
        /// Lecture d'un Nullable<Int32>
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static int? LectureNullableInt(XAttribute xattribute)
        {

            int result = 0;
            try
            {
                if (xattribute == null)
                {
                    return null;
                }

                if (!String.IsNullOrWhiteSpace(xattribute.Value))
                {
                    if (int.TryParse(xattribute.Value, out result))
                    {
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", xattribute.Value);
                return null;
            }
            return null;
        }

        /// <summary>
        /// Lecture d'un String
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static string LectureString(XAttribute xattribute)
        {
            if (xattribute == null)
            {
                return "";
            }
            else
            {
                return xattribute.Value;
            }
        }

        public static string LectureString(XElement xattribute)
        {
            if (xattribute == null)
            {
                return "";
            }
            else
            {
                return xattribute.Value;
            }
        }

        public static string LectureString(XmlAttribute xattribute)
        {
            if (xattribute == null)
            {
                return "";
            }
            else
            {
                return xattribute.Value;
            }
        }


        /// <summary>
        /// Lecture d'un String
        /// </summary>
        /// <param name="xattribute">attribut</param>
        /// <returns>valeur</returns>

        public static bool LectureBool(XAttribute xattribute)
        {
            try
            {
                if (xattribute == null)
                {
                    return false;
                }
                return bool.Parse(xattribute.Value);
            }
            catch(Exception ex)
            {
                LogTools.Logger.Error(ex, "Exception lors de la lecture de '{0}'", xattribute.Value);
                return false;
            }
        }

        /// <summary>
        /// XDocument -> XmlDocument
        /// </summary>
        /// <param name="xDocument">XDocument</param>
        /// <returns>XmlDocument</returns>

        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        /// <summary>
        /// XmlDocument -> XDocument
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>XDocument</returns>

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
