using KernelImpl;
using KernelImpl.Noyau.Deroulement;
using KernelImpl.Noyau.Organisation;
using KernelImpl.Noyau.Participants;
using KernelImpl.Noyau.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace AppPublication.ExtensionNoyau.Deroulement
{
    public class DataDeroulement
    {
        private List<GroupeEngagements> _groupesEngages = new List<GroupeEngagements>();

        /// <summary>
        /// Contient la liste des groupes de engages (par rapport a la derniere generation Par GetGroupesEngagements) par echelon
        /// </summary>
        public List<GroupeEngagements> GroupesEngages
        {
            get
            {
                return _groupesEngages;
            }
        }

        public void SyncAll(IJudoData DC)
        {
            GetGroupesEngagements(DC);
        }

        /// <summary>
        /// Retourne les types de groupement pour une competition donnee (notamment si le niveau de competition est inconnu)
        /// </summary>
        /// <param name="c">La competition</param>
        /// <returns>Le typ de groupement</returns>
        public static List<EchelonEnum> GetTypeGroupe(Competition c)
        {
            List<EchelonEnum> output = new List<EchelonEnum>();

            // on a toujours au moins par Nom
            output.Add(EchelonEnum.Aucun);

            // ajoute les niveaux en fonction de la celui de la competition
            switch (c.niveau)
            {
                case (int)EchelonEnum.Club:
                    {
                        output.Add(EchelonEnum.Club);
                        break;
                    }
                case (int)EchelonEnum.Departement:
                    {
                        output.Add(EchelonEnum.Club);
                        output.Add(EchelonEnum.Departement);
                        break;
                    }
                case (int)EchelonEnum.Ligue:
                    {
                        output.Add(EchelonEnum.Club);
                        output.Add(EchelonEnum.Departement);
                        output.Add(EchelonEnum.Ligue);
                        break;
                    }
                case (int)EchelonEnum.National:
                case (int)EchelonEnum.International:
                    {
                        output.Add(EchelonEnum.Club);
                        output.Add(EchelonEnum.Departement);
                        output.Add(EchelonEnum.Ligue);
                        output.Add(EchelonEnum.National);
                        break;
                    }
                default:
                    {
                        // Pas de niveau connu, on ajoute le niveau le plus bas (club)
                        output.Add(EchelonEnum.Club);
                        break;
                    }
            }

            return output;  
        }

        /// <summary>
        /// Genere la liste des groupes pour le niveau de la competition
        /// </summary>
        /// <param name="niveau"></param>
        /// <returns></returns>
        public void GetGroupesEngagements(IJudoData DC)
        {
            // Vide la precedente liste
            _groupesEngages.Clear();

            IList<Competition> competitions = DC.Organisation.Competitions.ToList();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            foreach (Competition competition in competitions)
            {
                // Pas de groupement en equipe
                if (competition.IsShiai() || competition.IsIndividuelle())
                {
                    // Genere les groupements
                    foreach (EpreuveSexeEnum s in Enum.GetValues(typeof(EpreuveSexeEnum)))
                    {
                        EpreuveSexe sexe = new EpreuveSexe(s);

                        // Recupere les epreuves de la competition pour ce sexe
                        IList<Epreuve> epreuvesSexe = DC.Organisation.Epreuves.Where(ep => ep.competition == competition.id && ep.sexeEnum.Enum == s).ToList();

                        // Recupere tous les judokas participant a une des epreuves (présents ou non)
                        // on s'assure de ne pas avoir de doublon avec Distinct
                        IList<vue_judoka> judokasParticipants = DC.Participants.Vuejudokas.Join(epreuvesSexe, vj => vj.idepreuve, ep => ep.id, (vj, ep) => vj).Distinct(new VueJudokaEqualityComparer()).ToList();

                        // Groupement par entite
                        Dictionary<EchelonEnum, List<string>> dictEntites = new Dictionary<EchelonEnum, List<string>>();

                        switch (competition.niveau)
                        {
                            case (int)EchelonEnum.Club:
                                {
                                    dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                    break;
                                }
                            case (int)EchelonEnum.Departement:
                                {
                                    dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                    dictEntites.Add(EchelonEnum.Departement, judokasParticipants.Select(o => o.comite).Distinct().ToList());
                                    break;
                                }
                            case (int)EchelonEnum.Ligue:
                                {
                                    dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                    dictEntites.Add(EchelonEnum.Departement, judokasParticipants.Select(o => o.comite).Distinct().ToList());
                                    dictEntites.Add(EchelonEnum.Ligue, judokasParticipants.Select(o => o.ligue).Distinct().ToList());
                                    break;
                                }
                            case (int)EchelonEnum.National:
                            case (int)EchelonEnum.International:
                                {
                                    dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                    dictEntites.Add(EchelonEnum.Departement, judokasParticipants.Select(o => o.comite).Distinct().ToList());
                                    dictEntites.Add(EchelonEnum.Ligue, judokasParticipants.Select(o => o.ligue).Distinct().ToList());
                                    dictEntites.Add(EchelonEnum.National, judokasParticipants.Select(o => o.pays.ToString()).Distinct().ToList());
                                    break;
                                }
                            default:
                                {
                                    LogTools.Logger.Error("Niveau de competition inconnu : {0}. Utilisation du niveau club par defaut", competition.niveau);
                                    dictEntites.Add(EchelonEnum.Club, judokasParticipants.Select(o => o.club).Distinct().ToList());
                                    break;
                                }
                        }
                        foreach (EchelonEnum typeEntite in dictEntites.Keys)
                        {
                            // Recupere la liste des entites pour le type
                            List<string> entites = dictEntites[typeEntite];

                            // Transforme les entites en groupe et ajoute a la liste generale
                            IEnumerable<GroupeEngagements> groupesEntites = entites.Select(o => { return new GroupeEngagements(competition.id, sexe, (int)typeEntite, o); });
                            _groupesEngages = _groupesEngages.Concat(groupesEntites).ToList();
                        }

                        // Ajoute le Groupement par nom qui est toujours present
                        foreach (char c in alphabet)
                        {  
                            int nj = judokasParticipants.Count(o => Char.ToUpper(o.nom.First()) == c);

                            if (nj > 0)
                            {
                                _groupesEngages.Add(new GroupeEngagements(competition.id, sexe, (int)EchelonEnum.Aucun, c.ToString()));
                            }
                        }
                    }
                }
            }
        }
    }
}
