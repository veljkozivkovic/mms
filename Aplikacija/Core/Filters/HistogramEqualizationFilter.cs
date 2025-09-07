using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Aplikacija.Core.Filters
{
    public class HistogramEqualizationFilter : IImageFilter
    {
        public string Name => "Histogram Equalization (Y, robust)";


        private const double TAIL_CUT = 0.005;

        public Bitmap Apply(Bitmap src)
        {
            int w = src.Width, h = src.Height, n = w * h;
            var rect = new Rectangle(0, 0, w, h);


            bool cloned = false;
            Bitmap src32;
            if (src.PixelFormat != PixelFormat.Format32bppArgb)
            {
                src32 = src.Clone(rect, PixelFormat.Format32bppArgb);
                cloned = true;
            }
            else
            {
                src32 = src;
            }

            byte[] Y = new byte[n];          
            int[] hist = new int[256];       

 
            var s = src32.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                unsafe
                {
                    byte* sBase = (byte*)s.Scan0;
                    int stride = s.Stride;

                    Parallel.For(0, h,
                        () => new int[256], 
                        (y, loop, localHist) =>
                        {
                            byte* p = sBase + y * stride;
                            int row = y * w;

                            for (int x = 0; x < w; x++)
                            {
                                int B = p[0], G = p[1], R = p[2];

                                
                                int yv = (19595 * R + 38470 * G + 7471 * B + (1 << 15)) >> 16;
                                if ((uint)yv > 255) yv = yv < 0 ? 0 : 255;

                                int idx = row + x;
                                Y[idx] = (byte)yv;
                                localHist[yv]++;

                                p += 4;
                            }

                            return localHist;
                        },
                        localHist =>
                        {
                            lock (hist)
                            {
                                for (int i = 0; i < 256; i++) hist[i] += localHist[i];
                            }
                        });
                }
            }
            finally { src32.UnlockBits(s); }

           
            int total = n;
            if (total == 0) return src.Clone(rect, PixelFormat.Format32bppArgb);

            int[] cdf = new int[256];
            {   
                int c = 0;
                for (int i = 0; i < 256; i++) { c += hist[i]; cdf[i] = c; }
            }

            
            int lowCount = (int)Math.Round(total * TAIL_CUT);
            int highCount = (int)Math.Round(total * (1.0 - TAIL_CUT));

            int lowIdx = 0; while (lowIdx < 256 && cdf[lowIdx] < lowCount) lowIdx++;
            int highIdx = 255; while (highIdx > 0 && cdf[highIdx] > highCount) highIdx--;

            
            if (lowIdx >= highIdx)
            {
                
                return src.Clone(rect, PixelFormat.Format32bppArgb);
            }

            int cdfLow = cdf[lowIdx];
            int cdfHigh = cdf[highIdx];
            int denom = cdfHigh - cdfLow;
            if (denom <= 0)
            {
                return src.Clone(rect, PixelFormat.Format32bppArgb);
            }

            
            byte[] map = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                int num = cdf[i] - cdfLow;
                if (num < 0) num = 0;
                if (num > denom) num = denom;

                int val = (int)((num * 255L + (denom >> 1)) / denom);
                if ((uint)val > 255) val = val < 0 ? 0 : 255;
                map[i] = (byte)val;
            }

           
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            var s2 = src32.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var d2 = dst.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    byte* sBase = (byte*)s2.Scan0; int sStride = s2.Stride;
                    byte* dBase = (byte*)d2.Scan0; int dStride = d2.Stride;

                    const int HALF = 1 << 15; 

                    Parallel.For(0, h, y =>
                    {
                        byte* sRow = sBase + y * sStride;
                        byte* dRow = dBase + y * dStride;
                        int row = y * w;

                        for (int x = 0; x < w; x++)
                        {
                            byte* sp = sRow + x * 4;
                            byte* dp = dRow + x * 4;

                            int B = sp[0], G = sp[1], R = sp[2], A = sp[3];

                            
                            int yOld = Y[row + x];
                            int yNew = map[yOld];

                            // U-128 i V-128 direktno iz RGB (Q16):
                            // U-128 = -0.168736R -0.331264G +0.5B
                            int u_m128 = (-11059 * R - 21709 * G + 32768 * B + HALF) >> 16;
                            // V-128 = +0.5R -0.418688G -0.081312B
                            int v_m128 = (32768 * R - 27439 * G - 5329 * B + HALF) >> 16;

                            // Inverzno u RGB:
                            // R = Y + 1.402 * (V-128)      => 1.402 * 65536 = 91881
                            int R2 = yNew + ((91881 * v_m128 + HALF) >> 16);
                            // G = Y - (0.344136*(U-128) + 0.714136*(V-128)) => 22554, 46802
                            int G2 = yNew - ((22554 * u_m128 + 46802 * v_m128 + HALF) >> 16);
                            // B = Y + 1.772 * (U-128)      => 1.772 * 65536 = 116130
                            int B2 = yNew + ((116130 * u_m128 + HALF) >> 16);

                            if ((uint)R2 > 255) R2 = R2 < 0 ? 0 : 255;
                            if ((uint)G2 > 255) G2 = G2 < 0 ? 0 : 255;
                            if ((uint)B2 > 255) B2 = B2 < 0 ? 0 : 255;

                            dp[0] = (byte)B2;
                            dp[1] = (byte)G2;
                            dp[2] = (byte)R2;
                            dp[3] = (byte)A; // ALFA ISTI
                        }
                    });
                }
            }
            finally
            {
                src32.UnlockBits(s2);
                dst.UnlockBits(d2);
                if (cloned) src32.Dispose();
            }

            return dst;
        }
    }
}
