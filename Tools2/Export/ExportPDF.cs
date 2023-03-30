using System.Collections.Generic;
using System.Xml;
using Tools.Enum;
using Tools.Outils;

namespace Tools.Export
{
    public abstract class ExportPDF
    {
        //public static byte[] ToPDF(string filename, bool landscape)
        //{
        //    return ExportPDF_EVO.ToPDF(filename, landscape);
        //}
        protected bool _margin = true;
        protected bool _landscape = false;
        protected bool _regenere = true;
        protected bool _background = false;
        protected bool _withlogo = false;
        protected string _fond = "";
        protected ExportEnum _type = ExportEnum.Diplome;
        
        public OutilsTools.MontreInformation1 MI1 = null;

        public ExportPDF()
        {

        }

        // abstract string ToPDF(List<ExportPDFItem> pdf);
        public abstract string ToPDF(List<string> pdf);
        public abstract string ToPDFClassement(XmlDocument xclassement, string filename);
        public abstract string ToPDFParticipation(XmlDocument xclassement, string filename);

        public bool Margin
        {
            get
            {
                return _margin;
            }

            set
            {
                _margin = value;
            }
        }

        public bool Landscape
        {
            get
            {
                return _landscape;
            }

            set
            {
                _landscape = value;
            }
        }

        public bool Regenere
        {
            get
            {
                return _regenere;
            }

            set
            {
                _regenere = value;
            }
        }

        public bool Background
        {
            get
            {
                return _background;
            }

            set
            {
                _background = value;
            }
        }

        public bool Withlogo
        {
            get
            {
                return _withlogo;
            }

            set
            {
                _withlogo = value;
            }
        }

        public string Fond
        {
            get
            {
                return _fond;
            }

            set
            {
                _fond = value;
            }
        }

        public ExportEnum Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }
    }
}
