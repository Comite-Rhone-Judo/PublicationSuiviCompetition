using FranceJudo.Metier.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Export
{
    public interface IReadOnlyConfigurationExportSiteInterne
    {
        public string Logo { get; }
        public long DelaiDeroulementSec { get; }
        public int NbProchainsCombats { get; }
        public string UrlRedirecteur { get; }
    }
}
