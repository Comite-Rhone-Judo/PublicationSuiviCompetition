using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Export
{
    public class ExportPDFItem
    {
        string _pdfName;        // la chaine de contenu
        bool _landscape = false;   // true si orientation landscape, false sinon

        /// <summary>
        /// Property pour acceder au contenu de l'item
        /// </summary>
        public string PdfName
        {
            get
            {
                return _pdfName;
            }

            set
            {
                _pdfName = value;
            }
        }

        /// <summary>
        /// Property pour savoir si l'item doit etre oriente en mode paysage (True), False sinon
        /// </summary>
        public bool IsLandscape
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
    }
}
