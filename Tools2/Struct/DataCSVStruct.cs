using System.Collections.Generic;

namespace Tools.Struct
{
    public struct DataCSVStruct
    {
        public IDictionary<string, int> columns { get; set; }
        public ICollection<Dictionary<string, string>> lines { get; set; }
    }
}
