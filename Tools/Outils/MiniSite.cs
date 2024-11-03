using FluentFTP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        #region MEMBRES
        private FtpProfile _ftp_profile = null;     // Le profile FTP a utiliser pour les connexions
        private Action<FtpProgress> _ftpProgressCallback = null;
        private long _nbSyncDistant = 0;
        private Dictionary<string, EntitePublicationFFJudo> _allEntitePublicationFFJudo = null;
        private Dictionary<string, ObservableCollection<EntitePublicationFFJudo>> _allEntitesPublicationFFJudo = null;

        #endregion

        #region CONSTRUCTEURS
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="local">Mode du minisite (local = true, distant = false)</param>
        public MiniSite(bool local)
        {
            if (local)
            {
                // Configure un site web local
                ServerHTTP = new ServeurHttp();

                // Initialise les interfaces
                InitInterfaces();
            }
            else
            {
                // Initialise la configuration pour la publication simplifiee France Judo
                InitPublicationFFJudo();

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

        private bool _easyConfig;

        public bool EasyConfig
        {
            get
            {
                return _easyConfig;
            }
            set
            {
                _easyConfig = value;
                AppSettings.SaveSetting("EasyConfig", _easyConfig.ToString());
                NotifyPropertyChanged("EasyConfig");
            }
        }

        private ObservableCollection<string> _listeNiveauxPublicationFFJudo;
        /// <summary>
        /// La liste des niveaux de publication
        /// </summary>
        public ObservableCollection<string> ListeNiveauxPublicationFFJudo
        {
            get
            {
                return _listeNiveauxPublicationFFJudo;
            }
            set
            {
                _listeNiveauxPublicationFFJudo = value;
                NotifyPropertyChanged("ListeNiveauxPublicationFFJudo");
            }
        }

        private ObservableCollection<EntitePublicationFFJudo> _listeEntitesPublicationFFJudo;

        /// <summary>
        /// La liste de toutes les entites de publication existantes pour le niveau de publication selectionne
        /// </summary>
        public ObservableCollection<EntitePublicationFFJudo> ListeEntitesPublicationFFJudo
        {
            get
            {
                return _listeEntitesPublicationFFJudo;
            }
            set
            {
                _listeEntitesPublicationFFJudo = value;
                NotifyPropertyChanged("ListeEntitesPublicationFFJudo");
            }
        }

        private EntitePublicationFFJudo _entitePublicationFFJudo;

        /// <summary>
        /// Entite de publication selectionnee
        /// </summary>
        public EntitePublicationFFJudo EntitePublicationFFJudo
        {
            get
            {
                return _entitePublicationFFJudo;
            }
            set
            {
                _entitePublicationFFJudo = value;
                if (value != null)
                {
                    // Garde en memoire la derniere valeur sauvegardee pour ce niveau
                    _allEntitePublicationFFJudo[_niveauPublicationFFJudo] = value;
                    AppSettings.SaveSetting("EntitePublicationFFJudo", _entitePublicationFFJudo.Nom);
                }
                NotifyPropertyChanged("EntitePublicationFFJudo");
            }
        }


        /// <summary>
        /// Les entites de publication selectionnees par niveau
        /// </summary>
        public Dictionary<string, EntitePublicationFFJudo> AllEntitePublicationFFJudo
        {
            get
            {
                return _allEntitePublicationFFJudo;
            }
            set
            {
                _allEntitePublicationFFJudo = value;
                NotifyPropertyChanged("AllEntitePublicationFFJudo");
            }
        }

        private string _niveauPublicationFFJudo;
        /// <summary>
        /// Le niveau de publication selectionne
        /// </summary>
        public string NiveauPublicationFFJudo
        {
            get
            {
                return _niveauPublicationFFJudo;
            }
            set
            {
                _niveauPublicationFFJudo = value;
                AppSettings.SaveSetting("NiveauPublicationFFJudo", _niveauPublicationFFJudo);

                // Ajuste la liste des entites et restaure le dernier element selectionne pour ce niveau
                ObservableCollection<EntitePublicationFFJudo> ent = null;
                try
                {
                    ent = _allEntitesPublicationFFJudo[_niveauPublicationFFJudo];
                }
                catch
                {
                    ent = null;
                }
                finally
                {
                    ListeEntitesPublicationFFJudo = ent;
                    EntitePublicationFFJudo = _allEntitePublicationFFJudo[_niveauPublicationFFJudo];
                }

                NotifyPropertyChanged("NiveauPublicationFFJudo");
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
                    if (_interfaceLocalPublication != null)
                    {
                        AppSettings.SaveSetting("InterfaceLocalPublication", _interfaceLocalPublication.ToString());
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
                AppSettings.SaveSetting("SiteFTPDistant", _ftpDistant);
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
                AppSettings.SaveSetting("LoginSiteFTPDistant", _ftpLoginDistant);
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
                AppSettings.SaveSetting("ModeActifFTPDistant", _modeFTPActif.ToString());
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
                AppSettings.SaveEncryptedSetting("PasswordSiteFTPDistant", _ftpPasswordDistant);
                NotifyPropertyChanged("PasswordSiteFTPDistant");
                IsChanged = true;
            }
        }

        private string _passwordPublicationFFJudo = string.Empty;
        /// <summary>
        /// Mot de passe FTP au site France Judo
        /// </summary>
        public string PasswordPublicationFFJudo
        {
            get
            {
                return _passwordPublicationFFJudo;
            }
            set
            {
                _passwordPublicationFFJudo = value;
                AppSettings.SaveEncryptedSetting("PasswordPublicationFFJudo", _passwordPublicationFFJudo);
                NotifyPropertyChanged("PasswordPublicationFFJudo");
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
                AppSettings.SaveSetting("SynchroniseDifferences", _syncDiff.ToString());
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
        /// Initialise la liste des comites et ligues pour la publication sur les serveurs France Judo
        /// </summary>
        private void InitPublicationFFJudo()
        {
            // Charge la structure XML en memoire depuis les resources
            XmlReader structureReader = XmlReader.Create(ResourcesTools.GetAssembyResource(ConstantResource.PublicationFFJUDO));

            XmlDocument doc = new XmlDocument();
            doc.Load(structureReader);

            if (doc.DocumentElement.HasChildNodes)
            {
                ObservableCollection<string> tmp = new ObservableCollection<string>();
                _allEntitePublicationFFJudo = new Dictionary<string, EntitePublicationFFJudo>();
                _allEntitesPublicationFFJudo = new Dictionary<string, ObservableCollection<EntitePublicationFFJudo>>();

                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    ObservableCollection<EntitePublicationFFJudo> tmpNiveau = new ObservableCollection<EntitePublicationFFJudo>();
                    if (node.HasChildNodes)
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.Attributes != null && childNode.Attributes["nom"] != null && childNode.Attributes["libelle"] != null)
                            {
                                tmpNiveau.Add(new EntitePublicationFFJudo(childNode.Attributes["nom"].Value, childNode.Attributes["libelle"].Value));
                            }
                        }

                        // On ne tient compte d'un niveau que s'il a des entites en dessous
                        if (tmpNiveau.Count > 0)
                        {
                            tmp.Add(node.Name);
                            _allEntitePublicationFFJudo.Add(node.Name, tmpNiveau.First());
                            _allEntitesPublicationFFJudo.Add(node.Name, tmpNiveau);
                        }
                    }
                }
                ListeNiveauxPublicationFFJudo = tmp;
            }
        }

        /// <summary>
        /// Initialise la configuraiton FTP a partir du cache de fichier AppConfig
        /// </summary>
        private void InitConfigFTP()
        {
            string valCache = string.Empty;

            try
            {
                SiteFTPDistant = AppSettings.ReadSetting("SiteFTPDistant", string.Empty);
                LoginSiteFTPDistant = AppSettings.ReadSetting("LoginSiteFTPDistant", string.Empty);
                PasswordSiteFTPDistant = AppSettings.ReadEncryptedSetting("PasswordSiteFTPDistant", string.Empty);
                PasswordPublicationFFJudo = AppSettings.ReadEncryptedSetting("PasswordPublicationFFJudo", string.Empty);
                ModeActifFTPDistant = AppSettings.ReadSetting("ModeActifFTPDistant", false);
                SynchroniseDifferences = AppSettings.ReadSetting("SynchroniseDifferences", false);
                EasyConfig = AppSettings.ReadSetting("EasyConfig", true);
                // Charge les valeurs pour la publication FFJudo. On doit faire le niveau en 1er pour
                // avoir la bonne liste d'entites ensuite
                NiveauPublicationFFJudo = AppSettings.ReadSetting<string>("NiveauPublicationFFJudo", ListeNiveauxPublicationFFJudo, o => o);
                EntitePublicationFFJudo = AppSettings.ReadSetting<EntitePublicationFFJudo>("EntitePublicationFFJudo", ListeEntitesPublicationFFJudo, o => o.Nom);
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
                string valCache = AppSettings.ReadSetting("InterfaceLocalPublication");
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
                    // TODO Ajouter la gestion du mode Easy/Advanced
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
                LogTools.Log(ex);
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
                LogTools.Log(ex);
            }
        }

        /// <summary>
        /// Valide la configuration du site distant et initialise le FtpProfile
        /// </summary>
        /// <returns></returns>
        private bool CheckConfigurationSiteDistant()
        {
            // TODO Ajouter la gestion du mode Easy/Advanced
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
                    LogTools.Log(ex);
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
                LogTools.Log(ex);
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
        /// <param name="distantDirectory">Repertoire distant (par rapport à la racine), "" pour charger directement a la racine</param>
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
                LogTools.Log(ex);
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
