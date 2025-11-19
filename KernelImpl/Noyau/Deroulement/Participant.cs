
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Tools.Enum;
using Tools.Outils;

namespace KernelImpl.Noyau.Deroulement
{
    /// <summary>
    /// Description des Participants
    /// </summary>
    public class Participant : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _judoka;
        public int judoka
        {
            get { return _judoka; }
            set
            {
                if (_judoka != value)
                {
                    _judoka = value;
                    OnPropertyChanged("judoka");
                }
            }
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

        private int _phase;
        public int phase
        {
            get { return _phase; }
            set
            {
                if (_phase != value)
                {
                    _phase = value;
                    OnPropertyChanged("phase");
                }
            }
        }

        private int _ranking;
        public int ranking
        {
            get { return _ranking; }
            set
            {
                if (_ranking != value)
                {
                    _ranking = value;
                    OnPropertyChanged("ranking");
                }
            }
        }

        private int _classementAvant;
        public int classementAvant
        {
            get { return _classementAvant; }
            set
            {
                if (_classementAvant != value)
                {
                    _classementAvant = value;
                    OnPropertyChanged("classementAvant");
                }
            }
        }

        private int _classementFinal;
        public int classementFinal
        {
            get { return _classementFinal; }
            set
            {
                if (_classementFinal != value)
                {
                    _classementFinal = value;
                    OnPropertyChanged("classementFinal");
                }
            }
        }

        private bool _qualifie;
        public bool qualifie
        {
            get { return _qualifie; }
            set
            {
                if (_qualifie != value)
                {
                    _qualifie = value;
                    OnPropertyChanged("qualifie");
                }
            }
        }

        private int _position;
        public int position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged("position");
                }
            }
        }


        private int _ordreTirage;
        public int ordreTirage
        {
            get { return _ordreTirage; }
            set
            {
                if (_ordreTirage != value)
                {
                    _ordreTirage = value;
                    OnPropertyChanged("ordreTirage");
                }
            }
        }

        private int _poule;
        public int poule
        {
            get { return _poule; }
            set
            {
                if (_poule != value)
                {
                    _poule = value;
                    OnPropertyChanged("poule");
                }
            }
        }

        private int _positionOriginal;
        public int positionOriginal
        {
            get { return _positionOriginal; }
            set
            {
                if (_positionOriginal != value)
                {
                    _positionOriginal = value;
                    OnPropertyChanged("positionOriginal");
                }
            }
        }

        private int _nbVictoires;
        public int nbVictoires
        {
            get { return _nbVictoires; }
            set
            {
                if (_nbVictoires != value)
                {
                    _nbVictoires = value;
                    OnPropertyChanged("nbVictoires");
                }
            }
        }

        private int _nbVictoiresInd;
        public int nbVictoiresInd
        {
            get { return _nbVictoiresInd; }
            set
            {
                if (_nbVictoiresInd != value)
                {
                    _nbVictoiresInd = value;
                    OnPropertyChanged("nbVictoiresInd");
                }
            }
        }

        private int _cumulPoints;
        public int cumulPoints
        {
            get { return _cumulPoints; }
            set
            {
                if (_cumulPoints != value)
                {
                    _cumulPoints = value;
                    OnPropertyChanged("cumulPoints");
                }
            }
        }

        private int _cumulPointsGRCH;
        public int cumulPointsGRCH
        {
            get { return _cumulPointsGRCH; }
            set
            {
                if (_cumulPointsGRCH != value)
                {
                    _cumulPointsGRCH = value;
                    OnPropertyChanged("cumulPointsGRCH");
                }
            }
        }

        private DateTime _dernierCombat;
        public DateTime dernierCombat
        {
            get { return _dernierCombat; }
            set
            {
                if (_dernierCombat != value)
                {
                    _dernierCombat = value;
                    OnPropertyChanged("dernierCombat");
                }
            }
        }

        public Participants.Judoka Judoka1(JudoData DC)
        {
            return DC.Participants.Judokas.FirstOrDefault(o => o.id == this.judoka);
        }

        public Participants.Equipe Equipe1(JudoData DC)
        {
            return DC.Participants.Equipes.FirstOrDefault(o => o.id == this.judoka);
        }

        public void LoadXml(XElement xinfo)
        {
            this.judoka = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_Judoka));
            this.id = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_ID));

            this.phase = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_Phase));
            this.ranking = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_Ranking));
            this.classementAvant = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_ClassementAvant));
            this.classementFinal = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_ClassementFinal));
            this.qualifie = XMLTools.LectureBool(xinfo.Attribute(ConstantXML.Participant_Qualifie));
            this.position = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_Position));
            this.ordreTirage = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_OrdreTirage));
            this.poule = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_Poule));
            this.positionOriginal = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_PositionOriginal));
            this.nbVictoires = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_NbVictoires));
            this.nbVictoiresInd = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_NbVictoiresInd));
            this.cumulPoints = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_CumulPoints));
            this.cumulPointsGRCH = XMLTools.LectureInt(xinfo.Attribute(ConstantXML.Participant_CumulPointsGRCH));
            this.dernierCombat = XMLTools.LectureDate(xinfo.Attribute(ConstantXML.Participant_DernierCombat), "dd/MM/yyyy HH:mm", DateTime.MinValue);
        }


        public XElement ToXml(JudoData DC)
        {
            XElement xparticipant = new XElement(ConstantXML.Participant);

            // Valeurs communes
            xparticipant.SetAttributeValue(ConstantXML.Participant_Judoka, judoka);
            xparticipant.SetAttributeValue(ConstantXML.Participant_Phase, phase);
            xparticipant.SetAttributeValue(ConstantXML.Participant_Ranking, ranking);
            xparticipant.SetAttributeValue(ConstantXML.Participant_ClassementAvant, classementAvant);
            xparticipant.SetAttributeValue(ConstantXML.Participant_ClassementFinal, classementFinal);
            xparticipant.SetAttributeValue(ConstantXML.Participant_Qualifie, qualifie);
            xparticipant.SetAttributeValue(ConstantXML.Participant_Position, position);
            xparticipant.SetAttributeValue(ConstantXML.Participant_OrdreTirage, ordreTirage);
            xparticipant.SetAttributeValue(ConstantXML.Participant_NbVictoires, nbVictoires);
            xparticipant.SetAttributeValue(ConstantXML.Participant_NbVictoiresInd, nbVictoiresInd);
            xparticipant.SetAttributeValue(ConstantXML.Participant_CumulPoints, cumulPoints);
            xparticipant.SetAttributeValue(ConstantXML.Participant_CumulPointsGRCH, cumulPointsGRCH);
            xparticipant.SetAttributeValue(ConstantXML.Participant_Poule, poule);
            xparticipant.SetAttributeValue(ConstantXML.Participant_PositionOriginal, positionOriginal);
            xparticipant.SetAttributeValue(ConstantXML.Participant_DernierCombat, dernierCombat.ToString("dd/MM/yyyy HH:mm"));

            return xparticipant;
        }


        /// <summary>
        /// Lecture des Participants
        /// </summary>
        /// <param name="xelement">élément décrivant les Participants</param>
        /// <param name="MI">fonction d'info</param>
        /// <returns>les Participants</returns>

        public static ICollection<Participant> LectureParticipant(XElement xelement, OutilsTools.MontreInformation1 MI)
        {
            ICollection<Participant> participants = new List<Participant>();
            foreach (XElement xinfo in xelement.Descendants(ConstantXML.Participant))
            {
                Participant participant = new Participant();
                participant.LoadXml(xinfo);
                participants.Add(participant);
            }
            return participants;
        }
    }
}
