using FluentFTP;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel.Channels;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Markup;
using Tools.Framework;
using Tools.Logging;
using Tools.Threading;


namespace Tools.Net
{
    #region CLASSES ANNEXES
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
    #endregion

    public abstract class MiniSite : NotificationBase
    {
        #region CONSTANTES
        private const int kMaxRetryFTP = 5;
        #endregion

        #region MEMBRES
        private FtpProfile _ftp_profile = null;     // Le profile FTP a utiliser pour les connexions
        private Action<FtpProgress> _ftpProgressCallback = null;
        private long _nbSyncDistant = 0;
        private string _instanceName = string.Empty;
        private int _maxRetryFTP = kMaxRetryFTP;

        private long _totalDeleteCount = 0;
        private long _currentDeleteCount = 0;
        #endregion

        #region CONSTRUCTEURS

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="local">Mode du minisite (local = true, distant = false)</param>
        /// <param name="instanceName">Nom de l'instance</param>
        public MiniSite(bool local, string instanceName = "")
        {
            // Initialise les caracteristiques du MiniSite
            InstanceName = instanceName;

            if (local)
            {
                // Configure un site web local
                ServerHTTP = new ServeurHttp();

                // Initialise les interfaces
                InitInterfaces();
            }
            else
            {
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
        /// Le nombre max d'essai pour charger un fichier
        /// </summary>
        public int MaxRetryFTP
        {
            get
            {
                return _maxRetryFTP;
            }
            set
            {
                _maxRetryFTP = value;
                NotifyPropertyChanged();
            }
        }

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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        IPAddress _interfaceLocalPublication;
        /// <summary>
        /// Interface (@IP) utilisée pour la publication du site en mode local
        /// doit etre presente dans la liste InterfacesLocal
        /// </summary>
        public virtual IPAddress InterfaceLocalPublication
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

                    NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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

        private string _ftpDistant = string.Empty;
        /// <summary>
        /// L'adresse du site FTP Distant
        /// </summary>
        public virtual string SiteFTPDistant
        {
            get
            {
                return _ftpDistant;
            }
            set
            {
                _ftpDistant = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        private string _ftpLoginDistant = string.Empty;
        /// <summary>
        /// Login de connexion au site FTP Distant
        /// </summary>
        public virtual string LoginSiteFTPDistant
        {
            get
            {
                return _ftpLoginDistant;
            }
            set
            {
                _ftpLoginDistant = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        private bool _modeFTPActif = false;

        /// <summary>
        /// Mode de fonctionnement FTP Actif (true) ou passif (false)
        /// </summary>
        public virtual bool ModeActifFTPDistant
        {
            get
            {
                return _modeFTPActif;
            }
            set
            {
                _modeFTPActif = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }

        private string _ftpPasswordDistant = string.Empty;
        /// <summary>
        /// Mot de passe FTP au site FTP Distant
        /// </summary>
        public virtual string PasswordSiteFTPDistant
        {
            get
            {
                return _ftpPasswordDistant;
            }
            set
            {
                _ftpPasswordDistant = value;
                NotifyPropertyChanged();
                IsChanged = true;
            }
        }  

        private bool _syncDiff = false;
        /// <summary>
        /// Synchronise uniquement les différences (True) sinon, transfere tous les fichier (False)
        /// </summary>
        public virtual bool SynchroniseDifferences
        {
            get
            {
                return _syncDiff;
            }
            set
            {
                _syncDiff = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                System.Windows.Application.Current.ExecOnUiThread(() =>
                {
                    _status = value;
                    NotifyPropertyChanged(nameof(Status));

                    // Actualise l'etat d'activite du site
                    // Le site doit etre arrete ou en cours de nettoyage
                    IsActif = !(_status.State == StateMiniSiteEnum.Stopped || _status.State == StateMiniSiteEnum.Cleaning);
                    IsCleaning = (_status.State == StateMiniSiteEnum.Cleaning);
                });
            }
        }

        #endregion

        #region METHODES

        /// <summary>
        /// Tente de définir l'interface de publication à partir d'une chaîne (ex: config).
        /// Si l'IP est invalide ou absente de la machine, sélectionne automatiquement la première interface disponible.
        /// </summary>
        /// <param name="ipString">La chaîne représentant l'adresse IP</param>
        public void SelectInterfaceOrDefault(string ipString)
        {
            IPAddress ipToUse = null;

            // 1. Tente de parser et de trouver l'IP dans la liste des interfaces locales
            if (!string.IsNullOrEmpty(ipString) && IPAddress.TryParse(ipString, out var parsedIp))
            {
                if (InterfacesLocal.Contains(parsedIp))
                {
                    ipToUse = parsedIp;
                }
            }

            // 2. Si aucune IP valide trouvée (ou config vide), Fallback sur la première dispo
            if (ipToUse == null && InterfacesLocal.Count > 0)
            {
                ipToUse = InterfacesLocal.First();
            }

            // 3. Application de la valeur (déclenche le Setter existant qui gère le serveur HTTP et la notif)
            if (ipToUse != null)
            {
                try
                {
                    // On passe par la propriété pour bénéficier des effets de bord (ServerHTTP update, PropertyChanged)
                    // Attention : le setter actuel lève une exception si l'IP n'est pas dans la liste, 
                    // mais ici on a garanti que 'ipToUse' est soit dans la liste, soit null (si liste vide).
                    this.InterfaceLocalPublication = ipToUse;
                }
                catch (Exception ex)
                {
                    LogTools.Logger.Debug(ex, "Erreur lors de la sélection de l'interface locale pour le MiniSite.");
                }
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
                Status = new StatusMiniSite(StateMiniSiteEnum.Cleaning, "Nettoyage FTP ...");

                string repertoire = RepertoireSiteFTPDistant;

                // Essaye de se connecter au serveur FTP
                ftpClient.Connect(_ftp_profile);

                if (ftpClient.IsConnected)
                {
                    _currentDeleteCount = 0;
                    _totalDeleteCount = 0;

                    // On commence par compter le nombre total de fichier a traiter (pour la progression)
                    // Recupere le contenu du repertoire
                    List<FtpListItem> ftpList = ftpClient.GetListing(repertoire, FtpListOption.Recursive).ToList();

                    // Compte les fichiers dans le repertoire courant
                    _totalDeleteCount = ftpList.Count(o => o.Type == FtpObjectType.File);

                    // Efface recursivement mais en calculant la progression (ce que ne fait pas ftpClient.DeleteDirectory)
                    InternalFtpRecursiveDeleteDirectory(repertoire, ftpClient, true);

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

                            // Fichier temporaire pour le transfert
                            string ftpTmpFile = ftpFileName + ".upl";

                            // Charge le fichier vers le fichier temporaire. on essaye 5 fois
                            FtpStatus fileUploadOut = FtpStatus.Failed;
                            int retry = 0;
                            bool done = false;
                            while (fileUploadOut != FtpStatus.Success && retry <= _maxRetryFTP)
                            {
                                fileUploadOut = ftpClient.UploadFile(localFileInfo.FullName,
                                                                                ftpTmpFile,
                                                                                FtpRemoteExists.Overwrite,
                                                                                true,
                                                                                FtpVerify.None,
                                                                                null);
                                retry++;
                                if (fileUploadOut != FtpStatus.Success)
                                {
                                    LogTools.Logger.Debug("Erreur lors du transfert du fichier {0} vers {1}, essai {2}", localFileInfo.FullName, ftpTmpFile, retry);
                                    Thread.Sleep(100);  // Attend 100ms avant de reessayer
                                }
                            }

                            if (fileUploadOut == FtpStatus.Success)
                            {
                                bool moved = false;
                                retry = 0;

                                // Deplace le fichier temporaire vers le fichier final
                                while(!moved && retry <= _maxRetryFTP)
                                {
                                    moved = ftpClient.MoveFile(ftpTmpFile, ftpFileName, FtpRemoteExists.Overwrite);
                                    retry++;
                                    if (!moved)
                                    {
                                        LogTools.Logger.Debug("Erreur lors du deplacement du fichier {0} vers {1}, essai {2}", ftpTmpFile, ftpFileName, retry);
                                        Thread.Sleep(100);  // Attend 100ms avant de reessayer
                                    }
                                }                                
                                
                                if (moved)
                                {
                                    // Un fichier de plus charge
                                    done = true;
                                    output.nbUpload++;
                                }
                                else
                                {
                                    done = false;
                                    LogTools.Logger.Debug("Erreur lors deplacement du fichier {0} vers {1}", ftpTmpFile, ftpFileName);
                                }
                            }
                            else
                            {
                                done = false;
                            }

                            if(!done)
                            {
                                // NOK car pas tous les fichiers charges
                                output.IsSuccess = false;
                                cStatus = new StatusMiniSite(cStatus.State, "Erreur FTP", "Impossible de charger certains fichiers");
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
                LogTools.Logger.Error(ex, "Erreur lors de upload FTP");
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


        #endregion

        #region METHODES PRIVEES

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

        /// <summary>
        /// Initialise la liste des interfaces locales disponibles via NetworkInterface.
        /// Plus rapide et fiable que Dns.GetHostName/GetHostAddresses.
        /// </summary>
        protected void InitInterfaces()
        {
            try
            {
                InterfacesLocal = new List<IPAddress>();

                // Récupère toutes les interfaces réseau
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                // On filtre pour ne garder que les interfaces opérationnelles (Up)
                // et on exclut le Loopback (127.0.0.1) qui n'est généralement pas utile pour la publication
                var activeInterfaces = interfaces
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up
                              && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                foreach (var adapter in activeInterfaces)
                {
                    var properties = adapter.GetIPProperties();

                    // Récupère les adresses unicast IPv4
                    var ipv4Addresses = properties.UnicastAddresses
                        .Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(ua => ua.Address);

                    InterfacesLocal.AddRange(ipv4Addresses);
                }

                // Suppression des doublons potentiels
                InterfacesLocal = InterfacesLocal.Distinct().ToList();

                // Sélection par défaut
                if (InterfacesLocal.Count > 0)
                {
                    InterfaceLocalPublication = InterfacesLocal.First();
                }
            }
            catch (Exception ex)
            {
                LogTools.Error(ex);
            }
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
        /// Supprime de maniere recursive tous les fichiers et repertoires dans un repertoire FTP
        /// </summary>
        /// <param name="repertoire"></param>
        /// <param name="ftpClient"></param>
        /// <param name="onlyContent">True pour ne pas effacer le repertoire lui meme</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        private void InternalFtpRecursiveDeleteDirectory(string repertoire, FtpClient ftpClient, bool onlyContent = true)
        {
            if (ftpClient == null || !ftpClient.IsConnected) { throw new ArgumentException("Le client FTP doit etre connecte"); }

            bool reportProgress = (_totalDeleteCount > 0);

            try
            {
                // Recupere le contenu du repertoire
                List<FtpListItem> ftpList = ftpClient.GetListing(repertoire).ToList();

                // Supprime les fichiers dans le repertoire courant
                List<FtpListItem> ftpFic = ftpList.Where(o => o.Type == FtpObjectType.File).ToList();

                foreach (var item in ftpFic)
                    {
                    // Supprime le fichier
                    ftpClient.DeleteFile(item.FullName);

                    // Un de plus a ce niveau d'execution
                    _currentDeleteCount++;

                    // Met a jour la progression du nettoyage
                    if (reportProgress) { CalculProgressionFTP(_currentDeleteCount, _totalDeleteCount); }
                }

                // Pour chaque sous-repertoire, lance un appel recursif
                List<FtpListItem> ftpDir = ftpList.Where(o => o.Type == FtpObjectType.Directory).ToList();

                foreach (var item in ftpDir)
                {
                    // Ce sont des sous repertoires, ont doit donc les effacer avec leur contenu
                    InternalFtpRecursiveDeleteDirectory(item.FullName, ftpClient, false);
                }

                // et on fini par supprimer le répertoire qui est vide maintenant sauf si on ne doit vider que le contenu
                if (!onlyContent) { ftpClient.DeleteDirectory(repertoire); }
            }
            catch (Exception ex)
            {
                LogTools.Logger.Error(ex, "Erreur lors de la suppression recursive des fichiers FTP dans le repertoire {0}", repertoire);
                throw new Exception("Erreur lors de la suppression recursive des fichiers FTP", ex);
            }
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
        private void CalculProgressionFTP(long index, long total)
        {
            int pct = -1;
            // Calcul le ratio de transfert du repertoire
            if (index >= 0 && total > 0)
            {
                // Pn majore la progression a 100% pour eviter les erreurs d'arrondis
                pct = Math.Min( 100, (int)Math.Round(((index + 1.0) / total) * 100));
            }

            Status.Progress = pct;
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
