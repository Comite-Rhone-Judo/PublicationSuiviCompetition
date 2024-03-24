using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublication.Tools
{
    public static class StreamTool
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            byte[] bytes;
            using (var binaryReader = new BinaryReader(stream))
            {
                bytes = binaryReader.ReadBytes((int)stream.Length);
            }
            return bytes;
        }
    }
}
