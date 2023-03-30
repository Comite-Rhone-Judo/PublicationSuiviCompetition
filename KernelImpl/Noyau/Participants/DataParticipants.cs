
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
        public void lecture_judokas(XElement element, /*bool suppression,*/ JudoData DC/*, ObservableCollection<i_vue_judoka> ItemsSource*/)
        {
            ICollection<Judoka> judokas = Judoka.LectureJudoka(element, null);

            //Ajout des nouveaux
            //int count = ItemsSource == null ? 0 : ItemsSource.Count;
            using (TimedLock.Lock((_judokas as ICollection).SyncRoot))
            {
                foreach (Judoka judoka in judokas)
                {
                    Judoka p = _judokas.FirstOrDefault(o => o.id == judoka.id);
                    //i_vue_judoka vj = new vue_judoka(judoka, DC);
                    if (p != null)
                    {
                        _judokas.Remove(p);
                    }
                    _judokas.Add(judoka);
                    //else
                    //{
                    //    p.categorie = vj.categorie;
                    //    p.ceinture = vj.ceinture;
                    //    p.club = vj.club;
                    //    p.datePesee = vj.datePesee;
                    //    p.etat = vj.etat;
                    //    p.id = vj.id;
                    //    p.licence = vj.licence;
                    //    p.modeControle = vj.modeControle;
                    //    p.modePesee = vj.modePesee;
                    //    p.modification = vj.modification;
                    //    p.naissance = vj.naissance;
                    //    p.nom = vj.nom;
                    //    p.passeport = vj.passeport;
                    //    p.pays = vj.pays;
                    //    p.poids = (int)vj.poids;
                    //    p.poidsKg = vj.poidsKg;
                    //    p.poidsMesure = (int)vj.poidsMesure;
                    //    p.prenom = vj.prenom;
                    //    p.present = vj.present;
                    //    p.remoteID = vj.remoteId;
                    //    p.sexe = vj.sexe;
                    //}
                }

                //if (suppression)
                //{

                //    //Suppression de ceux qui ont été supprimer
                //    ICollection<IJudoka> deleted_c = new List<IJudoka>();
                //    foreach (IJudoka judoka in _judokas)
                //    {
                //        IJudoka p = judokas.FirstOrDefault(o => o.id == judoka.id);
                //        if (p == null)
                //        {
                //            deleted_c.Add(judoka);
                //            //if (ItemsSource != null)
                //            //{
                //            //    i_vue_judoka vj = ItemsSource.FirstOrDefault(o => o.id == judoka.id);
                //            //    if (vj != null)
                //            //    {
                //            //        ItemsSource.Remove(vj);
                //            //        _vue_judokas.Remove(_vue_judokas.FirstOrDefault(o=>o.id == judoka.id));
                //            //    }
                //            //}
                //        }
                //    }
                //    foreach (IJudoka judoka in deleted_c)
                //    {
                //        _judokas.Remove(judoka);
                //    }
                //}
                lecture_vue_judokas(DC);
            }

            //if (ItemsSource != null && (count != ItemsSource.Count || ItemsSource.Count == 0))
            //{
            //    return true;
            //    //Outils.GetCommissairWindow().ItemsSource = collection;
            //}


            //return false;            
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
            using (TimedLock.Lock((_vjudokas_epreuve as ICollection).SyncRoot))
            {
                if (!_vjudokas_epreuve.ContainsKey(0))
                {
                    _vjudokas_epreuve.Add(0, new List<vue_judoka>());
                }

                if (DC.competition.IsEquipe())
                {
                    foreach(Organisation.Epreuve_Equipe ep in DC.Organisation.EpreuveEquipes)
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
                foreach (Judoka judoka in _judokas)
                {
                    vue_judoka p = _vue_judokas.FirstOrDefault(o => o.id == judoka.id);
                    vue_judoka vj = new vue_judoka(judoka, DC);
                    if (p != null)
                    {
                        _vue_judokas.Remove(p);
                    }
                    _vue_judokas.Add(vj);

                    foreach (int epreuveid in _vjudokas_epreuve.Keys)
                    {
                        vue_judoka p2 = _vjudokas_epreuve[epreuveid].FirstOrDefault(o => o.id == judoka.id);
                        if (p2 != null)
                        {
                            _vjudokas_epreuve[epreuveid].Remove(p2);
                        }
                    }

                    int id = vj.idepreuve_equipe != 0 ? vj.idepreuve_equipe : vj.idepreuve;
                    _vjudokas_epreuve[id].Add(vj);
                }
            }
        }
    }
}
