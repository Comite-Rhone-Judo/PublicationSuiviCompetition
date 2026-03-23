using AppPublication.Models; // Assurez-vous que ce namespace correspond à l'emplacement de EcranAppelModel
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace AppPublication.Models.EcransAppel
{
    public class EcranCollectionManager
    {
        #region MEMBRES

        // Cache interne pour la valeur de l'ID le plus élevé
        private int _lastId;

        private EcranAppelModel _default;

        #endregion

        #region PROPRIETES
        // La collection observable pour le binding UI
        public ObservableCollection<EcranAppelModel> Ecrans { get; private set; }

        // Accès en lecture seule au Cache
        public int LastId => _lastId;

        // Calcul du prochain ID disponible
        public int NextId => _lastId + 1;

        // Le nombre de tapis de la competition, utilisé pour la validation des écrans d'appel
        private int _nbTapis = 0;
        public int NbTapis {
            get
            {
                return _nbTapis;
            }
            set {
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
                        }
                    }
                }
            }
        }

        #endregion

        #region CONSTRUCTEUR
        public EcranCollectionManager()
        {
            Ecrans = new ObservableCollection<EcranAppelModel>();

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
        /// Crée un nouvel écran, l'ajoute à la liste et met à jour le cache ID.
        /// </summary>
        public EcranAppelModel Add()
        {
            var nouvelEcran = new EcranAppelModel
            {
                Id = NextId, // Utilise la propriété calculée
                Description = $"Ecran {NextId}"
            };

            Ecrans.Add(nouvelEcran);

            // Mise à jour du cache vers le haut
            _lastId = nouvelEcran.Id;

            return nouvelEcran;
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
            // Gestion de collision basique : si l'ID est déjà pris, on le change
            if (Ecrans.Any(e => e.Id == ecran.Id))
            {
                ecran.Id = NextId;
            }

            Ecrans.Add(ecran);

            // Si on ajoute un ID plus grand que le cache actuel, on met à jour le cache
            if (ecran.Id > _lastId)
            {
                _lastId = ecran.Id;
            }
        }

        /// <summary>
        /// Supprime un écran et recalcule le cache ID pour coller aux valeurs existantes.
        /// </summary>
        public void Remove(EcranAppelModel ecran)
        {
            if (Ecrans.Contains(ecran))
            {
                Ecrans.Remove(ecran);
                RecalculateHighWatermark();
            }
        }

        public void Remove(int id)
        {
            var ecran = Ecrans.FirstOrDefault(e => e.Id == id);
            if (ecran != null)
            {
                // Appelle la méthode principale pour bénéficier du recalcul
                Remove(ecran);
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
                _lastId = Ecrans.Max(e => e.Id);
            }
        }

        #endregion
    }
}