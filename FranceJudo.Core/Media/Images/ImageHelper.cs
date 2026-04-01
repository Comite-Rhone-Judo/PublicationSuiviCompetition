using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace FranceJudo.Core.Media.Images
{
    public static class ImageHelper
    {
        /// <summary>
        /// Traite un fichier IMAGE pour le dimensionner
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="maxW"></param>
        /// <param name="maxH"></param>
        /// <param name="tag"></param>
        /// <returns></returns>

        public static MemoryStream CreerImage(FileStream stream, int maxW, int maxH, string tag)
        {

            MemoryStream storeStream = new MemoryStream();
            using (Bitmap bmp = new Bitmap(stream))
            {
                double actualW = bmp.Width * 1.0;
                double actualH = bmp.Height * 1.0;

                if (actualH <= maxH && actualW <= maxW)
                {
                    return null;
                    //maxH = (int)actualH;
                    //maxW = (int)actualW;
                }

                int newW = 0;
                int newH = 0;
                if (maxW != 0 && maxH != 0 && actualW != 0 && actualH != 0)
                {

                    double rapportW = maxW / actualW;
                    double rapportH = maxH / actualH;
                    double rapport = rapportW;
                    if (rapportW > rapportH)
                    {
                        rapport = rapportH;
                    }

                    newW = (int)(actualW * rapport);
                    newH = (int)(actualH * rapport);
                }
                else
                {
                    newW = (int)actualW;
                    newH = (int)actualH;
                }

                using (Image thumbnail = new Bitmap(newW, newH))
                {
                    using (Graphics graphic = Graphics.FromImage(thumbnail))
                    {
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphic.SmoothingMode = SmoothingMode.HighQuality;
                        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphic.CompositingQuality = CompositingQuality.HighQuality;

                        graphic.DrawImage(bmp, 0, 0, newW, newH);

                        if (tag != "")
                        {

                            Font font = new Font("Times New Roman", 48);
                            SizeF sizef = graphic.MeasureString(tag, font, Int32.MaxValue);
                            int currfontsize = 48;

                            while ((sizef.Height > (newH / 4) || sizef.Width > (newW / 2)) && currfontsize >= 12)
                            {
                                currfontsize = currfontsize switch
                                {
                                    48 => 36,
                                    36 => 24,
                                    24 => 20,
                                    _ => currfontsize - 2,
                                };
                                font = new Font("Times New Roman", currfontsize);
                                sizef = graphic.MeasureString(tag, font, Int32.MaxValue);

                            }

                            SolidBrush blueBrush = new SolidBrush(System.Drawing.Color.Black);
                            RectangleF rect = new RectangleF(0, newH - sizef.Height, sizef.Width, sizef.Height);


                            graphic.FillRectangle(blueBrush, rect);
                            blueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

                            graphic.DrawString(tag, font, blueBrush, rect);
                        }

                        ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 100L);

                        //thumbnail.Save(Response.OutputStream, info[1], encoderParameters);
                        thumbnail.Save(storeStream, Info[1], encoderParameters);
                        return storeStream;
                    }
                }
            }
        }

        /// <summary>
        /// Serialise a image
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ImageToString(string path)
        {
            if (path == null)

                throw new ArgumentNullException("path");

            using (Image im = Image.FromFile(path))
            {
                using (MemoryStream ms = new MemoryStream())
                {

                    im.Save(ms, im.RawFormat);

                    byte[] array = ms.ToArray();

                    return Convert.ToBase64String(array);
                }
            }
        }

        /// <summary>
        /// Deserialize an image
        /// </summary>
        /// <param name="imageString"></param>
        /// <returns></returns>
        public static Image StringToImage(string imageString)
        {

            if (imageString == null)

                throw new ArgumentNullException("imageString");

            byte[] array = Convert.FromBase64String(imageString);

            Image image = Image.FromStream(new MemoryStream(array));

            return image;

        }
    }
}
