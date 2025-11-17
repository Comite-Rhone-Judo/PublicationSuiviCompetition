
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tools.Export;
using Tools.Outils;

namespace KernelImpl.Noyau.Organisation
{
    public class DataOrganisation
    {
        private IList<Competition> _competitions = new List<Competition>();
        public IList<Competition> Competitions { get { return _competitions; } }

        private IList<Epreuve> _epreuves = new List<Epreuve>();
        public IList<Epreuve> Epreuves { get { return _epreuves; } }

        private IList<Epreuve_Equipe> _epreuve_equipes = new List<Epreuve_Equipe>();
        public IList<Epreuve_Equipe> EpreuveEquipes { get { return _epreuve_equipes; } }


        private IList<vue_epreuve_equipe> _vepreuves_equipe = new List<vue_epreuve_equipe>();
        public IList<vue_epreuve_equipe> vepreuves_equipe { get { return _vepreuves_equipe; } }

        private IList<vue_epreuve> _vepreuves = new List<vue_epreuve>();
        public IList<vue_epreuve> vepreuves { get { return _vepreuves; } }

        /// <summary>
        /// lecture des compétitions
        /// </summary>
        /// <param name="element">element XML contenant les compétitions</param>
        /// <param name="DC"></param>
        public void lecture_competitions(XElement element, JudoData DC)
        {
            ICollection<Competition> competitions = Competition.LectureCompetitions(element, null);
            using (TimedLock.Lock((_competitions as ICollection).SyncRoot))
            {
                _competitions.Clear();
                //Ajout des nouveaux
                foreach (Competition competition in competitions)
                {
                    Competition p = _competitions.FirstOrDefault(o => o.id == competition.id);
                    /*
                    if (p != null)
                    {
                        _competitions.Remove(p);
                    }
                    */
                    _competitions.Add(competition);
                }

                DC.competition = _competitions.FirstOrDefault();
                DC.competitions = _competitions.ToList();
                ExportTools.default_competition = DC.competition.remoteId;
            }
        }


        /// <summary>
        /// lecture des épreuves (équipe)
        /// </summary>
        /// <param name="element">element XML contenant les épreuves (équipe)</param>
        /// <param name="DC"></param>
        public void lecture_epreuves_equipe(XElement element, JudoData DC)
        {
            ICollection<Epreuve_Equipe> epreuves = Epreuve_Equipe.LectureEpreuveEquipes(element, null);
            using (TimedLock.Lock((_epreuve_equipes as ICollection).SyncRoot))
            {
                _epreuve_equipes.Clear();
                foreach (Epreuve_Equipe epreuve in epreuves)
                {
                    Epreuve_Equipe p = _epreuve_equipes.FirstOrDefault(o => o.id == epreuve.id);
                    /*
                    if (p != null)
                    {
                        _epreuve_equipes.Remove(p);
                    }
                    */
                    _epreuve_equipes.Add(epreuve);
                }

                lecture_vue_epreuve_equipe(DC);
            }
        }
        public ICollection<Epreuve_Equipe> LectureEpreuveEquipes(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Epreuve_Equipe.LectureEpreuveEquipes(xelement, MI);
        }


        /// <summary>
        /// lecture des épreuves
        /// </summary>
        /// <param name="element">element XML contenant les épreuves</param>
        /// <param name="DC"></param>
        public void lecture_epreuves(XElement element, JudoData DC)
        {
            ICollection<Epreuve> epreuves = Epreuve.LectureEpreuves(element, null);
            //Ajout des nouveaux
            using (TimedLock.Lock((_epreuves as ICollection).SyncRoot))
            {
                _epreuves.Clear();
                foreach (Epreuve epreuve in epreuves)
                {
                    Epreuve p = _epreuves.FirstOrDefault(o => o.id == epreuve.id);
                    /*
                    if (p != null)
                    {
                        _epreuves.Remove(p);
                    }
                    */
                    _epreuves.Add(epreuve);
                }
                lecture_vue_epreuves(DC);
            }

        }
        public ICollection<Epreuve> LectureEpreuves(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            return Epreuve.LectureEpreuves(xelement, MI);
        }


        /// <summary>
        /// lecture des vue_épreuves (équipe)
        /// </summary>
        /// <param name="element">element XML contenant les épreuves (équipe)</param>
        /// <param name="DC"></param>
        private void lecture_vue_epreuve_equipe(JudoData DC)
        {
            using (TimedLock.Lock((_vepreuves_equipe as ICollection).SyncRoot))
            {
                foreach (Epreuve_Equipe epreuve in _epreuve_equipes)
                {
                    vue_epreuve_equipe p = _vepreuves_equipe.FirstOrDefault(o => o.id == epreuve.id);
                    vue_epreuve_equipe vep = new vue_epreuve_equipe(epreuve, DC);
                    if (p != null)
                    {
                        _vepreuves_equipe.Remove(p);
                    }
                    _vepreuves_equipe.Add(vep);
                    //else
                    //{
                    //    p.id = vep.id;
                    //    p.remoteID = vep.remoteID;
                    //    p.nom = vep.nom;
                    //    p.debut = vep.debut;
                    //    p.fin = vep.fin;
                    //    p.ordre = vep.ordre;
                    //    p.nom_compet = vep.nom_compet;
                    //    p.competition = vep.competition;
                    //    p.categorieAge = vep.categorieAge;
                    //    p.remoteId_cateage = vep.remoteId_cateage;
                    //    p.nom_cateage = vep.nom_cateage;
                    //    p.nom_catepoids = vep.nom_catepoids;
                    //    p.lib_sexe = vep.lib_sexe;

                    //    p.ceintureMin = vep.ceintureMin;
                    //    p.ceintureMax = vep.ceintureMax;
                    //    p.anneeMin = vep.anneeMin;
                    //    p.anneeMax = vep.anneeMax;

                    //    p.phase1 = vep.phase1;
                    //    p.phase2 = vep.phase2;
                    //}
                }

                ////Suppression de ceux qui ont été supprimer
                //ICollection<i_vue_epreuve_equipe> deleted_e = new List<i_vue_epreuve_equipe>();
                //foreach (i_vue_epreuve_equipe epreuve in _vepreuves_equipe)
                //{
                //    IEpreuve_Equipe p = _epreuve_equipes.FirstOrDefault(o => o.id == epreuve.id);
                //    if (p == null)
                //    {
                //        deleted_e.Add(epreuve);
                //    }
                //}
                //foreach (i_vue_epreuve_equipe epreuve in deleted_e)
                //{
                //    _vepreuves_equipe.Remove(epreuve);
                //}
            }
        }


        /// <summary>
        /// lecture des épreuves
        /// </summary>
        /// <param name="element">element XML contenant les épreuves</param>
        /// <param name="DC"></param>
        private void lecture_vue_epreuves(JudoData DC)
        {
            //Ajout des nouveaux
            using (TimedLock.Lock((_vepreuves as ICollection).SyncRoot))
            {
                foreach (Epreuve epreuve in _epreuves)
                {
                    vue_epreuve p = _vepreuves.FirstOrDefault(o => o.id == epreuve.id);
                    vue_epreuve vep = new vue_epreuve(epreuve, DC);
                    if (p != null)
                    {
                        _vepreuves.Remove(p);
                    }
                    _vepreuves.Add(vep);
                    //else
                    //{
                    //    p.id = vep.id;
                    //    p.remoteID = vep.remoteID;
                    //    p.nom = vep.nom;
                    //    p.debut = vep.debut;
                    //    p.fin = vep.fin;
                    //    p.ordre = vep.ordre;
                    //    p.nom_compet = vep.nom_compet;
                    //    p.competition = vep.competition;
                    //    p.categorieAge = vep.categorieAge;
                    //    p.remoteId_cateage = vep.remoteId_cateage;
                    //    p.nom_cateage = vep.nom_cateage;
                    //    p.nom_catepoids = vep.nom_catepoids;
                    //    p.lib_sexe = vep.lib_sexe;

                    //    p.ceintureMin = vep.ceintureMin;
                    //    p.ceintureMax = vep.ceintureMax;
                    //    p.anneeMin = vep.anneeMin;
                    //    p.anneeMax = vep.anneeMax;

                    //    p.phase1 = vep.phase1;
                    //    p.phase2 = vep.phase2;
                    //}
                }
                ////Suppression de ceux qui ont été supprimer
                //ICollection<i_vue_epreuve> deleted_e = new List<i_vue_epreuve>();
                //foreach (i_vue_epreuve epreuve in _vepreuves)
                //{
                //    IEpreuve p = _epreuves.FirstOrDefault(o => o.id == epreuve.id);
                //    if (p == null)
                //    {
                //        deleted_e.Add(epreuve);
                //    }
                //}
                //foreach (i_vue_epreuve epreuve in deleted_e)
                //{
                //    _vepreuves.Remove(epreuve);
                //}
            }
        }
    }
}
