using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FranceJudo.Metier.ExtensionNoyau.Engagement
{
    public class DataEngagement : IDataEngagement
    {
        private readonly List<GroupeEngagements> _groupesEngages;
        private readonly Dictionary<int, List<EchelonEnum>> _typesGroupes;

        public IReadOnlyList<GroupeEngagements> GroupesEngages => _groupesEngages;
        public IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes => _typesGroupes;

        public DataEngagement(IJudoData snapshot)
        {
            _typesGroupes = BuildTypesGroupes(snapshot);
            _groupesEngages = BuildGroupesEngagements(snapshot);
        }

        private Dictionary<int, List<EchelonEnum>> BuildTypesGroupes(IJudoData dataContext)
        {
            var dict = new Dictionary<int, List<EchelonEnum>>();

            // Ajout du ToList() ici par sécurité
            foreach (ICompetition comp in dataContext.Organisation.Competitions.ToList())
            {
                List<EchelonEnum> listEchelon = new List<EchelonEnum> { EchelonEnum.Aucun };

                switch (comp.niveau)
                {
                    case (int)EchelonEnum.Club:
                        listEchelon.Add(EchelonEnum.Club);
                        break;
                    case (int)EchelonEnum.Departement:
                        listEchelon.Add(EchelonEnum.Club);
                        listEchelon.Add(EchelonEnum.Departement);
                        break;
                    case (int)EchelonEnum.Ligue:
                        listEchelon.Add(EchelonEnum.Club);
                        listEchelon.Add(EchelonEnum.Departement);
                        listEchelon.Add(EchelonEnum.Ligue);
                        break;
                        // TODO En international, on n'est pas sur que les informations Dep & Ligue existe on ne va garder que Club et Pays 
                    case (int)EchelonEnum.National:
                    case (int)EchelonEnum.International:
                        listEchelon.Add(EchelonEnum.Club);
                        listEchelon.Add(EchelonEnum.Departement);
                        listEchelon.Add(EchelonEnum.Ligue);
                        listEchelon.Add(EchelonEnum.National);
                        break;
                    default:
                        listEchelon.Add(EchelonEnum.Club);
                        break;
                }
                dict.Add(comp.id, listEchelon);
            }
            return dict;
        }

        private List<GroupeEngagements> BuildGroupesEngagements(IJudoData DC)
        {
            var groupesUniques = new HashSet<GroupeEngagements>();

            // Le ToList() originel, indispensable
            IList<ICompetition> competitions = DC.Organisation.Competitions.ToList();

            foreach (ICompetition competition in competitions)
            {
                if (competition.IsShiai() || competition.IsIndividuelle())
                {
                    if (!_typesGroupes.TryGetValue(competition.id, out var echelonsCibles)) continue;

                    // Restauration de la boucle des sexes de votre ancien code
                        foreach (EpreuveSexeEnum s in Enum.GetValues(typeof(EpreuveSexeEnum)))
                        {
                        // 1er ToList() crucial : Fige les épreuves, relâche l'UI
                        IList<IEpreuve> epreuvesSexe = DC.Organisation.Epreuves
                            .Where(ep => ep.competition == competition.id && ep.sexeEnum.Enum == s)
                            .ToList();

                        // 2ème ToList() crucial : Restauration de la jointure qui charge les judokas en RAM 
                        IList<IVueJudoka> judokasParticipants = DC.Participants.Vuejudokas
                            .Join(epreuvesSexe, vj => vj.idepreuve, ep => ep.id, (vj, ep) => vj)
                            .Distinct(new VueJudokaEqualityComparer())
                            .ToList();

                        // L'extraction est maintenant 100% sécurisée sur notre copie de données locale
                        foreach (var judoka in judokasParticipants)
                        {
                            foreach (var groupe in GetGroupesCascadePourParticipant(judoka, echelonsCibles))
                            {
                                groupesUniques.Add(groupe);
                            }
                        }
                    }
                }
            }

            return groupesUniques.ToList();
        }

        // --- LA CASCADE CORRIGÉE ---
        private IEnumerable<GroupeEngagements> GetGroupesCascadePourParticipant(IVueJudoka p, List<EchelonEnum> echelonsCibles)
        {
            var groupes = new List<GroupeEngagements>();

            if (p == null) return groupes;

            // ATTENTION : L'ordre des paramètres est (Compétition, Sexe, TYPE, ENTITÉ)
            // J'avais inversé les deux derniers paramètres dans mon code précédent !

            if (echelonsCibles.Contains(EchelonEnum.Aucun) && !string.IsNullOrWhiteSpace(p.nom))
            {
                string premiereLettre = p.nom.Trim().Substring(0, 1).ToUpper();
                groupes.Add(new GroupeEngagements(p.idcompet, p.sexeEnum, premiereLettre, EchelonEnum.Aucun));
            }

            if (echelonsCibles.Contains(EchelonEnum.National) && p.pays != 0)
            {
                groupes.Add(new GroupeEngagements(p.idcompet, p.sexeEnum, p.pays.ToString(), EchelonEnum.National));
            }

            if (echelonsCibles.Contains(EchelonEnum.Ligue) && !string.IsNullOrEmpty(p.ligue))
            {
                groupes.Add(new GroupeEngagements(p.idcompet, p.sexeEnum, p.ligue, EchelonEnum.Ligue));
            }

            if (echelonsCibles.Contains(EchelonEnum.Departement) && !string.IsNullOrEmpty(p.comite))
            {
                groupes.Add(new GroupeEngagements(p.idcompet, p.sexeEnum, p.comite, EchelonEnum.Departement));
            }

            if (echelonsCibles.Contains(EchelonEnum.Club) && !string.IsNullOrEmpty(p.club))
            {
                groupes.Add(new GroupeEngagements(p.idcompet, p.sexeEnum, p.club, EchelonEnum.Club));
            }

            return groupes;
        }
    }
}