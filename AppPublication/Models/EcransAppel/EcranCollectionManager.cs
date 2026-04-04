using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace AppPublication.Models.EcransAppel
{
    public class EcranCollectionManager
    {
        #region MEMBRES
        private int _lastId;    // Cache interne pour la valeur de l'ID le plus élevé
        private readonly EcranAppelModel _default;
        private readonly List<EcranAppelModel> _ecrans;
        private readonly object _dataLock = new object();
        private volatile EcranCollectionSnapshot _currentSnapshot;  // --- GESTION DU SNAPSHOT ---
        #endregion

        #region PROPRIETES

        /// <summary>
        /// Accès en lecture seule pour initialiser l'UI 
        /// </summary>
        public IReadOnlyList<EcranAppelModel> Ecrans
        {
            get { lock (_dataLock) return _ecrans.ToList().AsReadOnly(); }
        }

        // Accès en lecture seule au Cache
        public int LastId => _lastId;

        // Calcul du prochain ID disponible
        public int NextId => _lastId + 1;

        // Le nombre de tapis de la competition, utilisé pour la validation des écrans d'appel
        private int _nbTapis = 0;
        public int NbTapis
        {
            get
            {
                return _nbTapis;
            }
            set
            {
                lock (_dataLock)
                {
                    if (_nbTapis != value)
                    {
                        if (_nbTapis >= 0)
                        {
                            _nbTapis = value;
                            // Actualise les tapis par défaut pour l'écran d'appel par défaut
                            if (_default != null)
                            {
                                // Sur l'ecran par defaut, on est prudent: 1 tapis par page, et on affiche tous les tapis disponibles
                                _default.Groupement = 1;
                                _default.NbCombatsPage = 8;
                                // Par defaut, on affiche tous les tapis
                                _default.TapisIds = Enumerable.Range(1, _nbTapis).ToList();

                                InvalidateSnapshot();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retourne un snapshot du model
        /// </summary>
        public EcranCollectionSnapshot Snapshot
        {
            get
            {
                var snap = _currentSnapshot;
                if (snap != null) return snap;

                lock (_dataLock)
                {
                    if (_currentSnapshot == null)
                    {
                        var clonedEcrans = _ecrans.Select(e => e.Clone());
                        var clonedDefault = _default?.Clone();
                        _currentSnapshot = new EcranCollectionSnapshot(clonedEcrans, clonedDefault);
                    }
                    return _currentSnapshot;
                }
            }
        }

        #endregion

        #region CONSTRUCTEUR
        public EcranCollectionManager()
        {
            _ecrans = new List<EcranAppelModel>();

            _lastId = 0;
            _default = new EcranAppelModel
            {
                Id = -1,
                Description = "Ecran par défaut",
                Groupement = 1,
                NbCombatsPage = 8,
                Disposition = EcranAppelModel.DispositionAffichage.Colonne,
                DispositionCombat = EcranAppelModel.DispositionAffichage.Colonne
            };


            NbTapis = 6;    // par defaut 6 tapis, valeur assez commune. Cela va initialiser le tapis par default
        }

        #endregion

        #region METHODES PUBLIQUES

        /// <summary>
        /// Marque le snapshot actuel comme obsolète, forçant sa recréation au prochain accès.
        /// </summary>
        public void InvalidateSnapshot()
        {
            _currentSnapshot = null;
        }

        /// <summary>
        /// Crée un nouvel écran, l'ajoute à la liste et met à jour le cache ID.
        /// </summary>
        public EcranAppelModel Add()
        {
            lock (_dataLock)
            {
                var nouvelEcran = new EcranAppelModel { Id = NextId, Description = $"Ecran {NextId}" };
                _ecrans.Add(nouvelEcran); // Utilisation de _ecrans
                _lastId = nouvelEcran.Id;

                InvalidateSnapshot();
                return nouvelEcran;
            }
        }

        public EcranAppelModel Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// Ajoute un écran existant (ex: import) et ajuste le cache si nécessaire.
        /// </summary>
        public void Add(EcranAppelModel ecran)
        {
            lock (_dataLock)
            {
                // Gestion de collision basique : si l'ID est déjà pris, on le change
                if (_ecrans.Any(e => e.Id == ecran.Id))
                {
                    ecran.Id = NextId; // NextId utilise _lastId, c'est thread-safe dans le lock
                }

                _ecrans.Add(ecran);

                // Si on ajoute un ID plus grand que le cache actuel, on met à jour le cache
                if (ecran.Id > _lastId)
                {
                    _lastId = ecran.Id;
                }

                // POINT D'INVALIDATION : La collection a changé, on invalide le cache
                InvalidateSnapshot();
            }
        }

        /// <summary>
        /// Supprime un écran et recalcule le cache ID pour coller aux valeurs existantes.
        /// </summary>
        public void Remove(EcranAppelModel ecran)
        {
            lock (_dataLock)
            {
                var itemToRemove = _ecrans.FirstOrDefault(e => e.Id == ecran.Id); // Utilisation de _ecrans
                if (itemToRemove != null)
                {
                    _ecrans.Remove(itemToRemove); // Utilisation de _ecrans
                    RecalculateHighWatermark();

                    InvalidateSnapshot();
                }
            }
        }

        /// <summary>
        /// Supprime un écran et recalcule le cache ID pour coller aux valeurs existantes.
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            lock (_dataLock)
            {
                var itemToRemove = _ecrans.FirstOrDefault(e => e.Id == id);
                if (itemToRemove != null)
                {
                    _ecrans.Remove(itemToRemove);
                    RecalculateHighWatermark();
                    InvalidateSnapshot();
                }
            }
        }
        #endregion

        #region METHODES PRIVEES

        /// <summary>
        /// Recalcule le _lastId en parcourant la liste.
        /// Garantit que le NextID sera immédiatement consécutif au plus grand ID restant.
        /// </summary>
        private void RecalculateHighWatermark()
        {
            if (Ecrans.Count == 0)
            {
                _lastId = 0;
            }
            else
            {
                // O(N) : Négligeable pour des listes d'écrans (< 1000 éléments)
                _lastId = _ecrans.Max(e => e.Id);
            }
        }

        #endregion
    }
}