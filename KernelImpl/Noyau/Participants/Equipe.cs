
using FranceJudo.Core.XML;
using FranceJudo.Metier.Noyau;
using FranceJudo.Metier.Noyau.Participants;
using FranceJudo.Metier.XML;
using KernelImpl.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace KernelImpl.Noyau.Participants
{
    /// <summary>
    /// Description des Equipes
    /// </summary>
    public class Equipe : IEquipe, INotifyPropertyChanged, IEntityWithKey<int>
    {
        int IEntityWithKey<int>.EntityKey => id;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id;
        public int id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("id");
                }
            }
        }

        private string _libelle;
        public string libelle
        {
            get { return _libelle; }
            set
            {
                if (_libelle != value)
                {
                    _libelle = value;
                    OnPropertyChanged("libelle");
                }
            }
        }

        private string _club;
        public string club
        {
            get { return _club; }
            set
            {
                if (_club != value)
                {
                    _club = value;
                    OnPropertyChanged("club");
                }
            }
        }


        private string _comite;
        public string comite
        {
            get { return _comite; }
            set
            {
                if (_comite != value)
                {
                    _comite = value;
                    OnPropertyChanged("comite");
                }
            }
        }

        private string _ligue;
        public string ligue
        {
            get { return _ligue; }
            set
            {
                if (_ligue != value)
                {
                    _ligue = value;
                    OnPropertyChanged("ligue");
                }
            }
        }

        private int _pays;
        public int pays
        {
            get { return _pays; }
            set
            {
                if (_pays != value)
                {
                    _pays = value;
                    OnPropertyChanged("pays");
                }
            }
        }

        private string _remoteId;
        public string remoteId
        {
            get { return _remoteId; }
            set
            {
                if (_remoteId != value)
                {
                    _remoteId = value;
                    OnPropertyChanged("remoteId");
                }
            }
        }

        public void LoadXml(XElement xequipe)
        {
            this.id = XMLTools.LectureInt(xequipe.Attribute(ConstantXML.Equipe_Id));
            this.libelle = XMLTools.LectureString(xequipe.Attribute(ConstantXML.Equipe_Nom));
            this.club = XMLTools.LectureString(xequipe.Attribute(ConstantXML.Equipe_Club));
            this.comite = XMLTools.LectureString(xequipe.Attribute(ConstantXML.Equipe_Comite));
            this.ligue = XMLTools.LectureString(xequipe.Attribute(ConstantXML.Equipe_Ligue));
            this.pays = XMLTools.LectureInt(xequipe.Attribute(ConstantXML.Equipe_Pays));
            this.remoteId = XMLTools.LectureString(xequipe.Attribute(ConstantXML.Equipe_RemoteId));
        }

        public XElement ToXml(IJudoData DC = null)
        {
            XElement xequipe = new XElement(ConstantXML.Equipe);

            xequipe.SetAttributeValue(ConstantXML.Equipe_Nom, libelle.ToString());
            xequipe.SetAttributeValue(ConstantXML.Equipe_Id, id.ToString());
            xequipe.SetAttributeValue(ConstantXML.Equipe_Club, club.ToString());
            xequipe.SetAttributeValue(ConstantXML.Equipe_Comite, comite.ToString());
            xequipe.SetAttributeValue(ConstantXML.Equipe_Ligue, ligue.ToString());
            xequipe.SetAttributeValue(ConstantXML.Equipe_Pays, pays.ToString());
            xequipe.SetAttributeValue(ConstantXML.Equipe_RemoteId, remoteId.ToString());

            return xequipe;
        }



        public Judoka CreateJudoka(Organisation.Epreuve epreuve, int judoka)
        {
            //int index = 0;

            //ON Creer les judokas avec des nom bidon

            Judoka j1 = new Judoka
            {
                club = this.club,
                nom = "Judoka " + judoka,
                prenom = "(" + epreuve.nom + ")",
                sexeEnum = epreuve.sexeEnum,
                equipe = this.id,
                naissance = new DateTime(epreuve.anneeMin, 01, 01),
                passeport = false,
                pays = 250,
                poids = 0,
                poidsMesure = 0,
                remoteID = "",
                qualifieE0 = 0,
                qualifieE1 = 0,
                ajoute = true,
                licence = "",
                //j1.modeControle = 1;
                modePesee = 1,
                modification = false,
                datePesee = DateTime.Now,
                categorie = epreuve.categorieAge,
                ceinture = epreuve.ceintureMin,
                etat = (int)EtatJudokaEnum.Inscrit,
                present = false
            };

            return j1;
        }

        /// <summary>
        /// Lecture des Equipes
        /// </summary>
        /// <param name="xelement">élément décrivant les Equipes</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>Equipes</returns>

        public static ICollection<Equipe> LectureEquipes(XElement xelement)
        {
            ICollection<Equipe> equipes = new List<Equipe>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Equipe))
            {
                Equipe equipe = new Equipe();
                equipe.LoadXml(xinfo);
                equipes.Add(equipe);
            }
            return equipes;
        }

    }
}
