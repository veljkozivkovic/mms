using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Aplikacija.Core.Filters
{
    public class BlacklightFilter : IImageFilter
    {
        private readonly int _weight;       
        private readonly bool _perceptualLuma; 

        public string Name => $"Blacklight (Tanner-Helland, w={_weight}{(_perceptualLuma ? ", luma" : ", avg")})";

        public BlacklightFilter(int weight = 2, bool perceptualLuma = true)
        {
            if (weight < 1) weight = 1;
            if (weight > 7) weight = 7;
            _weight = weight;
            _perceptualLuma = perceptualLuma;
        }

        public Bitmap Apply(Bitmap src)
        {
            int w = src.Width, h = src.Height;
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
                        int B = sp[0], G = sp[1], R = sp[2];

                        // 1) Luminansa (preciznija ili brza)
                        int L = _perceptualLuma
                            ? (222 * R + 707 * G + 71 * B + 500) / 1000  
                            : (R + G + B) / 3;

                        // 2)blacklight transformacija
                        int nR = Math.Abs(R - L) * _weight;
                        int nG = Math.Abs(G - L) * _weight;
                        int nB = Math.Abs(B - L) * _weight;

                        // 3) clamp [0,255]
                        if (nR > 255) nR = 255;
                        if (nG > 255) nG = 255;
                        if (nB > 255) nB = 255;

                        dp[0] = (byte)nB; dp[1] = (byte)nG; dp[2] = (byte)nR; dp[3] = 255;

                        sp += 4; dp += 4;
                    }
                }
            }

            src.UnlockBits(s);
            dst.UnlockBits(d);
            return dst;
        }
    }
}
