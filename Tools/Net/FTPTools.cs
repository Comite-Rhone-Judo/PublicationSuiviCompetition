using System;
using System.IO;
using System.Net;
using System.Text;
using Tools.Files;
using Tools.Logging;

namespace Tools.Net
{
    public class FTPTools
    {

        private static FtpWebRequest GetRequest(string uri, string method, string login, string mdp)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.Credentials = new NetworkCredential(login, mdp);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = true;
            return request;
        }

        public static bool CreateDir(string directory, string login, string mdp)
        {
            if (FTPTools.ExistDir(directory, login, mdp))
            {
                return true;
            }

            try
            {
                //create the directory
                FtpWebRequest requestDir = GetRequest(directory, WebRequestMethods.Ftp.MakeDirectory, login, mdp);
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }

        public static bool ExistDir(string directory, string login, string mdp)
        {
            try
            {
                FtpWebRequest requestDir = GetRequest(directory, WebRequestMethods.Ftp.ListDirectory, login, mdp);
                using (FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false;
                    }
                }
                return false;
            }
        }

        public static byte[] GetByteFile(string fullname)
        {
            byte[] fileContents = null;
            try
            {
                FileAndDirectTools.NeedAccessFile(fullname);
                StreamReader sourceStream = new StreamReader(fullname);
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
            finally
            {
                FileAndDirectTools.ReleaseFile(fullname);
            }

            return fileContents;
        }

        public static void UploadFile(byte[] file, string uri, string login, string mdp)
        {
            try
            {
                //create the directory
                FtpWebRequest request = GetRequest(uri, WebRequestMethods.Ftp.UploadFile, login, mdp);
                request.ContentLength = file.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(file, 0, file.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();

            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                response.Close();
                LogTools.Error(ex);
            }
        }
    }
}
