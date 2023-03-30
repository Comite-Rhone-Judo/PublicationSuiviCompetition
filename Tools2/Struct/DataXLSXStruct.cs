using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
