using FranceJudo.Core.Reflection;
using System.Reflection;

namespace FranceJudo.Metier.Resources
{
    /// <summary>
    /// Enumération des constants pour la gestion des fichier
    /// </summary>

    // TODO Verifier avec la nouvelle structure des namespace

    public class ResourceDictionnay
    {
        private static readonly Assembly _thisAssembly = typeof(ResourceDictionnay).Assembly;

        public static readonly string Root = _thisAssembly.GetName().Name + ".Resources.";
        public static readonly string Site = ResourceDictionnay.Root + "Site.";
        public static readonly string Site_Img = ResourceDictionnay.Site + "img.";
        public static readonly string Site_Js = ResourceDictionnay.Site + "js.";
        public static readonly string Site_Xslt = ResourceDictionnay.Site + "xslt.";
        public static readonly string Site_Style = ResourceDictionnay.Site + ".style";

        public static readonly string Site_Img_DefaultLogo = "logo-France-Judo.png";

        public static readonly string Referentiels = ResourceDictionnay.Root + "Referentiels.";
        public static readonly string Referentiels_PublicationFFJUDO = ResourceDictionnay.Referentiels + "PublicationFFJudo.xml";
        public static readonly string Referentiels_Structures = ResourceDictionnay.Referentiels + "structures.xml";


        /*
        public static readonly string Export_xslt = ResourceDictionnay.Export + "xslt.";
        public static readonly string Export_Site_res = ResourceDictionnay.Export_xslt + "Site.";

        public static readonly string Export_style_res = ResourceDictionnay.Export + "style.";
        public static readonly string Export_site_style = ResourceDictionnay.Export_style_res + "site.";


        public static readonly string Export_site_js = ResourceDictionnay.Export + "js.";
        public static readonly string Export_Diplome = ResourceDictionnay.Export + "img.fond.";
        public static readonly string Export_Icon = ResourceDictionnay.Export + "img.icon.";
        public static readonly string Export_DefaultLogo = "logo-France-Judo.png";

        public static readonly string Media = AssemblyResourceHelper.GetAssembyName() + ".data.media.";
        public static readonly string Media_Son = ResourceDictionnay.Media + "son.";
        public static readonly string Media_Video = ResourceDictionnay.Media + "video.";
        public static readonly string Media_Flags = ResourceDictionnay.Media + "flags.";


        public static readonly string XSDJudokas = AssemblyResourceHelper.GetAssembyName() + ".data.xml.judoka.xsd";
        public static readonly string XSDCompetitions = AssemblyResourceHelper.GetAssembyName() + ".data.xml.competition.xsd";

        public static readonly string MaskXLS = AssemblyResourceHelper.GetAssembyName() + ".data.mask.excel.xlsx";
        public static readonly string MaskCSV = AssemblyResourceHelper.GetAssembyName() + ".data.mask.csv.csv";

        public static readonly string CateAge = AssemblyResourceHelper.GetAssembyName() + ".data.data.c_ages.xml";
        public static readonly string CatePoids = AssemblyResourceHelper.GetAssembyName() + ".data.data.c_poids.xml";
        public static readonly string Grades = AssemblyResourceHelper.GetAssembyName() + ".data.data.grades.xml";
        public static readonly string GestionTemps = AssemblyResourceHelper.GetAssembyName() + ".data.data.g_temps.xml";
        public static readonly string Structures = AssemblyResourceHelper.GetAssembyName() + ".data.data.structures.xml";
        public static readonly string Ligues = AssemblyResourceHelper.GetAssembyName() + ".data.data.s_ligues.xml";
        public static readonly string Comites = AssemblyResourceHelper.GetAssembyName() + ".data.data.s_comites.xml";

        public static readonly string PublicationFFJUDO = AssemblyResourceHelper.GetAssembyName() + ".data.data.PublicationFFJudo.xml";
        */
    }
}
