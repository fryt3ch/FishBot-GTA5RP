using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tesseract;

namespace FishBot
{
    public static class ImageExtensions
    {
        public static Bitmap CropAtRect(this Bitmap bm, Rectangle rect)
        {
            Bitmap newbm = new Bitmap(rect.Width, rect.Height);

            using (Graphics g = Graphics.FromImage(newbm))
            {
                g.DrawImage(bm, -rect.X, -rect.Y);
                return newbm;
            }
        }

        public static Bitmap Update(this Bitmap bm, IntPtr hwnd, Rectangle rect)
        {
            bm?.Dispose();

            return Capture.GetBitmap(hwnd, rect);
        }

        public static Bitmap BinarizeImage(this Bitmap bm, int minDelta = 120)
        {
            using (var wr = new ImageWrapper(bm))
            {
                Color curCol = wr[0, 0];
                for (int i = 0; i < wr.Width; i++)
                {
                    for (int j = 0; j < wr.Height; j++)
                    {
                        curCol = wr[i, j];

                        Point pix = new Point(i, j);

                        int maxRGB = curCol.R + curCol.G + curCol.B + minDelta * 3;

                        if (maxRGB >= 765)
                            wr.SetPixel(pix, 0, 0, 0);
                        else
                            wr.SetPixel(pix, 255, 255, 255);
                    }
                }
            }

            return bm;
        }

        public static Bitmap InvertImage(this Bitmap bm)
        {
            using (var wr = new ImageWrapper(bm))
            {
                Color curCol = wr[0, 0];
                for (int i = 0; i < wr.Width; i++)
                {
                    for (int j = 0; j < wr.Height; j++)
                    {
                        curCol = wr[i, j];

                        Point pix = new Point(i, j);

                        int maxRGB = curCol.R + curCol.G + curCol.B;

                        if (maxRGB == 765)
                            wr.SetPixel(pix, 0, 0, 0);
                        else
                            wr.SetPixel(pix, 255, 255, 255);
                    }
                }
            }

            return bm;
        }

        public static (bool Bool, Point pnt) FindPixel(this Bitmap bm, Color color, int deltaHue = 8, float deltaSaturation = 0.2f, float deltaBrightness = 0.2f)
        {
            for (int i = 0; i < bm.Width; i++)
                for (int j = 0; j < bm.Height; j++)
                    if (bm.GetPixel(i, j).SimilarTo(color, deltaHue, deltaSaturation, deltaBrightness))
                        return (true, new Point(i, j));

            return (false, Point.Empty);
        }

        public static bool SimilarTo(this Color compColor, Color mainColor, double deltaHue = 8, float deltaSaturation = 0.2f, float deltaBrightness = 0.2f)
        {
            double minHue = mainColor.GetHue() - deltaHue;
            double maxHue = mainColor.GetHue() + deltaHue;

            float minSaturation = mainColor.GetSaturation() - deltaSaturation;
            float maxSaturation = mainColor.GetSaturation() + deltaSaturation;

            float minBrightness = mainColor.GetBrightness() - deltaBrightness;
            float maxBrightness = mainColor.GetBrightness() + deltaBrightness;

            double hue = compColor.GetHue();
            float saturation = compColor.GetSaturation();
            float brightness = compColor.GetBrightness();

            if (hue >= minHue && hue <= maxHue)
                if (saturation >= minSaturation && saturation <= maxSaturation)
                    if (brightness >= minBrightness && brightness <= maxBrightness)
                        return true;

            return false;
        }

        public static string GetText(this Bitmap bm, TesseractEngine tesseract)
        {
            string text = "";
            try
            {
                Page page = tesseract.Process(bm);
                text = page.GetText();

                page?.Dispose();
                bm?.Dispose();
            }
            catch
            {

            }

            return text;
        }
    }

    class Capture
    {
        public static TesseractEngine TesseractAPI;
        public static TesseractEngine TesseractNumAPI;

        public static (int Width, int Height) GetWindowBounds(IntPtr hwnd)
        {
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(hwnd, ref windowRect);

            return (windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);
        }

        public static Bitmap GetBitmap(IntPtr hwnd, Rectangle rect)
        {
            int width = rect.Width;
            int height = rect.Height;
            int x = rect.X; int y = rect.Y;
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(hwnd);
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(hwnd, hdcSrc);
            // get a .NET image object for it
            Bitmap img = Bitmap.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }

        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
}
