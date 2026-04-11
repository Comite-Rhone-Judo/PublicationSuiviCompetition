using System.IO;

namespace FranceJudo.Core.IO
{
    public static class StreamExtension
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
