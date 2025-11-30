using System.IO;

namespace AppPublication.Tools.Streams
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
