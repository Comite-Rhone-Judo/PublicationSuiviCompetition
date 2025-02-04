using System;
using System.Reflection;

// Les informations présentes dans ce fichier sont partagees et propagees dans tous les assemblys de la solution
// En cas d'ajout d'un nouveau projet, il convient de modifier le fichier AssemblyInfos.cs pour retirer
// les valeurs crees par defaut en doublon avec celles presentes ci-dessous. Ce fichier doit egalement etre inclus dans le projet

// Les informations de version pour un assembly se composent des quatre valeurs suivantes :
//
//      Version principale
//      Version secondaire 
//      Numéro de build
//      Révision
//
// Vous pouvez spécifier toutes les valeurs ou indiquer les numéros de build et de révision par défaut 
// en utilisant '*', comme indiqué ci-dessous :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.3.0.0")]
[assembly: AssemblyVersion("1.3.0.0")]
[assembly: AssemblyVersionBeta(0)]

[assembly: AssemblyCompany("FRANCE JUDO - Fédération Française de Judo et Disciplines Associées - RHONE")]
[assembly: AssemblyCopyright("Copyright © FRANCE JUDO RHONE 2023 - Tous droits réservés")]
[assembly: AssemblyTrademark("FRANCE JUDO - RHONE METROPOLE LYON JUDO")]


[AttributeUsage(AttributeTargets.Assembly)]
internal class AssemblyVersionBeta : Attribute
{
    public int Value { get; set; }
    public AssemblyVersionBeta(int valueTest)
        Value = valueTest;
    }
}