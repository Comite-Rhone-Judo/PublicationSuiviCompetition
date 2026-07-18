using FranceJudo.Core.Configuration.Json;
using static AppPublication.Models.EcransAppel.EcranAppelModel;

namespace AppPublication.Config.Generation
{
    public class EcranAppelParams : JsonConfigElement
    {
        private string _hostname = string.Empty;
        private int _id = 1;
        private string _description = "Nouvel Ecran";
        private string _adresseIp = string.Empty;
        private int _groupement = 1;
        private string _tapisIds = string.Empty;
        private DispositionAffichage _disposition = DispositionAffichage.Colonne;
        private DispositionAffichage _dispositionCombat = DispositionAffichage.Colonne;
        private bool _ajusteTexteAuto = false;
        private int _nbCombatsPage = 5;
        private bool _afficheCategorieAge = false;

        public string Hostname { get => _hostname; set => SetValue(ref _hostname, value); }
        public int Id { get => _id; set => SetValue(ref _id, value); }
        public string Description { get => _description; set => SetValue(ref _description, value); }
        public string AdresseIp { get => _adresseIp; set => SetValue(ref _adresseIp, value); }
        public int Groupement { get => _groupement; set => SetValue(ref _groupement, value); }
        public string TapisIds { get => _tapisIds; set => SetValue(ref _tapisIds, value); }
        public DispositionAffichage Disposition { get => _disposition; set => SetValue(ref _disposition, value); }
        public DispositionAffichage DispositionCombat { get => _dispositionCombat; set => SetValue(ref _dispositionCombat, value); }
        public bool AjusteTexteAuto { get => _ajusteTexteAuto; set => SetValue(ref _ajusteTexteAuto, value); }
        public int NbCombatsPage { get => _nbCombatsPage; set => SetValue(ref _nbCombatsPage, value); }

        public bool AfficheCategorieAge { get => _afficheCategorieAge; set => SetValue(ref _afficheCategorieAge, value); }
    }
}