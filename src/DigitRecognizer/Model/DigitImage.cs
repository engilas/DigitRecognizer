using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SimplePerceptron.Trains;
using Color = System.Drawing.Color;

namespace DigitRecognizer.Model
{
    public class DigitImage
    {
        public readonly int Width;
        public readonly int Height;
        public int PixelsCount => Width * Height;
        public readonly byte[] Pixels;
        public byte[] Bytes => Pixels.ToArray();
        public readonly byte Label;

        public DigitImage(int width, int height, byte[] pixels, byte label)
        {
            Width = width; Height = height;
            Pixels = pixels;
            Label = label;
        }

        public ImageSource ToBitmap(int mag)
        {
            Bitmap bmp = new Bitmap(Width, Height);
            Graphics gr = Graphics.FromImage(bmp);
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    int pixelColor = 255 - Pixels[i * Width + j];
                    Color c = Color.FromArgb(pixelColor, pixelColor, pixelColor);
                    SolidBrush sb = new SolidBrush(c);
                    gr.FillRectangle(sb, j * mag, i * mag, mag, mag);
                }
            }

            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));

            return bs;
        }
    }
}
