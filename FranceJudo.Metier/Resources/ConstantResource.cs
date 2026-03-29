using FranceJudo.Core.Reflection;

namespace FranceJudo.Metier.Resources
{
    /// <summary>
    /// Enumération des constants pour la gestion des fichier
    /// </summary>

    // TODO Verifier avec la nouvelle structure des namespace

    public class ConstantResource
    {
        public const string CacheMainWindow = "StoryboardCache";
        public const string MontreMainWindow = "StoryboardMontre";

        public static readonly string Export = AssemblyResourceHelper.GetAssembyName() + ".Export.";
        public static readonly string Export_xslt = ConstantResource.Export + "xslt.";
        public static readonly string Export_Common_res = ConstantResource.Export_xslt + "Common.";
        public static readonly string Export_Classement_res = ConstantResource.Export_xslt + "Classement.";
        public static readonly string Export_Judoka_res = ConstantResource.Export_xslt + "Judoka.";
        public static readonly string Export_Poule_res = ConstantResource.Export_xslt + "Poule.";
        public static readonly string Export_Site_res = ConstantResource.Export_xslt + "Site.";
        public static readonly string Export_Tableau_res = ConstantResource.Export_xslt + "Tableau.";

        public static readonly string Export_style_res = ConstantResource.Export + "style.";
        public static readonly string Export_site_style = ConstantResource.Export_style_res + "site.";
        public static readonly string Export_site_img = ConstantResource.Export + "img.site.";

        public static readonly string Export_site_js = ConstantResource.Export + "js.";
        public static readonly string Export_Diplome = ConstantResource.Export + "img.fond.";
        public static readonly string Export_Icon = ConstantResource.Export + "img.icon.";
        public static readonly string Export_DefaultLogo = "logo-France-Judo.png";

        public static readonly string Media = AssemblyResourceHelper.GetAssembyName() + ".data.media.";
        public static readonly string Media_Son = ConstantResource.Media + "son.";
        public static readonly string Media_Video = ConstantResource.Media + "video.";
        public static readonly string Media_Flags = ConstantResource.Media + "flags.";


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


        public static readonly string Data_Base = AssemblyResourceHelper.GetAssembyName() + ".data.bd.Judo.mdf";
        public static readonly string Data_Update = AssemblyResourceHelper.GetAssembyName() + ".data.bd.judobase.sql";
    }
}
