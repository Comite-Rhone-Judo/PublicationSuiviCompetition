using System.Windows.Media.Imaging;

namespace Tools.Struct
{
    public struct Sponsor
    {
        public int id { get; set; }
        public string uri { get; set; }
        public BitmapImage image { get; set; }
    }
}
