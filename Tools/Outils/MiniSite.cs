using FluentFTP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Xml;
using Tools;
using Tools.Enum;

namespace Tools.Outils
{
    public class UploadStatus
    {
        public UploadStatus()
        {
            IsSuccess = false;
            IsComplet = true;
            nbUpload = -1;
        }

        public bool IsSuccess;  // Etat de l'upload (True = Ok, False = Err)
        public bool IsComplet;  // Upload complet si true, diff only si false
        public int nbUpload;    // Nb de fichier charge
    }


    public class MiniSite : NotificationBase
    {
        #region CONSTANTES
        private const string kSettingSiteFTPDistant = "SiteFTPDistant";
        private const string kSettingLoginSiteFTPDistant = "LoginSiteFTPDistant";
        private const string kSettingPasswordSiteFTPDistant = "PasswordSiteFTPDistant";
        private const string kSettingModeActifFTPDistant = "ModeActifFTPDistant";
        private const string kSettingSynchroniseDifferences = "SynchroniseDifferences";
        private const string kSettingInterfaceLocalPublication ="InterfaceLocalPublication";

        #endregion

        #region MEMBRES
        private FtpProfile _ftp_profile = null;     // Le profile FTP a utiliser pour les connexions
        private Action<FtpProgress> _ftpProgressCallback = null;
        private long _nbSyncDistant = 0;
        private string _instanceName = string.Empty;

        #endregion

        #region CONSTRUCTEURS





        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="local">Mode du minisite (local = true, distant = false)</param>
        /// <param name="instanceName">Nom de l'instance</param>
        /// <param name="cacheCfg">True pour activer la mise en cache de la configuration</param>
        /// <param name="cachePwd">True pour activer la mise en cache du mot de passe</param>
        public MiniSite(bool local, string instanceName = "", bool cacheCfg = true, bool cachePwd = true)
        {
            // Initialise les caracteristiques du MiniSite
            InstanceName = instanceName;
            CacheConfig = cacheCfg;
            CachePassword = cachePwd;

            if (local)
            {
                // Configure un site web local
                ServerHTTP = new ServeurHttp();

                // Initialise les interfaces
                InitInterfaces();
            }
            else
            {
                // Site distant
                InitConfigFTP();

                // Initialise le callback de tracking
                _ftpProgressCallback = new Action<FtpProgress>(p =>
                {
                    if (IsActif)
                    {
                        CalculProgressionFTP(p);
                    }
                });
            }

            _local = local;
            Status = new StatusMiniSite();
        }

        #endregion

        #region PROPRIETES

        /// <summary>
        /// Indique le nom de l'instance du Minisite
        /// </summary>
        public string InstanceName
        {
            get
            {
                return _instanceName;
            }
            private set
            {
                _instanceName = value;
                NotifyPropertyChanged("InstanceName");
            }
        }

        List<IPAddress> _interfacesLocal;
        /// <summary>
        /// La liste des interfaces locales du PCen lecture seule, initialise lors de la creation si Local)
        /// </summary>
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
        /// <summary>
        /// Interface (@IP) utilisée pour la publication du site en mode local
        /// doit etre presente dans la liste InterfacesLocal
        /// </summary>
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
                    if (IsLocal && null != _interfaceLocalPublication && null != ServerHTTP)
                    {
                        ServerHTTP.ListeningIpAddress = _interfaceLocalPublication;
                    }
                    if (_interfaceLocalPublication != null && CacheConfig)
                    {
                        AppSettings.SaveSetting(kSettingInterfaceLocalPublication, _interfaceLocalPublication.ToString(), _instanceName);
                    }
                    NotifyPropertyChanged("InterfaceLocalPublication");
                    IsChanged = true;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("L'adresse d'interface selectionne n'existe pas sur le poste");
                }
            }
        }

        private ServeurHttp _server = null;
        /// <summary>
        /// Le serveur HTTP de publication locale (null si distant, lecture seule)
        /// </summary>
        public ServeurHttp ServerHTTP
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
        /// <summary>
        /// Indique qu'une modification a eu lieu sur le parametrage du site
        /// </summary>
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

        private bool _local = true;
        /// <summary>
        /// Indique si le site est en mode local (true) ou distant (false) - Lecture seule
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return _local;
            }
        }

        bool _cacheConfig = false;
        /// <summary>
        /// Indique si les donnees de configuration doivent etre gardees en cache
        /// </summary>
        public bool CacheConfig
        {
            get
            {
                return _cacheConfig;
            }
            private set
            {
                _cacheConfig = value;
                NotifyPropertyChanged("CacheConfig");
            }
        }

        bool _cachePassword = false;
        /// <summary>
        /// Indique si le mot de passe doit etre gardees en cache
        /// </summary>
        public bool CachePassword
        {
            get
            {
                return _cachePassword;
            }
            private set
            {
                _cachePassword = value;
                NotifyPropertyChanged("CachePassword");
            }
        }

        private string _ftpDistant = string.Empty;
        /// <summary>
        /// L'adresse du site FTP Distant
        /// </summary>
        public string SiteFTPDistant
        {
            get
            {
                return _ftpDistant;
            }
            set
            {
                _ftpDistant = value;
                if (CacheConfig)
                {
                    AppSettings.SaveSetting(kSettingSiteFTPDistant, _ftpDistant, _instanceName);
                }
                NotifyPropertyChanged("SiteFTPDistant");
                IsChanged = true;
            }
        }

        private string _ftRepertoireDistant = string.Empty;
        /// <summary>
        /// Le repertoire cible sur le serveur FTP
        /// </summary>
        public string RepertoireSiteFTPDistant
        {
            get
            {
                return _ftRepertoireDistant;
            }
            set
            {
                _ftRepertoireDistant = value;
                NotifyPropertyChanged("RepertoireSiteFTPDistant");
                IsChanged = true;
            }
        }

        private string _ftpLoginDistant = string.Empty;
        /// <summary>
        /// Login de connexion au site FTP Distant
        /// </summary>
        public string LoginSiteFTPDistant
        {
            get
            {
                return _ftpLoginDistant;
            }
            set
            {
                _ftpLoginDistant = value;
                if (CacheConfig)
                {
                    AppSettings.SaveSetting(kSettingLoginSiteFTPDistant, _ftpLoginDistant, _instanceName);
                }
                NotifyPropertyChanged("LoginSiteFTPDistant");
                IsChanged = true;
            }
        }

        private bool _modeFTPActif = false;

        /// <summary>
        /// Mode de fonctionnement FTP Actif (true) ou passif (false)
        /// </summary>
        public bool ModeActifFTPDistant
        {
            get
            {
                return _modeFTPActif;
            }
            set
            {
                _modeFTPActif = value;
                if (CacheConfig)
                {
                    AppSettings.SaveSetting(kSettingModeActifFTPDistant, _modeFTPActif.ToString(), _instanceName);
                }
                NotifyPropertyChanged("ModeActifFTPDistant");
                IsChanged = true;
            }
        }

        private string _ftpPasswordDistant = string.Empty;
        /// <summary>
        /// Mot de passe FTP au site FTP Distant
        /// </summary>
        public string PasswordSiteFTPDistant
        {
            get
            {
                return _ftpPasswordDistant;
            }
            set
            {
                _ftpPasswordDistant = value;
                if (CachePassword)
                {
                    AppSettings.SaveEncryptedSetting(kSettingPasswordSiteFTPDistant, _ftpPasswordDistant, _instanceName);
                }
                NotifyPropertyChanged("PasswordSiteFTPDistant");
                IsChanged = true;
            }
        }  

        private bool _syncDiff = false;
        public bool SynchroniseDifferences
        {
            get
            {
                return _syncDiff;
            }
            set
            {
                _syncDiff = value;
                if (CacheConfig)
                {
                    AppSettings.SaveSetting(kSettingSynchroniseDifferences, _syncDiff.ToString(), _instanceName);
                }
                NotifyPropertyChanged("SynchroniseDifferences");
                IsChanged = true;
            }
        }

        private bool _actif = false;
        /// <summary>
        /// Indique si le site est actif (lecture seule)
        /// </summary>
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

        private bool _cleaning = false;
        /// <summary>
        /// Indique si le site est entrain de faire du nettoyage (lecture seule)
        /// </summary>
        public bool IsCleaning
        {
            get
            {
                return _cleaning;
            }
            private set
            {
                _cleaning = value;
                NotifyPropertyChanged("IsCleaning");
            }
        }

        StatusMiniSite _status;
        /// <summary>
        /// Le statut textuelle du minisite (active, etc.)
        /// </summary>
        public StatusMiniSite Status
        {
            get
            {
                if (null == _status)
                {
                    _status = new StatusMiniSite();
                }
                return _status;
            }
            private set
            {
                _status = value;
                NotifyPropertyChanged("Status");

                // Actualise l'etat d'activite du site
                IsActif = !(_status.State == StateMiniSiteEnum.Stopped);
                IsCleaning = (_status.State == StateMiniSiteEnum.Cleaning);
            }
        }

        #endregion

        #region METHODES

       

        /// <summary>
        /// Initialise la configuraiton FTP a partir du cache de fichier AppConfig
        /// </summary>
        private void InitConfigFTP()
        {
            string valCache = string.Empty;

            try
            {
                SiteFTPDistant = AppSettings.ReadSetting(kSettingSiteFTPDistant, string.Empty, _instanceName);
                LoginSiteFTPDistant = AppSettings.ReadSetting(kSettingLoginSiteFTPDistant, string.Empty, _instanceName);
                PasswordSiteFTPDistant = AppSettings.ReadEncryptedSetting(kSettingPasswordSiteFTPDistant, string.Empty, _instanceName);
                ModeActifFTPDistant = AppSettings.ReadSetting(kSettingModeActifFTPDistant, false, _instanceName);
                SynchroniseDifferences = AppSettings.ReadSetting(kSettingSynchroniseDifferences, false, _instanceName);
            }
            catch { }
        }

        /// <summary>
        /// Initialise la liste des interfaces locales disponibles
        /// </summary>
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
                string valCache = AppSettings.ReadSetting(kSettingInterfaceLocalPublication, _instanceName);
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
                        LogTools.Debug(ex);
                    }
                }

                // on prend la 1ere interface de la liste si elle n'est pas dans la 
                if (!useCache)
                {
                    ipToUse = InterfacesLocal.First();
                }

                // Assigne la valeur (en dernier pour eviter les bindings successifs)
                InterfaceLocalPublication = ipToUse;
            }
        }

        /// <summary>
        /// Demarre le site
        /// </summary>
        public void StartSite()
        {
            StateMiniSiteEnum lStatus = StateMiniSiteEnum.Stopped;
            string lStatusMsg = "-";
            string lStatusDetail = string.Empty;

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
                        _server.ListeningIpAddress = InterfaceLocalPublication;

                        // Demarre le serveur Web local
                        _server.Start();

                        if (_server.IsStart)
                        {
                            // Active le site
                            lStatus = StateMiniSiteEnum.Listening;
                        }
                        else
                        {
                            // Le site n'a pas demarre
                            lStatusMsg = "Impossible de démarrer le serveur Web local";
                        }

                    }
                    else
                    {
                        lStatusMsg = "Serveur Web local indisponible";
                    }
                }
                else
                {
                    // Serveur distant
                    try
                    {
                        if (CheckConfigurationSiteDistant())
                        {
                            // Active le site
                            lStatus = StateMiniSiteEnum.Idle;
                            // RAZ le nb de synchronisation realisee
                            _nbSyncDistant = 0;
                        }
                        else
                        {
                            // La configuration ne permet pas de se connecter sur le site FTP
                            lStatusMsg = "Configuration incorrecte";
                        }
                    }
                    catch (Exception ex)
                    {
                        lStatusMsg = "Configuration incorrecte";
                        lStatusDetail = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                lStatusMsg = "Erreur au demarrage";
                lStatusDetail = ex.Message;
                LogTools.Error(ex);
            }

            // Met a jour les status du minisite
            Status = new StatusMiniSite(lStatus, lStatusMsg, lStatusDetail);
        }

        /// <summary>
        /// Arrete le site
        /// </summary>
        public void StopSite()
        {
            try
            {
                IsActif = false;
                Status = new StatusMiniSite(StateMiniSiteEnum.Stopped);

                if (IsLocal && null != _server)
                {
                    _server.Stop();
                }
            }
            catch (Exception ex)
            {
                Status = new StatusMiniSite(StateMiniSiteEnum.Stopped, "Erreur lors de l'arrêt");
                LogTools.Error(ex);
            }
        }

        /// <summary>
        /// Valide la configuration du site distant et initialise le FtpProfile
        /// </summary>
        /// <returns></returns>
        private bool CheckConfigurationSiteDistant()
        {
            bool output = false;
            if (!String.IsNullOrEmpty(SiteFTPDistant) && !String.IsNullOrEmpty(LoginSiteFTPDistant) && !string.IsNullOrEmpty(PasswordSiteFTPDistant))
            {
                // Test les parametres de connection
                // FtpClient ftpClient = new FtpClient(SiteFTPDistant, LoginSiteFTPDistant, PasswordSiteFTPDistant);
                FtpClient ftpClient = GetAndConfigureFtpClient();

                try
                {
                    List<FtpProfile> profiles = ftpClient.AutoDetect(true);

                    if (profiles.Count > 0)
                    {
                        _ftp_profile = profiles.First();
                        _ftp_profile.DataConnection = (ModeActifFTPDistant) ? FtpDataConnectionType.PORT : FtpDataConnectionType.PASV;
                        output = true;
                    }
                }
                catch (Exception ex)
                {
                    LogTools.Error(ex);
                    throw ex;
                }
                finally
                {
                    ftpClient?.Dispose();
                }
            }

            return output;
        }

        private FtpClient GetAndConfigureFtpClient()
        {
            // Le client FTP pour la connection
            FtpClient ftpClient = new FtpClient(SiteFTPDistant, LoginSiteFTPDistant, PasswordSiteFTPDistant);
            // Autorise l'utilisation de n'importe quel certificat
            ftpClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
            ftpClient.Config.ValidateAnyCertificate = true;

            return ftpClient;
        }

        /// <summary>
        /// Nettoyer le site distant (efface tous les fichiers et les repertoires)
        /// </summary>
        /// <returns></returns>
        public UploadStatus NettoyerSite()
        {
            UploadStatus output = new UploadStatus();
            StatusMiniSite cStatus = Status;  // Recupere le status courant pour le restaurer apres les operations

            if (IsLocal || IsActif)
            {
                // Si le site est local ou est actif
                return output;
            }

            if (String.IsNullOrEmpty(SiteFTPDistant) || String.IsNullOrEmpty(LoginSiteFTPDistant) || string.IsNullOrEmpty(PasswordSiteFTPDistant))
            {
                // Pas de configuration
                return output;
            }

            try
            {
                if (!CheckConfigurationSiteDistant())
                {
                    Status = new StatusMiniSite(StateMiniSiteEnum.Idle, "Configuration incorrecte");
                    return output;
                }
            }
            catch (Exception ex)
            {
                Status = new StatusMiniSite(StateMiniSiteEnum.Idle, "Configuration incorrecte", ex.Message);
                return output;
            }

            // Le client FTP pour la connection
            // FtpClient ftpClient = new FtpClient(SiteFTPDistant, LoginSiteFTPDistant, PasswordSiteFTPDistant);
            FtpClient ftpClient = GetAndConfigureFtpClient();

            try
            {
                Status = new StatusMiniSite(StateMiniSiteEnum.Syncing, "Nettoyage FTP ...");

                // Essaye de se connecter au serveur FTP
                ftpClient.Connect(_ftp_profile);

                if (ftpClient.IsConnected)
                {
                    List<FtpListItem> ftpList = ftpClient.GetListing(RepertoireSiteFTPDistant).ToList();
                    int idx = 0;
                    foreach (FtpListItem ftpItem in ftpList)
                    {
                        switch (ftpItem.Type)
                        {

                            case FtpObjectType.Directory:
                                {
                                    ftpClient.DeleteDirectory(ftpItem.FullName);
                                }
                                break;

                            case FtpObjectType.File:
                                {
                                    ftpClient.DeleteFile(ftpItem.FullName);
                                }
                                break;

                            case FtpObjectType.Link:
                                break;
                        }

                        // Met a jour la progression du nettoyage
                        CalculProgressionFTP(idx, ftpList.Count);
                        idx++;
                    }

                    // Disconnect
                    ftpClient.Disconnect();
                    output.IsSuccess = true;
                    cStatus = new StatusMiniSite(cStatus.State);
                }

            }
            catch (Exception ex)
            {
                cStatus = new StatusMiniSite(cStatus.State, "Erreur FTP", ex.Message);
                LogTools.Error(ex);
            }
            finally
            {
                // disconnect et forcer le dispose pour s'assurer que la connection est bien coupe (cf. pb de 2 cnx max surt Free)
                if (ftpClient.IsConnected)
                {
                    ftpClient.Disconnect();
                }

                // Detruit le client
                ftpClient?.Dispose();
            }

            // Restaure le status apres l'operation
            Status = cStatus;

            return output;
        }

        /// <summary>
        /// Calcul le % de progression, FTP en fonction des informations retournees par FtpProgress
        /// </summary>
        /// <param name="p"></param>
        private void CalculProgressionFTP(FtpProgress p)
        {
            if (p != null)
            {
                CalculProgressionFTP(p.FileIndex, p.FileCount);
            }
        }

        /// <summary>
        /// Calcul  le % de progressioon FTP
        /// </summary>
        /// <param name="index"></param>
        /// <param name="total"></param>
        private void CalculProgressionFTP(int index, int total)
        {
            int pct = -1;
            // Calcul le ratio de transfert du repertoire
            if (index >= 0 && total > -1)
            {
                pct = (int)Math.Round(((index + 1.0) / total) * 100);
            }

            Status.Progress = pct;
        }


        /// <summary>
        /// Charge la structure sur le site FTP
        /// </summary>
        /// <param name="localRootDirectory">Repertoire dont le contenu doit etre charge</param>
        /// <returns></returns>
        public UploadStatus UploadSite(string localRootDirectory, List<FileInfo> listFiles = null)
        {
            UploadStatus output = new UploadStatus();
            StatusMiniSite cStatus = Status;  // Recupere le status courant pour le restaurer apres les operations

            if (IsLocal || !IsActif)
            {
                // Si le site est local ou n'est pas actif
                return output;
            }

            if (String.IsNullOrEmpty(SiteFTPDistant) || String.IsNullOrEmpty(LoginSiteFTPDistant) || string.IsNullOrEmpty(PasswordSiteFTPDistant) || String.IsNullOrEmpty(localRootDirectory) || String.IsNullOrEmpty(RepertoireSiteFTPDistant))
            {
                // Pas de configuration
                return output;
            }

            // Le client FTP pour la connection
            // FtpClient ftpClient = new FtpClient(SiteFTPDistant, LoginSiteFTPDistant, PasswordSiteFTPDistant);
            FtpClient ftpClient = GetAndConfigureFtpClient();

            try
            {
                Status = new StatusMiniSite(StateMiniSiteEnum.Syncing, "Envoi FTP ...");

                // Essaye de se connecter au serveur FTP
                ftpClient.Connect(_ftp_profile);

                if (ftpClient.IsConnected)
                {
                    // La 1ere synchro est forcement complete ou si le flag de synchroniser les differences n'est pas leve
                    if (_nbSyncDistant >= 1 && SynchroniseDifferences && listFiles != null)
                    {
                        output.IsComplet = false;
                        output.IsSuccess = true;
                        cStatus = new StatusMiniSite(cStatus.State);
                        output.nbUpload = listFiles.Count;
                        int idx = 0;
                        foreach (FileInfo localFileInfo in listFiles)
                        {
                            // Calculer le repertoire de destination FTP en remplacant le repertoire racine local par la racine FTP
                            string ftpFileName = GetFTPFromLocal(localFileInfo.FullName, localRootDirectory);

                            // Charhe le fichier
                            FtpStatus fileUploadOut = ftpClient.UploadFile(localFileInfo.FullName,
                                                                            ftpFileName,
                                                                            FtpRemoteExists.Overwrite,
                                                                            true,
                                                                            FtpVerify.None,
                                                                            null);
                            if (fileUploadOut != FtpStatus.Success)
                            {
                                // NOK car pas tous les fichiers charges
                                output.IsSuccess = false;
                                cStatus = new StatusMiniSite(cStatus.State, "Erreur FTP", "Impossible de charger certains fichiers");
                            }
                            else
                            {
                                // Un fichier de plus charge
                                output.nbUpload++;
                            }

                            // Met a jour la progression du transfert
                            CalculProgressionFTP(idx, listFiles.Count);
                            idx++;
                        }
                    }
                    else
                    {
                        output.IsComplet = true;
                        output.IsSuccess = false;
                        cStatus = new StatusMiniSite(cStatus.State, "Erreur FTP", "Impossible de charger le repertoire complet");
                        // Charge le dossier du site vers le serveur FTP en mode miroir pour synchroniser
                        List<FtpResult> uploadOut = ftpClient.UploadDirectory(localRootDirectory,
                                                                                RepertoireSiteFTPDistant,
                                                                                FtpFolderSyncMode.Mirror,
                                                                                FtpRemoteExists.Overwrite,
                                                                                FtpVerify.None,
                                                                                null,
                                                                                _ftpProgressCallback);

                        if (uploadOut.Count > 0)
                        {
                            output.IsSuccess = true;
                            output.nbUpload = uploadOut.Count;
                            cStatus = new StatusMiniSite(cStatus.State);
                        }
                    }

                    // Disconnect
                    ftpClient.Disconnect();

                    // Incremente le nb de synchronisation realisee depuis le demarrage
                    _nbSyncDistant++;

                }
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                string msg = (ex.InnerException != null) ? String.Format("{0} ({1})", ex.Message, ex.InnerException.Message) : ex.Message;
                cStatus = new StatusMiniSite(cStatus.State, "Erreur FTP", msg);
                LogTools.Logger.Error("Erreur lors de upload FTP", ex);
            }
            finally
            {

                // disconnect et forcer le dispose pour s'assurer que la connection est bien coupe (cf. pb de 2 cnx max surt Free)
                if (ftpClient.IsConnected)
                {
                    ftpClient.Disconnect();
                }

                // Detruit le client
                ftpClient?.Dispose();
            }

            // RAZ le status apres la fin du transfert
            Status = cStatus;

            return output;
        }

        /// <summary>
        /// Calcul le repertoire de destination FTP a partir du nom de fichier local
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="localDirectoryName"></param>
        /// <returns></returns>
        private string GetFTPFromLocal(string localFileName, string localDirectoryName)
        {
            string output = string.Empty;
            if (!string.IsNullOrEmpty(localFileName) && !string.IsNullOrEmpty(localDirectoryName))
            {
                // Aligne les noms des repertoires pour n'avoir que des '/' au lieu de '\'
                string cleanLocalFileName = FluentFTP.Helpers.RemotePaths.GetFtpPath(localFileName);
                string cleanLocalDirName = FluentFTP.Helpers.RemotePaths.GetFtpPath(localDirectoryName);
                string cleanDistantDirName = FluentFTP.Helpers.RemotePaths.GetFtpPath(RepertoireSiteFTPDistant);

                // Remplace le repertoire racine local dans le nom du fichier local par le repertoire racine FTP
                string ftpDestination = cleanLocalFileName.Replace(cleanLocalDirName, cleanDistantDirName);

                // Nettoie  le chemin
                output = FluentFTP.Helpers.RemotePaths.GetFtpPath(ftpDestination);
            }

            return output;
        }
        #endregion
    }
}
