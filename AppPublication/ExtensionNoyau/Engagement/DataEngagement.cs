using FranceJudo.Core.Logging;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Organisation;
using FranceJudo.Metier.Noyau.Participants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppPublication.ExtensionNoyau.Engagement
{
    public class DataEngagement : IEngagementData
    {
        // 1. Les données deviennent ReadOnly. Une fois calculées, elles sont figées.
        private readonly IReadOnlyList<GroupeEngagements> _groupesEngages;
        private readonly IReadOnlyDictionary<int, List<EchelonEnum>> _typesGroupes;

        public IReadOnlyList<GroupeEngagements> GroupesEngages => _groupesEngages;
        public IReadOnlyDictionary<int, List<EchelonEnum>> TypesGroupes => _typesGroupes;

        // 2. LE CONSTRUCTEUR (Agit comme votre Factory)
        // Il est appelé uniquement lorsque le Lazy<DataEngagement>.Value est demandé.
        public DataEngagement(IJudoData snapshot)
        {
            _typesGroupes = BuildTypesGroupes(snapshot);
            _groupesEngages = BuildGroupesEngagements(snapshot);
        }

        // 3. Les méthodes de calcul (privées) retournent maintenant des dictionnaires/listes
        private IReadOnlyDictionary<int, List<EchelonEnum>> BuildTypesGroupes(IJudoData dataContext)
        {
            var dict = new Dictionary<int, List<EchelonEnum>>();

            foreach (ICompetition comp in dataContext.Organisation.Competitions)
            {
                List<EchelonEnum> listEchelon = new List<EchelonEnum>();
                listEchelon.Add(EchelonEnum.Aucun);

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

        private IReadOnlyList<GroupeEngagements> BuildGroupesEngagements(IJudoData DC)
        {
            var listGroupes = new List<GroupeEngagements>();
            IList<ICompetition> competitions = DC.Organisation.Competitions.ToList();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            foreach (ICompetition competition in competitions)
            {
                if (competition.IsShiai() || competition.IsIndividuelle())
                {
                    foreach (EpreuveSexeEnum s in Enum.GetValues(typeof(EpreuveSexeEnum)))
                    {
                        EpreuveSexe sexe = new EpreuveSexe(s);
                        IList<IEpreuve> epreuvesSexe = DC.Organisation.Epreuves.Where(ep => ep.competition == competition.id && ep.sexeEnum.Enum == s).ToList();

                        IList<IVueJudoka> judokasParticipants = DC.Participants.Vuejudokas
                            .Join(epreuvesSexe, vj => vj.idepreuve, ep => ep.id, (vj, ep) => vj)
                            .Distinct(new VueJudokaEqualityComparer())
                            .ToList();

                        Dictionary<EchelonEnum, List<string>> dictEntites = new Dictionary<EchelonEnum, List<string>>();

                        switch (competition.niveau)
                        {
                            case (int)EchelonEnum.Club:
                                dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                break;
                            case (int)EchelonEnum.Departement:
                                dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                dictEntites.Add(EchelonEnum.Departement, judokasParticipants.Select(o => o.comite).Distinct().ToList());
                                break;
                            case (int)EchelonEnum.Ligue:
                                dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                dictEntites.Add(EchelonEnum.Departement, judokasParticipants.Select(o => o.comite).Distinct().ToList());
                                dictEntites.Add(EchelonEnum.Ligue, judokasParticipants.Select(o => o.ligue).Distinct().ToList());
                                break;
                            case (int)EchelonEnum.National:
                            case (int)EchelonEnum.International:
                                dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                dictEntites.Add(EchelonEnum.Departement, judokasParticipants.Select(o => o.comite).Distinct().ToList());
                                dictEntites.Add(EchelonEnum.Ligue, judokasParticipants.Select(o => o.ligue).Distinct().ToList());
                                dictEntites.Add(EchelonEnum.National, judokasParticipants.Select(o => o.pays.ToString()).Distinct().ToList());
                                break;
                            default:
                                LogTools.Logger?.Error("Niveau de competition inconnu : {0}. Utilisation du niveau club par defaut", competition.niveau);
                                dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                break;
                        }

                        foreach (EchelonEnum typeEntite in dictEntites.Keys)
                        {
                            List<string> entites = dictEntites[typeEntite];
                            IEnumerable<GroupeEngagements> groupesEntites = entites.Select(o => new GroupeEngagements(competition.id, sexe, (int)typeEntite, o));

                            // Optimisation : AddRange est plus rapide que de faire des Concat.ToList() en boucle
                            listGroupes.AddRange(groupesEntites);
                        }

                        foreach (char c in alphabet)
                        {
                            int nj = judokasParticipants.Count(o => Char.ToUpper(o.nom.First()) == c);
                            if (nj > 0)
                            {
                                listGroupes.Add(new GroupeEngagements(competition.id, sexe, (int)EchelonEnum.Aucun, c.ToString()));
                            }
                        }
                    }
                }
            }

            return listGroupes;
        }
    }
}