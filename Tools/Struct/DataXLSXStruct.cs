using System.Collections.Generic;

namespace Tools.Struct
{
    public struct DataXLSXStruct
    {
        public int index { get; set; }
        public int maximum { get; set; }
        public IDictionary<string, int> columns { get; set; }
        public ICollection<Dictionary<string, string>> lines { get; set; }
    }
}
