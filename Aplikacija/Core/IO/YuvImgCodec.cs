using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplikacija.Core.IO
{
    public enum Subsampling { YUV420 = 0 } // proširivo

    public static class YuvImgCodec
    {
        private static readonly byte[] Magic = System.Text.Encoding.ASCII.GetBytes("YUVIMG1\0");

        public static void Save(string path, Bitmap bmp, Subsampling ss = Subsampling.YUV420)
        {
            int w = bmp.Width, h = bmp.Height;
            byte[] Y, U, V; ToYuv(bmp, out Y, out U, out V);
            // 4:2:0 – downsample U,V u oba smera
            byte[] Uds = Down2x2(U, w, h);
            byte[] Vds = Down2x2(V, w, h);

            using var fs = File.Create(path);
            using var bw = new BinaryWriter(fs);
            bw.Write(Magic);
            bw.Write(w); bw.Write(h);
            bw.Write((int)ss);
            bw.Write(Y.Length); bw.Write(Uds.Length); bw.Write(Vds.Length);
            bw.Write(Y); bw.Write(Uds); bw.Write(Vds);
        }

        public static Bitmap Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);
            var magic = br.ReadBytes(Magic.Length);
            if (!AreEqual(magic, Magic)) throw new InvalidDataException("Bad .yuvimg header");

            int w = br.ReadInt32(); int h = br.ReadInt32();
            var ss = (Subsampling)br.ReadInt32();
            int yLen = br.ReadInt32(), uLen = br.ReadInt32(), vLen = br.ReadInt32();

            byte[] Y = br.ReadBytes(yLen);
            byte[] Uds = br.ReadBytes(uLen);
            byte[] Vds = br.ReadBytes(vLen);

            // 4:2:0 – upsample nearest
            byte[] U = UpNearest2x2(Uds, w, h);
            byte[] V = UpNearest2x2(Vds, w, h);

            return FromYuv(Y, U, V, w, h);
        }

        private static void ToYuv(Bitmap bmp, out byte[] Y, out byte[] U, out byte[] V)
        {
            int w = bmp.Width, h = bmp.Height, n = w * h;
            Y = new byte[n]; U = new byte[n]; V = new byte[n];

            var r = new Rectangle(0, 0, w, h);
            var d = bmp.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < h; y++)
                {
                    byte* p = (byte*)d.Scan0 + y * d.Stride;
                    int row = y * w;
                    for (int x = 0; x < w; x++)
                    {
                        int B = p[0], G = p[1], R = p[2];
                        double yy = 0.299 * R + 0.587 * G + 0.114 * B;
                        double uu = -0.169 * R - 0.331 * G + 0.5 * B + 128.0;
                        double vv = 0.5 * R - 0.419 * G - 0.081 * B + 128.0;
                        Y[row + x] = (byte)Math.Clamp((int)Math.Round(yy), 0, 255);
                        U[row + x] = (byte)Math.Clamp((int)Math.Round(uu), 0, 255);
                        V[row + x] = (byte)Math.Clamp((int)Math.Round(vv), 0, 255);
                        p += 4;
                    }
                }
            }
            bmp.UnlockBits(d);
        }

        private static Bitmap FromYuv(byte[] Y, byte[] U, byte[] V, int w, int h)
        {
            var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            var r = new Rectangle(0, 0, w, h);
            var d = bmp.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < h; y++)
                {
                    byte* p = (byte*)d.Scan0 + y * d.Stride;
                    int row = y * w;
                    for (int x = 0; x < w; x++)
                    {
                        int yy = Y[row + x];
                        int u = U[row + x] - 128;
                        int v = V[row + x] - 128;
                        int R = Clamp(yy + (int)Math.Round(1.402 * v));
                        int G = Clamp(yy - (int)Math.Round(0.344136 * u + 0.714136 * v));
                        int B = Clamp(yy + (int)Math.Round(1.772 * u));
                        p[0] = (byte)B; p[1] = (byte)G; p[2] = (byte)R; p[3] = 255;
                        p += 4;
                    }
                }
            }
            bmp.UnlockBits(d);
            return bmp;
        }

        private static byte[] Down2x2(byte[] src, int w, int h)
        {
            int W = (w + 1) / 2, H = (h + 1) / 2;
            byte[] dst = new byte[W * H];
            for (int y = 0; y < h; y += 2)
                for (int x = 0; x < w; x += 2)
                {
                    int s00 = src[y * w + x];
                    int s01 = (x + 1 < w) ? src[y * w + x + 1] : s00;
                    int s10 = (y + 1 < h) ? src[(y + 1) * w + x] : s00;
                    int s11 = (x + 1 < w && y + 1 < h) ? src[(y + 1) * w + x + 1] : s00;
                    dst[(y / 2) * W + (x / 2)] = (byte)((s00 + s01 + s10 + s11 + 2) / 4);
                }
            return dst;
        }

        private static byte[] UpNearest2x2(byte[] src, int w, int h)
        {
            int W = (w + 1) / 2, H = (h + 1) / 2;
            byte[] dst = new byte[w * h];
            for (int y = 0; y < h; y++)
            {
                int sy = Math.Min(H - 1, y / 2);
                for (int x = 0; x < w; x++)
                {
                    int sx = Math.Min(W - 1, x / 2);
                    dst[y * w + x] = src[sy * W + sx];
                }
            }
            return dst;
        }

        private static bool AreEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            return true;
        }
        private static int Clamp(int v) => v < 0 ? 0 : (v > 255 ? 255 : v);
    }
}
