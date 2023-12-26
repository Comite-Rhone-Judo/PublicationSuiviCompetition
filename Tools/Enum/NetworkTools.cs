using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Outils;

namespace Tools.Enum
{
    /// <summary>
    /// Définit les outils réseaux (constante : ports, ...)
    /// </summary>
    public static class NetworkTools
    {
        public static int PortServerMin = 8480;
        public static int PortServerMax = OutilsTools.IsDebug ? 8480 : 8480;
        public static int PortSiteMin = 8080;
        public static int PortSiteMax = 8085;

        public static string FTP_EJUDO_SUIVI_URL = "ftp://www.ejudo.fr/tas/suivi/";        
        public static string FTP_EJUDO_SUIVI_LOG = "CRITT";
        public static string FTP_EJUDO_SUIVI_PASS = "Passw0rd";
        public static string HTTP_EJUDO_SUIVI_URL = "http://tas.ejudo.fr/suivi/";
        public static string HTTP_SUIVI_URL = "http://tas.ejudo.fr/suivi/";
    }
}
