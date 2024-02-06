using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Enum;

namespace AppPublication.Tools
{
    public class FilteredFileInfo
    {
        private FileInfo _fileinfo;
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        public string FullName
        {
            get
            {
                return _fileinfo.FullName;
            }
        }

        public FilteredFileInfo(FileInfo fileInfo)
        {
            if(null != fileInfo)
            {
                _fileinfo = fileInfo;
                Name = _fileinfo.Name.Replace(ConstantResource.Export_site_img, "");
            }
        }

    }
}
