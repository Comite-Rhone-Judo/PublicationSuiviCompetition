using FranceJudo.Core.Configuration.Json;

namespace AppPublication.Config.Generation
{
    public class GenerateurSiteInterneParams : JsonConfigElement
    {
        private int _delaiDeroulementSec = 30;
        private int _nbProchainsCombats = 6;

        public int DelaiDeroulementSec { get => _delaiDeroulementSec; set => SetValue(ref _delaiDeroulementSec, value); }
        public int NbProchainsCombats { get => _nbProchainsCombats; set => SetValue(ref _nbProchainsCombats, value); }
    }
}