using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Aplikacija.Core.Filters
{
    public class GaussianBlurFilter : IImageFilter
    {
        public string Name => "Gaussian Blur (3x3)";
        private static readonly int[,] K = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
        private const int DIV = 16;

        public Bitmap Apply(Bitmap src) => Convolve3x3(src, K, DIV);

        private static Bitmap Convolve3x3(Bitmap src, int[,] k, int div)
        {
            var w = src.Width; var h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var r = new Rectangle(0, 0, w, h);
            var sData = src.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dData = dst.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            
            unsafe
            {
                for (int y = 1; y < h - 1; y++)
                {
                    byte* dRow = (byte*)dData.Scan0 + y * dData.Stride;
                    for (int x = 1; x < w - 1; x++)
                    {
                        int sumB = 0, sumG = 0, sumR = 0;
                        for (int ky = -1; ky <= 1; ky++)
                        {
                            byte* sRow = (byte*)sData.Scan0 + (y + ky) * sData.Stride;
                            for (int kx = -1; kx <= 1; kx++)
                            {
                                byte* p = sRow + (x + kx) * 4;
                                int kk = k[ky + 1, kx + 1];
                                sumB += p[0] * kk; sumG += p[1] * kk; sumR += p[2] * kk;
                            }
                        }
                        var q = dRow + x * 4;
                        q[0] = (byte)(sumB / div); q[1] = (byte)(sumG / div); q[2] = (byte)(sumR / div); q[3] = 255;
                    }
                }
            }
            src.UnlockBits(sData); dst.UnlockBits(dData);
            return dst;
        }
    }
}
