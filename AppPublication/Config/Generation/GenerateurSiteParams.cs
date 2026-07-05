using FranceJudo.Core.Configuration.Json;
using System;

namespace AppPublication.Config.Generation
{
    public class GenerateurSiteParams : JsonConfigElement
    {
        private int _delaiActualisationClientSec = 30;
        private int _tailleMaxPouleColonnes = 5;
        private bool _pouleEnColonnes = false;
        private bool _pouleToujoursEnColonnes = false;
        private bool _publierProchainsCombats = false;
        private int _nbProchainsCombats = 6;
        private string _msgProchainsCombats = string.Empty;
        private bool _publierAffectationTapis = true;
        private bool _publierEngagements = true;
        private bool _publierStatistiques = true;
        private bool _engagementsAbsents = false;
        private bool _engagementsTousCombats = false;
        private bool _scoreEngagesGagnantPerdant = false;
        private bool _afficherPositionCombat = true;

        public int DelaiActualisationClientSec { get => _delaiActualisationClientSec; set => SetValue(ref _delaiActualisationClientSec, value); }
        public int TailleMaxPouleColonnes { get => _tailleMaxPouleColonnes; set => SetValue(ref _tailleMaxPouleColonnes, value); }
        public bool PouleEnColonnes { get => _pouleEnColonnes; set => SetValue(ref _pouleEnColonnes, value); }
        public bool PouleToujoursEnColonnes { get => _pouleToujoursEnColonnes; set => SetValue(ref _pouleToujoursEnColonnes, value); }
        public bool PublierProchainsCombats { get => _publierProchainsCombats; set => SetValue(ref _publierProchainsCombats, value); }
        public int NbProchainsCombats { get => _nbProchainsCombats; set => SetValue(ref _nbProchainsCombats, value); }
        public string MsgProchainsCombats { get => _msgProchainsCombats; set => SetValue(ref _msgProchainsCombats, value); }
        public bool PublierAffectationTapis { get => _publierAffectationTapis; set => SetValue(ref _publierAffectationTapis, value); }
        public bool PublierEngagements { get => _publierEngagements; set => SetValue(ref _publierEngagements, value); }
        public bool PublierStatistiques { get => _publierStatistiques; set => SetValue(ref _publierStatistiques, value); }
        public bool EngagementsAbsents { get => _engagementsAbsents; set => SetValue(ref _engagementsAbsents, value); }
        public bool EngagementsTousCombats { get => _engagementsTousCombats; set => SetValue(ref _engagementsTousCombats, value); }
        public bool ScoreEngagesGagnantPerdant { get => _scoreEngagesGagnantPerdant; set => SetValue(ref _scoreEngagesGagnantPerdant, value); }
        public bool AfficherPositionCombat { get => _afficherPositionCombat; set => SetValue(ref _afficherPositionCombat, value); }
    }
}