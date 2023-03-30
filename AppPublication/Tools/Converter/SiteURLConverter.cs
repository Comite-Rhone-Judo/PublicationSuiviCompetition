using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using Tools.Outils;
using Tools.Export;
using KernelImpl.Noyau.Organisation;

namespace AppPublication.Tools.Converter
{ 
    public class SiteURLConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string output = "URL indéfinie";

            // Verifie le type des donnees en entree
            if (values.Count() >= 2 )
            {
                MiniSite site = null;
                Competition compet = null;

                foreach (object item in values)
                {
                    if(item.GetType() == typeof(MiniSite))
                    {
                        site = (MiniSite)item;
                    }
                    if (item.GetType() == typeof(Competition))
                    {
                        compet = (Competition) item;
                    }
                }

                if(site != null && compet != null) { 

                    if (site.IsActif)
                    {
                        if (site.IsLocal)
                        {
                            if (!String.IsNullOrEmpty(compet.remoteId) && site.ServerHTTP != null && site.ServerHTTP.IpAddress != null && site.ServerHTTP.Port > 0)
                            {
                                output = ExportTools.GetURLSiteLocal(site.ServerHTTP.IpAddress.ToString(),
                                                                        site.ServerHTTP.Port,
                                                                        compet.remoteId);
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(compet.remoteId) && !String.IsNullOrEmpty(site.URLDistant))
                            {
                                output = ExportTools.GetURLSiteDistant(site.URLDistant, compet.remoteId);
                            }
                        }
                    }
                    else
                    {
                        output = "Site Inactif";
                    }
                }
            }

            return output;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
