
using KernelImpl.Internal;
using KernelImpl.Noyau.Organisation;
using NLog;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Participants
{
    public class DataParticipants
    {
        private readonly DeduplicatedCachedData<int, Equipe> _equipesCache = new DeduplicatedCachedData<int, Equipe>();
        private readonly DeduplicatedCachedData<int, Judoka> _judokasCache = new DeduplicatedCachedData<int, Judoka>();
        private readonly DeduplicatedCachedData<int, EpreuveJudoka> _epreuvejudokasCache = new DeduplicatedCachedData<int, EpreuveJudoka>();
        private readonly DeduplicatedCachedData<string, vue_judoka> _vue_judokasCache = new DeduplicatedCachedData<string, vue_judoka>();

        // Accesseurs O(1)
        public IReadOnlyList<Equipe> Equipes { get { return _equipesCache.Cache; } }
        public IReadOnlyList<Judoka> Judokas { get { return _judokasCache.Cache; } }
        public IReadOnlyList<EpreuveJudoka> EJS { get { return _epreuvejudokasCache.Cache; } }
        public IReadOnlyList<vue_judoka> vjudokas { get { return _vue_judokasCache.Cache; } }

        public IEnumerable<Judoka> GetJudokaEpreuve(int epreuve)
        {
            IEnumerable<int> judokas = EJS.Where(o => o.epreuve == epreuve).Select(o => o.judoka).Distinct();
            return Judokas.Where(o => judokas.Contains(o.id));
        }

        // Expose le dictionnaire courant. Note: IDictionary est utilisé pour la compatibilité, 
        // mais l'objet sous-jacent ne doit pas être modifié par le consommateur.
        private readonly SimpleCachedData<Dictionary<int, IList<vue_judoka>>> _vjudokasEpreuveMap  = new SimpleCachedData<Dictionary<int, IList<vue_judoka>>>();


        public IDictionary<int, IList<vue_judoka>> vjudokas_epreuve { get { return _vjudokasEpreuveMap.ListCache; } }

        /// <summary>
        /// lecture des judoka
        /// </summary>
        /// <param name="element">element XML contenant les judoka</param>
        /// <param name="DC"></param>
        public void lecture_judokas(XElement element, JudoData DC)
        {
            ICollection<Judoka> judokasRecu = Judoka.LectureJudoka(element, null);

            // Genere les vues et le dictionnaire associés
            var (vueJudokas, judokasParEpreuve) = GenererVueJudokas(DC);

            // Swap atomique des donnees
            _judokasCache.UpdateSnapshot(judokasRecu, o => o.id);
            _vue_judokasCache.UpdateSnapshot(vueJudokas, o => o.ClefUnique);
            _vjudokasEpreuveMap.UpdateSnapshot(judokasParEpreuve);
        }


        /// <summary>
        /// Lecture des Judoka
        /// </summary>
        /// <param name="xelement">élément décrivant les Judoka</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Judoka</returns>

        public ICollection<Judoka> LectureJudoka(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Judoka.LectureJudoka(xelement, MI);
        }


        /// <summary>
        /// lecture des équipes
        /// </summary>
        /// <param name="element">element XML contenant les équipes</param>
        /// <param name="DC"></param>
        public void lecture_equipes(XElement element)
        {
            ICollection<Equipe> equipes = Equipe.LectureEquipes(element, null);
            _equipesCache.UpdateSnapshot(equipes, o => o.id);

        }

        public ICollection<Equipe> LectureEquipes(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Equipe.LectureEquipes(xelement, MI);
        }

        /// <summary>
        /// lecture des épreuves des judokas
        /// </summary>
        /// <param name="element">element XML contenant les  épreuves des judokas</param>
        /// <param name="DC"></param>
        public void lecture_epreuves_judokas(XElement element, JudoData DC)
        {
            ICollection<EpreuveJudoka> ejs = EpreuveJudoka.LectureEpreuveJudokas(element, null);
            _epreuvejudokasCache.UpdateSnapshot(ejs, o => o.id);

            // 3. Propagation de l'état aux Judokas (Optimisation)

            // Accès rapide à la liste actuelle des judokas (O(1))
            // On utilise la propriété locale 'Judokas' (qui pointe vers _judokasData.ListCache)
            var listeJudokas = this.Judokas;

            // ÉTAPE CLÉ : On indexe les judokas dans un Dictionnaire temporaire.
            // Cela coûte O(N) une seule fois, mais permet ensuite des recherches immédiates.
            // On utilise l'ID comme clé.
            var judokaMap = listeJudokas.ToDictionary(j => j.id);

            // On parcourt les EpreuvesJudokas (O(M))
            foreach (EpreuveJudoka ej in EJS)
            {
                // Recherche instantanée O(1) grâce au dictionnaire
                if (judokaMap.TryGetValue(ej.judoka, out Judoka j))
                {
                    // Mise à jour de l'état
                    // Note : Comme on modifie une propriété d'un objet existant, 
                    // assurez-vous que l'objet Judoka implémente INotifyPropertyChanged 
                    // si vous voulez que l'UI se rafraîchisse automatiquement.
                    j.etat = ej.etat;
                }
            }
        }


        /// <summary>
        /// Lecture des Epreuve des Judoka
        /// </summary>
        /// <param name="xelement">élément décrivant les Epreuve des Judoka</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Epreuve des Judoka</returns>

        public ICollection<EpreuveJudoka> LectureEpreuveJudokas(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return EpreuveJudoka.LectureEpreuveJudokas(xelement, MI); ;
        }


        /// <summary>
        /// Fabrique la liste des vues et le dictionnaire associé à partir des snapshots actuels.
        /// Ne modifie aucun état de la classe.
        /// </summary>
        /// <returns>Un Tuple contenant la liste plate et le dictionnaire construits.</returns>
        private (List<vue_judoka> VueJudoka, Dictionary<int, IList<vue_judoka>> JudokasParEpreuve) GenererVueJudokas(JudoData DC)
        {
            // 1. Capture des Snapshots locaux (Lecture O(1)) pour cohérence durant le traitement
            var sourceJudokas = Judokas;
            var sourceEJS = EJS;
            var sourceEpreuves = DC.Organisation.Epreuves;

            // 2. Construction de la Liste Plate (LINQ)
            var requeteVues =
                from j in sourceJudokas
                join ej in sourceEJS on j.id equals ej.judoka
                join ep in sourceEpreuves on ej.epreuve equals ep.id
                select new vue_judoka(j, ep, DC);

            var listeVues = requeteVues.ToList();

            // 3. Construction du Dictionnaire
            var nouveauDico = new Dictionary<int, IList<vue_judoka>>();
            nouveauDico[0] = new List<vue_judoka>();

            // 3.1 Initialisation des clés
            var epreuvesAInitialiser = DC.competition.IsEquipe()
                ? DC.Organisation.EpreuveEquipes.Cast<dynamic>()
                : DC.Organisation.Epreuves.Cast<dynamic>();

            foreach (var ep in epreuvesAInitialiser)
            {
                if (!nouveauDico.ContainsKey(ep.id))
                    nouveauDico[ep.id] = new List<vue_judoka>();
            }

            // 3.2 Remplissage
            foreach (var vue in listeVues)
            {
                int targetId = (vue.idepreuve_equipe != 0) ? vue.idepreuve_equipe : vue.idepreuve;
                if (nouveauDico.TryGetValue(targetId, out IList<vue_judoka> listeCible))
                {
                    listeCible.Add(vue);
                }
                else
                {
                    nouveauDico[0].Add(vue);
                }
            }

            // 4. Retour des objets construits
            return (listeVues, nouveauDico);
        }
    }
}
