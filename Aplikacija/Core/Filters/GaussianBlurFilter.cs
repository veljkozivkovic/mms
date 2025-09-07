using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Aplikacija.Core.Filters
{
    public class GaussianBlurFilter : IImageFilter
    {

        public string Name => $"Gaussian Blur (fast box×3)";

        public Bitmap Apply(Bitmap src)
        {
            
            Bitmap input = src.PixelFormat == PixelFormat.Format32bppArgb
                ? (Bitmap)src.Clone()
                : Clone32(src);

            // jaci blur na manjim, umeren na ogromnim
            int w = input.Width, h = input.Height;
            long pixels = (long)w * h;
            int radius = pixels >= 60_000_000 ? 5       
                        : 6;                         

            
            return FastGaussianBox3(input, radius);
        }

        
        private static Bitmap FastGaussianBox3(Bitmap src, int radius)
        {
            int w = src.Width, h = src.Height;
            var rect = new Rectangle(0, 0, w, h);


            Bitmap a = src;
            Bitmap b = new Bitmap(w, h, PixelFormat.Format32bppArgb);

            var aData = a.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bData = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    byte* aBase = (byte*)aData.Scan0; int aStride = aData.Stride;
                    byte* bBase = (byte*)bData.Scan0; int bStride = bData.Stride;

                    const int PASSES = 3;
                    for (int p = 0; p < PASSES; p++)
                    {
                        
                        BoxBlurHorizontal(aBase, aStride, bBase, bStride, w, h, radius);
                        BoxBlurVertical(bBase, bStride, aBase, aStride, w, h, radius);
                    }
                }
            }
            finally
            {
                a.UnlockBits(aData);
                b.UnlockBits(bData);
            }

            b.Dispose(); 
            return a;
        }

        
        private static unsafe void BoxBlurHorizontal(byte* srcBase, int srcStride, byte* dstBase, int dstStride, int w, int h, int r)
        {
            int div = 2 * r + 1;
            int half = div >> 1;

            Parallel.For(0, h, y =>
            {
                byte* sRow = srcBase + y * srcStride;
                byte* dRow = dstBase + y * dstStride;

                //  klamp ivica
                int sumB = 0, sumG = 0, sumR = 0;
                for (int ix = -r; ix <= r; ix++)
                {
                    int cx = ix < 0 ? 0 : (ix >= w ? w - 1 : ix);
                    byte* p = sRow + cx * 4;
                    sumB += p[0]; sumG += p[1]; sumR += p[2];
                }

                for (int x = 0; x < w; x++)
                {
                    
                    byte* q = dRow + x * 4;
                    q[0] = (byte)((sumB + half) / div);
                    q[1] = (byte)((sumG + half) / div);
                    q[2] = (byte)((sumR + half) / div);
                    q[3] = 255;

                    //  +right, -left (sa klampom)
                    int left = x - r;
                    int right = x + r + 1;
                    if (left < 0) left = 0;
                    if (right >= w) right = w - 1;

                    byte* pL = sRow + left * 4;
                    byte* pR = sRow + right * 4;

                    sumB += pR[0] - pL[0];
                    sumG += pR[1] - pL[1];
                    sumR += pR[2] - pL[2];
                }
            });
        }

        // ====== 1D BOX VERTICAL ======
        private static unsafe void BoxBlurVertical(byte* srcBase, int srcStride, byte* dstBase, int dstStride, int w, int h, int r)
        {
            int div = 2 * r + 1;
            int half = div >> 1;

            Parallel.For(0, w, x =>
            {
                // klamp ivica)
                int sumB = 0, sumG = 0, sumR = 0;
                for (int iy = -r; iy <= r; iy++)
                {
                    int cy = iy < 0 ? 0 : (iy >= h ? h - 1 : iy);
                    byte* p = srcBase + cy * srcStride + x * 4;
                    sumB += p[0]; sumG += p[1]; sumR += p[2];
                }

                for (int y = 0; y < h; y++)
                {
                    byte* q = dstBase + y * dstStride + x * 4;
                    q[0] = (byte)((sumB + half) / div);
                    q[1] = (byte)((sumG + half) / div);
                    q[2] = (byte)((sumR + half) / div);
                    q[3] = 255;

                    int top = y - r;
                    int bottom = y + r + 1;
                    if (top < 0) top = 0;
                    if (bottom >= h) bottom = h - 1;

                    byte* pT = srcBase + top * srcStride + x * 4;
                    byte* pB = srcBase + bottom * srcStride + x * 4;

                    sumB += pB[0] - pT[0];
                    sumG += pB[1] - pT[1];
                    sumR += pB[2] - pT[2];
                }
            });
        }

        // ====== helper ======
        private static Bitmap Clone32(Bitmap src)
        {
            return src.Clone(new Rectangle(0, 0, src.Width, src.Height), PixelFormat.Format32bppArgb);
        }
    }
}
