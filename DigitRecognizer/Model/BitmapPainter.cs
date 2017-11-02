using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SimplePerceptron.Trains;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace DigitRecognizer.Model
{
    class BitmapPainter// : ITrainImage
    {
        private readonly int _width;
        private readonly int _height;
        private WriteableBitmap _image;

        public BitmapPainter(int width, int height)
        {
            _width = width;
            _height = height;
            InitializeImage();
        }

        public void DrawPixel(System.Windows.Point position)
        {
            try
            {
                var rect = new Int32Rect((int) Math.Round(position.X), (int) Math.Round(position.Y), 1, 1);
                _image.WritePixels(rect, new byte[] {0}, 1, 0);
            }
            catch { }
        }

        public void ClearImage()
        {
            InitializeImage();
        }

        public BitmapSource GetImage()
        {
            return _image;
        }

        public byte[] GetBytes()
        {
            int size = _width * _height * _image.Format.BitsPerPixel / 8;
            byte[] arr = new byte[size];
            IntPtr buffer = _image.BackBuffer;
            Marshal.Copy(buffer, arr, 0, size);
            return arr;
        }

        private void InitializeImage()
        {
            _image = new WriteableBitmap(_width,
                _height, 96, 96, PixelFormats.Gray8, null);
            var pixels = Enumerable.Repeat<byte>(0xff, _width * _height * _image.Format.BitsPerPixel / 8).ToArray();
            var rect = new Int32Rect(0, 0, _width, _height);
            _image.WritePixels(rect, pixels, _image.PixelWidth * _image.Format.BitsPerPixel / 8, 0);
        }
    }
}
