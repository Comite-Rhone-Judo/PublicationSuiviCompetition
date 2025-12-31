using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;
using Tools.Files;

namespace AppPublication.Tools.Files
{
    public static class FileTools
    {
        private static Dictionary<string, XDocument> notSave = new Dictionary<string, XDocument>();

        public static void SaveFile(XDocument doc, string fileType)
        {
            //if (!Directory.Exists(ConstantFile.SaveCOMDirectory))
            //{
            //    Directory.CreateDirectory(ConstantFile.SaveCOMDirectory);
            //}

            using (TimedLock.Lock((notSave as ICollection).SyncRoot))
            {
                notSave.Remove(fileType);
                notSave.Add(fileType, doc);

                foreach (string file in notSave.Keys.ToList())
                {
                    XDocument document = notSave[file];

                    string filename = Path.Combine(ConstantFile.SaveCOMDirectory, file + ConstantFile.ExtensionXML);
                    if (!File.Exists(filename) || !FileAndDirectTools.IsFileLocked(filename))
                    {
                        FileAndDirectTools.NeedAccessFile(filename);
                        try
                        {
                            using (FileStream fs = new FileStream(filename, FileMode.Create))
                            {
                                document.Save(fs);
                            }
                            notSave.Remove(file);
                        }
                        catch { }
                        finally
                        {
                            FileAndDirectTools.ReleaseFile(filename);
                        }
                    }
                }
            }

            //// Create the FileStream.
            //string filename = ConstantFile.SaveCOMDirectory + "/" + fileType + ConstantFile.ExtensionXML;
            //using (FileStream fs = new FileStream(filename, FileMode.Create))
            //{
            //    doc.Save(fs);
            //}
        }
    }
}