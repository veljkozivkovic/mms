using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplikacija.Core.Filters
{
    public class HistogramEqualizationFilter : IImageFilter
    {
        public string Name => "Histogram Equalization (Y)";

        public Bitmap Apply(Bitmap src)
        {
            int w = src.Width, h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            // 1) izračun Y + hist
            int[] hist = new int[256];
            byte[,] Y = new byte[h, w];
            byte[,] U = new byte[h, w];
            byte[,] V = new byte[h, w];

            var r = new Rectangle(0, 0, w, h);
            var s = src.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                for (int y = 0; y < h; y++)
                {
                    byte* p = (byte*)s.Scan0 + y * s.Stride;
                    for (int x = 0; x < w; x++)
                    {
                        int B = p[0], G = p[1], R = p[2];
                        double yv = 0.299 * R + 0.587 * G + 0.114 * B;
                        double uv = -0.169 * R - 0.331 * G + 0.5 * B + 128.0;
                        double vv = 0.5 * R - 0.419 * G - 0.081 * B + 128.0;
                        byte yb = (byte)Math.Clamp((int)Math.Round(yv), 0, 255);
                        byte ub = (byte)Math.Clamp((int)Math.Round(uv), 0, 255);
                        byte vb = (byte)Math.Clamp((int)Math.Round(vv), 0, 255);
                        Y[y, x] = yb; U[y, x] = ub; V[y, x] = vb;
                        hist[yb]++;
                        p += 4;
                    }
                }
            }
            src.UnlockBits(s);

            // 2) CDF
            int n = w * h;
            int c = 0; byte[] map = new byte[256];
            for (int i = 0; i < 256; i++) { c += hist[i]; map[i] = (byte)Math.Round((c - (double)hist[0]) / (n - 1) * 255.0); }

            // 3) nazad u RGB, ali sa equalizovanim Y
            var d = dst.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < h; y++)
                {
                    byte* p = (byte*)d.Scan0 + y * d.Stride;
                    for (int x = 0; x < w; x++)
                    {
                        int yv = map[Y[y, x]];
                        int u = U[y, x] - 128;
                        int v = V[y, x] - 128;
                        int R = Clamp(yv + (int)Math.Round(1.402 * v));
                        int G = Clamp(yv - (int)Math.Round(0.344136 * u + 0.714136 * v));
                        int B = Clamp(yv + (int)Math.Round(1.772 * u));
                        p[0] = (byte)B; p[1] = (byte)G; p[2] = (byte)R; p[3] = 255;
                        p += 4;
                    }
                }
            }
            dst.UnlockBits(d);
            return dst;
        }
        private static int Clamp(int v) => v < 0 ? 0 : (v > 255 ? 255 : v);
    }
}
