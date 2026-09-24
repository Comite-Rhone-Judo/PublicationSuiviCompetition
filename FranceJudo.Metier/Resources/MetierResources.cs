using FranceJudo.Core.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace FranceJudo.Metier.Resources
{
    /// <summary>
    /// Point d'accès unique et typé pour toutes les ressources embarquées du projet Métier.
    /// </summary>
    public static class MetierResources
    {
        // 1. L'instance du dictionnaire configurée pour ce projet
        public static readonly AssemblyResourceDictionary Dictionary =
            new AssemblyResourceDictionary(typeof(MetierResources).Assembly, "FranceJudo.Metier.Resources");

        // 2. Des constantes propres pour les dossiers (pour éviter les fautes de frappe dans le code appelant)
        [ExcludeFromCodeCoverage]
        public static class Folders
        {
            public const string Site = "Site";
            public const string SiteImg = "Site.img";
            public const string SiteJs = "Site.js";
            public const string SiteXslt = "Site.xslt";
            public const string SiteStyle = "Site.style";
            public const string Referentiels = "Referentiels";
        }

        [ExcludeFromCodeCoverage]
        public static class Files
        {
            public const string PublicationFFJudo = "PublicationFFJudo.xml";
            public const string DefaultLogo = "logo-France-Judo.png";
            public const string Structures = "structures.xml";
        }

        // 3. (Optionnel) Des méthodes "raccourcis" fortement typées pour les fichiers critiques
        // Cela évite de chercher le nom du fichier exact dans le code appelant

        public static Stream GetPublicationFFJudoXml()
            => Dictionary.GetStream(ResourcePath.Combine(Folders.Referentiels, Files.PublicationFFJudo));

        public static Stream GetStructuresXml()
            => Dictionary.GetStream(ResourcePath.Combine(Folders.Referentiels, Files.Structures));

        public static Stream GetDefaultLogo()
            => Dictionary.GetStream(ResourcePath.Combine(Folders.SiteImg, Files.DefaultLogo));
    }
}