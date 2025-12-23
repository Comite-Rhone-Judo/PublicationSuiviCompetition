using KernelImpl.Noyau.Arbitrage;
using KernelImpl.Noyau.Categories;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Logos;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using KernelImpl.Noyau.Structures;
using System;
using System.Threading;
using Tools.Framework;

namespace KernelImpl
{
    public class JudoData : NotificationBase, IJudoDataManager, IJudoData
    {
        #region MEMBRES
        // --- Verrouillage ---
        private readonly ReaderWriterLockSlim _globalLock = new ReaderWriterLockSlim();
        #endregion

        #region Constructeurs
        public JudoData()
        {
            Arbitrage = new DataArbitrage();
            Categories = new DataCategories();
            Deroulement = new DataDeroulement();
            Logos = new DataLogos();
            Organisation = new DataOrganisation();
            Participants = new DataParticipants();
            Structures = new DataStructures();
        }
        #endregion

        #region Implémentation IJudoDataManager

        /// <summary>
        /// Obtient un snapshot immuable et thread-safe.
        /// </summary>
        public IJudoData GetSnapshot()
        {
            _globalLock.EnterReadLock();
            try
            {
                // Crée le snapshot en copiant les références des listes actuelles
                return new JudoDataSnapshot(this);
            }
            finally
            {
                _globalLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Exécute une mise à jour (écriture) sous verrou exclusif.
        /// </summary>
        public void RunSafeDataUpdate(Action actionToRun)
        {
            _globalLock.EnterWriteLock();
            try
            {
                actionToRun();
            }
            finally
            {
                _globalLock.ExitWriteLock();
            }
        }

        #endregion

        #region Implémentation Explicite de IJudoDataSource

        // Ces propriétés ne sont visibles que lorsqu'on manipule l'objet via l'interface IJudoDataSource.
        // Elles redirigent vers vos propriétés concrètes existantes.

        IDeroulementData IJudoData.Deroulement => this.Deroulement;

        IParticipantsData IJudoData.Participants => this.Participants;

        IOrganisationData IJudoData.Organisation => this.Organisation;

        IStructuresData IJudoData.Structures => this.Structures;

        ICategoriesData IJudoData.Categories => this.Categories;

        IArbitrageData IJudoData.Arbitrage => this.Arbitrage;

        ILogosData IJudoData.Logos => this.Logos;

        #endregion

        #region Interface concrete

        private Noyau.Structures.DataStructures _structures = null;
        public Noyau.Structures.DataStructures Structures
        {
            get { return _structures; }
            set { _structures = value; }
        }

        private Noyau.Logos.DataLogos _logos = null;
        public Noyau.Logos.DataLogos Logos
        {
            get { return _logos; }
            set { _logos = value; }
        }

        private Noyau.Categories.DataCategories _categories = null;
        public Noyau.Categories.DataCategories Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        private DataOrganisation _organisation = null;
        public DataOrganisation Organisation
        {
            get { return _organisation; }
            set { _organisation = value; }
        }

        private Noyau.Participants.DataParticipants _participants = null;
        public Noyau.Participants.DataParticipants Participants
        {
            get { return _participants; }
            set { _participants = value; }
        }

        private Noyau.Deroulement.DataDeroulement _deroulement = null;
        public Noyau.Deroulement.DataDeroulement Deroulement
        {
            get { return _deroulement; }
            set { _deroulement = value; }
        }

        private Noyau.Arbitrage.DataArbitrage _arbitrage = null;
        public Noyau.Arbitrage.DataArbitrage Arbitrage
        {
            get { return _arbitrage; }
            set { _arbitrage = value; }
        }

        #endregion
    }
}
