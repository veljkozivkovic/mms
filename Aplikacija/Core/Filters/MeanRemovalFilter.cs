using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Aplikacija.Core.Filters
{
    public class MeanRemovalFilter : IImageFilter
    {
        public string Name => "Mean Removal";

        // 3×3 "mean removal" (ostrenje): center=9, okolo=-1, div=1
        private static readonly int[,] Kernel = new int[,]
        {
            { -1, -1, -1 },
            { -1,  9, -1 },
            { -1, -1, -1 }
        };
        private const int Div = 1;

        public Bitmap Apply(Bitmap src)
        {
            
            if (src.Width < 3 || src.Height < 3)
                return Clone32(src);

            return Convolve3x3(src, Kernel, Div);
        }

        
        private static Bitmap Convolve3x3(Bitmap src, int[,] k, int div)
        {
            if (div == 0) div = 1;

            int w = src.Width, h = src.Height;
            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, w, h);

            var sData = src.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var dData = dst.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    byte* sBase = (byte*)sData.Scan0; int sStride = sData.Stride;
                    byte* dBase = (byte*)dData.Scan0; int dStride = dData.Stride;

                    
                    for (int y = 0; y < h; y++)
                    {
                        byte* sRow = sBase + y * sStride;
                        byte* dRow = dBase + y * dStride;
                        Buffer.MemoryCopy(sRow, dRow, dStride, (long)w * 4); 
                    }

                    
                    int k00 = k[0, 0], k01 = k[0, 1], k02 = k[0, 2];
                    int k10 = k[1, 0], k11 = k[1, 1], k12 = k[1, 2];
                    int k20 = k[2, 0], k21 = k[2, 1], k22 = k[2, 2];

                    
                    Parallel.For(1, h - 1, y =>
                    {
                        byte* rowM = sBase + (y - 1) * sStride;
                        byte* rowC = sBase + y * sStride;
                        byte* rowP = sBase + (y + 1) * sStride;

                        for (int x = 1; x < w - 1; x++)
                        {
                            byte* pMm = rowM + (x - 1) * 4; byte* pMc = rowM + x * 4; byte* pMp = rowM + (x + 1) * 4;
                            byte* pCm = rowC + (x - 1) * 4; byte* pCc = rowC + x * 4; byte* pCp = rowC + (x + 1) * 4;
                            byte* pPm = rowP + (x - 1) * 4; byte* pPc = rowP + x * 4; byte* pPp = rowP + (x + 1) * 4;

                            int sumB =
                                pMm[0] * k00 + pMc[0] * k01 + pMp[0] * k02 +
                                pCm[0] * k10 + pCc[0] * k11 + pCp[0] * k12 +
                                pPm[0] * k20 + pPc[0] * k21 + pPp[0] * k22;

                            int sumG =
                                pMm[1] * k00 + pMc[1] * k01 + pMp[1] * k02 +
                                pCm[1] * k10 + pCc[1] * k11 + pCp[1] * k12 +
                                pPm[1] * k20 + pPc[1] * k21 + pPp[1] * k22;

                            int sumR =
                                pMm[2] * k00 + pMc[2] * k01 + pMp[2] * k02 +
                                pCm[2] * k10 + pCc[2] * k11 + pCp[2] * k12 +
                                pPm[2] * k20 + pPc[2] * k21 + pPp[2] * k22;

                            
                            int b = sumB / div; if ((uint)b > 255) b = b < 0 ? 0 : 255;
                            int g = sumG / div; if ((uint)g > 255) g = g < 0 ? 0 : 255;
                            int r = sumR / div; if ((uint)r > 255) r = r < 0 ? 0 : 255;

                            byte* q = dBase + y * dStride + x * 4;
                            q[0] = (byte)b;
                            q[1] = (byte)g;
                            q[2] = (byte)r;
                            q[3] = 255; // ALFA ISTI
                        }
                    });
                }
            }
            finally
            {
                src.UnlockBits(sData);
                dst.UnlockBits(dData);
            }

            return dst;
        }

        private static Bitmap Clone32(Bitmap src)
        {
            return src.Clone(new Rectangle(0, 0, src.Width, src.Height),
                             PixelFormat.Format32bppArgb);
        }
    }
}
