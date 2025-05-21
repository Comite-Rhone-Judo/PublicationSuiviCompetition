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
        // TODO verifier & clean-up des fichiers bak qui traines
        private IList<GroupeParticipants> _groupesParticipants = new List<GroupeParticipants>();

        /// <summary>
        /// Contient la liste des groupes de participant (par rapport a la derniere generation Par GetGroupesParticipants)
        /// </summary>
        public IList<GroupeParticipants> GroupesParticipants
        {
            get
            {
                return _groupesParticipants;
            }
        }

        public void SyncAll(JudoData DC)
        {
            GetGroupesParticipant(DC);
        }

        /// <summary>
        /// Retourne le type de groupement pour une competition donnee (notamment si le niveau de competition est inconnu)
        /// </summary>
        /// <param name="c">La competition</param>
        /// <returns>Le typ de groupement</returns>
        public static int GetTypeGroupe(Competition c)
        {
            int type = (int) EchelonEnum.Club;

            switch (c.niveau)
            {
                case (int)EchelonEnum.Club:
                case (int)EchelonEnum.Departement:
                case (int)EchelonEnum.Ligue:
                case (int)EchelonEnum.National:
                case (int)EchelonEnum.International:
                    {
                        type = c.niveau;
                        break;
                    }
                default:
                    {
                        type = (int) EchelonEnum.Club;
                        break;
                    }
            }

            return type;  
        }

        /// <summary>
        /// Genere la liste des groupes pour un niveau donne
        /// </summary>
        /// <param name="niveau"></param>
        /// <returns></returns>
        public void GetGroupesParticipant(JudoData DC)
        {
            // Vide la precedente liste
            _groupesParticipants.Clear();

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
                        IList<vue_judoka> judokasParticipants = DC.Participants.vjudokas.Join(epreuvesSexe, vj => vj.idepreuve, ep => ep.id, (vj, ep) => vj).ToList();

                        // Groupement par entite
                        IList<string> listEntite = null;
                        int type = competition.niveau;

                        switch (competition.niveau)
                        {
                            case (int)EchelonEnum.Club:
                                {
                                    listEntite = judokasParticipants.Select(o => o.club).Distinct().ToList();
                                    break;
                                }
                            case (int)EchelonEnum.Departement:
                                {
                                    listEntite = judokasParticipants.Select(o => o.comite).Distinct().ToList();
                                    break;
                                }
                            case (int)EchelonEnum.Ligue:
                                {
                                    listEntite = judokasParticipants.Select(o => o.ligue).Distinct().ToList();
                                    break;
                                }
                            case (int)EchelonEnum.National:
                            case (int)EchelonEnum.International:
                                {
                                    listEntite = judokasParticipants.Select(o => o.pays.ToString()).Distinct().ToList();
                                    break;
                                }
                            default:
                                {
                                    LogTools.Logger.Error("Niveau de competition inconnu : {0}. Utilisation du niveau club par defaut", competition.niveau);
                                    listEntite = judokasParticipants.Select(o => o.club).Distinct().ToList();
                                    type = (int) EchelonEnum.Club;    // Le niveau de competition est inconnu, on prend le plus bas par defaut
                                    break;
                                }
                        }
                        foreach (string entite in listEntite)
                        {
                            GroupeParticipants grp = new GroupeParticipants();
                            grp.Competition = competition.id;
                            grp.Sexe = sexe;
                            grp.Type = type;
                            grp.Entite = entite;
                            _groupesParticipants.Add(grp);
                        }

                        // Groupement par nom
                        foreach (char c in alphabet)
                        {
                            int nj = judokasParticipants.Count(o => Char.ToUpper(o.nom.First()) == c);

                            if (nj > 0)
                            {
                                GroupeParticipants grp = new GroupeParticipants();
                                grp.Competition = competition.id;
                                grp.Sexe = sexe;
                                grp.Type = (int) EchelonEnum.Aucun;
                                grp.Entite = c.ToString();
                                _groupesParticipants.Add(grp);
                            }
                        }
                    }
                }
            }
        }
    }
}
