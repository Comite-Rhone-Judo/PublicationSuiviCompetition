using FranceJudo.Core.XML;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace FranceJudo.Core.IO
{
    public class FileWithChecksum
    {
        #region CONSTANTES
        public const string checksums = "Checksums";
        public const string checksumFile = "ChecksumFile";
        public const string checksumFile_fichier = "fichier";
        public const string checksumFile_checksum = "checksum";
        #endregion

        #region CONSTRUCTEUR
        public FileWithChecksum(string fn)
        {
            File = new FileInfo(fn);
            Checksum = FileWithChecksum.ComputeHash(File);
        }
        public FileWithChecksum()
        {
            File = null;
            Checksum = string.Empty;
        }
        #endregion

        #region PROPRIETES
        private FileInfo _file;
        public FileInfo File
        {
            get
            {
                return _file;
            }
            set
            {
                _file = value;
            }
        }

        private string _checksum;
        public string Checksum
        {
            get
            {
                return _checksum;
            }
            set
            {
                _checksum = value;
            }
        }
        #endregion

        #region METHODES

        public static string ComputeHash(FileInfo fileInfo, HashAlgorithm hashAlgorithm = null)
        {
            HashAlgorithm algo = hashAlgorithm ?? MD5.Create();

            using (var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var hash = algo.ComputeHash(fs);
                algo.Dispose();
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
        public void LoadXml(XElement xinfo)
        {
            this.File = new FileInfo(XMLTools.LectureString(xinfo.Attribute(checksumFile_fichier)));
            this.Checksum = XMLTools.LectureString(xinfo.Attribute(checksumFile_checksum));
        }

        public XElement ToXml()
        {
            XElement xelem = new XElement(checksumFile);
            xelem.SetAttributeValue(checksumFile_fichier, File.FullName);
            xelem.SetAttributeValue(checksumFile_checksum, Checksum);

            return xelem;
        }
        #endregion
    }
}
