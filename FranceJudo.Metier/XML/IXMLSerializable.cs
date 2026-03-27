using FranceJudo.Metier.Noyau;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FranceJudo.Metier.XML
{
    public interface IXMLSerializable
    {
        public void LoadXml(XElement xinfo);
        public XElement ToXml(IJudoData DC = null);

    }
}
