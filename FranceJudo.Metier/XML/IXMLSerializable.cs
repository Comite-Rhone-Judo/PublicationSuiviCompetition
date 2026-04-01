using FranceJudo.Metier.Noyau;
using System.Xml.Linq;

namespace FranceJudo.Metier.XML
{
    public interface IXMLSerializable
    {
        public void LoadXml(XElement xinfo);
        public XElement ToXml(IJudoData DC = null);

    }
}
