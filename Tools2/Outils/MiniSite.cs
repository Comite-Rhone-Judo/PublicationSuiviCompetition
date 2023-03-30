using System;
using System.Linq;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using Tools.Enum;
using Tools.Export;
using System.ComponentModel;
using FluentFTP;

namespace Tools.Outils
{
    public class MiniSite : NotificationBase
    {
        private bool _actif = false;
        private ServerHttp _server = null;
        private bool _local = true;
        // TODO mettre ces valeurs dans un cache fichier
        private string _ftp = "";
        private string _ftp_log = "";
        private string _ftp_pass = "";
        private string _url_distant = "";
        private string _ftp_rep = "";
        private FtpProfile _ftp_profile = null;


        #region CONSTRUCTEURS
        public MiniSite(bool local = false)
        {
            if (local)
            {
                // Configure un site web local
                ServerHTTP = new ServerHttp();

                // Initialise les interfaces
                InitInterfaces();
            }
            else
            {
                // Site distant
                InitConfigFTP();
            }

            _local = local;
            IsActif = false;
        }

        #endregion

        #region PROPRIETES

        List<IPAddress> _interfacesLocal;
        public List<IPAddress> InterfacesLocal
        {
            get
            {
                return _interfacesLocal;
            }
            private set
            {
                _interfacesLocal = value;
                NotifyPropertyChanged("InterfacesLocal");
            }
        }

        IPAddress _interfaceLocalPublication;
        public IPAddress InterfaceLocalPublication
        {
            get
            {
                return _interfaceLocalPublication;
            }
            set
            {
                // Verifie que la valeur selectionnee est bien dans la liste des interfaces
                if (null == value || _interfacesLocal.Contains(value))
                {
                    _interfaceLocalPublication = value;

                    // Configure l'adresse du serveur de publication si on est en mode local
                    if(IsLocal && null != _interfaceLocalPublication && null != ServerHTTP)
                    {
                        ServerHTTP.IpAddress = _interfaceLocalPublication;
                    }
                    if (_interfaceLocalPublication != null)
                    {
                        AppSettings.SaveSettings("InterfaceLocalPublication", _interfaceLocalPublication.ToString());
                    }
                    NotifyPropertyChanged("InterfaceLocalPublication");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("L'adresse d'interface selectionne n'existe pas sur le poste");
                }
            }
        }

        public ServerHttp ServerHTTP
        {
            get
            {
                return _server;
            }
            private set
            {
                _server = value;
                NotifyPropertyChanged("ServerHTTP");
                IsChanged = true;
            }
        }

        bool _isChanged = false;
        public bool IsChanged
        {
            get
            {
                return _isChanged;
            }
            set
            {
                _isChanged = true;
                NotifyPropertyChanged("IsChanged");
            }
        }

        public bool IsLocal
        {
            get
            {
                return _local;
            }
        }

        public string URLDistant
        {
            get
            {
                return _url_distant;
            }
            set
            {
                _url_distant = value;
                AppSettings.SaveSettings("URLDistant", _url_distant);
                NotifyPropertyChanged("URLDistant");
                IsChanged = true;
            }
        }

        public string SiteFTPDistant
        {
            get
            {
                return _ftp;
            }
            set
            {
                _ftp = value;
                AppSettings.SaveSettings("SiteFTPDistant", _ftp);
                NotifyPropertyChanged("SiteFTPDistant");
                IsChanged = true;
            }
        }

        public string RepertoireSiteFTPDistant
        {
            get
            {
                return _ftp_rep;
            }
            set
            {
                _ftp_rep = value;
                AppSettings.SaveSettings("RepertoireSiteFTPDistant", _ftp_rep);
                NotifyPropertyChanged("RepertoireSiteFTPDistant");
                IsChanged = true;
            }
        }

        public string LoginSiteFTPDistant
        {
            get
            {
                return _ftp_log;
            }
            set
            {
                _ftp_log = value;
                AppSettings.SaveSettings("LoginSiteFTPDistant", _ftp_log);
                NotifyPropertyChanged("LoginSiteFTPDistant");
                IsChanged = true;
            }
        }

        public string PasswordSiteFTPDistant
        {
            get
            {
                return _ftp_pass;
            }
            set
            {
                _ftp_pass = value;
                AppSettings.SaveSettings("PasswordSiteFTPDistant", _ftp_pass);
                NotifyPropertyChanged("PasswordSiteFTPDistant");
                IsChanged = true;
            }
        }

        public bool IsActif
        {
            get
            {
                return _actif;
            }
            private set
            {
                _actif = value;
                NotifyPropertyChanged("IsActif");
                IsChanged = true;
            }
        }

        string _status = "-";
        public string Status
        {
            get
            {
                return _status;
            }
            private set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        #endregion

        #region METHODES

        private void InitConfigFTP()
        {
            string valCache = string.Empty;

            valCache = AppSettings.ReadSettings("URLDistant");
            URLDistant = (valCache == null) ? String.Empty : valCache;

            valCache = AppSettings.ReadSettings("SiteFTPDistant");
            SiteFTPDistant = (valCache == null) ? String.Empty : valCache;

            valCache = AppSettings.ReadSettings("LoginSiteFTPDistant");
            LoginSiteFTPDistant = (valCache == null) ? String.Empty : valCache;

            valCache = AppSettings.ReadSettings("PasswordSiteFTPDistant");
            PasswordSiteFTPDistant = (valCache == null) ? String.Empty : valCache;

            valCache = AppSettings.ReadSettings("RepertoireSiteFTPDistant");
            RepertoireSiteFTPDistant = (valCache == null) ? String.Empty : valCache;
        }

        private void InitInterfaces()
        {
            // Initialise la liste des interfaces
            string strHostName = Dns.GetHostName();
            InterfacesLocal = Dns.GetHostAddresses(strHostName).Where(o => o.AddressFamily == AddressFamily.InterNetwork).ToList();
            // Selectionne la 1ere interface de la liste
            InterfaceLocalPublication = null;

            // Si la liste contient au moins un element
            if (InterfacesLocal.Count >= 1)
            {
                // Cherche si une interface existe dans la configuration du fichier
                string valCache = AppSettings.ReadSettings("InterfaceLocalPublication");
                IPAddress ipToUse = null;
                bool useCache = false;

                if (valCache != null)
                {
                    try
                    {
                        // Lit l'adresse dans le fichier et verifie qu'elle est dans la liste
                        ipToUse = IPAddress.Parse(valCache);
                        useCache = InterfacesLocal.Contains(ipToUse);
                    }
                    catch (Exception ex)
                    {
                        // Soit l'IP configuree est incorrecte, soit elle n'est pas dans la liste
                        useCache = false;
                        LogTools.Log(ex);
                    }
                }

                // on prend la 1ere interface de la liste si elle n'est pas dans la 
                if(!useCache)
                {
                    ipToUse = InterfacesLocal.First();
                }

                // Assigne la valeur (en dernier pour eviter les bindings successifs)
                InterfaceLocalPublication = ipToUse;
            }
        }

        public void StartSite()
        {
            bool actif = false;
            string lStatus = "-";

            try
            {


                // Arrete le serveur local si necessaire
                if (IsLocal)
                {
                    if (null != _server)
                    {
                        // Stop le service s'il fonctionne deja
                        if (IsActif)
                        {
                            _server.Stop();
                        }

                        // Configure l'interface d'ecoute du minisite
                        _server.IpAddress = InterfaceLocalPublication;

                        // Demarre le serveur Web local
                        _server.Start();

                        if (_server.IsStart)
                        {
                            // Active le site
                            actif = true;
                        }
                        else
                        {
                            // Le site n'a pas demarre
                            lStatus = "Impossible de démarrer le serveur Web local";
                        }

                    }
                    else
                    {
                        lStatus = "Serveur Web local indisponible";
                    }
                }
                else
                {
                    // Serveur distant
                    if (CheckConfigurationSiteDistant())
                    {
                        // Active le site
                        actif = true;
                    }
                    else
                    {
                        // La configuration ne permet pas de se connecter sur le site FTP
                        lStatus = "Configuration incorrecte";
                    }
                }


            }
            catch (Exception ex)
            {
                Status = "Erreur au demarrage";
                LogTools.Log(ex);
            }

            // Met a jour les status du minisite
            IsActif = actif;
            Status = lStatus;
        }

        public void StopSite()
        {
            try
            {
                IsActif = false;
                Status = "-";

                if (!IsLocal && null != _server)
                {
                    _server.Stop();
                }
            }
            catch (Exception ex)
            {
                Status = "Erreur lors de l'arrêt";
                LogTools.Log(ex);
            }
        }

        private bool CheckConfigurationSiteDistant()
        {
            bool output = false;
            if (!String.IsNullOrEmpty(SiteFTPDistant) && !String.IsNullOrEmpty(LoginSiteFTPDistant) && !string.IsNullOrEmpty(PasswordSiteFTPDistant))
            {
                // Test les parametres de connection
                try
                {
                    FtpClient ftpClient = new FtpClient(SiteFTPDistant, LoginSiteFTPDistant, PasswordSiteFTPDistant);
                    List<FtpProfile> profiles = ftpClient.AutoDetect(true);

                    if (profiles.Count > 0)
                    {
                        _ftp_profile = profiles.First();
                        output = true;
                    }
                }
                catch(Exception ex)
                {
                    output = false;
                    LogTools.Log(ex);
                }
            }

            return output;
        }

        public bool UploadSite(string localRootDirectory, string distantDirectory)
        {
            bool output = false;

            if(IsLocal || !IsActif)
            {
                // Si le site est local ou n'est pas actif
                return false;
            }

            if (String.IsNullOrEmpty(SiteFTPDistant) || String.IsNullOrEmpty(LoginSiteFTPDistant) || string.IsNullOrEmpty(PasswordSiteFTPDistant) || String.IsNullOrEmpty(localRootDirectory) || String.IsNullOrEmpty(distantDirectory))
            {
                // Pas de configuration
                return false;
            }

            // Le client FTP pour la connection
            FtpClient ftpClient = new FtpClient(SiteFTPDistant, LoginSiteFTPDistant, PasswordSiteFTPDistant);

            try
            {
                // Essaye de se connecter au serveur FTP
                ftpClient.Connect(_ftp_profile);
    
                if(ftpClient.IsConnected)
                {
                    string distantRootDirectory = Path.Combine(RepertoireSiteFTPDistant, distantDirectory);

                    // Charge le dossier du site vers le serveur FTP en mode miroir pour synchroniser
                    List<FtpResult> uploadOut = ftpClient.UploadDirectory(localRootDirectory, distantRootDirectory, FtpFolderSyncMode.Mirror, FtpRemoteExists.Overwrite, FtpVerify.None);

                    if(uploadOut.Count > 0)
                    {
                        output = true;
                    }

                    // Disconnect
                    ftpClient.Disconnect();
                }
            }
            catch(Exception ex)
            {
                LogTools.Log(ex);
            }
            finally
            {
                if (ftpClient.IsConnected)
                {
                    ftpClient.Disconnect();
                }
            }

            return output;
        }

        
        #endregion
    }
}
