using FranceJudo.Core.Reflection;
using System.IO;

namespace FranceJudo.Core.IO
{
    public class FilteredFileInfo
    {
        private readonly FileInfo _fileinfo;
        private string _name = string.Empty;
        private readonly string _baseSuffix = string.Empty;

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

        public FilteredFileInfo(FileInfo fileInfo, string baseSuffix)
        {
            _baseSuffix = baseSuffix;

            if (null != fileInfo)
            {
                _fileinfo = fileInfo;
                Name = ResourcePath.GetRelativePath(fileInfo.Name, baseSuffix);
            }
        }

    }
}
