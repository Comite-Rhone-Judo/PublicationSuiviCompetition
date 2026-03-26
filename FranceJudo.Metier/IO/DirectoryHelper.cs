using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FranceJudo.Core.Environment;
using FranceJudo.Core.IO;
using FranceJudo.Metier.Resources;

namespace FranceJudo.Metier.IO
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// Racine par defaut pour des donnees France Judo
        /// </summary>
        /// <param name="racine"></param>
        /// <returns></returns>
        public static string GetExportDir(string racine)
        {
            return Path.Combine(racine, "FRANCE-JUDO");
        }

        /// <summary>
        /// Création des répertoires nécessaires au fonctionnement de l'application (sauf l'export du site !!)
        /// </summary>
        /// 

        // TODO Deplacer cette méthode comme membre d'une classe représentant la structure de repertoire de l'application
public static void InitDataDirectories()
{

    string directory = ConstantFile.Export_dir;

    FileSystemHelper.CreateDirectorie(ConstantFile.BD_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.Data_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.Export_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.ExportStyle_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.ExportStyleSite_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.ExportStyleIcon_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.ExportStyleDiplome_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.ExportJudoTV);
            FileSystemHelper.CreateDirectorie(ConstantFile.DirectorySave);
            FileSystemHelper.CreateDirectorie(ConstantFile.SaveCSDirectory);
            FileSystemHelper.CreateDirectorie(ConstantFile.SavePeseeDirectory);
            FileSystemHelper.CreateDirectorie(ConstantFile.SaveCOMDirectory);
            FileSystemHelper.CreateDirectorie(ConstantFile.Params_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.Logo1_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.Logo2_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.Logo3_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.Logo_tmp_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.MediaSon_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.MediaVideo_dir);
            FileSystemHelper.CreateDirectorie(ConstantFile.MediaFlags_dir);

            FileSystemHelper.CreateDirectorie(directory + "site");
    //if (!Directory.Exists(directory + "site"))
    //{
    //    Directory.CreateDirectory(directory + "site");
    //}

    if (AppEnvironment.GetAppDirectory() == AppEnvironment.GetDataDirectory())
    {
        //return;
    }

    string[] files = AssemblyResourceHelper.GetAssembyResourceName();
    foreach (string s1 in files)
    {
        if ((!s1.Contains(ConstantResource.Export) && !s1.Contains(ConstantResource.Media)) || s1.Contains(ConstantResource.Export_site_js) || s1.Contains(ConstantResource.Export_xslt))
        {
            continue;
        }

        string dir_copy = ConstantFile.ExportStyle_dir;
        string fileName = s1.Replace(ConstantResource.Export_style_res, "");

        if (s1.Contains(ConstantResource.Export_site_style))
        {
            dir_copy = ConstantFile.ExportStyleSite_dir;
            fileName = s1.Replace(ConstantResource.Export_site_style, "");
        }

        if (s1.Contains(ConstantResource.Export_Icon))
        {
            dir_copy = ConstantFile.ExportStyleIcon_dir;
            fileName = s1.Replace(ConstantResource.Export_Icon, "");
        }

        if (s1.Contains(ConstantResource.Export_Diplome))
        {
            dir_copy = ConstantFile.ExportStyleDiplome_dir;
            fileName = s1.Replace(ConstantResource.Export_Diplome, "");
        }

        if (s1.Contains(ConstantResource.Media_Son))
        {
            dir_copy = ConstantFile.MediaSon_dir;
            fileName = s1.Replace(ConstantResource.Media_Son, "");
        }

        if (s1.Contains(ConstantResource.Media_Video))
        {
            dir_copy = ConstantFile.MediaVideo_dir;
            fileName = s1.Replace(ConstantResource.Media_Video, "");
        }

        if (s1.Contains(ConstantResource.Media_Flags))
        {
            dir_copy = ConstantFile.MediaFlags_dir;
            fileName = s1.Replace(ConstantResource.Media_Flags, "");
        }

        var resource = AssemblyResourceHelper.GetAssembyResource(s1);
        try
        {
            FileSystemHelper.NeedAccessFile(dir_copy + fileName);
            using (FileStream fs = new FileStream(dir_copy + fileName, FileMode.Create))
            {
                byte[] bytes = new byte[resource.Length];
                resource.Read(bytes, 0, (int)resource.Length);
                fs.Write(bytes, 0, bytes.Length);
                resource.Close();
            }
        }
        catch { }
        finally
        {
            FileSystemHelper.ReleaseFile(dir_copy + fileName);
        }
    }
}
}
}
