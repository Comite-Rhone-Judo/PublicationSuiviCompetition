using FranceJudo.Core.Configuration.Json;

namespace AppPublication.Config.Publication
{
    public class SchedulerParams : JsonConfigElement
    {
        private string _id = string.Empty;
        private int _delaiGenerationSec = 30;

        public string ID { get => _id; set => SetValue(ref _id, value); }
        public int DelaiGenerationSec { get => _delaiGenerationSec; set => SetValue(ref _delaiGenerationSec, value); }
    }
}