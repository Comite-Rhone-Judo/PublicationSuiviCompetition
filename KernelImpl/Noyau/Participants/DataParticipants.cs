
using NLog;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Tools.Outils;

namespace KernelImpl.Noyau.Participants
{
    public class DataParticipants
    {
        private IList<Equipe> _equipes = new List<Equipe>();
        public IList<Equipe> Equipes { get { return _equipes; } }

        private IList<Judoka> _judokas = new List<Judoka>();
        public IList<Judoka> Judokas { get { return _judokas; } }


        private IList<EpreuveJudoka> _epreuvejudokas = new List<EpreuveJudoka>();
        public IList<EpreuveJudoka> EJS { get { return _epreuvejudokas; } }


        public IEnumerable<Judoka> GetJudokaEpreuve(int epreuve)
        {
            IEnumerable<int> judokas = _epreuvejudokas.Where(o => o.epreuve == epreuve).Select(o => o.judoka).Distinct();
            return _judokas.Where(o => judokas.Contains(o.id));
        }



        private IList<vue_judoka> _vue_judokas = new ObservableCollection<vue_judoka>();
        public IList<vue_judoka> vjudokas { get { return _vue_judokas; } }

        private IDictionary<int, IList<vue_judoka>> _vjudokas_epreuve = new Dictionary<int, IList<vue_judoka>>();
        public IDictionary<int, IList<vue_judoka>> vjudokas_epreuve { get { return _vjudokas_epreuve; } }



        public void clear_participants()
        {
            _equipes.Clear();
            _judokas.Clear();
            _epreuvejudokas.Clear();
            vjudokas.Clear();
        }


        /// <summary>
        /// lecture des judoka
        /// </summary>
        /// <param name="element">element XML contenant les judoka</param>
        /// <param name="DC"></param>
        public void lecture_judokas(XElement element, JudoData DC)
        {
            ICollection<Judoka> judokasRecu = Judoka.LectureJudoka(element, null);

            using (TimedLock.Lock((_judokas as ICollection).SyncRoot))
            {
                // TODO Ne serait-il pas plus simple de simplement vider tous les judokas et de remettre ceux recu a la place ??
                // Vide la collection
                _judokas.Clear();

                // Ajoute les judokas recus
                foreach (Judoka jr in judokasRecu)
                {
                    // Ajoute le judoka recu (CREATE ou UPDATE)
                    _judokas.Add(jr);
                }

                /*
                // Ajout des nouveaux et mise a jour
                foreach (Judoka jr in judokasRecu)
                {
                    Judoka p = _judokas.FirstOrDefault(o => o.id == jr.id);
                    //i_vue_judoka vj = new vue_judoka(judoka, DC)
                    if (p != null)
                    {
                        // Supprime le judoka pour le remplacer par le nouveau recu
                        _judokas.Remove(p);
                    }
                    // Ajoute le judoka recu (CREATE ou UPDATE)
                    _judokas.Add(jr);
                }

                // Suppression de ceux absents
                foreach (Judoka ji in _judokas)
                {
                    Judoka p = judokasRecu.FirstOrDefault(o => o.id == ji.id);

                    // Si aucun judoka trouve c'est qu'il n'est plus dans la liste recue donc on supprime
                    if (p == null)
                    {
                        _judokas.Remove(ji);
                    }
                }
                */

                // Met a jour les vues associees
                lecture_vue_judokas(DC);
            }        
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
            using (TimedLock.Lock((_equipes as ICollection).SyncRoot))
            {
                _equipes.Clear();
                foreach (Equipe equipe in equipes)
                {
                    _equipes.Add(equipe);
                }

                /*
                //Ajout des nouveaux
                foreach (Equipe equipe in equipes)
                {
                    Equipe p = _equipes.FirstOrDefault(o => o.id == equipe.id);
                    if (p != null)
                    {
                        _equipes.Remove(p);
                    }
                    _equipes.Add(equipe);
                }
                */
            }
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

            using (TimedLock.Lock((_epreuvejudokas as ICollection).SyncRoot))
            {
                _epreuvejudokas.Clear();

                foreach (EpreuveJudoka ej in ejs)
                {
                    _epreuvejudokas.Add(ej);
                }

                /*
                //Ajout des nouveaux
                foreach (EpreuveJudoka ej in ejs)
                {
                    EpreuveJudoka p = _epreuvejudokas.FirstOrDefault(o => o.id == ej.id);
                    if (p != null)
                    {
                        _epreuvejudokas.Remove(p);
                    }
                    _epreuvejudokas.Add(ej);
                }
                */

                foreach (EpreuveJudoka ej in _epreuvejudokas)
                {
                    Judoka j = DC.Participants.Judokas.FirstOrDefault(o => o.id == ej.judoka);
                    if (j != null)
                    {
                        j.etat = ej.etat;
                    }
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
        /// lecture des judokas
        /// </summary>
        /// <param name="element">element XML contenant les judokas</param>
        /// <param name="DC"></param>
        public void lecture_vue_judokas(/*XElement element, bool suppression,*/ JudoData DC/*, ObservableCollection<i_vue_judoka> ItemsSource*/)
        {
            // TODO idem, peut etre plus simple de vider le dictionnaire et le reconstruire si on sait que l'on recoit tous a chaque fois

            using (TimedLock.Lock((_vjudokas_epreuve as ICollection).SyncRoot))
            {
                _vjudokas_epreuve.Clear();

                // Rajoute le zero si besoin
                if (!_vjudokas_epreuve.ContainsKey(0))
                {
                    _vjudokas_epreuve.Add(0, new List<vue_judoka>());
                }

                if (DC.competition.IsEquipe())
                {
                    foreach (Organisation.Epreuve_Equipe ep in DC.Organisation.EpreuveEquipes)
                    {
                        if (!_vjudokas_epreuve.ContainsKey(ep.id))
                        {
                            _vjudokas_epreuve.Add(ep.id, new List<vue_judoka>());
                        }
                    }
                }
                else
                {
                    foreach (Organisation.Epreuve ep in DC.Organisation.Epreuves)
                    {
                        if (!_vjudokas_epreuve.ContainsKey(ep.id))
                        {
                            _vjudokas_epreuve.Add(ep.id, new List<vue_judoka>());
                        }
                    }
                }
            }

            //ICollection<IJudoka> judokas = Judoka.LectureJudoka(element, null);

            //Ajout des nouveaux
            //int count = ItemsSource == null ? 0 : ItemsSource.Count;
            using (TimedLock.Lock((_vue_judokas as ICollection).SyncRoot))
            {
                // TODO A voir s'il n'est pas plus efficace de vider la liste des vues directement et d'ajouter les donnees recues a la place

                _vue_judokas.Clear();

                foreach (Judoka judoka in _judokas)
                {
                    List<EpreuveJudoka> epreuvesJudoka = DC.Participants.EJS.Where(o => o.judoka == judoka.id).ToList();
                    foreach (EpreuveJudoka epreuve_judoka in epreuvesJudoka)
                    {
                        Organisation.Epreuve epreuve = DC.Organisation.Epreuves.FirstOrDefault(o => o.id == epreuve_judoka.epreuve);
                        if (epreuve != null)
                        {
                            // Nouvelle vue Judoka
                            vue_judoka vj = new vue_judoka(judoka, epreuve, DC);
                            _vue_judokas.Add(vj);

                            // Met a jour le dictionnaire des vjudokas par epreuve
                            foreach (int epreuveid in _vjudokas_epreuve.Keys)
                            {
                                // On retire toutes les vjudoka pour le judoka en cours
                                vue_judoka p2 = _vjudokas_epreuve[epreuveid].FirstOrDefault(o => o.id == judoka.id);
                                if (p2 != null)
                                {
                                    _vjudokas_epreuve[epreuveid].Remove(p2);
                                }
                            }

                            // Ajoute la vjudoka en cours
                            int id = vj.idepreuve_equipe != 0 ? vj.idepreuve_equipe : vj.idepreuve;
                            _vjudokas_epreuve[id].Add(vj);
                        }
                        else
                        {
                            // Epreuve inconnue ... pas normal, on trace en debug au cas ou
                            LogTools.Logger.Debug($"epreuve Id={epreuve_judoka.epreuve} manquante, referencee pour judoka '{epreuve_judoka.judoka}'");
                        }
                    }

                    /*
                    foreach (Judoka judoka in _judokas)
                    {
                        List<vue_judoka> vues_judoka = _vue_judokas.Where(o => o.id == judoka.id).ToList();
                        List<EpreuveJudoka> epreuvesJudoka = DC.Participants.EJS.Where(o => o.judoka == judoka.id).ToList();

                        foreach (EpreuveJudoka epreuve_judoka in epreuvesJudoka)
                        {
                            Organisation.Epreuve epreuve = DC.Organisation.Epreuves.FirstOrDefault(o => o.id == epreuve_judoka.epreuve);
                            if (epreuve != null)
                            {
                                vue_judoka vj = new vue_judoka(judoka, epreuve, DC);

                                vue_judoka p = _vue_judokas.FirstOrDefault(o => o.id == judoka.id && o.idepreuve == epreuve.id);                            
                                if (p != null)
                                {
                                    _vue_judokas.Remove(p);
                                }
                                _vue_judokas.Add(vj);

                                // Met a jour le dictionnaire des vjudokas par epreuve
                                foreach (int epreuveid in _vjudokas_epreuve.Keys)
                                {
                                    // On retire toutes les vjudoka pour le judoka en cours
                                    vue_judoka p2 = _vjudokas_epreuve[epreuveid].FirstOrDefault(o => o.id == judoka.id);
                                    if (p2 != null)
                                    {
                                        _vjudokas_epreuve[epreuveid].Remove(p2);
                                    }
                                }

                                // Ajoute la vjudoka en cours
                                int id = vj.idepreuve_equipe != 0 ? vj.idepreuve_equipe : vj.idepreuve;
                                _vjudokas_epreuve[id].Add(vj);
                            }
                            else
                            {
                                // Epreuve inconnue ... pas normal, on trace en debug au cas ou
                                LogTools.Logger.Debug($"epreuve Id={epreuve_judoka.epreuve} manquante, referencee pour judoka '{epreuve_judoka.judoka}'");
                            }
                        }
                    */
                }
            }
        }
    }
}
