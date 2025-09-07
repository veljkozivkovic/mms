using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplikacija.Core.Filters
{
    public class BlacklightFilter : IImageFilter
    {
        public string Name => "Blacklight (lite)";

        public Bitmap Apply(Bitmap src)
        {
            var w = src.Width; var h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var r = new Rectangle(0, 0, w, h);
            var s = src.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var d = dst.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < h; y++)
                {
                    byte* sp = (byte*)s.Scan0 + y * s.Stride;
                    byte* dp = (byte*)d.Scan0 + y * d.Stride;
                    for (int x = 0; x < w; x++)
                    {
                        int b = sp[0], g = sp[1], r8 = sp[2];
                        int avg = (r8 + g + b) / 3;
                        // blago podizanje mid-tona + ljubičasti cast
                        int nr = Clamp((int)(r8 * 0.95 + avg * 0.05 + 28));
                        int ng = Clamp((int)(g * 0.80 + avg * 0.20 + 12));
                        int nb = Clamp((int)(b * 0.90 + avg * 0.10 + 48));
                        dp[0] = (byte)nb; dp[1] = (byte)ng; dp[2] = (byte)nr; dp[3] = 255;
                        sp += 4; dp += 4;
                    }
                }
            }
            src.UnlockBits(s); dst.UnlockBits(d);
            return dst;
        }
        private static int Clamp(int v) => v < 0 ? 0 : (v > 255 ? 255 : v);
    }
}
